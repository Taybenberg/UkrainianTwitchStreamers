using System.Text;
using System.Linq;
using System.Collections.Generic;
using TwitchLib.Api;

namespace UkrainianTwitchStreamers
{
    public class UkrainianTwitch
    {
        private const string TwitchUrl = "https://www.twitch.tv/";

        private List<Channel> streams = new List<Channel>();

        public class Channel
        {
            public string user_name { get; set; }
            public string title { get; set; }
            public string name { get; set; } = "-";
            public string url
            {
                get
                {
                    return (TwitchUrl + user_name).ToLower();
                }
            }

            public override string ToString()
            {
                return
                    $"\nКористувач: {user_name}" +
                    $"\nНазва: {title}" +
                    $"\nГра: {name}" +
                    $"\nПосилання: {url}";
            }
        }

        public UkrainianTwitch(string clientId, string secret)
        {
            System.Console.WriteLine("ok");

            TwitchAPI api = new TwitchAPI();

            api.Settings.ClientId = clientId;
            api.Settings.Secret = secret;

            var r = api.Helix.Streams.GetStreamsAsync(first: 100, languages: new List<string> { "uk" }).Result;

            var gids = r.Streams.Where(x => !string.IsNullOrEmpty(x.GameId)).Select(x => x.GameId).ToList();

            var games = api.Helix.Games.GetGamesAsync(gids).Result.Games;

            var liveChannels = r.Streams.SelectMany(
                stream => games.Where(game => game.Id == stream.GameId).DefaultIfEmpty(),
                (stream, game) => new
                {
                    UserName = stream.UserName,
                    Title = stream.Title,
                    Name = game == null ? "-" : game.Name
                });

            foreach (var live in liveChannels)
                streams.Add(new Channel
                {
                    user_name = live.UserName,
                    title = live.Title,
                    name = live.Name
                });
        }
        public string ToString(string separator = null)
        {
            StringBuilder res = new StringBuilder();

            foreach (var stream in streams)
                res.Append($"{stream}{separator}");

            return res.ToString();
        }

        public string[] ToStringArray()
        {
            string[] res = new string[streams.Count];

            for (int i = 0; i < streams.Count; i++)
                res[i] = streams[i].ToString();

            return res;
        }

        public Channel[] ToArray()
        {
            return streams.ToArray();
        }

        public List<Channel> ToList()
        {
            return streams;
        }
    }
}