namespace CryptoTrackerAPI.Models
{

public class Account
{
        public double Balance{get;set;}=new double();
        public List<string> ActiveTradesId{get;set;}=new List<string>();
        public double Bonus{get;set;}=new double();
        public  string Id{get;set;}=string.Empty;
        public string Curreny{get;set;}=string.Empty;
        public List<string> TransactionHistory{get;set;}=new List<string>();
}
        public class NewsData:NewsDataDTO
        {
                public string Id{get;set;}=string.Empty;
                public DateTime Timestamp{get;set;}=DateTime.MinValue;
    
        }

        public class NewsDataDTO
        {
        public string title { get; set; }=string.Empty;
        public string link { get; set; }=string.Empty;
        public string? keywords { get; set; }=string.Empty;
        public string? creator { get; set; }=string.Empty;
        public string? video_url { get; set; }=string.Empty;
        public string? description { get; set; }=string.Empty;
        public string content { get; set; }=string.Empty;
        public DateTime?  pubDate { get; set; }=DateTime.MinValue;
        public string? image_url { get; set; }=string.Empty;
        public string source_id { get; set; }=string.Empty;
        public List<string> category { get; set; }=new List<string>();
        public List<string> country { get; set; }=new List<string>();
        public string language { get; set; }=string.Empty;
    
        }
        public class Market
        {
                public string Base{get;set;}=string.Empty;
                public string Quote{get;set;}=string.Empty;
                public Dictionary<double,long> Price{get;set;}=new Dictionary<double,long>();
                public List<long> Prices{get;set;}=new List<long>();
                public string BaseImageUrl{get;set;}=string.Empty;
                public string QuoteImageUrl{get;set;}=string.Empty;

        }
         
         public class Trade 
         {
                //Id of the trade
                public string Id { get; set; }=string.Empty;
                //Account that placed the trade
                public string AccountId{get;set;}=string.Empty;
                //Market of the trade eg. XAUUSD
                public string Market{get;set;}=string.Empty;
                //Opening price of the tade when the order was placed
                public double OpeningPrice{get;set;}=new double();
                //Time when the trade was placed
                public DateTime Timestamp=DateTime.MinValue;
                //The margin of the trade
                public long Margin{get;set;}=new long();
                //Leverage of the trade  eg 1:100=>100
                public long Leverage{get;set;}=new long();
                //The net value of the trade as profit or loss
                public double Equity{get;set;}=new double();
                //Whether the trade is silenced due to close of markets eg. false on weekends
                public bool Active{get;set;}=new bool();
                //Whether the trade has been placed or onorder or closed or stoploss or takeprofit or Nan as default 
                public string Status{get;set;}="Nan";
                //Set a takeprofit value
                public double TakeProfit{get;set;}=new double();
                //Set a Stop losss for the trade
                public double StopLoss{get;set;}=new double();
                //Target Value to place the trade
                public double OnOrder{get;set;}=new double();
                //Target Time to place the trade
                public DateTime OnTime{get;set;}=DateTime.MinValue;
         }
        public class Status
        {

                public string Id{get;set;}=string.Empty;
                public string Title{get;set;}=string.Empty;
                public long XP{get;set;}=new long();
                public string Description{get;set;}=string.Empty;
                public string ImageUrl{get;set;}=string.Empty;

                public double Bonud{get;set;}=new double();
        }
        public class User
        {
                public List<string> Accounts{get;set;}=new List<string>(); 
                public string FirstName{get;set;}=string.Empty;
                public string LastName{get;set;}=string.Empty;
                public string PasswordHarsh{get;set;}=string.Empty;
                public string Email{get;set;}=string.Empty;
                public string ImageUrl{get;set;}=string.Empty;
                public string Phone{get;set;}=string.Empty;
                public string NationalNumber{get;set;}=string.Empty;
                public string Address{get;set;}=string.Empty;
                public  Status status{get;set;}=new Status();
                public List<string> Trades{get;set;}=new List<string>();
                public DateTime CreatedDate{get;set;}=DateTime.MinValue;
                public string Location{get;set;}=string.Empty;
                public string Language{get;set;}=string.Empty;
                
                

        }

}