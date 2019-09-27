using System.Net;
using System.Text;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;

public class UkrainianTwitch
{
    private const string TwitchStreamsApi = "https://api.twitch.tv/helix/streams";
    private const string TwitchUrl = "https://www.twitch.tv/";
    private const string TwitchGameApi = "https://api.twitch.tv/helix/games";
    private const string UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/70.0.3538.102 Safari/537.36 Edge/18.17763";

    private List<Channel> streams = new List<Channel>();

    public class Channel
    {
        public string user_name { get; set; }
        public string title { get; set; }
        public int game_id { get; set; }
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

    private class Game
    {
        public int id { get; set; }
        public string name { get; set; }
    }

    public UkrainianTwitch(string token)
    {
        using (var webClient = new WebClient())
        {
            webClient.Encoding = Encoding.UTF8;

            webClient.Headers.Add("user-agent", UserAgent);
            webClient.Headers.Add("Client-ID", token);
            webClient.QueryString.Add("language", "uk");

            JObject jObject = JObject.Parse(webClient.DownloadString(TwitchStreamsApi));

            var results = jObject["data"].Children();

            string url = TwitchGameApi;
            int count = 0;

            foreach (JToken result in results)
            {
                var stream = result.ToObject<Channel>();
                streams.Add(stream);

                if (count == 0)
                    url += "?id=" + stream.game_id.ToString();
                else
                    url += "&id=" + stream.game_id.ToString();
                
                count++;
            }

            webClient.QueryString.Clear();

            if (streams.Count > 0)
            {
                jObject = JObject.Parse(webClient.DownloadString(url));

                results = jObject["data"].Children();

                foreach (JToken result in results)
                {
                    var game = result.ToObject<Game>();

                    for (int i = 0; i < streams.Count; i++)
                        if (streams[i].game_id == game.id)
                            streams[i].name = game.name;
                }
            }
        }
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
