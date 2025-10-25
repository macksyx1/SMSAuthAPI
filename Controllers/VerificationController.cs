using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Twilio;
using Twilio.Rest.Verify.V2.Service;

namespace OTPService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VerificationController : ControllerBase
    {
        private readonly IConfiguration _config;

        public VerificationController(IConfiguration config)
        {
            _config = config;
        }

        [HttpPost("send")]
        public IActionResult SendVerification([FromBody] string phoneNumber)
        {
            try
            {
                TwilioClient.Init(
                    _config["Twilio:AccountSid"],
                    _config["Twilio:AuthToken"]
                );

                var verification = VerificationResource.Create(
                    to: phoneNumber,
                    channel: "sms",
                    pathServiceSid: _config["Twilio:VerifyServiceSid"]
                );

                return Ok(new { verification.Status });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpPost("check")]
        public IActionResult VerifyCode([FromBody] VerificationRequest request)
        {
            try
            {
                TwilioClient.Init(
                    _config["Twilio:AccountSid"],
                    _config["Twilio:AuthToken"]
                );

                var verificationCheck = VerificationCheckResource.Create(
                    to: request.PhoneNumber,
                    code: request.Code,
                    pathServiceSid: _config["Twilio:VerifyServiceSid"]
                );

                return Ok(new { verificationCheck.Status });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }
    }

    public class VerificationRequest
    {
        public string PhoneNumber { get; set; }
        public string Code { get; set; }
    }
}

