namespace DotaBot.Models
{
    public class PlayerRecentMatchesInfo
    {
        public List<Matches> recentMatches { get; set; }
        public PlayerRecentMatchesInfo()
        {
            recentMatches = new List<Matches>();
        }
        public class Matches
        {
            public string player_slot { get; set; }
            public bool radiant_win { get; set; }
            public string match_id { get; set; }
            public string duration { get; set; }
            public string game_mode { get; set; }
            public string hero_id { get; set; }
            public string hero_name { get; set; }
            public string kills { get; set; }
            public string deaths { get; set; }
            public string assists { get; set; }
            public int xp_per_min { get; set; }
            public string gold_per_min { get; set; }
            public string hero_damage { get; set; }
            public string hero_healing { get; set; }
            public string last_hits { get; set; }

        }
    }
}
