using System.ComponentModel.DataAnnotations;

namespace BlazorApi.Models
{
    public class Jobs
    {
        [Key]
        public int JobId { get; set; }

        [Required]
        public string JobName { get; set; }

        public string JobDescription { get; set; } = string.Empty;

        public string JobStatus { get; set; } = "Open";

        public string JobType { get; set; }

        public string? Jobink { get; set; }

        public string JobRequirements { get; set; }

        public string SalaryRange { get; set; }

        public string Location { get; set; } = string.Empty;    

        public bool IsDeleted { get; set; } = false;

        public DateTime PostingDate { get; set; }

        public DateTime ClosingDate { get; set; }

        public int CompanyId { get; set; }

       
        //public Company Company { get; set; }

        //public ICollection<Application> Applications {get; set;}
    }

}

