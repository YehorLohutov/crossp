using System;

namespace Users.Contracts
{
    public class GetUserStatusRequest
    {
        public string UserExternalId { get; }
        public GetUserStatusRequest(string userExternalId)
        {
            if (string.IsNullOrEmpty(userExternalId))
                throw new ArgumentException($"Parameter {nameof(userExternalId)} passed into {nameof(GetUserStatusRequest)} is null or empty.");
            UserExternalId = userExternalId;
        }
    }
}
