using System;

namespace Projects.Contracts
{
    public class UserAccessToProjectResponse
    {
        public string UserExternalId { get; }
        public string ProjectExternalId { get; }
        public bool AccessAllowed { get; }
        public UserAccessToProjectResponse(string userExternalId, string projectExternalId, bool accessAllowed)
        {
            if (string.IsNullOrEmpty(userExternalId))
                throw new ArgumentException($"Parameter {nameof(userExternalId)} passed into {nameof(GetUserAccessToProjectRequest)} is null or empty.");

            if (string.IsNullOrEmpty(projectExternalId))
                throw new ArgumentException($"Parameter {nameof(projectExternalId)} passed into {nameof(GetUserAccessToProjectRequest)} is null or empty.");

            UserExternalId = userExternalId;
            ProjectExternalId = projectExternalId;
            AccessAllowed = accessAllowed;
        }
    }
}
