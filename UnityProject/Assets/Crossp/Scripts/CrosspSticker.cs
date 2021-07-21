using UnityEngine;
using UnityEngine.Video;
using UnityEngine.UI;

namespace Crossp
{
    public class CrosspSticker : MonoBehaviour
    {
        private RawImage rawImage = default;
        private VideoPlayer videoPlayer = default;
        [SerializeField]
        private Crossp crossp = default;



        private AvailableAd currentAd = default;

        void Awake()
        {
            rawImage = GetComponentInChildren<RawImage>();
            videoPlayer = GetComponentInChildren<VideoPlayer>();
        }

        void Update()
        {
            
            if (Input.GetMouseButtonDown(0) && crossp.IsReady)
            {
                currentAd = crossp.GetRandomAvailableAd();
                switch (currentAd.Type)
                {
                    case AvailableAd.FileType.Image:
                        rawImage.texture = crossp.GetTexture2DFrom(currentAd);
                        break;
                    case AvailableAd.FileType.Video:
                        rawImage.texture = crossp.CrosspSettings.RenderTexture;
                        videoPlayer.Stop();
                        videoPlayer.url = crossp.GetVideoFilePathFrom(currentAd);
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
            crossp.OpenAdUrl(currentAd);
        }
    }
}
