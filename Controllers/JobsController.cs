using BlazorApi.DTO;
using BlazorApi.Interfaces;
using BlazorApi.Models;
using BlazorApi.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BlazorApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class JobsController : Controller
    {
        private JobService _jobService;
        private readonly IDjobService _DjobService;

        //public JobsController(JobService jobService)
        //{
        //    _jobService = jobService;
        //}
        public JobsController( IDjobService djobService) 
        {
            _DjobService = djobService;
        }

        [HttpPost("postjob")]
        public async Task<ActionResult<Jobs>> posJob(JobsDtocs jobsDtocs)
        {
            try
            {
                var results = await _DjobService.PostJob(jobsDtocs);
                return Ok(results);
            }
            catch (DbUpdateException ex)
            {
                return Conflict(new { message = "A database update error occurred.", details = ex.InnerException?.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }



        [HttpDelete("DeleteJob/{JobId}")]

        public async Task<ActionResult> DeleteJob(int JobId)
        {
            try
            {
                var job = await _DjobService.DeleteJob(JobId);
                if (!job)
                {
                    return NotFound("Job Not Found");
                }
                //  return Ok(job);

                return Ok(new { message = $"Job with JobId {JobId} Successufully Deleted" });

            }
            catch (Exception ex) { return BadRequest(new { Message = "Internal Server Error.", error = ex.Message }); }
        }

        [HttpGet("GetAllJobs")]
        public async Task<ActionResult<List<Jobs>>> GetAllJobs()
        {
            try
            {
                var allJobs = await _DjobService.GetAlLJobs();
                if (allJobs == null)
                {
                    return BadRequest("No Jobs Found");
                }
                return Ok(allJobs);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal Server Error: {ex.Message}");
            }

        }

        [HttpGet("GetSingleJob/{JobId}")]
        public async Task<ActionResult<Jobs>> GetSingleJob(int JobId)
        {
            try
            {
                var job = await _DjobService.GetSingleJob(JobId);
                return Ok(job);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal Server Error: {ex.Message}");
            }
        }

        [HttpPut("updateJobPost/{JobId}")]
        public async Task<ActionResult<Jobs>> UpdateJobPost( JobsDtocs jobsDtocs)
        {
            try
            {
                var job = await _DjobService.UpdateJob (jobsDtocs);
                return Ok(job);
            }
            catch (Exception ex)
            {
                return BadRequest("Job Does Not Exist");
            }
        }


        [HttpPost("SearchJobs")]
        public async Task<ActionResult<List<Jobs>>> SearchJobs(JobSearchDto jobSearchDto)
        {
            try
            {
                var job = await _jobService.SearchJobs(jobSearchDto);
                if (job == null)
                {
                    return NotFound(new { message = "No jobs found matching the search criteria." });

                }
                return Ok(job);
            }
            catch (Exception ex)
            {
                return BadRequest("Job Does Not Exist");
            }
        }


        [HttpGet("GetJobLink/{Jobink}")]
        public async Task<ActionResult<Jobs>> GetJobLink(string Jobink)
        {
            try
            {
                var jobLink = await _jobService.GetJobLink(Jobink);
                if (jobLink == null)
                {
                    return StatusCode(500, $"No JobLink Found");
                }
                return Ok(jobLink);
            }
            catch (Exception ex)
            {
                {
                    return BadRequest("No job LInk Available");
                }
            }

        }

        [HttpGet("job-details/{JobId}")]
        public async Task<ActionResult<Jobs>> GetJobDetails(int JobId)
        {
            try
            {
                var job = await _jobService.GetSingleJob(JobId);
                if (job == null) return NotFound("Job not found.");
                return Ok(job);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

    }
}
