using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot.Types;
using Telegram.Bot;
using Telegram.Bot.Types.ReplyMarkups;
using Telegram.Bot.Types.Enums;
using DotaBot.Models;

namespace DotaBot
{
    public class Bot
    {

        DotaApiClient dotaclient = new DotaApiClient();
        TelegramBotClient client;
        const string _profileButton = "My Profile";
        const string _findMatchButton = "Match replay";
        const string _addProfileButton = "Add/Change Profile";
        const string _findProfileButton = "Find profile";
        const string _helpButton = "Help";
        const string _showMatchesButton = "Show recent matches";
        const string _mainMenuButton = "Back to main menu↩️";
        const string _translationsButton = "Professinal matches schedule";
        const string _ChatGPTButton = "ChatGPT";
        static Dictionary<long, bool> waitingForInput = new Dictionary<long, bool>();
        static Dictionary<long, string> answertype = new Dictionary<long, string>();
        public Bot()
        {
            client = new TelegramBotClient($"{Constants._apiKeyBot}");
            client.StartReceiving(updateHandler, errorHandler);

        }
        public Task errorHandler(ITelegramBotClient arg1, Exception arg2, CancellationToken arg3)
        {
            return Task.CompletedTask;
        }

        public async Task updateHandler(ITelegramBotClient client, Update update, CancellationToken token)
        {
            //waitingForInput допомогає перевіряти чи була натиснута кнопка, щоб потім обробити вхідне повідомлення.При натисканні кнопки значення ключа message.Chat.Id
            //стає true, після наступної обробки вхідного повідомлення від юзера стає false.answertype зберігає назву кнопку після натискання на неї, щоб потім
            //програма могла знати для якої кнопки робити обробку
            try
            {
                switch (update.Type)
                {
                    case (UpdateType)Telegram.Bot.Types.Enums.MessageType.Text:
                        var message = update.Message;
                        await Console.Out.WriteLineAsync(message.Text);
                        // explain
                        if (message.Text == "/start")
                        {
                            await client.SendTextMessageAsync(message.Chat.Id, "Hi, below you can see the list of functions that I can do. Press <Help> button for details", replyMarkup: GetMainButtons());
                            await Check();
                        }
                        if (message.Text == _helpButton)
                        {
                            string help = "My profile — view  information about your account\nFind profile — view information about someone else's profile by Dota Id\nMatch replay — enter Match Id to download replay\n" +
                                "Professional matches schedule — view professional matches schedule/set match reminder\nChatGPT — ask a question to AI";
                            await Check();
                            await client.SendTextMessageAsync(message.Chat.Id, help, replyMarkup: GetBackButton());

                        }
                        if (message.Text == _mainMenuButton)
                        {
                            await Console.Out.WriteAsync(message.Chat.Id.ToString());
                            await client.SendTextMessageAsync(message.Chat.Id, "↩️", replyMarkup: GetMainButtons());
                            await Console.Out.WriteAsync(message.Chat.Id.ToString());
                            await Check();
                        }

                        //explain
                        if (message.Text == _findProfileButton)
                        {
                            await client.SendTextMessageAsync(message.Chat.Id, "Enter Dota ID to get information", replyMarkup: GetBackButton());
                            await Check();
                            waitingForInput[message.Chat.Id] = true;
                            answertype[message.Chat.Id] = _findProfileButton;
                            
                        }
                        else if (waitingForInput.ContainsKey(message.Chat.Id) && waitingForInput[message.Chat.Id] && answertype[message.Chat.Id] == _findProfileButton)
                        {
                            await SendPlayerInfo(message.Text, message.Chat.Id, true);
                        }
                        //eplain
                        if (message.Text == _showMatchesButton)
                        {
                            var player = await dotaclient.GetTgUserASync(message.Chat.Id, true);
                            if (player.Count != 0)
                            {
                                var matches = await dotaclient.GetPlayerMatchesASync(player[0].DotaId);
                                if (matches.Count == 0)
                                {
                                    await client.SendTextMessageAsync(message.Chat.Id, "Unfortunately, I don't have access to your matches. This could be due to your privacy settings", replyMarkup: GetBackButton());
                                    break;
                                }
                                StringBuilder idList = new StringBuilder();
                                for (int i = 0; i < matches.Count; i++)
                                {
                                    idList.Append($"{i + 1}) {matches[i].match_id}\n");
                                }
                                string answer = $"IDs of the last 20 matches:\n\n{idList}\nEnter the number(1-20) of the match you want to learn more about";
                                await client.SendTextMessageAsync(message.Chat.Id, answer, replyMarkup: GetBackButton());
                                waitingForInput[message.Chat.Id] = true;
                                answertype[message.Chat.Id] = _showMatchesButton;
                            }

                        }
                        else if (waitingForInput.ContainsKey(message.Chat.Id) && waitingForInput[message.Chat.Id] && answertype[message.Chat.Id] == _showMatchesButton)
                        {
                            var player = await dotaclient.GetTgUserASync(message.Chat.Id, true);
                            var matches = await dotaclient.GetPlayerMatchesASync(player[0].DotaId);
                            if (int.TryParse(message.Text, out int number) && number > 0 && number < 21)
                            {
                                var match = matches[number - 1];
                                var heroes = await dotaclient.GetHeroesASync();
                                var hero = heroes.Where(h => h.id.ToString() == match.hero_id).Select(h => h.localized_name).ToList()[0];
                                var win = match.radiant_win == (int.Parse(match.player_slot) < 128) ? "\"<b>Win</b>" : "<b>Lose</b>";
                                string answer = $"Match ID: {match.match_id}\n{win}\nDuration: {int.Parse(match.duration) / 60} minutes\n" +
                                    $"Hero: {hero}\nKills: {match.kills}\nDeaths: {match.deaths}\nAssists: {match.assists}\n" +
                                    $"Xp per minute: {match.xp_per_min}\nGold per minute: {match.gold_per_min}\n" +
                                    $"Hero damage: {match.hero_damage}\nHero healing {match.hero_healing}\nLast hits: {match.last_hits}";
                                await client.SendTextMessageAsync(message.Chat.Id, answer, parseMode: ParseMode.Html, replyMarkup: GetBackButton());
                            }
                            else
                                await client.SendTextMessageAsync(message.Chat.Id, "Invalid number. Try again", replyMarkup: GetBackButton());
                        }
                        //explain
                        if (message.Text == _addProfileButton)
                        {
                            await client.SendTextMessageAsync(message.Chat.Id, "Enter Dota ID to add/change your profile", replyMarkup: GetBackButton());
                            //Check();
                            waitingForInput[message.Chat.Id] = true;
                            answertype[message.Chat.Id] = _addProfileButton;

                        }
                        else if (waitingForInput.ContainsKey(message.Chat.Id) && waitingForInput[message.Chat.Id] && answertype[message.Chat.Id] == _addProfileButton)
                        {
                            var playerInfo = int.TryParse(message.Text, out int id) ? await dotaclient.GetPlayerASync(id) : null;
                            if (playerInfo != null && playerInfo.profile != null)
                            {
                                var temporaryUser = await dotaclient.GetTgUserASync(message.Chat.Id, false);
                                await dotaclient.AddTgUserASync(message.Chat.Id, int.Parse(message.Text), false);
                                if (temporaryUser.Count != 0)
                                {
                                    await dotaclient.DeleteTgUserASync(temporaryUser[0].DotaId, false);
                                }
                                await client.SendTextMessageAsync(message.Chat.Id, "Your profile was successfully added. Press the <My profile> button for details", replyMarkup: GetBackButton());
                            }
                            else
                                await client.SendTextMessageAsync(message.Chat.Id, "Invalid ID. Try again", replyMarkup: GetBackButton());

                        }
                        //explain
                        if (message.Text == _profileButton)
                        {
                            await Check();
                            var user = await dotaclient.GetTgUserASync(message.Chat.Id, false);
                            if (user.Count == 0)
                            {
                                await client.SendTextMessageAsync(message.Chat.Id, "You haven't added a profile yet");
                            }
                            else
                            {
                                var profile = await dotaclient.GetTgUserASync(message.Chat.Id, false);
                                var chatId = profile[0].ChatId;
                                var dotaId = profile[0].DotaId;
                                await SendPlayerInfo(dotaId.ToString(), chatId, true);
                            }
                        }
                        if (message.Text == _findMatchButton)
                        {
                            await Check();
                            await client.SendTextMessageAsync(message.Chat.Id, "Enter  Match ID to get a replay link", replyMarkup: GetBackButton());
                            waitingForInput[message.Chat.Id] = true;
                            answertype[message.Chat.Id] = _findMatchButton;
                        }
                        else if (waitingForInput.ContainsKey(message.Chat.Id) && waitingForInput[message.Chat.Id] && answertype[message.Chat.Id] == _findMatchButton)
                        {
                            var matchInfo = long.TryParse(message.Text, out long id) ? await dotaclient.GetMatchUrlAsync(id) : null;
                            if (matchInfo != null && matchInfo.replay_url != null)
                            {
                                await client.SendTextMessageAsync(message.Chat.Id, $"Don't forget to install VirtualDub or something similar to open the replay.\nHere is replay link:\n{matchInfo.replay_url}", replyMarkup: GetBackButton());
                                waitingForInput[message.Chat.Id] = false;
                                answertype[message.Chat.Id] = null;

                            }
                        }
                        if (message.Text == _ChatGPTButton)
                        {
                            await Check();
                            waitingForInput[message.Chat.Id] = true;
                            answertype[message.Chat.Id] = _ChatGPTButton;
                            await client.SendTextMessageAsync(message.Chat.Id, "You can ask the AI ​​any question, like which hero to take against certain heroes or something like that. " +
                                "Remember that the question must be clear and understandable if you want to get a quality answer. Otherwise, the result may be unpredictable", replyMarkup: GetBackButton());
                        }
                        else if (waitingForInput.ContainsKey(message.Chat.Id) && waitingForInput[message.Chat.Id] && answertype[message.Chat.Id] == _ChatGPTButton)
                        {
                            string question = message.Text;
                            var answer = await dotaclient.GetAnswerAsync(question);
                            await client.SendTextMessageAsync(message.Chat.Id, answer, replyMarkup: GetBackButton());
                            waitingForInput[message.Chat.Id] = false;
                            answertype[message.Chat.Id] = null;
                        }
                        if (message.Text == _translationsButton)
                        {
                            await Check();
                            string schedule = await SendSchedule("657645352");
                            await client.SendTextMessageAsync(message.Chat.Id, schedule, parseMode: ParseMode.Html, replyMarkup: GetBackButton());
                            await client.SendTextMessageAsync(message.Chat.Id, "Enter the match number to set the notification. I will remind you about the match ", replyMarkup: GetBackButton());
                            waitingForInput[message.Chat.Id] = true;
                            answertype[message.Chat.Id] = _translationsButton;
                        }
                        else if (waitingForInput.ContainsKey(message.Chat.Id) && waitingForInput[message.Chat.Id] && answertype[message.Chat.Id] == _translationsButton)
                        {
                            var info = await dotaclient.GetTwitchInfoAsync("657645352");
                            if (int.TryParse(message.Text, out int number) && number > 0 && number < info.data.segments.Count)
                            {
                                await client.SendTextMessageAsync(message.Chat.Id, "Notification has been successfully added",replyMarkup:GetBackButton());
                                await dotaclient.AddNotification(message.Chat.Id, info.data.segments[number].start_time.AddHours(3).ToString(), info.data.segments[number].title);
                                waitingForInput[message.Chat.Id] = false;
                                answertype[message.Chat.Id] = null;
                                var timer = new Timer(TimerCallback, null, 0, 60000);
                                async void TimerCallback(Object o)
                                {
                                    var now = DateTime.Now;
                                    var notificationDate = info.data.segments[number].start_time.AddHours(3);
                                    var notifications = await dotaclient.GetNotifications();
                                    foreach (NotificationInfo date in notifications)
                                    {
                                        var datetime = DateTime.Parse(date.date);
                                        if (now.Minute == datetime.Minute && now.Hour == datetime.Hour && now.Date == datetime.Date && now.Year == datetime.Year)
                                        {
                                            await client.SendTextMessageAsync(
                                                chatId: date.ChatId,
                                                text: $"Hello! Don't forget to watch <b>{date.title}</b>. It is starting right now. Here is your link:\nhttps://www.twitch.tv/dota2mc_ua",parseMode:ParseMode.Html
                                            );
                                            dotaclient.DeleteNotification(date.ChatId);

                                        }
                                    }
                                }
                            }
                            else
                            {
                                await client.SendTextMessageAsync(message.Chat.Id, "Invalid number. Try again", replyMarkup: GetBackButton());
                            }
                        }
                         async Task Check()
                        {
                            var temporaryUser = await dotaclient.GetTgUserASync(message.Chat.Id, true);
                            //if (waitingForInput.ContainsKey(message.Chat.Id))
                            //{
                                waitingForInput[message.Chat.Id] = false;
                                answertype[message.Chat.Id] = null;
                            //}
                            if (temporaryUser.Count != 0)
                            {
                                await dotaclient.DeleteTgUserASync(temporaryUser[0].DotaId, true);
                            }
                        }
                        break;
                }
            }

            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.StackTrace);
            }
            return;




        }
        private async Task<string> SendSchedule(string broadcast)
        {
            var twitchInfo = await dotaclient.GetTwitchInfoAsync(broadcast);
            StringBuilder sb = new StringBuilder();
            var schedule = twitchInfo.data.segments;
            sb.Append("<b>Upcoming professional matches:</b>\n\n");
            for (int i = 1; i < schedule.Count; i++)
            {
                sb.Append($"{i}. {schedule[i].title}\nWhen: {schedule[i].start_time.AddHours(3).ToString()}\n\n");
            }
            sb.Append("______________________________\n");
            sb.Append($"<b>Live professinal match:</b>\n\n{schedule[0].title}\nhttps://www.twitch.tv/dota2mc_ua\n");
            return sb.ToString();
        }
        private async Task SendPlayerInfo(string message, long ChatId, bool temporary)
        {
            var playerInfo = int.TryParse(message, out int id) ? await dotaclient.GetPlayerASync(id) : null;
            if (playerInfo != null && playerInfo.profile != null)
            {
                var wl = await dotaclient.GetPlayerWLASync(id);
                var totalMatches = int.Parse(wl.win) + int.Parse(wl.lose) == 0 ? "unknown" : (int.Parse(wl.win) + int.Parse(wl.lose)).ToString();
                var rank = playerInfo.rank_tier == null ? "unknown" : playerInfo.rank_tier;
                rank = rank == "unknown" ? "unknown" : playerInfo.ranks[int.Parse(rank[0].ToString()) - 1] + $" {rank[1]}";
                await dotaclient.AddTgUserASync(ChatId, int.Parse(message), temporary);
                string answer = $"Nickname: {playerInfo.profile.personaname}\n Rank: {rank}\nTotal matches: {totalMatches}\nSteam Id: {playerInfo.profile.steamid}\nSteam profile link: {playerInfo.profile.profileurl}";
                await client.SendTextMessageAsync(ChatId, answer, replyMarkup: GetProfileButtons());
                waitingForInput[ChatId] = false;
            }
            else
                await client.SendTextMessageAsync(ChatId, "Invalid ID. Try again", replyMarkup: GetBackButton());

        }
        private IReplyMarkup? GetMainButtons()
        {
            ReplyKeyboardMarkup replyKeyboardMarkup = new(new[]
{
                new KeyboardButton[] { _profileButton },
                new KeyboardButton[] {_addProfileButton, _findProfileButton, _findMatchButton},
                new KeyboardButton[] {_translationsButton, _ChatGPTButton, _helpButton}
            });
            return replyKeyboardMarkup;

        }
        private IReplyMarkup? GetBackButton()
        {
            ReplyKeyboardMarkup replyKeyboardMarkup = new(new[]
{

                new KeyboardButton[] { _mainMenuButton },
            });
            return replyKeyboardMarkup;

        }
        private IReplyMarkup? GetProfileButtons()
        {
            ReplyKeyboardMarkup replyKeyboardMarkup = new(new[]
{

                new KeyboardButton[] { _showMatchesButton },
                new KeyboardButton[] {_mainMenuButton}
            });
            return replyKeyboardMarkup;

        }
    }
}
