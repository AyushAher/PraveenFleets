using Enums.Employee;
using Enums.Trips;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.Configuration;
using Shared.Responses.Common;
using static Utility.Extensions.EnumExtensions;

namespace Api.Controllers;

[Route("api/[controller]")]
[ApiController]

public class EnumController : ControllerBase
{

    [HttpGet("Gender")]
    public async Task<ApiResponse<List<EnumResponse>>> GetGenderEnums()
        => await GetAllEnums<Gender>();

    [HttpGet("Salutation")]
    public async Task<ApiResponse<List<EnumResponse>>> GetSalutationEnums()
        => await GetAllEnums<Salutation>();

    [HttpGet("WeekDays")]
    public async Task<ApiResponse<List<EnumResponse>>> GetWeekDaysEnums()
        => await GetAllEnums<WeekDays>();
    
    [HttpGet("VehicleTypes")]
    public async Task<ApiResponse<List<EnumResponse>>> GetVehicleTypesEnums()
        => await GetAllEnums<VehicleTypes>();

}