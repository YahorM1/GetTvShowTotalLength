using System.Text.Json;
using System.Text.Json.Serialization;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Collections.Generic;
using System;


namespace  ConsoleApp
{
    public class RootShow
    {
        public Show show { get; set; }
    }
    public class Show
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }
        
        [JsonPropertyName("name")]
        public string Name { get; set; }
    }
    public class Episode
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }
        
        [JsonPropertyName("runtime")]
        public int? Runtime { get; set; }
        [JsonPropertyName("type")]
        public string Type { get; set; }
    }
    class Program
    {
        private static HttpClient _client = new HttpClient();
        static async Task<Show> GetShowIdByName(Show show)
        {
            HttpResponseMessage httpResponseMessage = await
                _client.GetAsync($"https://api.tvmaze.com/search/shows?q={show.Name}");
            if (httpResponseMessage.IsSuccessStatusCode)
            {
                string content = await httpResponseMessage.Content.ReadAsStringAsync();
                List<RootShow> rootShows = JsonSerializer.Deserialize<List<RootShow>>(content);
                Show foundShow = rootShows[0].show;
                show.Id = foundShow.Id;
                show.Name = foundShow.Name;
                return show;
            }
            throw new Exception();
        }

        static async Task<List<Episode>> GetEpisodeListById(Show show)
        {
            HttpResponseMessage newMessage = await _client.GetAsync($"https://api.tvmaze.com/shows/{show.Id}/episodes");
            if (newMessage.IsSuccessStatusCode)
            {
                string content = await newMessage.Content.ReadAsStringAsync();
                List<Episode> episodes = JsonSerializer.Deserialize<List<Episode>>(content);

                return episodes;
            }

            throw new Exception();
        }

        static int GetTvShowTotalLength(List<Episode> episodes)
        {
            int totalRuntime = 0;
            foreach (var episode in episodes)
            {
                if (episode.Type == "regular" && episode.Runtime.HasValue)
                {
                    totalRuntime = totalRuntime + episode.Runtime.Value;
                }
            }

            return totalRuntime;
        }
        public static async Task Main(string[] args)
        {
            Show show = new Show();
            List<Episode> episodes = new List<Episode>();
            int RuntimeAnswer = 0;
            if (args.Length > 0)
            {
                show.Name = args[0];
            }

            try
            {
                show = await GetShowIdByName(show);
                episodes = await GetEpisodeListById(show);
                RuntimeAnswer = GetTvShowTotalLength(episodes);
                Console.WriteLine(RuntimeAnswer);
            }
            catch (Exception ex)
            {
                Environment.Exit(10);
            }
            
        }
    }
}