using BlazorApi.DTO;
using BlazorApi.Interfaces;
using BlazorApi.Models;
using Dapper;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using MySql.Data.MySqlClient;
using Org.BouncyCastle.Bcpg;
using System.Data;

namespace BlazorApi.DataService
{
    public class DjobService : IDjobService
    {
        private readonly ILogger<DjobService> _logger;
        private readonly string _connectionstring;
        public DjobService(string connectionstring, ILogger<DjobService> logger)
        {
            _connectionstring = connectionstring ?? throw new ArgumentNullException(nameof(connectionstring));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }
        private IDbConnection CreateConnection()
        {
            var connection = new MySqlConnection(_connectionstring);
            connection.Open();
            return connection;
        }
        public async Task<IEnumerable<Jobs>> GetAlLJobs()
        {
            try
            {
                using var connection = CreateConnection();
                const string query = "SELECT * FROM Jobs";
                var jobs = await connection.QueryAsync<Jobs>(query);
                return jobs.ToList();
            }
            catch(Exception  ex)
            {
                _logger.LogError(ex, "No jobs Available");
                throw;
            }
        }

        public async Task<Jobs?> GetSingleJob(int JobId)
        {
            try
            {
                using var connection = CreateConnection();
                const string query = "SELECT * FROM Jobs WHERE JobId = @JobId";
                var param = new DynamicParameters();
                param.Add("JobId", JobId);
                var jobs = await connection.QuerySingleOrDefaultAsync<Jobs>(query, param);
                if (jobs == null)
                {
                    _logger.LogWarning("No Jobs Found on the System");
                }
                return jobs;
            }catch(Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving Job with JobId: {JobId}, JobId");
                throw new Exception("Database Error occured...TRY AGAIN");
            }
        }

      public async Task<bool> DeleteJob(int jobId)
        {
            try
            {
                using var connection = CreateConnection();
                const string query = "SELECT * FROM Jobs WHERE JobId = @JobId";
                var JobExisting = await connection.QueryFirstOrDefaultAsync<Jobs>(query, new { JobId = jobId });
                if (JobExisting == null)
                {
                    return false;
                }
                const string deleteJob = "DELETE * FROM Jobs WHERE JobId = @JobId ";
                int deletedItem = await connection.ExecuteAsync(deleteJob,new {JobId = jobId});
                return true;

            }catch(Exception ex)
            {
                _logger.LogError(ex, "DELETE OPERATION NOT SUCCESSFUL");
                throw;
            }
        }

        public async Task<Jobs> UpdateJob(JobsDtocs jobsDtocs)
        {
            try
            {
                using var connection = CreateConnection();
                const string findJob = "SELECT * FROM Jobs WHERE JobId = @JobId";
                var jobIdentified = await connection.QueryFirstOrDefaultAsync<Jobs>(findJob, new { jobsDtocs.JobId});
                if (jobIdentified == null)
                {
                    return null;
                }
                if (jobsDtocs.ClosingDate < jobsDtocs.PostingDate)
                {
                    Console.WriteLine("ClosingDate Cannot be Earlier than PostingDate");
                }
                const string updatequery = @"UPDATE Jobs 
                     SET JobName = @JobName, JobDescription = @JobDescription,JobRequirements = @JobRequirements, JobType = @JobType, 
                     SalaryRange = @SalaryRange, PostingDate = @PostingDate, ClosingDate = @ClosingDate
                     WHERE JobId =@JobId";
                int updatedJob = await connection.ExecuteAsync(updatequery, new
                {
                    jobsDtocs.JobId,
                    jobsDtocs.JobName,
                    jobsDtocs.JobType,
                    jobsDtocs.JobDescription,
                    jobsDtocs.SalaryRange,
                    jobsDtocs.JobRequirements,
                    jobsDtocs.ClosingDate,
                    jobsDtocs.PostingDate
                });
                if (updatedJob > 0)
                {
                    return await connection.QueryFirstOrDefaultAsync<Jobs>(findJob, new { jobsDtocs.JobId });
                }
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Update Operation Was Not Succesfull");
                throw;
            }
        }

        public async Task<Jobs> PostJob(JobsDtocs jobsDtocs)
        {
            try
            {
                using var connection = CreateConnection();

                // 1️⃣ Check if job already exists
                const string query = "SELECT * FROM Jobs WHERE JobName = @JobName AND CompanyId = @CompanyId";
                var jobExists = await connection.QueryFirstOrDefaultAsync<Jobs>(query, new
                {
                    jobsDtocs.JobName,
                    jobsDtocs.CompanyId
                });

                if (jobExists != null)  // ✅ Fix condition
                {
                    throw new Exception("Job With the same Title Exists.");
                }

                // 2️⃣ Insert job and get the new JobId
                const string postJob = @"
            INSERT INTO Jobs (JobName, JobDescription, JobRequirements, JobStatus, 
                              JobType, Location, PostingDate, SalaryRange, ClosingDate, 
                              CompanyId, IsDeleted) 
            VALUES (@JobName, @JobDescription, @JobRequirements, @JobStatus, 
                    @JobType, @Location, @PostingDate, @SalaryRange, @ClosingDate, 
                    @CompanyId, @IsDeleted);
            SELECT LAST_INSERT_ID();";  // ✅ Fix: Get inserted JobId

                int jobId = await connection.QuerySingleAsync<int>(postJob, new
                {
                    jobsDtocs.JobName,
                    jobsDtocs.JobDescription,
                    jobsDtocs.JobRequirements,
                    JobStatus = jobsDtocs.JobStatus ?? "Not Applied",
                    jobsDtocs.JobType,
                    jobsDtocs.Location,
                    jobsDtocs.PostingDate,
                    jobsDtocs.SalaryRange,
                    jobsDtocs.ClosingDate,
                    jobsDtocs.CompanyId,
                    IsDeleted = false
                });

                if (jobId == 0)  // ✅ Fix check for jobId
                {
                    throw new Exception("Job posting failed. No ID was returned.");
                }

                // 3️⃣ Generate JobLink
                string Jobink = $"https://localhost:7185/job-details/{jobId}";

                // 4️⃣ Update JobLink in the database
                const string updateJobinkQuery = "UPDATE Jobs SET Jobink = @Jobink WHERE JobId = @JobId";
                await connection.ExecuteAsync(updateJobinkQuery, new { Jobink = Jobink, JobId = jobId });

                // 5️⃣ Retrieve and return the created job
                const string getJobQuery = "SELECT * FROM Jobs WHERE JobId = @JobId";
                var createdJob = await connection.QueryFirstOrDefaultAsync<Jobs>(getJobQuery, new { JobId = jobId });

                if (createdJob == null)
                {
                    throw new Exception("Job was inserted but could not be retrieved.");
                }

                return createdJob;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Job posting failed");
                throw new Exception("A database error occurred while posting the job.");
            }
        }

    }
}
