using System;

namespace Projects.Contracts
{
    public class ProjectDeletedMessage
    {
        public string ProjectExternalId { get; } = default;

        public ProjectDeletedMessage(string projectExternalId)
        {
            if (string.IsNullOrWhiteSpace(projectExternalId))
                throw new ArgumentException($"Invalid {nameof(projectExternalId)} passed into {nameof(ProjectDeletedMessage)} constructor.");

            ProjectExternalId = projectExternalId;
        }
    }
}
