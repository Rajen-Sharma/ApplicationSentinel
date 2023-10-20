using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Security.Claims;


namespace ApplicationSentinel.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ApplicationSecretsController : ControllerBase
    {
        private readonly ILogger<ApplicationSecretsController> _logger;

        private readonly IHttpContextAccessor _httpContextAccessor;

        public static IConfiguration _configuration;

        public ApplicationSecretsController(ILogger<ApplicationSecretsController> logger, IHttpContextAccessor httpContextAccessor, IConfiguration config)
        {
            _configuration = config;
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
            if (GlobalData.ConnectionString == "")
            {
                GlobalData.ConnectionString = SqlHelper.GetDBConnectionString();
            }
        }

        [HttpGet("GenerateEncryptionKey")]
        public ActionResult GenerateEncryptionKey()
        {
            try
            {
                var hostNameOrIP = Dns.GetHostEntry(_httpContextAccessor.HttpContext.Connection.RemoteIpAddress.ToString()).HostName;
                var userName = "";
                try
                {
                    userName = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
                }
                catch (Exception ex) { }

                string encryptionKey = DataEncryptor.GenerateAESKey(hostNameOrIP, userName);


                return Ok(encryptionKey);
            }
            catch (Exception ex)
            {
                return StatusCode(400, ex.Message);
            }
        }

        [HttpGet("EncryptApplicationSecret")]
        public ActionResult EncryptString(string key, string str)
        {
            try
            {
                var text = DataEncryptor.EncryptString(key, str);
                return Ok(DataEncryptor.EncryptString("XMOPZRBLN5VC1NU3", text));
            }
            catch (Exception ex)
            {
                return StatusCode(400, ex.Message);
            }
        }



        [HttpGet("DecryptApplicationSecret")]
        public ActionResult DecryptString(string key, string str)
        {
            try
            {
                var hostNameOrIP = Dns.GetHostEntry(_httpContextAccessor.HttpContext.Connection.RemoteIpAddress.ToString()).HostName;
                var userName = "";
                try
                {
                    userName = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
                }
                catch (Exception ex) { }

                string text = DataEncryptor.DecryptString("XMOPZRBLN5VC1NU3", str, "127.0.0.1", "");
                return Ok(DataEncryptor.DecryptString(key, text, hostNameOrIP, userName));
            }
            catch (Exception ex)
            {
                return StatusCode(400, ex.Message);
            }
        }

    }
}