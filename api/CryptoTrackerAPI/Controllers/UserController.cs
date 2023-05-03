using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using CryptoTrackerAPI.Data;
using CryptoTrackerAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace CryptoTrackerAPI.Controllers;
public class CancelTradeRequest
{
        public string UserId{get;set;}=string.Empty;
        public string TradeId{get;set;}=string.Empty;

}
public class CreateAccountRequest
{
        public string Currency{get;set;}
        public string UserId{get;set;}

        
}
public class LogoutRequest
{
        public string Email{get;set;}=string.Empty;
        public string Id{get;set;}=string.Empty;
}
public class TradeOpenRequest
{
        public string UserId{get;set;}=string.Empty;
        public string AccountId{get;set;}=string.Empty;
        public string MarketId{get;set;}=string.Empty;
        public long Leverage {get;set;}=new long();
        public double Amount{get;set;}=new double();
        public double StopLoss{get;set;}=new double();
        public double TakeProfit{get;set;}=new double();
}
[AllowAnonymous]
[Route("/api/v2/users/")]
public class UserController:ControllerBase
{
        readonly IConfiguration configuration;
        readonly CryptoTrackerDB database;
        public UserController(IConfiguration _config,CryptoTrackerDB db)
        {
                configuration=_config;
                database=db;
        }
        [HttpGet("user-activate/{id}")]
        [Authorize(Roles="ADMIN,STAFF")]
        public ActionResult<bool> ActivateUser([FromQuery] string id)
        {
                User user=database.Users.Where(x=>x.Id==id).FirstOrDefault();
                if(user!=null){
                user.Active=true;
        
                database.Users.Update(user);
                return Ok("User Deactivated");
                }
                return BadRequest($"User of id {id} doesn't exists ");
     
        } 
        [HttpGet("user-deactivate/{id}")]
        [Authorize(Roles="ADMIN,STAFF")]
        public ActionResult<bool> DeactivateUser([FromQuery] string id)
        {
                User user=database.Users.Where(x=>x.Id==id).FirstOrDefault();
                if(user!=null){
                user.Active=false;
                database.Users.Update(user);
                return Ok("User Deactivated");
                }
                return BadRequest($"User of id {id} doesn't exists ");

        }
        [HttpPost("user-closealltrades/{id}")]
        [Authorize(Roles="ADMIN,STAFF")]
        public ActionResult<bool> CloseAllUserTrades([FromBody] string userid)
        {
                User user=database.Users.Where(x=>x.Id==userid).FirstOrDefault();
                if(user!=null)
                {
                        foreach(string tradeid in user.Trades)
                        {
                                Trade trade=database.Trades.Where(x=>x.Id==tradeid).FirstOrDefault();
                                trade.Active=false;
                                trade.ClosingTime=DateTime.Now;
                                user.Trades.Remove(tradeid);
                                database.Trades.Update(trade);
                                database.Users.Update(user);
                                database.SaveChanges();

                        }
                        return Ok(true);
                }

                return Ok(false);
        }
        [HttpPost("user-opentrade")]
        [Authorize(Roles="USER")]
        public ActionResult<List<Trade>> OpenTrade([FromBody] TradeOpenRequest request)
        {
                User user=database.Users.Where(x=>x.Id==request.UserId).FirstOrDefault();
                if(user!=null)
                {
                        if(user.Accounts.Contains(request.AccountId))
                        {
                                Account account=database.Accounts.Where(x=>x.Id==request.AccountId).FirstOrDefault();
                                account.Balance-=request.Amount;
                                if (!(account.Balance>=0))
                                {
                                        account.Balance+=request.Amount;
                                        return BadRequest("Insufficient Amount");
                                }
                                else 
                                {
                                        Trade trade=new Trade();
                                        trade.Id=Guid.NewGuid().ToString();
                                        trade.Active=true;
                                        trade.AccountId=account.Id;
                                        trade.Leverage=request.Leverage;
                                        trade.StopLoss=request.StopLoss;
                                        trade.TakeProfit=request.TakeProfit;
                                        user.Trades.Add(trade.Id);
                                        database.Accounts.Update(account);
                                        database.Trades.Add(trade);
                                        database.Users.Update(user);
                                        database.SaveChanges();
                                        return Ok(trade);
                                }

                        }
                }
                return BadRequest($"user of id {request.UserId} does not exists");
        }
      
        [HttpPost("user-closetrade")]
        [Authorize(Roles="USER")]
        public ActionResult<List<Trade>> CloseTrade([FromBody] CancelTradeRequest cancelrequest )
        {
                User user=database.Users.Where(x=>x.Trades.Where(trade=>trade==cancelrequest.UserId).FirstOrDefault().Count()!=0&&x.Id==cancelrequest.UserId).FirstOrDefault();
                Trade trade=database.Trades.Where(x=>x.Id==cancelrequest.TradeId).FirstOrDefault();
                if(user!=null)
                {
                        trade.Active=false;
                        trade.ClosingTime=DateTime.Now;
                        user.Trades.Remove(cancelrequest.TradeId);
                        database.Trades.Update(trade);
                        database.Users.Update(user);
                        database.SaveChanges();


                }
                return BadRequest("User Does Not Exist");
        }

        [HttpPost("create-account")]
        public ActionResult<Account> CreatAccount([FromBody] CreateAccountRequest request)
        {
                
                User user=database.Users.Where(x=>x.Id==request.UserId).FirstOrDefault();
                if(user!=null)
                {
                        Account account =new Account();
                        account.Id= Guid.NewGuid().ToString();
                        account.Currency=request.Currency;
                        account.Bonus=00.0;
                        account.Balance=0.0;
                        account.ActiveTradesId=new List<string>();
                        user.Accounts.Add(account.Id);
                        database.Users.Update(user);
                        database.Accounts.Add(account);
                        database.SaveChanges();
                        return account;

                }
                return Ok($"user of id {request.UserId} not found");
        }
        [HttpPost("user/")]
        [Authorize(Roles="USER")]
        public ActionResult<User> GetUser([FromQuery] string id)
        {
                User user=database.Users.Where(x=>x.Id==id).FirstOrDefault();
                if(user!=null)
                {
                        return Ok(user);
                }
                 return Ok($"user  with id {id} does not exists");
        }
        
}
public class UserLoginDTO
{
        public string Email{get;set;}=string.Empty;
        public string Password{get;set;}=string.Empty;

}
public class RegisterUser
{
        public string Email{get;set;}=string.Empty;
        public string FirstName{get;set;}=string.Empty;
        public string LastName{get;set;}=string.Empty;
        public string Password{get;set;}=string.Empty;
        public string NationalId {get;set;}=string.Empty;
        public string Address{get;set;}=string.Empty;
        public string Phone{get;set;}=string.Empty;
}
[ApiController]
[Route("api/v2/auth/")]
public class AuthenticationController:ControllerBase
{
        User user;
        readonly CryptoTrackerDB database;
        readonly IConfiguration configuration;
        public AuthenticationController(IConfiguration config_,CryptoTrackerDB db_)
        {
                configuration=config_;
                database=db_;
                Console.WriteLine($"{nameof(AuthenticationController)} has been initialized");
        }
        [HttpPost("login/")]
        public ActionResult<User> Login([FromBody] UserLoginDTO logindto)
        {
                User user;
                user=database.Users.Where(x=>x.Email==logindto.Email).FirstOrDefault();
                if(user!=null)
                {
                        if (!(BCrypt.Net.BCrypt.Verify(logindto.Password,user.PasswordHarsh)))
                                {
                                
                                        return BadRequest("Invalid Email or Password");

                                }
                        user.Online=true;
                        user.LastSeen=DateTime.Parse(DateTime.Now.ToString("dd-MM-yyyy"));
                        database.Users.Update(user);
                        database.SaveChanges();
                        Response.Headers.Add("token",CreateUserToken(user));
                        return Ok(user);
                }
                return Ok("user with email does not exists");
                
        }
        [HttpPost("logout/")]
        [Authorize(Roles="USER")]
         public ActionResult<string> Logout([FromBody] LogoutRequest request)
        {
                User user=database.Users.Where(x=>x.Id==request.Id&& x.Email==request.Email).FirstOrDefault();
                if(user!=null)
                {
                        user.LastSeen=DateTime.Parse(DateTime.Now.ToString("dd-MM-yyyy"));
                        user.Online=false;
                        database.Users.Update(user);
                        return Ok("Logged out successfully");    
                }
                return BadRequest($"user id {request.Id} doesn't exists");
                
        }
        [HttpPost("register/")]
        public ActionResult<User> CreateUser([FromBody] RegisterUser userdto)
        {
                if(database.Users.Where(x=>x.Email==userdto.Email).Count()==0)
                {
                        User user=new User();
                        user.FirstName=userdto.FirstName;
                        user.LastName=userdto.LastName;
                        user.PasswordHarsh=BCrypt.Net.BCrypt.HashPassword(userdto.Password);
                        
                        user.Id=Guid.NewGuid().ToString();
                        user.Address=userdto.Address;
                        user.Email=userdto.Email;
                        user.Accounts=new List<string>();
                        user.CreatedDate=DateTime.Parse(DateTime.Now.ToString("dd-MM-yyyy"));
                        user.ImageUrl="";
                        user.NationalNumber=userdto.NationalId;
                        user.Phone=userdto.Phone;
                        user.Role="USER";
                        user.Trades=new List<string>();
                        

                        //Add feafult account for user
                        Account defaultacc=new Account();
                        defaultacc.Currency="USD";
                        defaultacc.Balance=0.0;
                        defaultacc.Id=Guid.NewGuid().ToString();
                        user.Accounts.Add(defaultacc.Id);
                        //Add User to database
                        database.Users.Add(user);
                        //Add Account to database
                        database.Accounts.Add(defaultacc);
                        database.SaveChanges();
                        //Generate session token
                        string token=CreateUserToken(user);
                        Response.Headers.Add("token",token);
                        return Ok(user);
                }
                return BadRequest("Email already exists");
        }
        [Authorize(Roles="USER,ADMIN")]
        
        private string CreateUserToken(User user)
        {
			List<Claim> claims = new List<Claim>() {
			new Claim(ClaimTypes.Name,user.FirstName+"\t"+user.LastName)
			,new Claim(ClaimTypes.Email,user.Email),
			new Claim(ClaimTypes.Expiration,DateTime.Now.AddDays(1).ToString()),
			new Claim(ClaimTypes.StreetAddress,user.Address),
			new Claim(ClaimTypes.Authentication,"customer"),
			new Claim(ClaimTypes.AuthenticationMethod,"jwt"),
			new Claim(ClaimTypes.Role,user.Role),
			new Claim(ClaimTypes.MobilePhone,user.Phone),
				new Claim(ClaimTypes.Hash,user.PasswordHarsh),
			new Claim(ClaimTypes.Locality,"EN")
			};
			var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration.GetSection("AppSettings:Token").Value!));

			var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature);
			var jwt = new JwtSecurityToken(
				claims: claims
				, expires: DateTime.Now.AddDays(1),
				signingCredentials: credentials);

			var token = new JwtSecurityTokenHandler().WriteToken(jwt);
                        return token;
        }

}