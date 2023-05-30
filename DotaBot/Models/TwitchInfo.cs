namespace DotaBot.Models
{
    public class TwitchInfo
    {
        public Data data { get; set; }
    }

    public class Data
    {
        public string broadcaster_id { get; set; }
        public string broadcaster_name { get; set; }
        public string broadcaster_login { get; set; }
        public List<Segment> segments { get; set; }
    }

    public class Segment
    {
        public string id { get; set; }
        public DateTime start_time { get; set; }
        public string end_time { get; set; }
        public string title { get; set; }
        public string category_id { get; set; }
        public bool is_recurring { get; set; }
    }
}
