using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication.Models;

namespace WebApplication.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class FilesController : ControllerBase
    {
        private readonly ApplicationDBContext context;
        private readonly IWebHostEnvironment appEnvironment;

        public FilesController(ApplicationDBContext context, IWebHostEnvironment appEnvironment)
        {
            this.context = context;
            this.appEnvironment = appEnvironment;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<WebApplication.Models.File>> GetFile(int id)
        {
            var file = await context.Files.FindAsync(id);

            if (file == null)
            {
                return NotFound();
            }

            return file;
        }

        [Route("userfiles")]
        //[Route("getuserfiles/userlogin-{userLogin}")]
        public async Task<ActionResult<List<WebApplication.Models.File>>> GetFiles([FromQuery] int userId)
        {
            User user = await context.Users.FirstOrDefaultAsync(user => user.Id == userId);

            if (user is null)
                return NotFound();

            List<Models.File> files = await context.Files.Where(file => file.UserId == user.Id).ToListAsync();
            
            files.Add(await context.GetDefaultPNGFileAsync());
            files.Add(await context.GetDefaultMP4FileAsync());


            return files;
        }

        [HttpPost, DisableRequestSizeLimit]
        [Route("uploadfile")]
        public async Task<IActionResult> UploadFile([FromQuery] int userId)
        {
            User user = await context.Users.FirstOrDefaultAsync(user => user.Id == userId);
            if (user is null)
                return NotFound();

            IFormFile file = Request.Form.Files[0];
            if (file == null)
                return BadRequest();

            DirectoryInfo dirInfo = 
                new DirectoryInfo($"{appEnvironment.WebRootPath}/Storages/{user.Id}");
            if (!dirInfo.Exists)
                dirInfo.Create();

            string filepath = $"/Storages/{user.Id}/{file.FileName}";
            using (var fileStream = new FileStream(appEnvironment.WebRootPath + filepath, FileMode.Create))
                await file.CopyToAsync(fileStream);

            WebApplication.Models.File newFile = new Models.File()
            {
                Path = filepath,
                Name = Path.GetFileNameWithoutExtension(filepath),
                Extension = Path.GetExtension(filepath),
                UserId = user.Id
            };

            await context.Files.AddAsync(newFile);
            await context.SaveChangesAsync();
            return Ok();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<Models.File>> DeleteFile(int id)
        {
            Models.File file = await context.Files.FindAsync(id);
            if (file == null)
                return NotFound();

            if (ApplicationDBContext.IsFileDefault(file))
                return BadRequest("The file can't be deleted.");

            if (await context.Ads.AnyAsync(ad=>ad.FileId == id))
                return BadRequest("The file cannot be deleted because it is being used in ad.");

            FileInfo fileInf = new FileInfo(appEnvironment.WebRootPath + file.Path);
            if (fileInf.Exists)
                fileInf.Delete();

            context.Files.Remove(file);
            await context.SaveChangesAsync();
            return file;
        }

        //// GET: api/Files
        //[HttpGet]
        //public async Task<ActionResult<IEnumerable<File>>> GetFile()
        //{
        //    return await _context.Files.ToListAsync();
        //}

        //// GET: api/Files/5
        //[HttpGet("{id}")]
        //public async Task<ActionResult<File>> GetFile(int id)
        //{
        //    var file = await _context.Files.FindAsync(id);

        //    if (file == null)
        //    {
        //        return NotFound();
        //    }

        //    return file;
        //}

        //// PUT: api/Files/5
        //// To protect from overposting attacks, enable the specific properties you want to bind to, for
        //// more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        //[HttpPut("{id}")]
        //public async Task<IActionResult> PutFile(int id, File file)
        //{
        //    if (id != file.Id)
        //    {
        //        return BadRequest();
        //    }

        //    _context.Entry(file).State = EntityState.Modified;

        //    try
        //    {
        //        await _context.SaveChangesAsync();
        //    }
        //    catch (DbUpdateConcurrencyException)
        //    {
        //        if (!FileExists(id))
        //        {
        //            return NotFound();
        //        }
        //        else
        //        {
        //            throw;
        //        }
        //    }

        //    return NoContent();
        //}

        //// POST: api/Files
        //// To protect from overposting attacks, enable the specific properties you want to bind to, for
        //// more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        //[HttpPost]
        //public async Task<ActionResult<File>> PostFile(File file)
        //{
        //    _context.Files.Add(file);
        //    await _context.SaveChangesAsync();

        //    return CreatedAtAction("GetFile", new { id = file.Id }, file);
        //}

        //// DELETE: api/Files/5
        //[HttpDelete("{id}")]
        //public async Task<ActionResult<File>> DeleteFile(int id)
        //{
        //    var file = await _context.Files.FindAsync(id);
        //    if (file == null)
        //    {
        //        return NotFound();
        //    }

        //    _context.Files.Remove(file);
        //    await _context.SaveChangesAsync();

        //    return file;
        //}

        //private bool FileExists(int id)
        //{
        //    return _context.Files.Any(e => e.Id == id);
        //}
    }
}
