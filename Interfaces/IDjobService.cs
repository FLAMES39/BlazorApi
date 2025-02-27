using BlazorApi.DTO;
using BlazorApi.Models;

namespace BlazorApi.Interfaces
{
    public interface IDjobService
    {
        Task<IEnumerable<Jobs>> GetAlLJobs();
        Task<Jobs?> GetSingleJob(int JobId);
        Task<Jobs> PostJob(JobsDtocs jobsDtocs);
        Task<bool> DeleteJob(int jobId);

        Task<Jobs> UpdateJob(JobsDtocs jobsDtocs);
    }
}
