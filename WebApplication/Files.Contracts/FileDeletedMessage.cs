using System;

namespace Files.Contracts
{
    public class FileDeletedMessage
    {
        public string FileExternalId { get; } = default;

        public FileDeletedMessage(string fileExternalId)
        {
            if (string.IsNullOrEmpty(fileExternalId))
                throw new ArgumentException($"Invalid {nameof(fileExternalId)} passed into {nameof(FileDeletedMessage)} constructor.");

            FileExternalId = fileExternalId;
        }
    }
}
