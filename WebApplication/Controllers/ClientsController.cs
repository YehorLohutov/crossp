using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApplication.Models;
using Microsoft.EntityFrameworkCore;

namespace WebApplication.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class ClientsController : ControllerBase
    {
        private readonly ApplicationDBContext _context;

        public ClientsController(ApplicationDBContext context)
        {
            _context = context;
        }
        // GET: api/Ads
        [HttpGet]
        [Route("availableads")]
        public async Task<ActionResult<List<Ad>>> GetAds([FromQuery] string externalId)
        {
            Project project = await _context.Projects.FirstOrDefaultAsync(proj => proj.ExternalId.Equals(externalId));
            if (project == null)
                return BadRequest();

            List<Ad> availableAds = await _context.Ads.Include(s => s.File).Where(ad => ad.ProjectId == project.Id).ToListAsync();
            return availableAds;
        }
    }
}
