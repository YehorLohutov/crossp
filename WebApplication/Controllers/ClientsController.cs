using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApplication.Models;
using Microsoft.EntityFrameworkCore;
using System.IO;
using Microsoft.AspNetCore.Hosting;

namespace WebApplication.Controllers
{
    [Route("[controller]")]
    [ApiController]
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

        //[HttpGet]
        //[Route("adfile")]
        //public async Task<ActionResult<FileResult>> GetAdFile([FromQuery] int adFileId)
        //{
        //    Models.File adFile = await context.Files.FirstOrDefaultAsync(f => f.Id == adFileId);
        //    if (adFile == null)
        //        return BadRequest();

        //    byte[] bytes = await System.IO.File.ReadAllBytesAsync(appEnvironment.WebRootPath + adFile.Path);

        //    return File(bytes, "application/octet-stream", "asdasds");
        //}

        //public FileResult DownloadFile(string fileName)
        //{
        //    //Build the File Path.
        //    string path = Path.Combine(this.Environment.WebRootPath, "Files/") + fileName;

        //    //Read the File data into Byte Array.
        //    byte[] bytes = System.IO.File.ReadAllBytes(path);

        //    //Send the File to Download.
        //    return File(bytes, "application/octet-stream", fileName);
        //}
    }
}
