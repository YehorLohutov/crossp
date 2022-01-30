using System;
using System.Collections.Generic;
using System.Text;

namespace Projects.Contracts
{
    public class GetProjectStatusRequest
    {
        public string ProjectExternalId { get; }
        public GetProjectStatusRequest(string projectExternalId)
        {
            if (string.IsNullOrWhiteSpace(projectExternalId))
                throw new ArgumentException($"Parameter {nameof(projectExternalId)} passed into {nameof(GetProjectStatusRequest)} is null or empty.");
            ProjectExternalId = projectExternalId;
        }
    }
}
