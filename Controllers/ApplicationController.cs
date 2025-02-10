using BlazorApi.DTO;
using BlazorApi.Models;
using BlazorApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace BlazorApi.Controllers
{
    public class ApplicationController : Controller
    {
        private ApplicationService _ApplicationService;

        public ApplicationController(ApplicationService ApplicationService)
        {
            _ApplicationService = ApplicationService;
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
                      return NotFound("ApplicationDoes not Exist");
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
                return Ok(new { message = $"Job with JobId {ApplicationId} Successufully Deleted" });
            }
            catch (Exception ex) 
            {
                return StatusCode (500, $"Internal Server Error, {ex.Message}. ");
            }
        }

        [HttpPost("ApplyJob")]
        public async Task<ActionResult<Applications>> ApplyJob([FromForm]ApplicationApplyDto applicationApplyDto)
        {
            try
            {
                var application = await _ApplicationService.ApplyJob(applicationApplyDto);
                if(application == null)
                {
                    return NotFound("Application Doesn't Exist");
                }
                return Ok(application);
            }catch (Exception ex)
            {
                return StatusCode (500, $"Internal Server Error, Application was not Processed.  {ex.Message}" );
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

    }
}