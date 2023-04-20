using System.IdentityModel.Tokens.Jwt;
using System.Linq.Expressions;
using System.Net;
using System.Net.Mail;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using ApplicationServices.MappingProfile.Account;
using AutoMapper;
using DB.Extensions;
using DnsClient;
using Domain.Account;
using Interfaces.Account;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Shared.Configuration;
using Shared.Requests.Account;
using Shared.Responses.Account;
using Utility.Email;
using Utility.Extensions;

namespace ApplicationServices.Account;

public class UserService : IUserService
{
    private readonly IMapper _mapper;
    private readonly ILogger<UserService> _logger;
    private readonly ICacheConfiguration<ApplicationUser> _cache;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly RoleManager<ApplicationRole> _roleManager;
    private readonly AppConfiguration _appConfig;

    private readonly IMailGenerator _mailGenerator;
    private readonly IEMailService _mailService;
    private readonly IUnitOfWork<Guid> _unitOfWork;

    public UserService(
        ICacheConfiguration<ApplicationUser> cache,
        IMapper mapper,
        ILogger<UserService> logger,
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signinManager,
        RoleManager<ApplicationRole> roleManager,
        IOptions<AppConfiguration> appConfig,
        IMailGenerator mailGenerator,
        IEMailService eMailService,
        IUnitOfWork<Guid> unitOfWork
        )
    {
        _roleManager = roleManager;
        _mailGenerator = mailGenerator;
        _mapper = mapper;
        _logger = logger;
        _cache = cache;
        _signInManager = signinManager;
        _userManager = userManager;
        _mailService = eMailService;
        _appConfig = appConfig.Value;
        _roleManager = roleManager;
        _unitOfWork = unitOfWork;
    }

    /// <summary>
    /// Register new User
    /// </summary>
    /// <param name="request">Request containing all user details</param>
    /// <returns>User Id</returns>
    public async Task<ApiResponse<UserResponse>> RegisterUserAsync(RegisterRequest request)
    {
        try
        {
            if (!await IsValidAsync(request.Email))
            {
                var str = "The email address {0} looks invalid. Please correct the same!";
                _logger.LogError(str, request.Email);
                return await ApiResponse<UserResponse>.FailAsync("E:" + string.Format(str, request.Email), _logger);
            }

            if (await _userManager.FindByNameAsync(request.Email) != null)
            {
                _logger.LogError("User with email address {0} [{1}] is already registered and attempted again!",
                    request.Email, request.FirstName + " " + request.LastName);
                return await ApiResponse<UserResponse>.FailAsync("E:" + string.Format("User with eMail {0} is already taken.", request.Email), _logger);
                
            }

            if (!string.IsNullOrWhiteSpace(request.PhoneNumber))
            {
                var userExists = _userManager.Users.FirstOrDefault(
                    (Expression<Func<ApplicationUser, bool>>)(x => x.PhoneNumber == request.PhoneNumber));
                if (userExists != null)
                {
                    _logger.LogError("User with phone number {0} [{1}] is already registered and attempted again!",
                        request.PhoneNumber, request.FirstName + " " + request.LastName);
                    return await ApiResponse<UserResponse>.FailAsync("E:" + string.Format("Phone number {0} is already registered.",
                        request.PhoneNumber), _logger);
                }
            }

            // TODO Send Emails


            if (request.Password != request.ConfirmPassword)
            {
                return await ApiResponse<UserResponse>.FailAsync("E: Password and confirm password does not match", _logger);
            }

            var userId = Guid.NewGuid();

            var user = new ApplicationUser
            {

                Id = userId,
                UserName = request.Email,
                FirstName = request.FirstName,
                LastName = request.LastName,
                Email = request.Email,
                PhoneNumber = request.PhoneNumber,
                EmailConfirmed = request.EmailVerified,
                CreatedOn = DateTime.UtcNow,
                TwoFactorEnabled = false,
                UserType = request.UserType,
                ParentEntityId = request.ParentEntityId,
            };

            var result = await _userManager.CreateAsync(user, request.Password);

            if (!result.Succeeded)
            {
                _logger.LogError("Unable to create the User {0} / {1}. Check Error Log!", request.Email,
                    request.FirstName + " " + request.LastName);
                foreach (var error in result.Errors)
                    _logger.LogError("Error for User {0} - Code : {1}; Description : {2}", request.Email,
                        error.Code, error.Description);
                return await ApiResponse<UserResponse>.FailAsync("E:" + "Sorry we have a failure. Please contact Support!", _logger);
            }

            _logger.LogInformation("Attaching the user {0} to {1}!", request.Email, request.Role);

             _ = await SendConfirmEMailCode(user);
            _logger.LogInformation("User {0} created successfully!", request.Email);

            var getUserById = await GetAsync(user.Id);
            var obj = _mapper.Map<ApplicationUser>(getUserById.Data);
            _cache.SetInCacheMemoryAsync(obj);

            return await ApiResponse<UserResponse>.SuccessAsync(getUserById.Data);
            
        }
        catch (Exception ex)
        {
            _logger.LogCritical(ex, "Registration Via email failed for user {0}", request.Email);
            return await ApiResponse<UserResponse>.FailAsync("E:Registration Via email failed for user " + request.Email, _logger);
        }
    }

    /// <summary>
    /// Login existing users
    /// </summary>
    /// <param name="request">Login Credentials</param>
    /// <returns>JWT Authentication Token</returns>
    public async Task<ApiResponse<TokenResponse>> LoginAsync(LoginRequest request)
    {
        try
        {
            var user = await _userManager.FindByNameAsync(request.EMail);

            if (user == null)
            {
                _logger.LogError("The User with the email address " + request.EMail + " was not found!");
                return await ApiResponse<TokenResponse>.FailAsync("Some Error Occurred while registering new user",
                    _logger);
            }


            if (!user.EmailConfirmed)
            {
                _logger.LogError("The User with the email address " + request.EMail +
                                 " has not yet confirmed the email address and attempted to login!");
                // return await ApiResponse<TokenResponse>.FailAsync("E-Mail not confirmed.",
                //   _logger);
            }

            if (!await _userManager.CheckPasswordAsync(user, request.Password))
            {
                _logger.LogError("The User with the email address " + request.EMail +
                                 " supplied incorrect password!");
                return await ApiResponse<TokenResponse>.FailAsync("Invalid Credentials.",
                    _logger);
            }



            user.RefreshToken = GenerateRefreshToken();
            user.RefreshTokenExpiryTime = DateTime.Now.AddDays(7.0);

            await _userManager.UpdateAsync(user);
            var jwtAsync = await GenerateJwtAsync(user);

            //var file = await _fileService.GetFileURLAsync(Guid.Parse(user.ProfilePictureUrl));

            var data = new TokenResponse
            {
                Token = jwtAsync,
                RefreshToken = user.RefreshToken,
                UserId = user.Id,
                UserImageUrl = ""
            };

            _logger.LogInformation("Sussesful Login for user {0}", request.EMail);
            return await ApiResponse<TokenResponse>.SuccessAsync(data);
        }
        catch (Exception ex)
        {
            _logger.LogCritical(ex, "Login Via email failed for user {0}", request.EMail);
            return await ApiResponse<TokenResponse>.FatalAsync(ex, _logger);
        }

    }

    /// <summary>
    /// Regenerate Email Confirmation Code and send email again
    /// </summary>
    /// <param name="emailAddress">The email address of user</param>
    /// <returns>Base Api Response</returns>
    public async Task<BaseApiResponse> RegenEmailConfirmCode(string emailAddress)
    {

        try
        {
            var user = await _userManager.FindByNameAsync(emailAddress);
            if (user == null)
            {
                _logger.LogError("The User with the email address " + emailAddress + " was not found!");
                return await BaseApiResponse.FailAsync("User Not Found.", _logger);
            }

            _ = await SendConfirmEMailCode(user);

            return await BaseApiResponse.SuccessAsync(string.Format(
                "EMailed User '{0}' with the code. Please check your Mailbox to verify!", user.FullName));
        }
        catch (Exception ex)
        {
            _logger.LogCritical(ex, "Registration Via email failed for user {0}", emailAddress);
            return await BaseApiResponse.FatalAsync(ex, _logger);
        }
    }

    /// <summary>
    /// Confirm user email address
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    public async Task<BaseApiResponse> ConfirmEmailAsync(ConfirmEmailRequest request)
    {
        try
        {
            var user = await _userManager.FindByNameAsync(request.EMail);
            if (user == null)
            {
                _logger.LogError("The User with the email address " + request.EMail + " was not found!");
                return await BaseApiResponse.FailAsync("User Not Found.", _logger);
            }

            var token = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(request.Token));
            var identityResult = await _userManager.ConfirmEmailAsync(user, token);
            if (identityResult.Succeeded)
            {
                _logger.LogInformation("Successfully confirmed email address for user " + request.EMail);

                _ = await SendWelcomeEMail(user);

                return await BaseApiResponse.SuccessAsync(
                    string.Format("Account Confirmed for '{0}'. You can now use Praveen Fleets", user.FullName));
            }

            _logger.LogError("Unable to confirm user with " + request.EMail + ". Identity returned error.");

            foreach (var error in identityResult.Errors)
                _logger.LogError("Error for User {0} - Code : {1}; Description : {2}", request.EMail,
                    error.Code, error.Description);

            return await ApiResponse<TokenResponse>.FailAsync(
                string.Format("An error occurred while confirming '{0}'", user.FullName),
                _logger);
        }
        catch (Exception ex)
        {
            _logger.LogCritical(ex, "Confirmation of Email of user {0} failed", request.EMail);
            return await BaseApiResponse.FatalAsync(ex, _logger);
        }

    }

    /*
    /// <summary>
    /// Reset password as user forgot password
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    public async Task<BaseApiResponse> ForgotPasswordAsync(ForgotPasswordRequest request)
    {
        try
        {
            var user = await _userManager.FindByEmailAsync(request.EMail);
            var flag = user == null;
            if (!flag)
                flag = !await _userManager.IsEmailConfirmedAsync(user);
            if (flag)
            {
                _logger.LogError("Either the user is not found or the EMail is still not confirmed for user {0}!",
                    request.EMail);
                return await BaseApiResponse.FailAsync("An Error has occurred!", _logger);
            }

            var passwordResetTokenAsync = await _userManager.GeneratePasswordResetTokenAsync(user);
            _logger.LogInformation("Password reset code for '" + request.EMail + "' is '" +
                                   passwordResetTokenAsync + "'");

            var str = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(passwordResetTokenAsync));
            var verifyURI = QueryHelpers.AddQueryString(


                QueryHelpers.AddQueryString(
                    new Uri((_appConfig.SiteUrl ?? "") + _appConfig.AccResetPath).ToString(), "user",
                    request.EMail), "token", str);

            string mailContent;
            string mailSubject;
             _mailGenerator.ForgotPwdEMailAlert(user.FullName, verifyURI, out mailContent, out mailSubject);

             if (!string.IsNullOrEmpty(mailContent))
             {
                 var request1 = new EMailRequest()
                 {
                     Body = mailContent,
                     Subject = mailSubject
                 };
                 request1.ToAddresses.Add(new EMailAddress(user.FullName, user.Email));
                 _ = await _mailService.SendEMailAsync(request1);
             }

            _logger.LogInformation("Password Reset Mail sent to use with email {0}.", request.EMail);
            return await BaseApiResponse.SuccessAsync("Password Reset Mail has been sent to your authorized Email.");
        }
        catch (Exception ex)
        {
            _logger.LogCritical(ex, "Password reset for user {0} failed", request.EMail);
            return await BaseApiResponse.FatalAsync(ex, _logger);
        }
    }
    */

    /// <summary>
    /// Reset password
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    public async Task<BaseApiResponse> ResetPasswordAsync(ResetPasswordRequest request)
    {
        try
        {
            var byEmailAsync = await _userManager.FindByEmailAsync(request.EMail);
            if (byEmailAsync == null)
            {
                _logger.LogError("User {0} is not found while resetting the password!", request.EMail);
                return await BaseApiResponse.FailAsync("Invalid User! Contact Praveen Fleets Support.", _logger);
            }

            var token = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(request.Token));
            var identityResult = await _userManager.ResetPasswordAsync(byEmailAsync, token, request.Password);

            if (identityResult.Succeeded)
            {
                _logger.LogInformation("Password of user {0} has been reset successfully.", request.EMail);
                return await BaseApiResponse.SuccessAsync("Password Reset Successful!");
            }

            _logger.LogError("Password of user {0} not Reset!", request.EMail);

            foreach (var error in identityResult.Errors)
                _logger.LogError("Error for User {0} - Code : {1}; Description : {2}", request.EMail, error.Code,
                    error.Description);
            var errors = identityResult.Errors;

            return await BaseApiResponse.FailAsync(errors.First()?.Description, _logger);
        }
        catch (Exception ex)
        {
            _logger.LogCritical(ex, "Reset password for user {0} failed!", request.EMail);
            return await BaseApiResponse.FatalAsync(ex, _logger);
        }
    }

    /// <summary>
    /// Update user password
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    public async Task<BaseApiResponse> UpdatePasswordAsync(UpdatePasswordRequest request)
    {

        try
        {
            if (request.NewPassword != request.NewPasswordConfirm)
                return await BaseApiResponse.FailAsync("Passwords do not match!", _logger);

            var user = await _userManager.FindByIdAsync(request.UserId.ToString());
            if (user == null)
            {
                var stringAndClear =
                    string.Format("The User with the ID {0} was not found!", request.UserId.ToString());
                var objArray = Array.Empty<object>();
                _logger.LogError(stringAndClear, objArray);
                return await BaseApiResponse.FailAsync("User Not Found.", _logger);
            }

            var identityResult =
                await _userManager.ChangePasswordAsync(user, request.CurrentPassword, request.NewPassword);
            if (identityResult.Succeeded)
            {
                var stringAndClear = string.Format("Password of User with the ID {0} has been rest successfully!",
                    request.UserId.ToString());
                var objArray = Array.Empty<object>();
                _logger.LogInformation(stringAndClear, objArray);
                return await BaseApiResponse.SuccessAsync("Password changed successfully!");
            }

            var stringAndClear1 = string.Format("Password of User with the ID {0} was not changed!",
                request.UserId.ToString());

            var objArray1 = Array.Empty<object>();
            _logger.LogError(stringAndClear1, objArray1);

            foreach (var error in identityResult.Errors)
                _logger.LogError("Error for User {0} - Code : {1}; Description : {2}", user.Email, error.Code,
                    error.Description);
            return await BaseApiResponse.FailAsync("An Error has occurred. Please try again!", _logger);
        }
        catch (Exception ex)
        {
            _logger.LogCritical(ex, "Updating password of user with id {0} failed", request.UserId);
            return await BaseApiResponse.FatalAsync(ex, _logger);
        }
    }

    /// <summary>
    /// Update users details
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    public async Task<BaseApiResponse> UpdateUser(UpdateProfileRequest request)
    {

        try
        {
            if (!string.IsNullOrWhiteSpace(request.PhoneNumber))
            {
                var applicationUser = _userManager.Users.FirstOrDefault(
                    (Expression<Func<ApplicationUser, bool>>)(x => x.PhoneNumber == request.PhoneNumber));
                if (applicationUser != null && !(applicationUser.Id != request.UserId))
                {
                    _logger.LogError("The phone number " + request.PhoneNumber + " is already in use by another user!");
                    return await BaseApiResponse.FailAsync(
                        string.Format("Phone number {0} is already used.", request.PhoneNumber), _logger);
                }
            }

            var byEmailAsync = await _userManager.FindByEmailAsync(request.EMail);

            if (byEmailAsync != null && !(byEmailAsync.Id != request.UserId))
            {
                _logger.LogError("The email address " + request.EMail + " is already in use by another user!");
                return await BaseApiResponse.FailAsync(string.Format("Email {0} is already used.", request.EMail),
                    _logger);
            }

            var user = await _userManager.FindByIdAsync(request.UserId.ToString());
            if (user == null)
            {
                _logger.LogError("The User with the email address " + request.EMail + " was not found!");
                return await BaseApiResponse.FailAsync("User Not Found.", _logger);
            }

            var phoneNumber = await _userManager.GetPhoneNumberAsync(user);
            var email = await _userManager.GetEmailAsync(user);
            if (request.PhoneNumber != phoneNumber)
            {
                var identityResult = await _userManager.SetPhoneNumberAsync(user, request.PhoneNumber);
                if (identityResult != null && !identityResult.Succeeded)
                {
                    var stringAndClear =
                        string.Format("Unable to set the phone number of the user {0} .", request.UserId);
                    var objArray = Array.Empty<object>();
                    _logger.LogError(stringAndClear, objArray);

                    foreach (var error in identityResult.Errors)
                        _logger.LogError("Error for User {0} - Code : {1}; Description : {2}", request.UserId,
                            error.Code, error.Description);

                    return await BaseApiResponse.FailAsync("An error occurred while updating user",
                        _logger);
                }
            }

            if (request.EMail != email)
            {
                var identityResult = await _userManager.SetEmailAsync(user, request.EMail);
                if (identityResult != null && !identityResult.Succeeded)
                {
                    var stringAndClear = string.Format("Unable to set the Email of the user {0} .", request.UserId);
                    var objArray = Array.Empty<object>();
                    _logger.LogError(stringAndClear, objArray);

                    foreach (var error in identityResult.Errors)
                        _logger.LogError("Error for User {0} - Code : {1}; Description : {2}", request.UserId,
                            error.Code, error.Description);

                    return await BaseApiResponse.FailAsync("An error occurred while updating user", _logger);
                }
            }

            user.FirstName = request.FirstName;
            user.LastName = request.LastName;
            user.UserName = request.EMail;

            var identityResult1 = await _userManager.UpdateAsync(user);

            if (identityResult1 != null && !identityResult1.Succeeded)
            {
                var stringAndClear = string.Format("Unable to update details of the user {0}.", request.UserId);
                var objArray = Array.Empty<object>();
                _logger.LogError(stringAndClear, objArray);

                foreach (var error in identityResult1.Errors)
                    _logger.LogError("Error for User {0} - Code : {1}; Description : {2}", request.UserId, error.Code,
                        error.Description);

                return await BaseApiResponse.FailAsync("An error occurred while updating user", _logger);
            }

            await _signInManager.RefreshSignInAsync(user);
            return await BaseApiResponse.SuccessAsync("User details updated!");
        }
        catch (Exception ex)
        {
            _logger.LogCritical(ex, "Updating User details of user with id {0} failed", request.UserId);
            return await BaseApiResponse.FatalAsync(ex, _logger);
        }
    }

    /// <summary>
    /// Delete User Account
    /// </summary>
    /// <param name="request">User login credentials</param>
    /// <returns></returns>
    public async Task<BaseApiResponse> RemoveAccount(LoginRequest request) =>
        await BaseApiResponse.FailAsync("Contact Support with the code NI", _logger);

    /// <summary>
    /// Get User's Count
    /// </summary>
    /// <returns></returns>
    public int GetCountAsync() => _userManager.Users.Count();

    /// <summary>
    /// Get User Data
    /// </summary>
    /// <param name="userId"></param>
    /// <returns></returns>
    public async Task<ApiResponse<UserResponse>> GetAsync(Guid userId)
    {
        var user = _userManager.Users.FirstOrDefault(u => u.Id == userId);

        if (user != null)
        {
            _cache.SetInCacheMemoryAsync(user);
        }

        var data = await ApiResponse<UserResponse>.SuccessAsync(_mapper.Map<UserResponse>(user));
        return data;
    }

    /// <summary>
    /// Get All Users
    /// </summary>
    /// <returns></returns>
    public async Task<ApiResponse<List<UserResponse>>> GetAllAsync() =>
        await ApiResponse<List<UserResponse>>.SuccessAsync(
            _mapper.Map<List<UserResponse>>(_userManager.Users.ToList()));

    /// <summary>
    /// Update user's role
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    public async Task<BaseApiResponse> UpdateRolesAsync(UpdateUserRolesRequest request)
    {
        var user = await _userManager.FindByIdAsync(request.UserId.ToString());
        var roles = await _userManager.GetRolesAsync(user);

        var selectedRoles = request.UserRoles.Where(x => x.Selected).ToList();

        if (!await _userManager.IsInRoleAsync(await _userManager.FindByIdAsync(request.UserId.ToString()), "Admin"))
            return await BaseApiResponse.FailAsync(
                "Not Allowed to add or delete Role if you have not an Administrator.", _logger);

        _ = await _userManager.RemoveFromRolesAsync(user, roles);
        _ = await _userManager.AddToRolesAsync(user, selectedRoles.Select(y => y.RoleName));

        return await BaseApiResponse.SuccessAsync("Roles Updated");
    }

    /// <summary>
    /// Resend Welcome Email
    /// </summary>
    /// <param name="emailAddress"></param>
    /// <returns></returns>
    public async Task<BaseApiResponse> RegenWelcomeEMail(string emailAddress)
    {

        try
        {
            var user = await _userManager.FindByNameAsync(emailAddress);
            if (user == null)
            {
                _logger.LogError("The User with the email address " + emailAddress + " was not found!");
                return await BaseApiResponse.FailAsync("User Not Found.", _logger);
            }

            _ = await SendWelcomeEMail(user);
            return await BaseApiResponse.SuccessAsync(string.Format(
                "EMailed User '{0}' with the Welcome EMail. Please check your Mailbox to verify!",
                user.FullName));
        }
        catch (Exception ex)
        {
            _logger.LogCritical(ex, "Registration Via email failed for user {0}", emailAddress);
            return await BaseApiResponse.FatalAsync(ex, _logger);
        }
    }
    /*
    /// <summary>
    /// Generate Referral code as per User Id
    /// </summary>
    /// <param name="userId"></param>
    /// <returns></returns>
    public async Task<string> GetReferralCode(Guid userId)
    {
        try
        {
            var curUser = await _userManager.FindByIdAsync(userId.ToString());
            if (curUser == null)
                return string.Empty;
            if (string.IsNullOrEmpty(curUser.ReferalCode))
            {
                curUser.ReferalCode = RandomString(10);
                var identityResult = await _userManager.UpdateAsync(curUser);
                if (identityResult != null && !identityResult.Succeeded)
                {
                    _logger.LogError("Unable to supdate details of the user " + curUser.FullName + ".");
                    foreach (var error in identityResult.Errors)
                        _logger.LogError("Error for User {0} - Code : {1}; Description : {2}",
                            curUser.FullName, error.Code, error.Description);
                    return string.Empty;
                }
            }

            return curUser.ReferalCode;
        }
        catch (Exception ex)
        {
            _logger.LogCritical(ex, "Referral Code for user {0} failed", userId.ToString());
            return string.Empty;
        }
    }
    */
    /// <summary>
    /// Request to change/update user's email address
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    public async Task<ApiResponse<bool>> RequestEMailAddressChange(UpdateEmailRequest request)
    {

        try
        {
            var validationResult = new UpdateEmailRequestValidator().Validate(request);

            if (!validationResult.IsValid)
            {
                // ISSUE: reference to a compiler-generated method
                return await ApiResponse<bool>.FailAsync(validationResult.Errors.Select(x => x.ErrorMessage).ToList(),
                    _logger);
            }

            var user = await _userManager.FindByEmailAsync(request.OldEmailAddress);

            if (user == null)
                return await ApiResponse<bool>.FailAsync("User Not Found", _logger);

            if (await _userManager.FindByEmailAsync(request.NewEmailAddress) != null)
                return await ApiResponse<bool>.FailAsync("Email already exists!", _logger);

            var verificationCode = new Random().Next(10000, 100000).ToString();
            foreach (var claim in (await _userManager.GetClaimsAsync(user)).Where<Claim>(x =>
                         x.Type == "EmailConfirmationCode"))
            {
                var identityResult = await _userManager.RemoveClaimAsync(user, claim);
                if (identityResult is not { Succeeded: true })
                {
                    return await ApiResponse<bool>.FailAsync("Unable to update EMail Address", _logger);
                }
            }

            var claim1 = new Claim("EmailConfirmationCode", verificationCode);
            var identityResult1 = await _userManager.AddClaimAsync(user, claim1);
            if (identityResult1 is not { Succeeded: true })
            {
                return await ApiResponse<bool>.FailAsync("Unable to update EMail Address", _logger);
            }

            var request1 = new EMailRequest()
            {
                Body = "Verification Code " + verificationCode,
                Subject = "Verification code"
            };

            request1.ToAddresses.Add(new EMailAddress(user.FullName, request.NewEmailAddress));

            return await ApiResponse<bool>.SuccessAsync(await _mailService.SendEMailAsync(request1));
        }
        catch (Exception ex)
        {
            return await ApiResponse<bool>.FatalAsync(ex, _logger);
        }
    }

    /// <summary>
    /// update email address confirmation
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    public async Task<ApiResponse<bool>> ConfirmEMailAddressChange(UpdateEmailRequest request)
    {
        try
        {
            var validationResult = new UpdateEmailRequestValidator().Validate(request);

            if (!validationResult.IsValid)
            {
                // ISSUE: reference to a compiler-generated method
                return await ApiResponse<bool>.FailAsync(validationResult.Errors, _logger);
            }

            var user = await _userManager.FindByEmailAsync(request.OldEmailAddress);

            if (user == null)
                return await ApiResponse<bool>.FailAsync("Customer Not Found", _logger);

            if (await _userManager.FindByEmailAsync(request.NewEmailAddress) != null)
                return await ApiResponse<bool>.FailAsync("Email already Exists", _logger);

            var emailConfClaim =
                (await _userManager.GetClaimsAsync(user)).FirstOrDefault(x => x.Type == "EmailConfirmationCode");

            if (emailConfClaim == null)
                return await ApiResponse<bool>.FailAsync("Unable to update EMail Address", _logger);

            if (!string.Equals(emailConfClaim.Value, request.VerificationCode))
                return await ApiResponse<bool>.FailAsync("Verification Code is Incorrect", _logger);

            user.EmailConfirmed = true;
            user.Email = request.NewEmailAddress;
            user.UserName = request.NewEmailAddress;
            user.NormalizedUserName = _userManager.NormalizeName(request.NewEmailAddress);
            user.NormalizedEmail = _userManager.NormalizeEmail(request.NewEmailAddress);

            var identityResult1 = await _userManager.UpdateAsync(user);

            if (identityResult1 is not { Succeeded: true })
            {
                return await ApiResponse<bool>.FailAsync("Unable to update EMail Address", _logger);
            }

            var identityResult2 = await _userManager.RemoveClaimAsync(user, emailConfClaim);
            if (identityResult2 is not { Succeeded: true })
            {
                return await ApiResponse<bool>.FailAsync("Unable to update EMail Address", _logger);
            }

            var request1 = new EMailRequest
            {
                Body = "Email Updated",
                Subject = "Email Updated"
            };
            request1.ToAddresses.Add(new EMailAddress(user.FullName, request.OldEmailAddress));
            request1.ToAddresses.Add(new EMailAddress(user.FullName, request.NewEmailAddress));

            _ = await _mailService.SendEMailAsync(request1);
            return await ApiResponse<bool>.SuccessAsync();
        }
        catch (Exception ex)
        {
            return await ApiResponse<bool>.FatalAsync(ex, _logger);
        }
    }

    /// <summary>
    /// Check if User exists
    /// </summary>
    /// <param name="email">EmailId of user</param>
    /// <returns></returns>
    public async Task<ApiResponse<bool>> UserExists(string email)
    {

        var user = await _userManager.FindByEmailAsync(email);
        var userExists = user != null;

        return await ApiResponse<bool>.SuccessAsync(userExists);
    }

    /// <summary>
    /// check if email is valid
    /// </summary>
    /// <param name="email"></param>
    /// <returns></returns>
    private static Task<bool> IsValidAsync(string email)
    {
        try
        {
            return CheckDnsEntriesAsync(new MailAddress(email).Host);
        }
        catch
        {
            return Task.FromResult(false);
        }
    }

    /// <summary>
    /// check if email is valid
    /// </summary>
    /// <param name="domain"></param>
    /// <returns></returns>
    private static async Task<bool> CheckDnsEntriesAsync(string domain)
    {
        try
        {
            var options = new LookupClientOptions(IPAddress.Parse("8.8.8.8"), IPAddress.Parse("8.8.4.4"))
            {
                Timeout = TimeSpan.FromSeconds(5.0)
            };

            var dnsQueryResponse = await new LookupClient(options)
                .QueryAsync(domain.ToLower(), QueryType.MX)
                .ConfigureAwait(false);

            return dnsQueryResponse is { Answers.Count: > 0 };
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// Send Email confirmation code
    /// </summary>
    /// <param name="user"></param>
    /// <returns></returns>
    private async Task<bool> SendConfirmEMailCode(ApplicationUser user)
    {
        var verificationUrl = await GenerateVerificationUrl(user);

        _mailGenerator.NewRegistrationViaEMail(user.FullName, verificationUrl, out var mailContent, out var mailSubject);
        
        if (mailContent == string.Empty) return false;
        var request = new EMailRequest
        {
            Body = mailContent,
            Subject = mailSubject
        };
        request.ToAddresses.Add(new EMailAddress(user.FullName, user.Email));
        _ = await _mailService.SendEMailAsync(request);

        return true;
    }

    /// <summary>
    /// Generate Url to verify new user's Email Id's
    /// </summary>
    /// <param name="user"></param>
    /// <returns>New Url</returns>
    private async Task<string> GenerateVerificationUrl(ApplicationUser user)
    {
        var str =
            WebEncoders.Base64UrlEncode(
                Encoding.UTF8.GetBytes(await _userManager.GenerateEmailConfirmationTokenAsync(user)));
        _logger.LogInformation("EMail verification code for " + user.Email + " is " + str);

        return QueryHelpers.AddQueryString(QueryHelpers.AddQueryString(new Uri((_appConfig.SiteUrl ?? "") + _appConfig.AccConfirmPath).ToString(), "userEmail",user.Email), "token", str);
    }

    /// <summary>
    ///  Regenerate Token
    /// </summary>
    /// <returns></returns>
    private static string GenerateRefreshToken()
    {
        var numArray = new byte[32];
        using var randomNumberGenerator = RandomNumberGenerator.Create();
        randomNumberGenerator.GetBytes(numArray);
        return Convert.ToBase64String(numArray);
    }

    /// <summary>
    /// Generate JWT Authentication token
    /// </summary>
    /// <param name="user">The user whose token is generated</param>
    /// <returns>Encrypted JWT Authentication Token</returns>
    private async Task<string> GenerateJwtAsync(ApplicationUser user)
    {
        var signingCredentials = GetSigningCredentials();
        var claimsAsync = await GetClaimsAsync(user);
        var encryptedToken = GenerateEncryptedToken(signingCredentials, claimsAsync);
        return encryptedToken;
    }

    /// <summary>
    /// Generate JWT Authentication token
    /// </summary>
    /// <param name="signingCredentials"></param>
    /// <param name="claims">Claims required for the user</param>
    /// <returns>Encrypted JWT Authentication Token</returns>
    private string GenerateEncryptedToken(
        SigningCredentials signingCredentials,
        IEnumerable<Claim> claims)
    {
        var jwtIssuer = _appConfig.JwtIssuer;
        var jwtAudience = _appConfig.JwtAudience;
        var expires = DateTime.UtcNow.AddHours(_appConfig.JwtExpiryInHours);
        var notBefore = new DateTime?();

        return new JwtSecurityTokenHandler().WriteToken(new JwtSecurityToken(jwtIssuer, jwtAudience,
            claims, notBefore, expires, signingCredentials));
    }

    /// <summary>
    /// Generate signing credentials
    /// </summary>
    /// <returns>Signing Credentials</returns>
    private SigningCredentials GetSigningCredentials() => new(
        new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_appConfig.JwtSecurityKey)), "HS256");

    /// <summary>
    /// Get all claims of the user
    /// </summary>
    /// <param name="user"></param>
    /// <returns>List of Claims</returns>
    private async Task<IEnumerable<Claim>> GetClaimsAsync(ApplicationUser user)
    {
        IList<Claim> userClaims = await _userManager.GetClaimsAsync(user);
        IList<string> rolesAsync = await _userManager.GetRolesAsync(user);
        var roleClaims = new List<Claim>();
        var permissionClaims = new List<Claim>();

        foreach (var roleName in rolesAsync)
        {
            roleClaims.Add(new Claim("http://schemas.microsoft.com/ws/2008/06/identity/claims/role", roleName));
            var byNameAsync = await _roleManager.FindByNameAsync(roleName);
            permissionClaims.AddRange(await _roleManager.GetClaimsAsync(byNameAsync));
        }


        var claimsAsync = new List<Claim>
            {
                new("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier", user.Id.ToString()),
                new("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress", user.Email!),
                new("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name", user.FirstName),
                new("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/surname", user.LastName),
                new("http://schemas.xmlsoap.org/ws/2009/09/identity/claims/actor", user.UserType.ToDescriptionString()),
            }.Union(userClaims)
            .Union(roleClaims)
            .Union(permissionClaims);

        return claimsAsync;
    }

    /// <summary>
    /// Send Welcome email when user confirms their email
    /// </summary>
    /// <param name="user">"User" object</param>
    /// <returns>boolean value</returns>
    private async Task<bool> SendWelcomeEMail(ApplicationUser user)
    {
        var mailContent = string.Empty;
        var mailSubject = string.Empty;

        //_mailGenerator.WelcomeEmail(user.FullName, _appConfig.SiteUrl, out mailContent,
        //  out mailSubject);

        if (string.IsNullOrEmpty(mailContent)) return false;

        var request = new EMailRequest
        {
            Body = mailContent,
            Subject = mailSubject
        };

        request.ToAddresses.Add(new EMailAddress(user.FullName, user.Email));
        request.CcAddresses.Add(new EMailAddress("Support", _appConfig.SupportMailAddress));
        _ = await _mailService.SendEMailAsync(request);
        return true;
    }

}