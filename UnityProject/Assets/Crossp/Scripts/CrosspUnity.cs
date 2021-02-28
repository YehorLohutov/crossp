using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

namespace Crossp {
    public class CrosspUnity : Crossp
    {
        public CrosspUnity(string serverURL, string externalId) : base(serverURL, externalId)
        {
        }

        public static Texture2D GetTexture2DFrom(AvailableAd availableAd)
        {
            if (availableAd.Type != AvailableAd.FileType.Image)
                throw new System.Exception("Incorrect AvailableAd FileType.");
            Texture2D texture2D = new Texture2D(0, 0);
            texture2D.LoadImage(availableAd.FileData);// LoadRawTextureData(pcxFile);
            texture2D.Apply();
            texture2D.EncodeToPNG();
            return texture2D;
        }
        public static Sprite GetSpriteFrom(AvailableAd availableAd)
        {
            Texture2D texture2D = GetTexture2DFrom(availableAd);
            Sprite sprite = Sprite.Create(texture2D, new Rect(0, 0, texture2D.width, texture2D.height), new Vector2(0.5f, 0.5f));
            return sprite;
        }

        public static string GetVideoFilePathFrom(AvailableAd availableAd)
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
