using System.ComponentModel.DataAnnotations;

namespace BlazorApi.Models
{
    public class Applications
    {
        [Key]
        public int ApplicationId { get; set; }

        [Required]
        public int JobId { get; set; }

        [Required]  
        public int UserId { get; set; }

        public string UserName { get; set;}

        [Required]
        public string Email { get; set; }

        public DateTime ApplicationDate { get; set; }

        public string Status { get; set; } = "Pending";

        public string ResumePath { get; set; }

        public string CoverLetter { get; set; }

        public string City { get; set; }

        public string Street { get; set; }

        public string PhoneNumber { get; set; }

        public string PostalCode { get; set; }

        public bool IsDelete { get; set; } = false;

        public DateTime DeleteAt { get; set; }

        public string? TemporaryPassword { get; set; }
        public DateTime? TempPasswordExpiry { get; set; }
        public string? TemporaryEmail { get; set; }

    }
}
