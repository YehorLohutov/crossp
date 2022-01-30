using System;

namespace Projects.Contracts
{
    public class GetUserAccessToProjectRequest
    {
        public string UserExternalId { get; }
        public string ProjectExternalId { get; }
        public GetUserAccessToProjectRequest(string userExternalId, string projectExternalId)
        {
            if (string.IsNullOrEmpty(userExternalId))
                throw new ArgumentException($"Parameter {nameof(userExternalId)} passed into {nameof(GetUserAccessToProjectRequest)} is null or empty.");

            if (string.IsNullOrEmpty(projectExternalId))
                throw new ArgumentException($"Parameter {nameof(projectExternalId)} passed into {nameof(GetUserAccessToProjectRequest)} is null or empty.");

            UserExternalId = userExternalId;
            ProjectExternalId = projectExternalId;
        }
    }

}
