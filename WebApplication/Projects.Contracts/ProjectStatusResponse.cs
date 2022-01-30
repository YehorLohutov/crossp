using System;

namespace Projects.Contracts
{
    public class ProjectStatusResponse
    {
        public string ProjectExternalId { get; }
        public bool Exists { get; }
        public ProjectStatusResponse(string projectExternalId, bool exists)
        {
            if (string.IsNullOrWhiteSpace(projectExternalId))
                throw new ArgumentException($"Parameter {nameof(projectExternalId)} passed into {nameof(ProjectStatusResponse)} is null or empty.");

            ProjectExternalId = projectExternalId;
            Exists = exists;
        }
    }
}
