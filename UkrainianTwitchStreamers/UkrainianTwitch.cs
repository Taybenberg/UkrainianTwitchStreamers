using System;
using System.Net;
using System.Text;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;

public class UkrainianTwitch
{
    private const string ApiUrl = "https://api.twitch.tv/kraken/streams";
    private const string TwitchUrl = "https://www.twitch.tv/";
    private const string UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/70.0.3538.102 Safari/537.36 Edge/18.17763";

    private string[] users { get; set; } = { "playua", "violinua", "alex_d20", "yvan_snig", "pad0n", "sbt_localization", "tenevyk", "gamer_fm", "telpecarne", "dpalladinno", "bigdriftermen", "logos_ua", "gamecraft", "gameon_ua", "dobra_divka", "gamewithua", "taitake", "brokengamestudio", "bravetaras", "meatball_ua", "jopreston", "vatmanua", "hell_ua", "angryukrainiangamer", "ditho_play", "vitaliyfors", "ihor4uk_ua", "tequila860", "offmisterzoltan", "tigerualviv", "shaddixua", "uesfdota1" };

    private List<Channel> streams = new List<Channel>();

    public class Channel
    {
        public bool online { get; set; }

        public string display_name { get; set; }
        public string status { get; set; }
        public string game { get; set; }
        public string url { get; set; }

        public override string ToString()
        {
            return
                $"Користувач: {display_name}" +
                $"\nСтатус: " + (online ? "Онлайн" +
                $"\nНазва: {status}" +
                $"\nГра: {game}" : "Офлайн") +
                $"\nПосилання: {url}";
        }
    }

    public UkrainianTwitch(string token)
    {
        using (var webClient = new WebClient())
        {
            webClient.Encoding = Encoding.UTF8;

            webClient.Headers.Add("user-agent", UserAgent);
            webClient.QueryString.Add("channel", string.Join(",", users));
            webClient.QueryString.Add("client_id", token);
            webClient.UseDefaultCredentials = true;

            JObject jObject = JObject.Parse(webClient.DownloadString(ApiUrl));

            var results = jObject["streams"].Children();

            foreach (JToken result in results)
            {
                var stream = result["channel"].ToObject<Channel>();
                stream.online = true;

                users[Array.IndexOf(users, result["channel"]["name"].ToString())] = null;

                streams.Add(stream);
            }
        }

        foreach (var user in users)
            if (user != null)
            {
                var stream = new Channel();

                stream.online = false;
                stream.display_name = user;
                stream.url = $"{TwitchUrl}{user}";

                streams.Add(stream);
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

    Channel[] ToArray()
    {
        return streams.ToArray();
    }

    List<Channel> ToList()
    {
        return streams;
    }
}