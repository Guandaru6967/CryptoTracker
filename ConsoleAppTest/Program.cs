using Microsoft.AspNetCore.Http.Connections;
using Microsoft.AspNetCore.SignalR.Client;
using RestSharp;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Text.Json;
using Websocket.Client;

public class Market
{
               public string Id { get; set; } = string.Empty;
               public string Base { get; set; } = string.Empty;
               public string Quote { get; set; } = string.Empty;
               public Dictionary<string, double> Price { get; set; } = new Dictionary<string, double>();
               public string Symbol { get; set; } = string.Empty;
               public string SymbolUrl { get; set; } = string.Empty;
               public string BaseImageUrl { get; set; } = string.Empty;
               public string QuoteImageUrl { get; set; } = string.Empty;

}

public class ConnectionService
{
               public bool IsConnected { get => connection?.State == HubConnectionState.Connected; }
               HubConnection connection;

               public ConnectionService()
               {
                              Console.WriteLine("Connecting");
                              connection = new HubConnectionBuilder().WithUrl(new Uri("http://localhost:5051/Markets"), HttpTransportType.WebSockets,
                                             options => options.HttpMessageHandlerFactory = _ =>
                                                            new HttpClientHandler
                                                            {
                                                                           ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
                                                            }

                                             )
                                             .WithAutomaticReconnect().Build();

                              InitConnection();
                              Console.WriteLine(IsConnected);
               }
               public async void InitConnection()
               {
                              Console.WriteLine(1);
                              connection.On<string, string>("ConnectedMessage", (id, message) =>
                              {
                                             Console.WriteLine($"My id is {id} and message is {message}");
                              });
                              Console.WriteLine(2);
                              connection.On<string>("markets", (market) =>
                             {
                                            Console.WriteLine($"markets received {market}");
                                            connection.InvokeAsync("SendMarkets");
                             });
                              Thread threading = new Thread(() =>
                              {
                                             while (connection.State != HubConnectionState.Connected)
                                             {
                                                            //Console.WriteLine();
                                             }
                                             Console.WriteLine("Connected To Hub!");
                                             while (connection.State == HubConnectionState.Connected)
                                             {
                                                            // connection.SendAsync("SendMarkets");
                                                            connection.InvokeAsync("SendMarkets");
                                             }
                              });
                              threading.Start();
                              Console.WriteLine(3);
                              //connection.SendAsync("");

                              await connection.StartAsync();


                              Console.WriteLine(4);


                              Console.WriteLine(5);



               }

}
public class MarketDTO
{
               public string id { get; set; }
               public string base_currency { get; set; }
               public string quote_currency { get; set; }
               public string quote_increment { get; set; }
               public string base_increment { get; set; }
               public string display_name { get; set; }
               public string min_market_funds { get; set; }
               public bool margin_enabled { get; set; }
               public bool post_only { get; set; }
               public bool limit_only { get; set; }
               public bool cancel_only { get; set; }
               public string status { get; set; }
               public string status_message { get; set; }
               public bool trading_disabled { get; set; }
               public bool fx_stablecoin { get; set; }
               public string max_slippage_percentage { get; set; }
               public bool auction_mode { get; set; }
               public string high_bid_limit_percentage { get; set; }
}
public class MarketSocketDTO
{
               public string type { get; set; }
               public int sequence { get; set; }
               public string product_id { get; set; }
               public string price { get; set; }
               public string open_24h { get; set; }
               public string volume_24h { get; set; }
               public string low_24h { get; set; }
               public string high_24h { get; set; }
               public string volume_30d { get; set; }
               public string best_bid { get; set; }
               public string best_bid_size { get; set; }
               public string best_ask { get; set; }
               public string best_ask_size { get; set; }
               public string side { get; set; }
               public DateTime time { get; set; }
               public int trade_id { get; set; }
               public string last_size { get; set; }
}


public class WebsocketData
{
               public string type { get; set; } = string.Empty;
               public string product_id { get; set; } = string.Empty;
               public List<List<string>> bids { get; set; } = new List<List<string>>();
               public List<List<string>> asks { get; set; } = new List<List<string>>();
}

public class MarketService
{
               public string coinbase { get; set; } = "wss://ws-feed.pro.coinbase.com";
               public string marketurls { get; set; } = "https://api.pro.coinbase.com/products";
               public static RestClient restclient = new RestClient();

               public List<string>? Markets { get; set; }
               WebsocketClient webclient;
               public ObservableCollection<WebsocketData>? MarketData { get; set; }=new ObservableCollection<WebsocketData>();
               protected void MarketChanged(object sender,NotifyCollectionChangedEventArgs args) 
               {
                              Console.WriteLine("MarketData Modified");
               }
               public MarketService()
               {
                              MarketData.CollectionChanged += MarketChanged;
                              InitWebsocket();
               }

               public void InitWebsocket()
               {
                              webclient = new WebsocketClient(new Uri(coinbase));
                              webclient.DisconnectionHappened.Subscribe(message =>
                              {
                                             Debug.WriteLine($"Closing Connection{message.CloseStatusDescription}");
                              });
                              webclient.MessageReceived.Subscribe(message =>
                              {
                                             Console.WriteLine($" market :{message.Text}");
                                             
                                            var  responsedata=JsonSerializer.Deserialize<Dictionary<string, object>>(message.Text);
                                             if (responsedata["type"].ToString() == "ticker") 
                                             {
                                                            WebsocketData data = new WebsocketData();
                                                            data.bids = new List<List<string>>() { new List<string>() { responsedata["best_bid"] .ToString(), responsedata["best_bid_size"].ToString() } };
                                                            data.asks = new List<List<string>>() { new List<string>() { responsedata["best_ask"].ToString(), responsedata["best_ask_size"].ToString()} };
                                                            //data.product_id = Guid.NewGuid().ToString();
                                                            data.product_id = responsedata["product_id"].ToString();
                                                            if (MarketData.Where(x => x.product_id == data.product_id).Count ()> 0)
                                                            {
                                                                           MarketData.Remove(MarketData.Where(x => x.product_id == data.product_id).FirstOrDefault());
                                                            }
                                                            MarketData.Add(data);
                                             }
                                             else if (responsedata["type"].ToString() == "error") 
                                             {
                                                            Console.WriteLine($"Websocket Error {responsedata["message"].ToString()} {responsedata["reason"].ToString()}");

                                             }

                              });
                              if (GetMarkets().Result)
                              {
                                             var body = new Dictionary<string, object>()
                                             {
                                                                                          {"type","subscribe"},
                                                                           {"product_ids",Markets},
                                                                           {"channels", new List<string>(){"ticker"}}
                                             };
                                             webclient.Send(JsonSerializer.Serialize(body));
                                             webclient.Start();
                                             Console.WriteLine("Success Getting Markets");
                                             Thread threading = new Thread(() =>
                                             {
                                                            while (true)
                                                            {

                                                            }
                                             });
                                             threading.Start();

                              }
                              else
                              {
                                             throw new Exception("Error Initializing Websocket");
                              }


               }

               public async Task<bool> GetMarkets()
               {
                              RestRequest request = new RestRequest("https://api.pro.coinbase.com/products", Method.Get);
                              var response = restclient.Get(request);



                              if (response.IsSuccessStatusCode)
                              {

                                             List<MarketDTO> marketdata = JsonSerializer.Deserialize<List<MarketDTO>>(response.Content);
                                            // Console.WriteLine(response.Content);
                                             Console.WriteLine(marketdata.Count);
                                             Markets = new List<string>();
                                             foreach(MarketDTO   mktdto  in marketdata)
                                             {
                                                            Console.WriteLine(mktdto.id);
                                                                  Markets.Add(mktdto.id);
                                                            
                                             }
                                             return true;


                              }
                              else
                              {
                                             Console.WriteLine(response.StatusCode);
                                             Console.WriteLine(response.Content.ToString());
                              }
                              return false;
               }
}

public class Program

{


               static void Main(string[] args)
               {
                              ConnectionService connection=new ConnectionService();
                              //MarketService service = new MarketService();

                             // HttpRequestMessage requestmessage = new HttpRequestMessage(HttpMethod.Get, "https://api.pro.coinbase.com/products");
                              //var responsedto=client.Send(requestmessage);




               }
}