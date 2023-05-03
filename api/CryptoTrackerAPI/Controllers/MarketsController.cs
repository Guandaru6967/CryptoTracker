
using CryptoTrackerAPI.Models;
using Microsoft.AspNetCore.SignalR;
using System.Text.Json;
using  CryptoTrackerAPI.Data;

namespace CryptoTrackerAPI.Controllers;

public class MarketHub : Hub
{
               readonly CryptoTrackerDB database;
               readonly IConfiguration configuration;
               public List<Market> Markets = new List<Market>();
               readonly MarketService marketservice;
               public override async Task OnConnectedAsync()
               {

                              Console.WriteLine("Connection Made!");
                              System.Diagnostics.Debug.WriteLine("Connection made!");
                              await ConnectedMessage();

                              //Clients.All.SendAsync("markets",);
                              await SendMarkets();
                              await base.OnConnectedAsync();

               }
               public MarketHub(CryptoTrackerDB _database, IConfiguration _configuration, MarketService _market)
               {

                              configuration = _configuration;
                              database = _database;
                              marketservice = _market;
                              Console.WriteLine("Hub Initialized");
                              if (marketservice?.MarketData?.Count <= 0|| marketservice.MarketData == null)
                              {

                                             marketservice.InitWebsocket();
                                             Console.WriteLine("Websocket Initialized");
                              }
                              Thread thread = new Thread(() =>
                              {
                                             while (marketservice?.MarketData.Count >= 0|| marketservice?.MarketData==null)
                                             {
                                                            marketservice.InitWebsocket();
                                                            foreach (WebsocketData data in marketservice?.MarketData)
                                                            {
                                                                           //
                                                                           Market market = new Market();
                                                                           market.Price.Add("BUY", Convert.ToInt64(data.bids[0][0]));
                                                                           market.Price.Add("SELL", Convert.ToInt64(data.asks[0][0]));
                                                                           //
                                                                           string [] symbol=data.product_id.Split("-");
                                                                           market.Base = symbol[0];
                                                                           market.Quote =symbol[1];
                                                                           market.Symbol = market.Base + market.Quote;
                                                                           market.BaseImageUrl = "";
                                                                           Markets.Add(market);

                                                            }
                                             }
                              });

               }
               private async Task  SendMarkets()
               {
                              string marketsstring=JsonSerializer.Serialize(marketservice.MarketData.ToList());
                              await Clients.All.SendAsync("markets",marketsstring);
               
               }
               private async Task SendMarket(string SYMBOL,string user)
               {
                              Market market=Markets.Where(x=>x.Symbol==SYMBOL).FirstOrDefault();
                              await Clients.Caller.SendAsync("ReceiveMarket",JsonSerializer.Serialize(market));
                
        
               }
               private  async Task ConnectedMessage(string userid)
               {
                              //Context.GetHttpContext().Request.Body.ReadAsync(,0,1024);
                              User user=database.Users.Where(x => x.Id == userid).FirstOrDefault();
                              user.SocketId = Guid.NewGuid().ToString();
                              database.Users.Update(user);
                              await Clients.All.SendAsync("ConnectedMessage", user.SocketId, "Connected Successfully");
               }
}
