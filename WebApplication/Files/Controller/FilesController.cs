using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using Files.Models;
using Files.Repository;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using Files.Contracts;
using MassTransit.Mediator;
using MassTransit;
using Users.Contracts;
using Files.DataTransferObjects;
using AutoMapper;

namespace Files.Controller
{
    [ApiVersion("1.0")]
    [Authorize]
    [ApiController]
    [Route("files-management")]
    public class FilesController : ControllerBase
    {
        private readonly IFilesRepository databaseRepository;
        private readonly IWebHostEnvironment appEnvironment;
        private readonly IMediator mediator;
        private readonly IMapper mapper;

        public FilesController(IFilesRepository databaseRepository, IWebHostEnvironment appEnvironment, IMediator mediator, IMapper mapper)
        {
            this.databaseRepository = databaseRepository;
            this.appEnvironment = appEnvironment;
            this.mediator = mediator;
            this.mapper = mapper;
        }

        [HttpGet]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Get))]
        [AllowAnonymous]
        [Route("version")]
        public ActionResult<string> Version(ApiVersion apiVersion) => Ok($"Active {nameof(FilesController)} API ver is {apiVersion}");

        [HttpGet]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Get))]
        [Route("users/{userid}/files/{fileid}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(FileDto))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<FileDto>> GetFile(string userid, string fileid)
        {
            if (!ExternalIdPassedGuidValidation(userid))
                return StatusCode(StatusCodes.Status400BadRequest, $"Invalid {nameof(userid)}.");

            if (!ExternalIdPassedGuidValidation(fileid))
                return StatusCode(StatusCodes.Status400BadRequest, $"Invalid {nameof(fileid)}.");

            try
            {
                if (!await UserExists(userid))
                    return StatusCode(StatusCodes.Status400BadRequest, "Unknown user external id.");
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }

            Files.Models.File file;
            try
            {
                file = await databaseRepository.GetFileAsync(fileid);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status400BadRequest, ex.Message);
            }

            if (!UserHaveAccessToFile(userid, file))
                return StatusCode(StatusCodes.Status400BadRequest, $"Access denied to file with external id: {fileid}.");

            FileDto fileDto;
            try
            {
                fileDto = mapper.Map<FileDto>(file);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }

            return Ok(fileDto);
        }

        [HttpGet]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Get))]
        [Route("users/{userid}/files")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<FileDto>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<IEnumerable<FileDto>>> GetUserFiles(string userid)
        {
            if (!ExternalIdPassedGuidValidation(userid))
                return StatusCode(StatusCodes.Status400BadRequest, $"Invalid {nameof(userid)}.");

            try
            {
                if (!await UserExists(userid))
                    return StatusCode(StatusCodes.Status400BadRequest, "Unknown user external id.");
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }

            IEnumerable<Files.Models.File> files;
            try
            {
                files = await databaseRepository.GetUserFilesAsync(userid);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }

            IEnumerable<FileDto> filesDtos;
            try
            {
                filesDtos = mapper.Map<IEnumerable<FileDto>>(files);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }

            return Ok(filesDtos);
        }

        [HttpPost, DisableRequestSizeLimit]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Post))]
        [Route("users/{userid}/files")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(FileDto))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<FileDto>> UploadFile(string userid)
        {
            if (!ExternalIdPassedGuidValidation(userid))
                return StatusCode(StatusCodes.Status400BadRequest, $"Invalid {nameof(userid)}.");

            try
            {
                if (!await UserExists(userid))
                    return StatusCode(StatusCodes.Status400BadRequest, "Unknown user external id.");
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }

            IFormFile file;
            try
            {
                file = Request.Form.Files[0];
                if (file == default)
                    throw new NullReferenceException("File in request body does not exists.");
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status400BadRequest, ex.Message);
            }

            string filepath;
            try
            {
                System.IO.DirectoryInfo dirInfo =
                    new System.IO.DirectoryInfo($"{appEnvironment.WebRootPath}/Storages/{userid}");

                if (!dirInfo.Exists)
                    dirInfo.Create();

                filepath = $"/Storages/{userid}/{file.FileName}";
                using (System.IO.FileStream fileStream = new System.IO.FileStream(appEnvironment.WebRootPath + filepath, System.IO.FileMode.Create))
                    await file.CopyToAsync(fileStream);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }

            Files.Models.File newFile;
            try 
            {           
                newFile = new Files.Models.File(filepath) { UserExternalId = userid };
                await databaseRepository.InsertFileAsync(newFile);
                await databaseRepository.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
            
            FileDto fileDto;
            try
            {
                fileDto = mapper.Map<FileDto>(newFile);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }

            return Ok(fileDto);
        }

        [HttpDelete]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Delete))]
        [Route("users/{userid}/files/{fileid}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(FileDto))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<FileDto>> DeleteFile(string userid, string fileid)
        {
            if (!ExternalIdPassedGuidValidation(userid))
                return StatusCode(StatusCodes.Status400BadRequest, $"Invalid {nameof(userid)}.");

            if (!ExternalIdPassedGuidValidation(fileid))
                return StatusCode(StatusCodes.Status400BadRequest, $"Invalid {nameof(fileid)}.");

            Files.Models.File file;
            try
            {
                file = await databaseRepository.GetFileAsync(fileid);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status400BadRequest, ex.Message);
            }

            if (databaseRepository.IsFileDefault(file))
                return StatusCode(StatusCodes.Status400BadRequest, "The file can't be deleted.");

            try
            {         
                if (!await UserExists(userid))
                    return StatusCode(StatusCodes.Status400BadRequest, "Unknown user external id.");
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }

            if (!UserHaveAccessToFile(userid, file))
                return StatusCode(StatusCodes.Status400BadRequest, $"Access denied to file with external id: {fileid}.");

            try
            {                
                await mediator.Publish<FileDeletedMessage>(new FileDeletedMessage(fileid));
                
                System.IO.FileInfo fileInf = new System.IO.FileInfo(appEnvironment.WebRootPath + file.Path);
                if (fileInf.Exists)
                    fileInf.Delete();

                databaseRepository.DeleteFile(file);            
                await databaseRepository.SaveChangesAsync();      
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }

            FileDto fileDto;
            try
            {
                fileDto = mapper.Map<FileDto>(file);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }

            return Ok(fileDto);
        }

        [HttpGet]
        [AllowAnonymous]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Get))]
        [Route("files/{fileid}/raw")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(FileContentResult))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> GetRawFile(string fileid)
        {
            if (!ExternalIdPassedGuidValidation(fileid))
                return StatusCode(StatusCodes.Status400BadRequest, $"Invalid {nameof(fileid)}.");

            Files.Models.File adFile;
            try
            {
                adFile = await databaseRepository.GetFileAsync(fileid);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex);
            }

            FileContentResult fileContentResult;
            try
            {
                byte[] fileData = await System.IO.File.ReadAllBytesAsync(appEnvironment.WebRootPath + adFile.Path);
                string fileType = //"image/jpeg";
                    "application/octet-stream";
                string fileName = //$"{adFile.Name}.jpg";
                    $"{adFile.Name}";
                fileContentResult = File(fileData, fileType, fileName);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex);
            }

            return fileContentResult;
        }

        private bool UserHaveAccessToFile(string userExternalId, Files.Models.File file) => file.UserExternalId.Equals(userExternalId);
        private async Task<bool> UserExists(string userExternalId)
        {
            IRequestClient<GetUserStatusRequest> client = mediator.CreateRequestClient<GetUserStatusRequest>();
            Response<UserStatusResponse> response = await client.GetResponse<UserStatusResponse>(new GetUserStatusRequest(userExternalId));
            return response.Message.Exists;
        }

        private bool ExternalIdPassedGuidValidation(string externalId) => Guid.TryParse(externalId, out _);
    }
}
