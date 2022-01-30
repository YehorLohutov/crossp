using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Ads.Repository;
using Ads.Models;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Projects.Contracts;
using MassTransit.Mediator;
using MassTransit;
using Files.Contracts;
using AutoMapper;
using Ads.DataTransferObjects;

namespace Ads.Controllers
{
    [ApiVersion("1.0")]
    [Authorize]
    [ApiController]
    [Route("ads-management")]
    public class AdsController : ControllerBase
    {
        private readonly IConfiguration configuration;
        private readonly IAdsRepository adsRepository;
        private readonly IMediator mediator;
        private readonly IMapper mapper;

        public AdsController(IConfiguration configuration, IAdsRepository adsRepository, IMediator mediator, IMapper mapper)
        {
            this.configuration = configuration;
            this.adsRepository = adsRepository;
            this.mediator = mediator;
            this.mapper = mapper;
        }

        [HttpGet]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Get))]
        [AllowAnonymous]
        [Route("version")]
        public ActionResult<string> Version(ApiVersion apiVersion) => Ok($"Active {nameof(AdsController)} API ver is {apiVersion}");

        [HttpGet]
        [Route("users/{userid}/projects/{projectid}/ads")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<AdDto>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IEnumerable<AdDto>>> GetUserProjectAds(string userid, string projectid)
        {
            if (!ExternalIdPassedGuidValidation(userid))
                return StatusCode(StatusCodes.Status400BadRequest, $"Invalid {nameof(userid)}.");

            if (!ExternalIdPassedGuidValidation(projectid))
                return StatusCode(StatusCodes.Status400BadRequest, $"Invalid {nameof(projectid)}.");

            bool userHaveAccessToProject;
            try
            {
                userHaveAccessToProject = await UserHaveAccessToProject(userid, projectid); 
            } 
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex);
            }

            if (!userHaveAccessToProject)
                return StatusCode(StatusCodes.Status400BadRequest, "Access to project ads denied.");

            IEnumerable<Ad> ads;
            try
            {
                ads = await adsRepository.GetAdsAsync(projectid);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }

            IEnumerable<AdDto> adsDtos;
            try
            {
                adsDtos = mapper.Map<IEnumerable<AdDto>>(ads);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }

            return Ok(adsDtos);
        }

        [HttpGet]
        [Route("projects/{projectid}/ads")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<AdDto>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IEnumerable<AdDto>>> GetProjectAds(string projectid)
        {
            if (!ExternalIdPassedGuidValidation(projectid))
                return StatusCode(StatusCodes.Status400BadRequest, $"Invalid {nameof(projectid)}.");

            IEnumerable<Ad> ads;
            try
            {
                ads = await adsRepository.GetAdsAsync(projectid);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }

            IEnumerable<AdDto> adsDtos;
            try
            {
                adsDtos = mapper.Map<IEnumerable<AdDto>>(ads);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }

            return Ok(adsDtos);
        }

        [HttpGet]
        [Route("users/{userid}/projects/{projectid}/ads/{adid}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<AdDto>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<AdDto>> GetAd(string userid, string projectid, string adid)
        {
            if (!ExternalIdPassedGuidValidation(userid))
                return StatusCode(StatusCodes.Status400BadRequest, $"Invalid {nameof(userid)}.");

            if (!ExternalIdPassedGuidValidation(projectid))
                return StatusCode(StatusCodes.Status400BadRequest, $"Invalid {nameof(projectid)}.");

            if (!ExternalIdPassedGuidValidation(adid))
                return StatusCode(StatusCodes.Status400BadRequest, $"Invalid {nameof(adid)}.");

            bool userHaveAccessToProject;
            try
            {
                userHaveAccessToProject = await UserHaveAccessToProject(userid, projectid);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex);
            }

            if (!userHaveAccessToProject)
                return StatusCode(StatusCodes.Status400BadRequest, "Access to project ads denied.");

            Ad ad;
            try
            {
                ad = await adsRepository.GetAdAsync(adid);
            } 
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex);
            }

            AdDto adDto;
            try
            {
                adDto = mapper.Map<AdDto>(ad);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }

            return Ok(adDto);
        }

        [HttpPut]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Put))]
        [Route("users/{userid}/projects/{projectid}/ads")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> PutAd(string userid, string projectid, [FromBody] AdDto updatedAd)
        {
            if (!ExternalIdPassedGuidValidation(userid))
                return StatusCode(StatusCodes.Status400BadRequest, $"Invalid {nameof(userid)}.");

            if (!ExternalIdPassedGuidValidation(projectid))
                return StatusCode(StatusCodes.Status400BadRequest, $"Invalid {nameof(projectid)}.");

            bool userHaveAccessToProject;
            try
            {
                userHaveAccessToProject = await UserHaveAccessToProject(userid, projectid);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex);
            }

            if (!userHaveAccessToProject)
                return StatusCode(StatusCodes.Status400BadRequest, "Access to project ads denied.");

            Ad ad;
            try
            {
                ad = await adsRepository.GetAdAsync(updatedAd.ExternalId);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status400BadRequest, ex.Message);
            }

            if (!ad.ExternalId.Equals(updatedAd.ExternalId) || !ad.ProjectExternalId.Equals(updatedAd.ProjectExternalId))
                return StatusCode(StatusCodes.Status400BadRequest, "Updated ad external id or project id missmatch.");

            try
            {
                ad.Name = updatedAd.Name;
                ad.Url = updatedAd.Url;
                ad.FileExternalId = updatedAd.FileExternalId;
                await adsRepository.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }

            return Ok();
        }

        [HttpPost]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Post))]
        [Route("users/{userid}/projects/{projectid}/ads")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<AdDto>> CreateAd(string userid, string projectid)
        {
            if (!ExternalIdPassedGuidValidation(userid))
                return StatusCode(StatusCodes.Status400BadRequest, $"Invalid {nameof(userid)}.");

            if (!ExternalIdPassedGuidValidation(projectid))
                return StatusCode(StatusCodes.Status400BadRequest, $"Invalid {nameof(projectid)}.");

            bool userHaveAccessToProject;
            try
            {
                userHaveAccessToProject = await UserHaveAccessToProject(userid, projectid);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex);
            }

            if (!userHaveAccessToProject)
                return StatusCode(StatusCodes.Status400BadRequest, "Access to project ads denied.");

            Ad newAd;
            try
            {
                IRequestClient<GetDefaultFileExternalIdRequest> client = mediator.CreateRequestClient<GetDefaultFileExternalIdRequest>();
                Response<DefaultFileExternalIdResponse> response = await client.GetResponse<DefaultFileExternalIdResponse>(new GetDefaultFileExternalIdRequest());
                string defaultFileExternalId = response.Message.FileExternalId;


                newAd = new Ad(projectid, defaultFileExternalId);

                await adsRepository.InsertAdAsync(newAd);
                await adsRepository.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }

            AdDto adDto;
            try
            {
                adDto = mapper.Map<AdDto>(newAd);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }

            return Ok(adDto);
        }

        [HttpDelete]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Delete))]
        [Route("users/{userid}/projects/{projectid}/ads/{adid}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(AdDto))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<AdDto>> DeleteAd(string userid, string projectid, string adid)
        {
            if (!ExternalIdPassedGuidValidation(userid))
                return StatusCode(StatusCodes.Status400BadRequest, $"Invalid {nameof(userid)}.");

            if (!ExternalIdPassedGuidValidation(projectid))
                return StatusCode(StatusCodes.Status400BadRequest, $"Invalid {nameof(projectid)}.");

            if (!ExternalIdPassedGuidValidation(adid))
                return StatusCode(StatusCodes.Status400BadRequest, $"Invalid {nameof(adid)}.");

            Ad ad;
            try
            {
                ad = await adsRepository.GetAdAsync(adid);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status400BadRequest, ex.Message);
            }

            bool userHaveAccessToProject;
            try
            {
                userHaveAccessToProject = await UserHaveAccessToProject(userid, projectid);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex);
            }

            if (!userHaveAccessToProject)
                return StatusCode(StatusCodes.Status400BadRequest, "Access to project ads denied.");

            try
            {
                adsRepository.DeleteAd(ad);
                await adsRepository.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }

            AdDto adDto;
            try
            {
                adDto = mapper.Map<AdDto>(ad);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }

            return Ok(adDto);
        }

        [HttpPost]
        [AllowAnonymous]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Post))]
        [Route("projects/{projectid}/ads/{adid}/clicks")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> AdClickReport(string projectid, string adid)
        {
            if (!ExternalIdPassedGuidValidation(projectid))
                return StatusCode(StatusCodes.Status400BadRequest, $"Invalid {nameof(projectid)}.");

            if (!ExternalIdPassedGuidValidation(adid))
                return StatusCode(StatusCodes.Status400BadRequest, $"Invalid {nameof(adid)}.");

            try 
            {
                if (!await ProjectExists(projectid))
                    return StatusCode(StatusCodes.Status400BadRequest, "Unknown project.");
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status400BadRequest, ex);
            }

            try
            {
                if (!await adsRepository.AdExistsAsync(adid))
                    return StatusCode(StatusCodes.Status400BadRequest, "Unknown ad.");
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status400BadRequest, ex);
            }

            Ad ad;
            try
            {
                ad = await adsRepository.GetAdAsync(adid);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }

            DateTime date = DateTime.UtcNow.Date;
            
            try 
            {
                bool adClicksStatsExists = await adsRepository.AdClicksStatsExistsAsync(ad.Id, date);
                if (!adClicksStatsExists)
                {
                    AdClicksStats adClicksStats = new AdClicksStats()
                    {
                        AdId = ad.Id,
                        Date = date,
                        Number = 1
                    };
                    await adsRepository.InsertAdClicksStatsAsync(adClicksStats);
                }
                else
                {
                    AdClicksStats adClicksStats = await adsRepository.GetAdClicksStatsAsync(ad.Id, date);
                    adClicksStats.Number++;
                }
                await adsRepository.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }

            return Ok();
        }

        [HttpPost]
        [AllowAnonymous]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Post))]
        [Route("projects/{projectid}/ads/{adid}/show")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> AdShowReport(string projectid, string adid)
        {
            if (!ExternalIdPassedGuidValidation(projectid))
                return StatusCode(StatusCodes.Status400BadRequest, $"Invalid {nameof(projectid)}.");

            if (!ExternalIdPassedGuidValidation(adid))
                return StatusCode(StatusCodes.Status400BadRequest, $"Invalid {nameof(adid)}.");

            try
            {
                if (!await ProjectExists(projectid))
                    return StatusCode(StatusCodes.Status400BadRequest, "Unknown project.");
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status400BadRequest, ex);
            }

            try
            {
                if (!await adsRepository.AdExistsAsync(adid))
                    return StatusCode(StatusCodes.Status400BadRequest, "Unknown ad.");
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status400BadRequest, ex);
            }

            Ad ad;
            try
            {
                ad = await adsRepository.GetAdAsync(adid);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }

            DateTime date = DateTime.UtcNow.Date;

            try
            {
                bool adShowStatsExists = await adsRepository.AdShowStatsExistsAsync(ad.Id, date);
                if (!adShowStatsExists)
                {
                    AdShowStats adShowStats = new AdShowStats()
                    {
                        AdId = ad.Id,
                        Date = date,
                        Number = 1
                    };
                    await adsRepository.InsertAdShowStatsAsync(adShowStats);
                }
                else
                {
                    AdShowStats adShowStats = await adsRepository.GetAdShowStatsAsync(ad.Id, date);
                    adShowStats.Number++;
                }

                await adsRepository.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }

            return Ok();
        }

        //[HttpGet]
        //[Route("adclicksstats")]
        //public async Task<ActionResult<IEnumerable<AdClicksStats>>> GetAdClicksStats([FromQuery] int adId)
        //{
        //    Ad ad = await context.Ads.FirstOrDefaultAsync(ad => ad.Id == adId);
        //    if (ad == null)
        //        return BadRequest();

        //    AdClicksStats firstAdClicksStats = await context.AdClicksStats.Where(st => st.AdId == adId).OrderBy(o => o.Date).FirstOrDefaultAsync();
        //    if (firstAdClicksStats == null)
        //        return BadRequest();
        //    DateTime from = firstAdClicksStats.Date;

        //    AdClicksStats lastAdClicksStats = await context.AdClicksStats.Where(st => st.AdId == adId).OrderBy(o => o.Date).LastAsync();
        //    DateTime to = lastAdClicksStats.Date;

        //    List<AdClicksStats> result = new List<AdClicksStats>();

        //    for (DateTime temp = from; DateTime.Compare(temp, to) <= 0; temp = temp.AddDays(1))
        //    {
        //        AdClicksStats tempDateAdClicksStats = await context.AdClicksStats
        //            .Where(st => st.AdId == adId && st.Date.Date == temp.Date)
        //            .FirstOrDefaultAsync();
        //        if (tempDateAdClicksStats == default)
        //            tempDateAdClicksStats = new AdClicksStats(temp, ad) { Number = 0 };
        //        result.Add(tempDateAdClicksStats);
        //    }
        //    return result;
        //}

        //[HttpGet]
        //[Route("adclicksstatsrange")]
        //public async Task<ActionResult<IEnumerable<AdClicksStats>>> GetAdClicksStatsRange([FromQuery] int adId, [FromQuery] DateTime from, [FromQuery] DateTime to)
        //{
        //    Ad ad = await context.Ads.FirstOrDefaultAsync(ad => ad.Id == adId);
        //    if (ad == null)
        //        return BadRequest();

        //    if (DateTime.Compare(from, to) > 0)
        //        return BadRequest();

        //    List<AdClicksStats> result = new List<AdClicksStats>();



        //    for (DateTime temp = from; DateTime.Compare(temp, to) <= 0; temp = temp.AddDays(1))
        //    {
        //        AdClicksStats tempDateAdClicksStats = await context.AdClicksStats
        //            .Where(st => st.AdId == adId && st.Date.Date == temp.Date)
        //            .FirstOrDefaultAsync();
        //        if (tempDateAdClicksStats == default)
        //            tempDateAdClicksStats = new AdClicksStats(temp, ad) { Number = 0 };
        //        result.Add(tempDateAdClicksStats);
        //    }
        //    return result;
        //}





        //[HttpGet]
        //[Route("adshowstats")]
        //public async Task<ActionResult<IEnumerable<AdShowStats>>> GetAdShowStats([FromQuery] int adId)
        //{
        //    Ad ad = await context.Ads.FirstOrDefaultAsync(ad => ad.Id == adId);
        //    if (ad == null)
        //        return BadRequest();

        //    AdShowStats firstAdShowStats = await context.AdShowStats.Where(st => st.AdId == adId).OrderBy(o => o.Date).FirstOrDefaultAsync();
        //    if (firstAdShowStats == null)
        //        return BadRequest();
        //    DateTime from = firstAdShowStats.Date;

        //    AdShowStats lastAdShowStats = await context.AdShowStats.Where(st => st.AdId == adId).OrderBy(o => o.Date).LastAsync();
        //    DateTime to = lastAdShowStats.Date;

        //    List<AdShowStats> result = new List<AdShowStats>();

        //    for (DateTime temp = from; DateTime.Compare(temp, to) <= 0; temp = temp.AddDays(1))
        //    {
        //        AdShowStats tempDateAdClicksStats = await context.AdShowStats
        //            .Where(st => st.AdId == adId && st.Date.Date == temp.Date)
        //            .FirstOrDefaultAsync();
        //        if (tempDateAdClicksStats == default)
        //            tempDateAdClicksStats = new AdShowStats(temp, ad) { Number = 0 };
        //        result.Add(tempDateAdClicksStats);
        //    }
        //    return result;
        //}

        //[HttpGet]
        //[Route("adshowstatsrange")]
        //public async Task<ActionResult<IEnumerable<AdShowStats>>> GetAdShowStatsRange([FromQuery] int adId, [FromQuery] DateTime from, [FromQuery] DateTime to)
        //{
        //    Ad ad = await context.Ads.FirstOrDefaultAsync(ad => ad.Id == adId);
        //    if (ad == null)
        //        return BadRequest();

        //    if (DateTime.Compare(from, to) > 0)
        //        return BadRequest();

        //    List<AdShowStats> result = new List<AdShowStats>();



        //    for (DateTime temp = from; DateTime.Compare(temp, to) <= 0; temp = temp.AddDays(1))
        //    {
        //        AdShowStats tempDateAdClicksStats = await context.AdShowStats
        //            .Where(st => st.AdId == adId && st.Date.Date == temp.Date)
        //            .FirstOrDefaultAsync();
        //        if (tempDateAdClicksStats == default)
        //            tempDateAdClicksStats = new AdShowStats(temp, ad) { Number = 0 };
        //        result.Add(tempDateAdClicksStats);
        //    }
        //    return result;
        //}

        private async Task<bool> UserHaveAccessToProject(string userExternalId, string projectExternalId)
        {
            IRequestClient<GetUserAccessToProjectRequest> client = mediator.CreateRequestClient<GetUserAccessToProjectRequest>();
            Response<UserAccessToProjectResponse> response = await client.GetResponse<UserAccessToProjectResponse>(new GetUserAccessToProjectRequest(userExternalId, projectExternalId));
            return response.Message.AccessAllowed;
        }
        private async Task<bool> ProjectExists(string projectExternalId)
        {
            IRequestClient<GetProjectStatusRequest> client = mediator.CreateRequestClient<GetProjectStatusRequest>();
            Response<ProjectStatusResponse> response = await client.GetResponse<ProjectStatusResponse>(new GetProjectStatusRequest(projectExternalId));
            return response.Message.Exists;
        }
        private bool ExternalIdPassedGuidValidation(string externalId) => Guid.TryParse(externalId, out _);
    }
}
