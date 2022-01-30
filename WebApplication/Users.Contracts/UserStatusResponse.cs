using System;

namespace Users.Contracts
{
    public class UserStatusResponse
    {
        public string UserExternalId { get; }
        public bool Exists { get; }
        public UserStatusResponse(string userExternalId, bool exists)
        {
            if (string.IsNullOrEmpty(userExternalId))
                throw new ArgumentException($"Parameter {nameof(userExternalId)} passed into {nameof(UserStatusResponse)} is null or empty.");
            
            UserExternalId = userExternalId;
            Exists = exists;
        }
    }
}
