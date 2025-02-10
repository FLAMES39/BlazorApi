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


        public async Task<Jobs> PostJob(JobsDtocs jobDto)
        {
            var isExistingJob = await _context.Jobs.AnyAsync(j => j.JobName == jobDto.JobName && j.CompanyId == jobDto.CompanyId);
            if (isExistingJob) {
                 Console.WriteLine("Job Already exists");
               // return new ConflictObjectResult(new { message = "Job Already Exists" });
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

        public async Task<bool> DeleteJob(int JobId)
        {
            var job = await _context.Jobs.FirstOrDefaultAsync(j => j.JobId ==JobId);
            if (job == null) return false;

            job.IsDeleted = true;
            await _context.SaveChangesAsync();
            return true;
        }


        public async Task<List<Jobs>> GetAllJobs()
        {

            var jobs = await _context.Jobs
                .Where(j => j.IsDeleted == j.IsDeleted)
                .ToListAsync();


            return jobs;
        }

        public async Task<ActionResult<Jobs>> GetSingleJob( int JobId)
        {
            try
            {
                var job = await _context.Jobs.FirstOrDefaultAsync(j => j.JobId == JobId);

                if (job is null)
                {
                    return new ConflictObjectResult(new { message = "Job Doesn't Exist" });
                }
                return job;
            }
            catch (Exception ex) {
                return  new ObjectResult(new { message = " An Error Occured while getting Jobs", error = ex.Message });
            }

        }

        public async Task<ActionResult<List<Jobs>>> UpdateJobPost( int JobId, JobsDtocs jobsDtocs)
        {
            var job = await _context.Jobs.FirstOrDefaultAsync(j =>j.JobId ==  JobId && !j.IsDeleted);
            if (job is null) {
                return new ObjectResult(new { message = "Job Not Found" });
            }

            if (jobsDtocs.ClosingDate < jobsDtocs.PostingDate) 
            {
                Console.WriteLine("ClosingDate Cannot be Earlier than PostingDta");
            }
            job.JobName = jobsDtocs.JobName?? job.JobName;
            job.JobDescription = jobsDtocs.JobDescription?? job.JobDescription;
            job.JobRequirements = jobsDtocs.JobRequirements?? job.JobRequirements;
            job.JobType = jobsDtocs.JobType?? job.JobType;
            job.JobDescription = jobsDtocs.JobDescription ?? job.JobDescription;
            job.SalaryRange = jobsDtocs.SalaryRange?? job.SalaryRange;
            job.PostingDate = jobsDtocs.PostingDate;
            job.ClosingDate = jobsDtocs.ClosingDate;


            await _context.SaveChangesAsync();
            var updatedJob = await _context.Jobs.Where(j => j.JobId == JobId).ToListAsync();
            return updatedJob;

        }

        public async Task<ActionResult<List<Jobs>>> SearchJobs(JobSearchDto jobSearchDto)
        {
            var jobFound =  _context.Jobs.AsQueryable();
            if (!string.IsNullOrEmpty(jobSearchDto.JobName)) {
                jobFound = jobFound.Where(j => j.JobName.Contains(jobSearchDto.JobName));
            }
            if (jobSearchDto.CompanyId != null) { 
                jobFound = jobFound.Where(j => j.CompanyId ==  jobSearchDto.CompanyId.Value);
            }
            if (jobSearchDto.JobType != null) {
                jobFound = jobFound.Where(j => j.JobType == jobSearchDto.JobType);
            }
            
            var job = await jobFound.ToListAsync();
            if (job is null || job.Count == 0) {
                return new NotFoundObjectResult(new { message = " No Jbs Available at The mMoment" });
            }
            return job;
        }
    }
}
