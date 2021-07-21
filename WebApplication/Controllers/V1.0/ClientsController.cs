﻿using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApplication.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Hosting;

namespace WebApplication.Controllers.Version_1_0
{
    [ApiVersion("1.0")]
    [ApiController]
    [Route("[controller]")]
    public class ClientsController : ControllerBase
    {
        private readonly ApplicationDBContext context;
        private readonly IWebHostEnvironment appEnvironment;

        public ClientsController(ApplicationDBContext context, IWebHostEnvironment appEnvironment)
        {
            this.context = context;
            this.appEnvironment = appEnvironment;
        }
        
        [HttpGet]
        [Route("availableads")]
        public async Task<ActionResult<List<Ad>>> GetAds([FromQuery] string externalId)
        {
            Project project = await context.Projects.FirstOrDefaultAsync(proj => proj.ExternalId.Equals(externalId));
            if (project == null)
                return BadRequest();

            List<Ad> availableAds = await context.Ads.Include(s => s.File).Where(ad => ad.ProjectId == project.Id).ToListAsync();
            return availableAds;
        }


        [HttpGet]
        [Route("adfile")]
        public async Task<FileContentResult> GetAdFile([FromQuery] int adFileId)
        {
            Models.File adFile = await context.Files.FirstOrDefaultAsync(f => f.Id == adFileId);
            if (adFile == null)
                return null;// BadRequest();

            byte[] bytes = await System.IO.File.ReadAllBytesAsync(appEnvironment.WebRootPath + adFile.Path);

            string file_type = //"image/jpeg";
            "application/octet-stream";
            string file_name = $"{adFile.Name}.jpg";

            FileContentResult fileContentResult = File(bytes, file_type, file_name);
            return fileContentResult;
        }

        [HttpGet]
        [Route("adclickreport")]
        public async Task<IActionResult> AdClickReport([FromQuery] string externalId, [FromQuery] int adId)
        {
            Project project = await context.Projects.FirstOrDefaultAsync(proj => proj.ExternalId.Equals(externalId));
            if (project == null)
                return BadRequest();

            Models.Ad ad = await context.Ads.FirstOrDefaultAsync(a => a.Id == adId && a.ProjectId == project.Id);
            if (ad == null)
                return BadRequest();

            DateTime date = DateTime.UtcNow.Date;
            AdClicksStats adClicksStats = await context.AdClicksStats.FirstOrDefaultAsync(st => st.AdId == adId && st.Date.Date == date.Date);
            if (adClicksStats == null)
            {
                adClicksStats = new AdClicksStats()
                {
                    AdId = adId,
                    Date = date,
                    Number = 1
                };
                await context.AdClicksStats.AddAsync(adClicksStats);
            }
            else
            {
                adClicksStats.Number++;
            }

            await context.SaveChangesAsync();
            return Ok();
        }

        [HttpGet]
        [Route("adshowreport")]
        public async Task<IActionResult> AdShowReport([FromQuery] string externalId, [FromQuery] int adId)
        {
            Project project = await context.Projects.FirstOrDefaultAsync(proj => proj.ExternalId.Equals(externalId));
            if (project == null)
                return BadRequest();

            Models.Ad ad = await context.Ads.FirstOrDefaultAsync(a => a.Id == adId && a.ProjectId == project.Id);
            if (ad == null)
                return BadRequest();

            DateTime date = DateTime.UtcNow.Date;
            AdShowStats adShowStats = await context.AdShowStats.FirstOrDefaultAsync(st => st.AdId == adId && st.Date.Date == date.Date);
            if (adShowStats == null)
            {
                adShowStats = new AdShowStats()
                {
                    AdId = adId,
                    Date = date,
                    Number = 1
                };
                await context.AdShowStats.AddAsync(adShowStats);
            }
            else
            {
                adShowStats.Number++;
            }

            await context.SaveChangesAsync();
            return Ok();
        }
    }
}
