using BlazorApi.DTO;
using BlazorApi.Models;
using BlazorApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace BlazorApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class JobsController : Controller
    {
        private JobService _jobService;

        public JobsController( JobService jobService)
        {
            _jobService = jobService;
        }

        [HttpPost("/postjob")]
        public async Task<ActionResult<Jobs>> posJob(JobsDtocs jobsDtocs)
        {
            try
            {
                var results = await _jobService.PostJob (jobsDtocs);
                return Ok (results);
            }
            catch (Exception ex) { 
                throw new Exception(ex.Message);
            }
        }


        [HttpDelete("/{JobId}")]

        
        public async Task<IActionResult> SoftDelete(int JobId)
        {
            var result = await _jobService.SoftDelete(JobId);
            if (!result)
            {
                return NotFound(new { Message = "Job not found or already deleted." });
            }

            return Ok(new { Message = "Job successfully soft deleted." });
        }

    }
}
