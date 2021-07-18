using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication.Models;

namespace WebApplication.Controllers
{
    [ApiVersion("1.0")]
    [Authorize]
    [ApiController]
    [Route("V{version:apiVersion}/[controller]")]
    public class AdsController : ControllerBase
    {
        private readonly ApplicationDBContext context;
        private readonly IWebHostEnvironment _appEnvironment;

        public AdsController(ApplicationDBContext context, IWebHostEnvironment appEnvironment)
        {
            this.context = context;
            _appEnvironment = appEnvironment;
        }

        // GET: api/Ads
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Ad>>> GetAds()
        {
            return await context.Ads.ToListAsync();
        }

        [HttpGet]
        [Route("projectid-{projectId:int}")]
        public async Task<ActionResult<IEnumerable<Ad>>> GetAds(int projectId)
        {
            return await context.Ads.Where(ad => ad.ProjectId == projectId).ToListAsync();
        }

        // GET: api/Ads/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Ad>> GetAd(int id)
        {
            var ad = await context.Ads.FirstOrDefaultAsync(ad => ad.Id.Equals(id));

            if (ad == null)
            {
                return NotFound();
            }
            //if (ad.FileId == null)
            //{
            //    Models.File defaultFile = await _context.GetDefaultPNGFileAsync();
            //    ad.FileId = defaultFile.Id;
            //}

            return ad;
        }

        // PUT: api/Ads/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutAd(int id, [FromBody] Ad ad)
        {
            if (id != ad.Id)
            {
                return BadRequest();
            }

            context.Entry(ad).State = EntityState.Modified;

            try
            {
                await context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AdExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        [Route("Create/{id}")]
        public async Task<ActionResult<Ad>> CreateAd(int id)
        {
            var project = await context.Projects.FindAsync(id);

            if (project is null)
                return NotFound();

            Ad newAd = new Ad(ApplicationDBContext.DEFAULT_AD_NAME, ApplicationDBContext.DEFAULT_AD_URL, project);
            newAd.FileId = (await context.GetDefaultPNGFileAsync()).Id;

            await context.Ads.AddAsync(newAd);
            await context.SaveChangesAsync();
            return newAd;
        }






        // POST: api/Ads
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost]
        public async Task<ActionResult<Ad>> PostAd(Ad ad)
        {
            context.Ads.Add(ad);
            await context.SaveChangesAsync();

            return CreatedAtAction("GetAd", new { id = ad.Id }, ad);
        }

        // DELETE: api/Ads/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Ad>> DeleteAd(int id)
        {
            var ad = await context.Ads.FindAsync(id);
            if (ad == null)
            {
                return NotFound();
            }

            context.Ads.Remove(ad);
            await context.SaveChangesAsync();

            return ad;
        }

        private bool AdExists(int id)
        {
            return context.Ads.Any(e => e.Id == id);
        }

        [HttpGet]
        [Route("adclicksstats")]
        public async Task<ActionResult<IEnumerable<AdClicksStats>>> GetAdClicksStats([FromQuery] int adId)
        {
            Ad ad = await context.Ads.FirstOrDefaultAsync(ad => ad.Id == adId);
            if (ad == null)
                return BadRequest();

            AdClicksStats firstAdClicksStats = await context.AdClicksStats.Where(st => st.AdId == adId).OrderBy(o => o.Date).FirstOrDefaultAsync();
            if (firstAdClicksStats == null)
                return BadRequest();
            DateTime from = firstAdClicksStats.Date;

            AdClicksStats lastAdClicksStats = await context.AdClicksStats.Where(st => st.AdId == adId).OrderBy(o => o.Date).LastAsync();
            DateTime to = lastAdClicksStats.Date;

            List<AdClicksStats> result = new List<AdClicksStats>();

            for (DateTime temp = from; DateTime.Compare(temp, to) <= 0; temp = temp.AddDays(1))
            {
                AdClicksStats tempDateAdClicksStats = await context.AdClicksStats
                    .Where(st => st.AdId == adId && st.Date.Date == temp.Date)
                    .FirstOrDefaultAsync();
                if (tempDateAdClicksStats == default)
                    tempDateAdClicksStats = new AdClicksStats(temp, ad) { Number = 0 };
                result.Add(tempDateAdClicksStats);
            }
            return result;
        }

        [HttpGet]
        [Route("adclicksstatsrange")]
        public async Task<ActionResult<IEnumerable<AdClicksStats>>> GetAdClicksStatsRange([FromQuery] int adId, [FromQuery] DateTime from, [FromQuery] DateTime to)
        {
            Ad ad = await context.Ads.FirstOrDefaultAsync(ad => ad.Id == adId);
            if (ad == null)
                return BadRequest();

            if(DateTime.Compare(from, to) > 0)
                return BadRequest();

            List<AdClicksStats> result = new List<AdClicksStats>();



            for (DateTime temp = from; DateTime.Compare(temp, to) <= 0; temp = temp.AddDays(1))
            {
                AdClicksStats tempDateAdClicksStats = await context.AdClicksStats
                    .Where(st => st.AdId == adId && st.Date.Date == temp.Date)
                    .FirstOrDefaultAsync();
                if (tempDateAdClicksStats == default)
                    tempDateAdClicksStats = new AdClicksStats(temp, ad) { Number = 0 };
                result.Add(tempDateAdClicksStats);
            }
            return result;
        }





        [HttpGet]
        [Route("adshowstats")]
        public async Task<ActionResult<IEnumerable<AdShowStats>>> GetAdShowStats([FromQuery] int adId)
        {
            Ad ad = await context.Ads.FirstOrDefaultAsync(ad => ad.Id == adId);
            if (ad == null)
                return BadRequest();

            AdShowStats firstAdShowStats = await context.AdShowStats.Where(st => st.AdId == adId).OrderBy(o => o.Date).FirstOrDefaultAsync();
            if (firstAdShowStats == null)
                return BadRequest();
            DateTime from = firstAdShowStats.Date;

            AdShowStats lastAdShowStats = await context.AdShowStats.Where(st => st.AdId == adId).OrderBy(o => o.Date).LastAsync();
            DateTime to = lastAdShowStats.Date;

            List<AdShowStats> result = new List<AdShowStats>();

            for (DateTime temp = from; DateTime.Compare(temp, to) <= 0; temp = temp.AddDays(1))
            {
                AdShowStats tempDateAdClicksStats = await context.AdShowStats
                    .Where(st => st.AdId == adId && st.Date.Date == temp.Date)
                    .FirstOrDefaultAsync();
                if (tempDateAdClicksStats == default)
                    tempDateAdClicksStats = new AdShowStats(temp, ad) { Number = 0 };
                result.Add(tempDateAdClicksStats);
            }
            return result;
        }

        [HttpGet]
        [Route("adshowstatsrange")]
        public async Task<ActionResult<IEnumerable<AdShowStats>>> GetAdShowStatsRange([FromQuery] int adId, [FromQuery] DateTime from, [FromQuery] DateTime to)
        {
            Ad ad = await context.Ads.FirstOrDefaultAsync(ad => ad.Id == adId);
            if (ad == null)
                return BadRequest();

            if (DateTime.Compare(from, to) > 0)
                return BadRequest();

            List<AdShowStats> result = new List<AdShowStats>();



            for (DateTime temp = from; DateTime.Compare(temp, to) <= 0; temp = temp.AddDays(1))
            {
                AdShowStats tempDateAdClicksStats = await context.AdShowStats
                    .Where(st => st.AdId == adId && st.Date.Date == temp.Date)
                    .FirstOrDefaultAsync();
                if (tempDateAdClicksStats == default)
                    tempDateAdClicksStats = new AdShowStats(temp, ad) { Number = 0 };
                result.Add(tempDateAdClicksStats);
            }
            return result;
        }
    }
}
