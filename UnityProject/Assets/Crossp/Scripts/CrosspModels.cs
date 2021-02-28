using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Crossp
{
    public class Ad
    {
        public int Id { get; set; } = default;
        public string Name { get; set; } = default;
        public string Url { get; set; } = default;
        public File File { get; set; } = default;
    }
    public class File
    {
        public int Id { get; set; } = default;
        public string Name { get; set; } = default;
        public string Path { get; set; } = default;
        public string Extension { get; set; } = default;
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
