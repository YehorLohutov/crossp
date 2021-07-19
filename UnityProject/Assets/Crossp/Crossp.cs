using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Crossp;
using UnityEngine.Networking;
using System;
using Newtonsoft.Json;

namespace NewCrossp
{
    public class Crossp : MonoBehaviour
    {
        [SerializeField]
        private CrosspSettings crosspSettings = default;




        void Awake()
        {
            StartCoroutine(GetAds((ads) =>
            {
                foreach (var ad in ads)
                    Debug.Log(ad.Name);
            }));
        }

        IEnumerator GetAvailableAds(Action<List<AvailableAd>> response)
        {
            List<Ad> ads = default;
            yield return StartCoroutine(GetAds(response => ads = response));

            List<AvailableAd> availableAds = new List<AvailableAd>();
            foreach (Ad ad in ads)
            {
                byte[] fileData = default;
                yield return StartCoroutine(DownloadFile(ad.File.Id, response => fileData = response));
                availableAds.Add(new AvailableAd(ad, fileData, ad.File.Extension.Equals(".mp4") ? AvailableAd.FileType.Video : AvailableAd.FileType.Image));
            }

        }

        IEnumerator GetAds(Action<List<Ad>> response)
        {
            string url = $"{crosspSettings.ServerURL}/Clients/availableads?externalId={crosspSettings.ExternalId}";
            UnityWebRequest request = UnityWebRequest.Get(url);

            yield return request.SendWebRequest();

            if (!request.isHttpError && !request.isNetworkError)
            {
                string responseJson = request.downloadHandler.text;
                List<Ad> deserializedAds = JsonConvert.DeserializeObject<List<Ad>>(responseJson);
                response(deserializedAds);
            }
            else
            {
                Debug.LogErrorFormat("error request [{0}, {1}]", url, request.error);
                response(null);
            }

            request.Dispose();
        }





        IEnumerator DownloadFile(int adFileId, Action<byte[]> response)
        {
            string url = $"{crosspSettings.ServerURL}/Clients/adfile?adFileId={adFileId}";
            UnityWebRequest request = UnityWebRequest.Get(url);

            yield return request.SendWebRequest();

            if (!request.isHttpError && !request.isNetworkError)
            {
                byte[] fileData = request.downloadHandler.data;
                response(fileData);
            }
            else
            {
                Debug.LogErrorFormat("error request [{0}, {1}]", url, request.error);
                response(null);
            }

            request.Dispose();
        }




    }
}
