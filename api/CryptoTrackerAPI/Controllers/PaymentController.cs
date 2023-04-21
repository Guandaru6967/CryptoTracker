using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

public class DepositRequest
{
        public string Id{get;set;}=string.Empty;
        public string PasswordHarsh{get;set;}=string.Empty;
        public double Amount{get;set;}=new double();
        public string AccountId{get;set;}=string.Empty;

}
[Authorize]
[ApiController]
[Route("api/v2/payments")]
public class PaymentController:ControllerBase
{
        readonly IConfiguration configuration;
        public PaymentController(IConfiguration _configuration)
        {
                configuration=_configuration;
        }
        [HttpPost("deposit")]
        public  ActionResult<String> Deposit(string name)
        {
                
                return Ok("Deposti Successfull .");

        }
        [HttpPost("withdraw")]
        public  ActionResult<String> Withdraw(string name)
        {
                
                return Ok("Withdrawal Successfull. ");
        }

}