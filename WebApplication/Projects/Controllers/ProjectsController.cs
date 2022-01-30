using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Projects.Repository;
using Projects.Models;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Projects.Contracts;
using MassTransit.Mediator;
using MassTransit;
using Users.Contracts;
using Projects.DataTransferObjects;
using AutoMapper;

namespace Projects.Controllers
{
    [ApiVersion("1.0")]
    [Authorize]
    [ApiController]
    [Route("projects-management")]
    public class ProjectsController : ControllerBase
    {
        private readonly IConfiguration configuration;
        private readonly IProjectsRepository databaseRepository;
        private readonly IMediator mediator;
        private readonly IMapper mapper;

        public ProjectsController(IConfiguration configuration, IProjectsRepository databaseRepository, IMediator mediator, IMapper mapper)
        {
            this.configuration = configuration;
            this.databaseRepository = databaseRepository;
            this.mediator = mediator;
            this.mapper = mapper;
        }

        [HttpGet]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Get))]
        [AllowAnonymous]
        [Route("version")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(string))]
        public ActionResult<string> Version(ApiVersion apiVersion) => Ok($"Active {nameof(ProjectsController)} API ver is {apiVersion}");

        [HttpGet]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Get))]
        [Route("users/{userid}/projects")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<ProjectDto>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IEnumerable<ProjectDto>>> GetUserProjects(string userid)
        {
            if (!ExternalIdPassedGuidValidation(userid))
                return StatusCode(StatusCodes.Status400BadRequest, "Invalid user external id.");

            try
            {       
                if (!await UserExists(userid))
                    return StatusCode(StatusCodes.Status400BadRequest, "Unknown user external id.");
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }

            IEnumerable<Project> projects;
            try
            {
                projects = await databaseRepository.GetUserProjectsAsync(userid);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status400BadRequest, ex.Message);
            }

            IEnumerable<ProjectDto> projectsDtos;
            try
            {
                projectsDtos = mapper.Map<IEnumerable<ProjectDto>>(projects);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }

            return Ok(projectsDtos);
        }


        [HttpGet]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Get))]
        [Route("users/{userid}/projects/{projectid}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ProjectDto))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ProjectDto>> GetProject(string userid, string projectid)
        {
            if (!ExternalIdPassedGuidValidation(userid))
                return StatusCode(StatusCodes.Status400BadRequest, $"Invalid {nameof(userid)}.");

            if (!ExternalIdPassedGuidValidation(projectid))
                return StatusCode(StatusCodes.Status400BadRequest, $"Invalid {nameof(projectid)}.");

            Project project;
            try
            {
                project = await databaseRepository.GetProjectAsync(projectid);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status400BadRequest, ex.Message);
            }

            if (!UserHaveAccessToProject(userid, project))
                return StatusCode(StatusCodes.Status400BadRequest, $"User ({nameof(userid)} = {userid}) does not have access to the project ({nameof(projectid)} = {projectid}).");

            ProjectDto projectDto;
            try
            {
                projectDto = mapper.Map<ProjectDto>(project);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }

            return Ok(projectDto);
        }

        [HttpPut]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Put))]
        [Route("users/{userid}/projects")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ActionResult))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> PutProject(string userid, [FromBody] ProjectDto updatedProject)
        {
            if (!ExternalIdPassedGuidValidation(userid))
                return StatusCode(StatusCodes.Status400BadRequest, $"Invalid {nameof(userid)}.");

            if (!ExternalIdPassedGuidValidation(updatedProject.ExternalId))
                return StatusCode(StatusCodes.Status400BadRequest, "Invalid projectid.");

            Project project;
            try
            {
                project = await databaseRepository.GetProjectAsync(updatedProject.ExternalId);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status400BadRequest, ex.Message);
            }

            if (!UserHaveAccessToProject(userid, project))
                return StatusCode(StatusCodes.Status400BadRequest, $"Access denied to project with external id: {project.ExternalId}.");

            try
            {
                project.Name = updatedProject.Name;
                await databaseRepository.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }

            return Ok();
        }

        [HttpPost]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Post))]
        [Route("users/{userid}/projects")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ProjectDto))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ProjectDto>> CreateProject(string userid)
        {
            if (!ExternalIdPassedGuidValidation(userid))
                return StatusCode(StatusCodes.Status400BadRequest, "Invalid user external id.");

            try
            {         
                if (!await UserExists(userid))
                    return StatusCode(StatusCodes.Status400BadRequest, "Unknown user external id.");
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }

            Project newProject;
            try
            {
                newProject = new Project(userid);
                await databaseRepository.InsertProjectAsync(newProject);
                await databaseRepository.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }

            ProjectDto projectDto;
            try
            {
                projectDto = mapper.Map<ProjectDto>(newProject);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }

            return Ok(projectDto);
        }

        [HttpDelete]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Delete))]
        [Route("users/{userid}/projects/{projectid}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ProjectDto))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ProjectDto>> DeleteProject(string userid, string projectid)
        {
            if (!ExternalIdPassedGuidValidation(userid))
                return StatusCode(StatusCodes.Status400BadRequest, $"Invalid {nameof(userid)}.");

            if (!ExternalIdPassedGuidValidation(projectid))
                return StatusCode(StatusCodes.Status400BadRequest, $"Invalid {nameof(projectid)}.");

            Project project;
            try
            {
                project = await databaseRepository.GetProjectAsync(projectid);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status400BadRequest, ex.Message);
            }

            if (!UserHaveAccessToProject(userid, project))
                return StatusCode(StatusCodes.Status400BadRequest, $"Access denied to project with external id: {projectid}.");

            try
            {
                databaseRepository.DeleteProject(project);
                await mediator.Publish<ProjectDeletedMessage>(new ProjectDeletedMessage(projectid));
                await databaseRepository.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }

            ProjectDto projectDto;
            try
            {
                projectDto = mapper.Map<ProjectDto>(project);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }

            return Ok(projectDto);
        }

        private bool UserHaveAccessToProject(string userExternalId, Project project) => project.UserExternalId.Equals(userExternalId);
    
        private async Task<bool> UserExists(string userExternalId)
        {
            IRequestClient<GetUserStatusRequest> client = mediator.CreateRequestClient<GetUserStatusRequest>();
            Response<UserStatusResponse> response = await client.GetResponse<UserStatusResponse>(new GetUserStatusRequest(userExternalId));
            return response.Message.Exists;
        }

        private bool ExternalIdPassedGuidValidation(string externalId) => Guid.TryParse(externalId, out _); 
    }
}
