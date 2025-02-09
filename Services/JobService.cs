using BlazorApi.Data;
using BlazorApi.DTO;
using BlazorApi.Interfaces;
using BlazorApi.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


namespace BlazorApi.Services
{
    public class JobService : IJobs
    {
        private readonly DataContext _context;

        public JobService(DataContext context)
        {
            _context = context; 
        }


        public async Task<ActionResult<Jobs>> PostJob(JobsDtocs jobDto)
        {
            var isExistingJob = await _context.Jobs.AnyAsync(j => j.JobName == jobDto.JobName && j.CompanyId == jobDto.CompanyId);
            if (isExistingJob) {
                // Console.WriteLine("Job Already exists");
                return new ConflictObjectResult(new { message = "Job Already Exists" });
            }

            var job = new Jobs
            {
                JobName = jobDto.JobName,
                JobDescription = jobDto.JobDescription,
                JobRequirements = jobDto.JobRequirements,
                JobStatus = jobDto.JobStatus ?? "Not Applied",
                JobType = jobDto.JobType,
                PostingDate = jobDto.PostingDate,
                SalaryRange = jobDto.SalaryRange,
                ClosingDate = jobDto.ClosingDate,
                CompanyId = jobDto.CompanyId,
                IsDeleted = false,

            };
              
            await _context.Jobs.AddAsync(job);
            await _context.SaveChangesAsync();
            return job;

        }

        public async Task<bool> SoftDelete(int jobId)
        {
            var job = await _context.Jobs
                 .IgnoreQueryFilters()
                 .FirstOrDefaultAsync(j => j.JobId == jobId);

            if (job == null) return false;

            if (job.IsDeleted) return false;

            job.IsDeleted = true;
            await _context.SaveChangesAsync();
            return true;
        }



    }
}
