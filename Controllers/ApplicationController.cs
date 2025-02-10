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
}
}