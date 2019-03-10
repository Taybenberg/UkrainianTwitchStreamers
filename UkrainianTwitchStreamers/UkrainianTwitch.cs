using System.Net;
using System.Text;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;

public class UkrainianTwitch
{
    private const string ApiUrl = "https://api.twitch.tv/kraken/streams";
    private const string TwitchUrl = "https://www.twitch.tv/";
    private const string UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/70.0.3538.102 Safari/537.36 Edge/18.17763";

    private List<Channel> streams = new List<Channel>();

    public class Channel
    {
        public string display_name { get; set; }
        public string status { get; set; }
        public string game { get; set; }
        public string url { get; set; }

        public override string ToString()
        {
            return
                $"\nКористувач: {display_name}" +
                $"\nНазва: {status}" +
                $"\nГра: {game}" +
                $"\nПосилання: {url}";
        }
    }

    public UkrainianTwitch(string token)
    {
        using (var webClient = new WebClient())
        {
            webClient.Encoding = Encoding.UTF8;

            webClient.Headers.Add("user-agent", UserAgent);
            webClient.QueryString.Add("client_id", token);
            webClient.QueryString.Add("broadcaster_language", "uk");
            webClient.UseDefaultCredentials = true;

            JObject jObject = JObject.Parse(webClient.DownloadString(ApiUrl));

            var results = jObject["streams"].Children();

            foreach (JToken result in results)
                streams.Add(result["channel"].ToObject<Channel>());
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