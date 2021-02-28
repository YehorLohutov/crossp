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
                AvailableAd randomAd = crosspUnity.GetRandomAvailableAd();
                switch (randomAd.Type)
                {
                    case AvailableAd.FileType.Image:
                        rawImage.texture = CrosspUnity.GetTexture2DFrom(randomAd);
                        break;
                    case AvailableAd.FileType.Video:
                        rawImage.texture = CrosspSettings.Instance.RenderTexture;
                        videoPlayer.Stop();
                        videoPlayer.url = CrosspUnity.GetVideoFilePathFrom(randomAd);
                        videoPlayer.Prepare();
                        break;
                }
            }

            if (videoPlayer.isPrepared && !videoPlayer.isPlaying)
            {
                videoPlayer.Play();
            }
        }
    }
}
