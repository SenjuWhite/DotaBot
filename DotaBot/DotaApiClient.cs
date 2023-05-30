using DotaBot.Models;
using Newtonsoft.Json;

namespace DotaBot
{
    public class DotaApiClient
    {
        private HttpClient client;
        private static string address;
        public DotaApiClient()
        {
            client = new HttpClient();
            address = Constants._address;
            client.BaseAddress = new Uri(address);
        }

        public async Task<string> GetAnswerAsync(string q)
        {
            var response = await client.GetAsync($"/api/Dota/ChatGPT?message={q}");
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();

            return content;
        }
        public async Task<PlayerInfo> GetPlayerASync(int id)
        {
            var response = await client.GetAsync($"/api/Dota/player?id={id}");
            response.EnsureSuccessStatusCode();
            var content = response.Content.ReadAsStringAsync().Result;
            var result = JsonConvert.DeserializeObject<PlayerInfo>(content);
            return result;
        }
        public async Task<List<HeroesInfo.Hero>> GetHeroesASync()
        {
            var response = await client.GetAsync($"/api/Dota/heroes");
            response.EnsureSuccessStatusCode();
            var content = response.Content.ReadAsStringAsync().Result;
            var result = JsonConvert.DeserializeObject<List<HeroesInfo.Hero>>(content);
            return result;
        }
        public async Task<List<PlayerRecentMatchesInfo.Matches>> GetPlayerMatchesASync(int id)
        {
            var response = await client.GetAsync($"/api/Dota/player/matches?id={id}");
            response.EnsureSuccessStatusCode();
            var content = response.Content.ReadAsStringAsync().Result;
            var result = JsonConvert.DeserializeObject<List<PlayerRecentMatchesInfo.Matches>>(content);
            return result;
        }
        public async Task<MatchInfo> GetMatchUrlAsync(long id)
        {
            var response = await client.GetAsync($"/api/Dota/match?id={id}");
            response.EnsureSuccessStatusCode();
            var content = response.Content.ReadAsStringAsync().Result;
            var result = JsonConvert.DeserializeObject<MatchInfo>(content);
            return result;
        }
        public async Task<List<TgBotUser>> GetTgUserASync(long chatId, bool temporary)
        {
            var response = await client.GetAsync($"/api/UserProfile/get_user?ChatId={chatId}&temporary={temporary}");
            response.EnsureSuccessStatusCode();
            var content = response.Content.ReadAsStringAsync().Result;
            var result = JsonConvert.DeserializeObject<List<TgBotUser>>(content);
            return result;
        }
        public async Task AddTgUserASync(long chatId, int dotaId, bool temporary)
        {
            await client.PostAsync($"/api/UserProfile/create_user?ChatId={chatId}&DotaId={dotaId}&temporary={temporary}", null);

        }
        public async Task DeleteTgUserASync(int dotaId, bool temporary)
        {
            await client.DeleteAsync($"/api/UserProfile/delete_user?DotaId={dotaId}&temporary={temporary}");
        }
        public async Task DeleteNotification(long chatId)
        {
            await client.DeleteAsync($"/api/UserProfile/delete_notification?ChatId={chatId}");
        }
        public async Task AddNotification(long chatId, string date, string title)
        {
            await client.PostAsync($"/api/UserProfile/post_notification?ChatId={chatId}&date={date}&title={title}", null);
        }
        public async Task<List<NotificationInfo>> GetNotifications()
        {
            var response = await client.GetAsync($"/api/UserProfile/get_notifications");
            response.EnsureSuccessStatusCode();
            var content = response.Content.ReadAsStringAsync().Result;
            var result = JsonConvert.DeserializeObject<List<NotificationInfo>>(content);
            return result;
        }
        public async Task<PlayerWLInfo> GetPlayerWLASync(int id)
        {
            var response = await client.GetAsync($"/api/Dota/player/wl?id={id}");
            response.EnsureSuccessStatusCode();
            var content = response.Content.ReadAsStringAsync().Result;
            var result = JsonConvert.DeserializeObject<PlayerWLInfo>(content);
            return result;
        }
        public async Task<TwitchInfo> GetTwitchInfoAsync(string broadcastId)
        {
            var response = await client.GetAsync($"/api/Twitch/twitch?broadcastId={broadcastId}");
            response.EnsureSuccessStatusCode();
            var content = response.Content.ReadAsStringAsync().Result;
            var result = JsonConvert.DeserializeObject<TwitchInfo>(content);
            return result;
        }


    }
}
