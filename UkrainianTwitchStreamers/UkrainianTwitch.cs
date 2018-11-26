using System.Net;
using System.Text;
using Newtonsoft.Json.Linq;

public class UkrainianTwitch
{
    private const string ApiUrl = "https://api.twitch.tv/kraken/streams/";
    private const string TwitchUrl = "https://www.twitch.tv/";
    private const string UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/70.0.3538.102 Safari/537.36 Edge/18.17763";

    public string[] users { get; set; } = { "playua", "violinua", "alex_d20", "yvan_snig", "pad0n", "sbt_localization", "tenevyk", "gamer_fm", "telpecarne", "dpalladinno", "bigdriftermen", "logos_ua", "gamecraft", "gameon_ua", "dobra_divka", "gamewithua", "taitake", "brokengamestudio", "bravetaras", "meatball_ua", "jopreston", "vatmanua", "hell_ua", "angryukrainiangamer", "ditho_play", "vitaliyfors", "ihor4uk_ua", "tequila860", "offmisterzoltan", "tigerualviv", "shaddixua", "uesfdota1" };

    private Stream[] streams;

    public class Stream
    {
        public bool online { get; set; }

        public string name { get; set; }
        public string title { get; set; }
        public string game { get; set; }
        public string url { get; set; }

        public override string ToString()
        {
            return
                $"Користувач: {name}" +
                $"\nСтатус: " + (online ? "Онлайн" +
                $"\nНазва: {title}" +
                $"\nГра: {game}" : "Офлайн") +
                $"\nПосилання: {url}";
        }
    }

    public UkrainianTwitch(string token)
    {
        streams = new Stream[users.Length];

        using (var webClient = new WebClient())
        {
            webClient.Headers.Add("user-agent", UserAgent);
            webClient.QueryString.Add("client_id", token);
            webClient.UseDefaultCredentials = true;

            for (int i = 0; i < users.Length; i++)
            {
                JObject jObject = JObject.Parse(webClient.DownloadString($"{ApiUrl}{users[i]}"));

                if (jObject["stream"].Type != JTokenType.Null)
                {
                    streams[i] = new Stream();
                    streams[i].online = true;
                    streams[i].url = $"{TwitchUrl}{users[i]}";
                    streams[i].name = users[i];
                    streams[i].game = jObject["stream"]["game"].ToString();
                    streams[i].title = jObject["stream"]["title"].ToString();
                }
                else
                {
                    streams[i] = new Stream();
                    streams[i].online = false;
                    streams[i].url = $"{TwitchUrl}{users[i]}";
                    streams[i].name = users[i];
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
        string[] res = new string[streams.Length];

        for (int i = 0; i < streams.Length; i++)
            res[i] = streams[i].ToString();

        return res;
    }

    Stream[] ToArray()
    {
        return streams;
    }
}