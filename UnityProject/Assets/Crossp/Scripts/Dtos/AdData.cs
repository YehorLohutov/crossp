namespace Crossp.DataTransferObjects
{
    public class AdData
    {
        public enum FileType { Image, Video }
        public AdDto Ad { get; private set; }
        public byte[] FileData { get; private set; }
        public FileType Type { get; private set; }

        public AdData(AdDto ad, byte[] fileData, FileType type)
        {
            Ad = ad;
            FileData = fileData;
            Type = type;
        }
    }
}
