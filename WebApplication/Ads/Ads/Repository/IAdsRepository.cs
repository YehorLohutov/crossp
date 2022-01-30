using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Ads.Models;

namespace Ads.Repository
{
    public interface IAdsRepository
    {
        Task<IEnumerable<Ad>> GetAdsAsync(string projectExternalId);
        Task<IEnumerable<Ad>> GetAdsWhichUsingFileAsync(string fileExternalId);
        Task<Ad> GetAdAsync(string adExternalId);

        Task<AdClicksStats> GetAdClicksStatsAsync(int adId, DateTime date);
        Task<AdShowStats> GetAdShowStatsAsync(int adId, DateTime date);

        Task<bool> AdExistsAsync(string adExternalId);
        Task<bool> AdClicksStatsExistsAsync(int adId, DateTime date);
        Task<bool> AdShowStatsExistsAsync(int adId, DateTime date);


        void UpdateAd(Ad project);
        void UpdateAdClicksStats(AdClicksStats adClicksStats);
        void UpdateAdShowStats(AdShowStats adShowStats);

        Task InsertAdAsync(Ad ad);
        void DeleteAd(Ad ad);

        Task InsertAdClicksStatsAsync(AdClicksStats adClicksStats);
        Task InsertAdShowStatsAsync(AdShowStats adShowStats);


        Task SaveChangesAsync();
    }
}
