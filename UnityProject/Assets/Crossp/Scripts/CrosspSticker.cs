using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using UnityEngine.UI;

namespace Crossp
{
    public class CrosspSticker : MonoBehaviour
    {
        private RawImage rawImage = default;
        private VideoPlayer videoPlayer = default;
        private CrosspUnity crosspUnity = default;

        private AvailableAd currentAd = default;

        void Awake()
        {
            rawImage = GetComponentInChildren<RawImage>();
            videoPlayer = GetComponentInChildren<VideoPlayer>();
            crosspUnity = new CrosspUnity(CrosspSettings.Instance.ServerURL, CrosspSettings.Instance.ExternalId);
        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space) && crosspUnity.IsReady())
            {
                currentAd = crosspUnity.GetRandomAvailableAd();
                switch (currentAd.Type)
                {
                    case AvailableAd.FileType.Image:
                        rawImage.texture = CrosspUnity.GetTexture2DFrom(currentAd);
                        break;
                    case AvailableAd.FileType.Video:
                        rawImage.texture = CrosspSettings.Instance.RenderTexture;
                        videoPlayer.Stop();
                        videoPlayer.url = CrosspUnity.GetVideoFilePathFrom(currentAd);
                        videoPlayer.Prepare();
                        break;
                }
            }

            if (videoPlayer.isPrepared && !videoPlayer.isPlaying)
            {
                videoPlayer.Play();
            }
        }

        public void OpenUrl()
        {
            crosspUnity.OpenAdUrl(currentAd, (result) => {
                Debug.Log($"Ad id: {currentAd.Ad.Id} report result {result}");
            });
            //Application.OpenURL(currentAd.Ad.Url);
        }
    }
}
