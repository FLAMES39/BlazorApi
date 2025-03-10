﻿namespace BlazorApi.DTO
{
    public class JobsDtocs
    {
        public  int JobId { get;  set; }
        public string JobName { get; set; }

        public string JobDescription { get; set; } = string.Empty;

        public string JobStatus { get; set; } = "Not Applied";

        public string JobType { get; set; }

        public string JobRequirements { get; set; }

        public string Location { get; set; }

        public string SalaryRange { get; set; }

        public string Jobink { get; set; }

        public bool IsDeleted { get; set; }

        public int CompanyId { get; set; }

        public DateTime PostingDate { get; set; }

        public DateTime ClosingDate { get; set; }
    }


}
