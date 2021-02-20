using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System;

namespace Crossp
{
    public class Crossp
    {
        private const string API_CONTROLLER = "Clients";
        private static string serverURL = default;
        private static string externalId = default;

        public static byte[] image = default;
        public static void Initialize(string serverURL, string externalId)
        {
            Crossp.serverURL = serverURL;
            Crossp.externalId = externalId;
            Task.Run(Run);
            //CrosspSettings.Instance
        }
        private static async void Run()
        {
            List<Ad> availableAds = await GetAvailableAds();
            Ad firstAd = availableAds[0];
            image = await DownloadFile($"{serverURL}{firstAd.File.Path}");
        }
        private static async Task<List<Ad>> GetAvailableAds()
        {
            string responseJson = default;
            using (HttpClient httpClient = new HttpClient())
            {
                string url = $"{serverURL}/{API_CONTROLLER}/availableads?externalId={externalId}";
                HttpResponseMessage httpResponseMessage = await httpClient.GetAsync(url);
                responseJson = await httpResponseMessage.Content.ReadAsStringAsync();
            }
            List<Ad> deserializedAds = JsonConvert.DeserializeObject<List<Ad>>(responseJson);
            return deserializedAds;
        }

        private static async Task<byte[]> DownloadFile(string url) {
            Uri uri = new Uri(url);
            byte[] file = default;
            using (WebClient webClient = new WebClient())
            {
                file = await webClient.DownloadDataTaskAsync(uri);
                
                //webClient.DownloadFile(new Uri(url), @"c:\temp\image35.png");
                // OR 
                //client.DownloadFileAsync(new Uri(url), @"c:\temp\image35.png");
            }
            return file;
        }


        public static bool IsReady()
        {
            return image != default;
        }
    }
}
