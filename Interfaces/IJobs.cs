
using BlazorApi.Models;
using Microsoft.AspNetCore.Mvc;
using BlazorApi.DTO;
namespace BlazorApi.Interfaces
{
    public interface IJobs
    {
        Task<ActionResult<Jobs>> PostJob(JobsDtocs jobsDtocs);
        Task<bool> DeleteJob(int JobId);
        Task<ActionResult<List<Jobs>>> GetAllJobs();
        Task<ActionResult<Jobs>> GetSingleJob(int JobId);
        Task<ActionResult<List<Jobs>>> UpdateJobPost(int JobId, JobsDtocs jobsDtocs);

        Task<ActionResult<List<Jobs>>> SearchJobs(JobSearchDto jobSearchDto);

    }

}
