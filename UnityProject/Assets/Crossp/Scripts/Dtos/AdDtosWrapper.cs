using System.Collections.Generic;
using System;
using UnityEngine;

namespace Crossp.DataTransferObjects
{
    [Serializable]
    public class AdDtosWrapper
    {
        [SerializeField]
        private List<AdDto> ads = new List<AdDto>();
        public List<AdDto> Ads => ads;
    }
}