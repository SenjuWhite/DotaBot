namespace DotaBot.Models
{
    public class PlayerInfo
    {
        public List<string> ranks;
        public PlayerInfo()
        {
            ranks = new List<string> { "Herald", "Guardian", "Crusader", "Archon", "Legend", "Ancient", "Divine", "Immortal" };
        }
        public Profile profile { get; set; }
        public class Profile
        {
            public string personaname { get; set; }
            public string steamid { get; set; }
            public string avatarfull { get; set; }
            public string profileurl { get; set; }

        }
        public string rank_tier { get; set; }
    }
}
