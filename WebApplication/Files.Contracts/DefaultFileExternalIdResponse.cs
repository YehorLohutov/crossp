using System;

namespace Files.Contracts
{
    public class DefaultFileExternalIdResponse
    {
        public string FileExternalId { get; }
        public DefaultFileExternalIdResponse(string fileExternalId)
        {
            if (string.IsNullOrEmpty(fileExternalId))
                throw new ArgumentException($"Parameter {nameof(fileExternalId)} passed into {nameof(DefaultFileExternalIdResponse)} is null or empty.");
            FileExternalId = fileExternalId;
        }
    }
}
