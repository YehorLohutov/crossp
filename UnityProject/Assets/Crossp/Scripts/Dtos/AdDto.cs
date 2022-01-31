using System;
using UnityEngine;

namespace Crossp.DataTransferObjects
{
    [Serializable]
    public class AdDto
    {
        [SerializeField]
        private string externalId = default;
        public string ExternalId => externalId;

        [SerializeField]
        private string name = default;
        public string Name => name;

        [SerializeField]
        private string url = default;
        public string Url => url;

        [SerializeField]
        private string projectExternalId = default;
        public string ProjectExternalId => projectExternalId;

        [SerializeField]
        private string fileExternalId = default;
        public string FileExternalId => fileExternalId;
    }
}
