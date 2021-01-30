using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using WebApplication.Models;
using System.Text.Json;
using Microsoft.AspNetCore.Authorization;

namespace WebApplication.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Authorize]
    public class ProjectsController : ControllerBase
    {
        private readonly ApplicationDBContext context;

        public ProjectsController(ApplicationDBContext context)
        {
            this.context = context;
        }

        //GET: Projects
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Project>>> GetProjects() => await context.Projects.ToListAsync();
        

        // GET: Projects/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Project>> GetProject(int id)
        {
            var project = await context.Projects.FindAsync(id);

            if (project is null)
                return NotFound();

            return project;
        }

        // GET: Projects/create?userid={userid}
        [Route("create")]
        public async Task<ActionResult<Project>> CreateProject([FromQuery]int userId)
        {
            User user = await context.Users.FirstOrDefaultAsync(user => user.Id == userId);
            if (user is null)
                return BadRequest();

            Project newProject = new Project(user);
            await context.Projects.AddAsync(newProject);
            await context.SaveChangesAsync();
            return newProject;
        }

        // PUT: Projects/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutProject(int id, [FromBody] Project project)
        {
            if (id != project.Id)
                return BadRequest();

            context.Entry(project).State = EntityState.Modified;

            try
            {
                await context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProjectExists(id))
                    return NotFound();
                else
                    throw;
            }

            return NoContent();
        }

        // DELETE: Projects/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Project>> DeleteProject(int id)
        {
            var project = await context.Projects.FindAsync(id);
            if (project == null)
            {
                return NotFound();
            }

            context.Projects.Remove(project);
            await context.SaveChangesAsync();

            return project;
        }

        private bool ProjectExists(int id)
        {
            return context.Projects.Any(e => e.Id == id);
        }
    }
}
