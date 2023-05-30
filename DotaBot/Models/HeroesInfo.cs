namespace DotaBot.Models
{
    public class HeroesInfo
    {
        public List<Hero> heroes { get; set; }
        public HeroesInfo()
        {
            heroes = new List<Hero>();
        }
        public class Hero
        {
            public int id { get; set; }
            public string localized_name { get; set; }
        }
    }
}
