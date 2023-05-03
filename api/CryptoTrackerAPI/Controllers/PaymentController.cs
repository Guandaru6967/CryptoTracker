using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CryptoTrackerAPI.Models;
using CryptoTrackerAPI.Data;
namespace CryptoTrackerAPI.Controllers;
public class DepositRequest
{
        public string UserId{get;set;}=string.Empty;
        public string PasswordHarsh{get;set;}=string.Empty;
        public string TransactionId{get;set;}=string.Empty;
        public double Amount{get;set;}=new double();
        public string AccountId{get;set;}=string.Empty;

}
public class WithDrawRequest
{
        public string AccountId{get;set;}=string.Empty;
        public string UserId{get;set;}=string.Empty;
        public double Amount=new double();
}
[Authorize]
[ApiController]
[Route("api/v2/payments")]
public class PaymentController:ControllerBase
{
        readonly IConfiguration configuration;
        readonly CryptoTrackerDB database;
        public PaymentController(IConfiguration _configuration,CryptoTrackerDB _database)
        {
                configuration=_configuration;
                database=_database;
        }
        [HttpPost("deposit")]
        [Authorize(Roles="USER")]
        public  ActionResult<String> Deposit([FromBody]DepositRequest request)
        {
                User user=database.Users.Where(x=>x.Id==request.AccountId).FirstOrDefault();
                if(user!=null)
                {

                }
                return Ok("User Id does not exist.");

        }
        [HttpPost("withdraw")]
        public  ActionResult<String> Withdraw(string name)
        {
                
                return Ok("Withdrawal Successfull. ");
        }

}