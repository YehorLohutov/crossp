using System;
using UnityEngine;

namespace Crossp
{
    [Serializable]
    public class Ad
    {
        [SerializeField]
        private int id = default;
        public int Id => id;

        [SerializeField]
        private string name = default;
        public string Name => name;

        [SerializeField]
        private string url = default;
        public string Url => url;

        [SerializeField]
        private File file = default;
        public File File => file;
    }

    [Serializable]
    public class File
    {
        [SerializeField]
        private int id = default;
        public int Id => id;

        [SerializeField]
        private string name = default;
        public string Name => name;

        [SerializeField]
        private string path = default;
        public string Path => path;

        [SerializeField]
        private string extension = default;
        public string Extension => extension;
    }

    public class AvailableAd
    {
        public enum FileType { Image, Video }
        public Ad Ad { get; private set; } = default;
        public byte[] FileData { get; private set; } = default;

        public FileType Type { get; private set; } = default;
        public AvailableAd(Ad ad, byte[] fileData, FileType type)
        {
            Ad = ad;
            FileData = fileData;
            Type = type;
        }
    }
}
