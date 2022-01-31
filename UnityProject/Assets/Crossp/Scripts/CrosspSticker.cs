using UnityEngine;
using UnityEngine.Video;
using UnityEngine.UI;
using Crossp.DataTransferObjects;

namespace Crossp
{
    public class CrosspSticker : MonoBehaviour
    {
        [SerializeField]
        private Crossp crossp = default;
        [SerializeField]
        private GameObject visual = default;
        [SerializeField]
        private RawImage rawImage = default;
        [SerializeField]
        private VideoPlayer videoPlayer = default;

        private AdData currentAd = default;

        void Update()
        {
            if (!visual.activeInHierarchy)
                return;

            if (videoPlayer.isPrepared && !videoPlayer.isPlaying)
            {
                videoPlayer.Play();
            }
        }
        public void Show()
        {
            visual.SetActive(true);
        }

        public void Hide()
        {
            videoPlayer.Stop();
            visual.SetActive(false);
        }

        public void Play()
        {
            if (!crossp.IsReady)
                return;

            currentAd = crossp.GetRandomAvailableAd();
            switch (currentAd.Type)
            {
                case AdData.FileType.Image:
                    rawImage.texture = crossp.GetTexture2DFrom(currentAd);
                    break;
                case AdData.FileType.Video:
                    rawImage.texture = crossp.CrosspSettings.RenderTexture;
                    videoPlayer.Stop();
                    videoPlayer.url = crossp.GetVideoFilePathFrom(currentAd);
                    videoPlayer.Prepare();
                    break;
            }
        }

        public void OnStickerClick() => crossp.OpenAdUrl(currentAd);
    }
}
