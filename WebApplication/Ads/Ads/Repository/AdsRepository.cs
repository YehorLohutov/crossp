using Ads.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;

namespace Ads.Repository
{
    public class AdsRepository : IAdsRepository
    {
        private readonly AdsDBContext adsDBContext = default;

        public AdsRepository(AdsDBContext adsDBContext)
        {
            this.adsDBContext = adsDBContext;
        }

        public async Task<IEnumerable<Ad>> GetAdsAsync(string projectExternalId)
        {
            if (string.IsNullOrWhiteSpace(projectExternalId))
                throw new ArgumentException($"Parameter {nameof(projectExternalId)} passed into {nameof(GetAdsAsync)} is null or empty.");

            return await adsDBContext.Ads.Where(ad => ad.ProjectExternalId.Equals(projectExternalId)).ToArrayAsync();
        }

        public async Task<IEnumerable<Ad>> GetAdsWhichUsingFileAsync(string fileExternalId)
        {
            if (string.IsNullOrWhiteSpace(fileExternalId))
                throw new ArgumentException($"Parameter {nameof(fileExternalId)} passed into {nameof(GetAdsWhichUsingFileAsync)} is null or empty.");

            return await adsDBContext.Ads.Where(ad => ad.FileExternalId.Equals(fileExternalId)).ToArrayAsync();
        }

        public async Task<Ad> GetAdAsync(string adExternalId)
        {
            if (string.IsNullOrWhiteSpace(adExternalId))
                throw new ArgumentException($"Parameter {nameof(adExternalId)} passed into {nameof(GetAdAsync)} is null or empty.");

            Ad ad = await adsDBContext.Ads.FirstOrDefaultAsync(t => t.ExternalId.Equals(adExternalId));

            if (ad == default)
                throw new NullReferenceException($"{nameof(Ad)} with {nameof(adExternalId)} = {adExternalId} not exist.");

            return ad;
        }

        public async Task<AdClicksStats> GetAdClicksStatsAsync(int adId, DateTime date)
        {
            AdClicksStats adClicksStats = await adsDBContext.AdClicksStats.FirstOrDefaultAsync(t => t.AdId == adId && t.Date.Date == date.Date);

            if (adClicksStats == default)
                throw new NullReferenceException($"{nameof(AdClicksStats)} with {nameof(adId)} = {adId} and {nameof(date)} = {date.Date} not exist.");

            return adClicksStats;
        }

        public async Task<AdShowStats> GetAdShowStatsAsync(int adId, DateTime date)
        {
            AdShowStats adShowStats = await adsDBContext.AdShowStats.FirstOrDefaultAsync(t => t.AdId == adId && t.Date.Date == date.Date);

            if (adShowStats == default)
                throw new NullReferenceException($"{nameof(AdShowStats)} with {nameof(adId)} = {adId} and {nameof(date)} = {date.Date} not exist.");

            return adShowStats;
        }

        public async Task<bool> AdExistsAsync(string adExternalId) =>
            await adsDBContext.Ads.AnyAsync(t => t.ExternalId.Equals(adExternalId));

        public async Task<bool> AdClicksStatsExistsAsync(int adId, DateTime date) =>
            await adsDBContext.AdClicksStats.AnyAsync(t => t.AdId == adId && t.Date.Date == date.Date);

        public async Task<bool> AdShowStatsExistsAsync(int adId, DateTime date) =>
            await adsDBContext.AdShowStats.AnyAsync(t => t.AdId == adId && t.Date.Date == date.Date);

        public void UpdateAd(Ad ad)
        {
            if (ad == default)
                throw new NullReferenceException($"Parameter {nameof(ad)} passed into {nameof(UpdateAd)} is null.");

            adsDBContext.Ads.Update(ad);
        }

        public void UpdateAdClicksStats(AdClicksStats adClicksStats)
        {
            if (adClicksStats == default)
                throw new NullReferenceException($"Parameter {nameof(adClicksStats)} passed into {nameof(UpdateAdClicksStats)} is null.");

            adsDBContext.AdClicksStats.Update(adClicksStats);
        }
        public void UpdateAdShowStats(AdShowStats adShowStats)
        {
            if (adShowStats == default)
                throw new NullReferenceException($"Parameter {nameof(adShowStats)} passed into {nameof(UpdateAdShowStats)} is null.");

            adsDBContext.AdShowStats.Update(adShowStats);
        }

        public async Task InsertAdAsync(Ad ad)
        {
            if (ad == default)
                throw new NullReferenceException($"Parameter {nameof(ad)} passed into {nameof(InsertAdAsync)} is null.");
            await adsDBContext.AddAsync(ad);
        }

        public async Task InsertAdClicksStatsAsync(AdClicksStats adClicksStats)
        {
            if (adClicksStats == default)
                throw new NullReferenceException($"Parameter {nameof(adClicksStats)} passed into {nameof(InsertAdClicksStatsAsync)} is null.");
            await adsDBContext.AddAsync(adClicksStats);
        }

        public async Task InsertAdShowStatsAsync(AdShowStats adShowStats)
        {
            if (adShowStats == default)
                throw new NullReferenceException($"Parameter {nameof(adShowStats)} passed into {nameof(InsertAdShowStatsAsync)} is null.");
            await adsDBContext.AddAsync(adShowStats);
        }

        public void DeleteAd(Ad ad)
        {
            if (ad == default)
                throw new NullReferenceException($"Parameter {nameof(ad)} passed into {nameof(DeleteAd)} is null.");

            adsDBContext.Ads.Remove(ad);
        }
        public async Task SaveChangesAsync() => await adsDBContext.SaveChangesAsync();
    }
}
