using AenonSrp.Application.Interfaces.Services.GlobalSetting;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AenonSrp.Server.Controllers.GlobaleSettings
{

    [Authorize]
    [Route("api/GlobaleSettings/globalSetting")]
    [ApiController]
    public class GlobalSettingController : ControllerBase
    {
        private readonly IGlobalSettingService _globalSetting;
        public GlobalSettingController(IGlobalSettingService globalSetting)
        {
            _globalSetting = globalSetting;
        }

        [AllowAnonymous]
        [HttpGet("getGlobalSettingValueByName/{globalSettingName}")]
        public async Task<ActionResult> GetGlobalSettingValueByName(string globalSettingName)
        {
            var globalSettingResponse = _globalSetting.GetGlobalSettingValueByName(globalSettingName);
            return Ok(globalSettingResponse);
        }

        [AllowAnonymous]
        [HttpPut("updateIactivityTime")]
        public async Task<IActionResult> UpdateIactivityTime(int inactivityTime)
        {
            return Ok(_globalSetting.UpdateIactivityTime(inactivityTime));
        }
    }
}
