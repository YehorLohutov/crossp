using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System;
using System.IO;
using Crossp.DataTransferObjects;

namespace Crossp
{
    public class Crossp : MonoBehaviour
    {
        [SerializeField]
        private CrosspSettings crosspSettings = default;
        public CrosspSettings CrosspSettings => crosspSettings;

        private List<AdData> availableAds = default;
        public bool IsReady => availableAds?.Count > 0;

        void Awake()
        {
            StartCoroutine(Initialize(
                loadedAds =>
                {
                    availableAds = loadedAds;
                    Debug.Log($"Crossp initialized. Project external id: {crosspSettings.ExternalId}. Available ads count: {availableAds.Count}.");
                },
                errorMessage => Debug.LogError($"Crossp initialization failed. Error: {errorMessage}")));
        }

        IEnumerator Initialize(Action<List<AdData>> onAdDatasLoaded, Action<string> onAdDatasFailedToLoad)
        {
            if (Application.internetReachability == NetworkReachability.NotReachable)
            {
                onAdDatasFailedToLoad("No internet connection.");
                yield break;
            }

            List<AdDto> ads = default;
            yield return StartCoroutine(GetAds(
                loadedAdDtos => ads = loadedAdDtos,
                errorMessage => onAdDatasFailedToLoad(errorMessage)));

            if (ads == default || ads.Count == 0)
            {
                onAdDatasFailedToLoad("Available ads are null or empty.");
                yield break;
            }

            List<AdData> availableAds = new List<AdData>();
            foreach (AdDto ad in ads)
            {
                byte[] fileData = default;
                yield return StartCoroutine(DownloadFile(ad, responseFileData => fileData = responseFileData));

                if (fileData != default)
                    availableAds.Add(new AdData(ad, fileData, AdData.FileType.Video));
            }

            if (availableAds.Count == 0)
                onAdDatasFailedToLoad("Available ads exist but ad data cannot be loaded.");
            else
                onAdDatasLoaded(availableAds);
        }

        IEnumerator GetAds(Action<List<AdDto>> onAdDtosLoaded, Action<string> onAdDtosFailedToLoad)
        {
            if (Application.internetReachability == NetworkReachability.NotReachable)
            {
                onAdDtosFailedToLoad("No internet connection.");
                yield break;
            }
            
            string url = $"{crosspSettings.ServerURL}/ads-management/projects/{crosspSettings.ExternalId}/ads?api-version={crosspSettings.ApiVersion}";

            using (UnityWebRequest request = UnityWebRequest.Get(url))
            {
                yield return request.SendWebRequest();

                if (!request.isHttpError && !request.isNetworkError)
                {
                    string responseJson = request.downloadHandler.text;
                    List<AdDto> deserializedAds = JsonUtility.FromJson<AdDtosWrapper>("{\"ads\":" + responseJson + "}").Ads;

                    onAdDtosLoaded(deserializedAds);
                }
                else
                {
                    onAdDtosFailedToLoad($"Request error when trying to get ads from {url}. Response code: {request.responseCode}. Error: {request.error}");
                }
            }
        }

        IEnumerator DownloadFile(AdDto ad, Action<byte[]> response)
        {
            if (Application.internetReachability == NetworkReachability.NotReachable)
            {
                response(default);
                yield break;
            }
            
            string url = $"{crosspSettings.ServerURL}/files-management/files/{ad.FileExternalId}/raw?api-version={crosspSettings.ApiVersion}";

            using (UnityWebRequest request = UnityWebRequest.Get(url))
            {
                yield return request.SendWebRequest();

                if (!request.isHttpError && !request.isNetworkError)
                {
                    byte[] fileData = request.downloadHandler.data;
                    response(fileData);
                }
                else
                {
                    Debug.LogError($"Request error when trying to download file data from {url}. File external id: {ad.FileExternalId}. Response code: {request.responseCode}. Error: {request.error}");
                    response(null);
                }
            }
        }

        public AdData GetRandomAvailableAd()
        {
            if (!IsReady)
                throw new System.Exception("No ads available");
            int randomIndex = UnityEngine.Random.Range(0, availableAds.Count);
            //
            StartCoroutine(AdShowReport(availableAds[randomIndex]));
            //
            return availableAds[randomIndex];
        }

        private IEnumerator AdShowReport(AdData availableAd)
        {
            if (Application.internetReachability == NetworkReachability.NotReachable)
                yield break;

            string url = $"{crosspSettings.ServerURL}/ads-management/projects/{availableAd.Ad.ProjectExternalId}/ads/{availableAd.Ad.ExternalId}/show?api-version={crosspSettings.ApiVersion}";
            UnityWebRequest request = UnityWebRequest.Post(url, "");

            yield return request.SendWebRequest();

            if (!request.isHttpError && !request.isNetworkError)
                Debug.Log($"Ad show report completed with response code: {request.responseCode}");
            else
                Debug.LogError($"Request error when trying to report an ad show. Url: {url}. Ad external id: {availableAd.Ad.ExternalId}. Response code: {request.responseCode}. Error: {request.error}");

            request.Dispose();
        }


        private IEnumerator AdClickReport(AdData availableAd)
        {
            if (Application.internetReachability == NetworkReachability.NotReachable)
                yield break;

            string url = $"{crosspSettings.ServerURL}/ads-management/projects/{availableAd.Ad.ProjectExternalId}/ads/{availableAd.Ad.ExternalId}/clicks?api-version={crosspSettings.ApiVersion}";
            UnityWebRequest request = UnityWebRequest.Post(url, "");

            yield return request.SendWebRequest();

            if (!request.isHttpError && !request.isNetworkError)
                Debug.Log($"Ad click report completed with response code: {request.responseCode}");
            else
                Debug.LogError($"Request error when trying to report an ad click. Url: {url}. Ad external id: {availableAd.Ad.ExternalId}. Response code: {request.responseCode}. Error: {request.error}");

            request.Dispose();
        }

        public void OpenAdUrl(AdData availableAd)
        {
            if (availableAd == null)
                throw new System.NullReferenceException();
            Application.OpenURL(availableAd.Ad.Url);
            StartCoroutine(AdClickReport(availableAd));
        }

        public Texture2D GetTexture2DFrom(AdData availableAd)
        {
            if (availableAd.Type != AdData.FileType.Image)
                throw new System.Exception("Incorrect AvailableAd FileType.");
            Texture2D texture2D = new Texture2D(0, 0);
            texture2D.LoadImage(availableAd.FileData);// LoadRawTextureData(pcxFile);
            texture2D.Apply();
            texture2D.EncodeToPNG();
            return texture2D;
        }
        public Sprite GetSpriteFrom(AdData availableAd)
        {
            Texture2D texture2D = GetTexture2DFrom(availableAd);
            Sprite sprite = Sprite.Create(texture2D, new Rect(0, 0, texture2D.width, texture2D.height), new Vector2(0.5f, 0.5f));
            return sprite;
        }

        public string GetVideoFilePathFrom(AdData availableAd)
        {
            if (availableAd.Type != AdData.FileType.Video)
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
