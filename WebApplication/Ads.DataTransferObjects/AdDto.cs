namespace Ads.DataTransferObjects
{
    public class AdDto
    {
        public string ExternalId { get; set; }
        public string Name { get; set; }
        public string Url { get; set; }
        public string ProjectExternalId { get; set; } = default;
        public string FileExternalId { get; set; } = default;
    }
}
