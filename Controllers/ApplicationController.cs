using BlazorApi.DTO;
using BlazorApi.Models;
using BlazorApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace BlazorApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ApplicationController : Controller
    {
        private ApplicationService _ApplicationService;
        private readonly TemporaryCredentialsService _temporaryCredentialsService;
        public ApplicationController(ApplicationService ApplicationService, TemporaryCredentialsService temporaryCredentialsService)
        {
            _ApplicationService = ApplicationService;
            _temporaryCredentialsService = temporaryCredentialsService;

        }

        [HttpGet("GetAllApplications")]
        public async Task<ActionResult<List<Applications>>> GetAllApplications()
        {
            try
            {
                var application = await _ApplicationService.GetAllApplications();
                if (application == null)
                {
                    return BadRequest("Application Not Found");
                }
                return Ok(application);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal Server Error Occured, {ex.Message}.");
            } 

        }


          [HttpGet("GetApplicationById/{ApplicationId}")]
          public async Task<ActionResult<Applications>> GetApplicationById(int ApplicationId)
          {
              try
              {
                  var application = await _ApplicationService.GetApplicationById(ApplicationId);
                  if (application == null)
                  {
                      return NotFound("Application Does not Exist");
                  }
                  return Ok(application);
              }
              catch (Exception ex) 
              {
                  return StatusCode(500, $"Internal Server Error Occured, {ex.Message}.");

              }

          }



        [HttpDelete("Delete/{ApplicationId}")]
        public async Task<ActionResult> DeleteApplication(int ApplicationId)
        {
            try
            {
                var application = await _ApplicationService.DeleteApplication(ApplicationId);
                if(!application)
                {
                    return NotFound("Application Not Found");
                }
                return Ok(new { message = $"Application with ApplicationId {ApplicationId} Successufully Deleted" });
            }
            catch (Exception ex) 
            {
                return StatusCode (500, $"Internal Server Error, {ex.Message}. ");
            }
        }
        [HttpPost("ApplyJob")]
        public async Task<ActionResult<Applications>> ApplyJob([FromForm] ApplicationApplyDto applicationApplyDto)
        {
            try
            {
                if (applicationApplyDto.CoverLetter == null || applicationApplyDto.ResumePath == null)
                {
                    return BadRequest("Both Resume and Cover Letter are required.");
                }

                var application = await _ApplicationService.ApplyJob(applicationApplyDto);
                if (application == null)
                {
                    return NotFound("Application failed.");
                }

                return Ok(application);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in ApplyJob: {ex.Message}");
                return StatusCode(500, $"Internal Server Error: {ex.Message}");
            }
        }




        private async Task<string> SaveFileOnDisk(IFormFile file, string folderName)
        {
            if (file == null || file.Length == 0) return null;

            var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "Uploads", folderName);
            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }

            var uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
            var filePath = Path.Combine(uploadsFolder, uniqueFileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            return filePath; // Return the file path to store in the database
        }


        [HttpPut("EditApplication/{ApplicationId}")]
        public async Task<ActionResult<Applications>> EditApplication(int ApplicationId, ApplicationApplyDto applicationApplyDto)
        {
            try
            {
                var application = await _ApplicationService.EditApplication(ApplicationId, applicationApplyDto);
                if (application is null)
                {
                    return null;
                }
                return application;
            }
            catch (Exception ex) 
            {
                return StatusCode(500, $"Internal Server Error, {ex.Message}");
            }
        }

        [HttpPost("GenerateTemporaryCredentials")]
        public async Task<ActionResult> GenerateTemporaryCredentials(int UserId)
        {
            try
            {
                var result = await _temporaryCredentialsService.GenerateTemporaryCredentials(UserId);
                if (result == null)
                {
                    return NotFound("Applicant not found or failed to generate temporary credentials.");
                }
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}