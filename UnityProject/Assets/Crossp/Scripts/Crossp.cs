using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System;
using System.IO;

namespace Crossp
{
    public class Crossp : MonoBehaviour
    {
        [SerializeField]
        private CrosspSettings crosspSettings = default;
        public CrosspSettings CrosspSettings => crosspSettings;

        private List<AvailableAd> availableAds = default;
        public bool IsReady => availableAds?.Count > 0;

        void Awake()
        {
            StartCoroutine(GetAvailableAds(response => availableAds = response));
        }

        IEnumerator GetAvailableAds(Action<List<AvailableAd>> response)
        {
            if (Application.internetReachability == NetworkReachability.NotReachable)
            {
                response(default);
                yield break;
            }

            List<Ad> ads = default;
            yield return StartCoroutine(GetAds(responseAds => ads = responseAds));

            if (ads == default || ads.Count == 0)
            {
                response(default);
                yield break;
            }

            List<AvailableAd> availableAds = new List<AvailableAd>();
            foreach (Ad ad in ads)
            {
                byte[] fileData = default;
                yield return StartCoroutine(DownloadFile(ad.File.Id, responseFileData => fileData = responseFileData));

                if (fileData != default)
                    availableAds.Add(new AvailableAd(ad, fileData, ad.File.Extension.Equals(".mp4") ? AvailableAd.FileType.Video : AvailableAd.FileType.Image));
            }

            response(availableAds);
        }

        [Serializable]
        private class AdsWrapper
        {
            [SerializeField]
            private List<Ad> ads = new List<Ad>();
            public List<Ad> Ads => ads;
        }

        IEnumerator GetAds(Action<List<Ad>> response)
        {
            if (Application.internetReachability == NetworkReachability.NotReachable)
            {
                response(default);
                yield break;
            }

            string url = $"{crosspSettings.ServerURL}/Clients/availableads?api-version={crosspSettings.ApiVersion}&externalId={crosspSettings.ExternalId}";
            UnityWebRequest request = UnityWebRequest.Get(url);

            yield return request.SendWebRequest();

            if (!request.isHttpError && !request.isNetworkError)
            {
                string responseJson = request.downloadHandler.text;
                List<Ad> deserializedAds = JsonUtility.FromJson<AdsWrapper>("{\"ads\":" + responseJson + "}").Ads;
                response(deserializedAds);
            }
            else
            {
                Debug.LogError($"Get ads request error. Url: {url}. Error: {request.error}");
                response(default);
            }

            request.Dispose();
        }

        IEnumerator DownloadFile(int adFileId, Action<byte[]> response)
        {
            if (Application.internetReachability == NetworkReachability.NotReachable)
            {
                response(default);
                yield break;
            }

            string url = $"{crosspSettings.ServerURL}/Clients/adfile?api-version={crosspSettings.ApiVersion}&adFileId={adFileId}";
            UnityWebRequest request = UnityWebRequest.Get(url);

            yield return request.SendWebRequest();

            if (!request.isHttpError && !request.isNetworkError)
            {
                byte[] fileData = request.downloadHandler.data;
                response(fileData);
            }
            else
            {
                Debug.LogError($"DownloadFile request error. Url: {url}. Error: {request.error}.");
                response(null);
            }
            request.Dispose();
        }

        public AvailableAd GetRandomAvailableAd()
        {
            if (!IsReady)
                throw new System.Exception("No ads available");
            int randomIndex = UnityEngine.Random.Range(0, availableAds.Count);
            //
            StartCoroutine(AdShowReport(availableAds[randomIndex]));
            //
            return availableAds[randomIndex];
        }

        private IEnumerator AdShowReport(AvailableAd availableAd)
        {
            if (Application.internetReachability == NetworkReachability.NotReachable)
            {
                yield break;
            }

            string url = $"{crosspSettings.ServerURL}/Clients/adshowreport?api-version={crosspSettings.ApiVersion}&externalId={crosspSettings.ExternalId}&adId={availableAd.Ad.Id}";
            UnityWebRequest request = UnityWebRequest.Get(url);

            yield return request.SendWebRequest();

            if (!request.isHttpError && !request.isNetworkError)
            {
                Debug.Log($"AdShowReport response code {request.responseCode}");
            }
            else
            {
                Debug.LogError($"DownloadFile request error. Url: {url}. Error: {request.error}.");
            }
            request.Dispose();
        }


        private IEnumerator AdClickReport(AvailableAd availableAd)
        {
            if (Application.internetReachability == NetworkReachability.NotReachable)
            {
                yield break;
            }

            string url = $"{crosspSettings.ServerURL}/Clients/adclickreport?api-version={crosspSettings.ApiVersion}&externalId={crosspSettings.ExternalId}&adId={availableAd.Ad.Id}";
            UnityWebRequest request = UnityWebRequest.Get(url);

            yield return request.SendWebRequest();

            if (!request.isHttpError && !request.isNetworkError)
            {
                Debug.Log($"AdShowReport response code {request.responseCode}");
            }
            else
            {
                Debug.LogError($"DownloadFile request error. Url: {url}. Error: {request.error}.");
            }
            request.Dispose();
        }

        public void OpenAdUrl(AvailableAd availableAd)
        {
            if (availableAd == null)
                throw new System.NullReferenceException();
            Application.OpenURL(availableAd.Ad.Url);
            StartCoroutine(AdClickReport(availableAd));
        }

        public Texture2D GetTexture2DFrom(AvailableAd availableAd)
        {
            if (availableAd.Type != AvailableAd.FileType.Image)
                throw new System.Exception("Incorrect AvailableAd FileType.");
            Texture2D texture2D = new Texture2D(0, 0);
            texture2D.LoadImage(availableAd.FileData);// LoadRawTextureData(pcxFile);
            texture2D.Apply();
            texture2D.EncodeToPNG();
            return texture2D;
        }
        public Sprite GetSpriteFrom(AvailableAd availableAd)
        {
            Texture2D texture2D = GetTexture2DFrom(availableAd);
            Sprite sprite = Sprite.Create(texture2D, new Rect(0, 0, texture2D.width, texture2D.height), new Vector2(0.5f, 0.5f));
            return sprite;
        }

        public string GetVideoFilePathFrom(AvailableAd availableAd)
        {
            if (availableAd.Type != AvailableAd.FileType.Video)
                throw new System.Exception("Incorrect AvailableAd FileType.");

            string videoFilePath = $"{UnityEngine.Application.persistentDataPath}/crossptemp.mp4";

            if (System.IO.File.Exists(videoFilePath))
                System.IO.File.Delete(videoFilePath);

            using (FileStream fstream = new FileStream(videoFilePath, FileMode.Create))
            {
                fstream.Write(availableAd.FileData, 0, availableAd.FileData.Length);
            }

            return videoFilePath;
        }

    }
}
