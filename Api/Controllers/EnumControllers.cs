using Enums.Employee;
using Microsoft.AspNetCore.Mvc;
using Shared.Configuration;
using Shared.Responses.Common;
using Utility.Extensions;

namespace Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class EnumController : ControllerBase
{
    [HttpGet("Gender")]
    public async Task<ApiResponse<List<EnumResponse>>> GetGenderEnums()
        => await EnumExtensions.GetAllEnums<Gender>();

    [HttpGet("Salutation")]
    public async Task<ApiResponse<List<EnumResponse>>> GetSalutationEnums()
        => await EnumExtensions.GetAllEnums<Salutation>();

    [HttpGet("WeekDays")]
    public async Task<ApiResponse<List<EnumResponse>>> GetWeekDaysEnums()
        => await EnumExtensions.GetAllEnums<WeekDays>();

}