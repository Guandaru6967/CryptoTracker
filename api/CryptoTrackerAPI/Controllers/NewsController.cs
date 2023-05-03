using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CryptoTrackerAPI.Models;
using  CryptoTrackerAPI.Data;
namespace CryptoTrackerAPI.Controllers;
public class NewsallRequest
{
        public int count { get; set; } = 20;
        public string category { get; set; } = string.Empty;
        public string language { get; set; } = string.Empty;
}


[Route("/api/v2/news")]
[ApiController]
public class NewsController : ControllerBase
{
        readonly CryptoTrackerDB database;
        readonly IConfiguration configuration;
        readonly NewsService NewsService;
        HttpClient client = new HttpClient();
       
        public DateTime lastfetch { get; set; }
        
        public  NewsController(CryptoTrackerDB _database, IConfiguration _configation, NewsService _newsservice)
        {
                database = _database;
                configuration = _configation;
                NewsService = _newsservice;
                if(NewsService.News.Count==0)
                {
                NewsService.FetchNews();
                }

        }
        [HttpPost("all/")]
        public async Task<ActionResult<IEnumerable<NewsData>>> GetAllNews([FromBody] NewsallRequest newsrequest)
        {
                //database.Add("");
                System.Diagnostics.Debug.WriteLine("Fetched!");
                if(NewsService.News.Count==0)
                {
                        if(NewsService.FetchNews().Result)
                        {
                                foreach(var news in NewsService.News)
                                {
                                        database.News.Add(news);
                                        database.SaveChanges();
                                }
                                
                                Console.WriteLine($"News Count:{NewsService.News.Count} {NewsService.News.Count==0}");
                                return Ok(NewsService.News);

                        }
                        else 
                        {
                                return Ok("Failed to Fetch news Data");}
                }
                return Ok("Server Error");
               


        }

        [HttpPost("category/")]
        public async Task<ActionResult<IEnumerable<NewsData>>> GetNewsData([FromBody] string category)
        {
                if(NewsService.News.Count==0)
                {
                        await NewsService.FetchNews();
                }
                return Ok(NewsService.News.Where(x=>x.category.Contains(category) ||category.Contains(category.ToString().ToLower())));

        }

        
}