using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System;
using System.IO;

namespace Crossp
{
    public class Crossp
    {
        private const string API_CONTROLLER = "Clients";
        private string serverURL = default;
        private string externalId = default;

        private List<AvailableAd> availableAds = new List<AvailableAd>();

        public Crossp(string serverURL, string externalId)
        {
            this.serverURL = serverURL;
            this.externalId = externalId;
            Task.Run(Run);
        }
        private async void Run()
        {
            foreach(Ad ad in await GetAds())
            {
                byte[] fileData = await DownloadFile(ad.File.Id);
                availableAds.Add(new AvailableAd(ad, fileData, ad.File.Extension.Equals(".mp4") ? AvailableAd.FileType.Video : AvailableAd.FileType.Image));
            }
        }
        private async Task<List<Ad>> GetAds()
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

        //private static async Task<byte[]> DownloadFile(string url) {
        //    Uri uri = new Uri(url);
        //    byte[] file = default;
        //    using (WebClient webClient = new WebClient())
        //    {
        //        file = await webClient.DownloadDataTaskAsync(uri);
        //    }
        //    return file;
        //}

        private async Task<byte[]> DownloadFile(int adFileId)
        {
            byte[] file = default;
            using (HttpClient httpClient = new HttpClient())
            {
                string url = $"{serverURL}/{API_CONTROLLER}/adfile?adFileId={adFileId}";
                HttpResponseMessage httpResponseMessage = await httpClient.GetAsync(url);
                file = await httpResponseMessage.Content.ReadAsByteArrayAsync();
            }
            return file;
        }

        public bool IsReady()
        {
            return availableAds?.Count > 0;
        }

        public AvailableAd GetRandomAvailableAd()
        {
            if (!IsReady())
                throw new System.Exception("No ads available");
            Random random = new Random();
            int randomIndex = random.Next(0, availableAds.Count);
            return availableAds[randomIndex];
        }

    }
}
