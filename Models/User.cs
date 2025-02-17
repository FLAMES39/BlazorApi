namespace BlazorApi.Models
{
    public class User
    {
        public int UserId { get; set; }

        public int ApplicantId { get; set; }

        public string Names { get; set; } 

        public string Password { get; set; }

        public string Description { get; set; } = string.Empty;

        public string Email { get; set; }

        public string Role { get; set; } = "User";

        public string PhoneNumber { get; set; }

        public bool IsDeleted { get; set; } = false;

        // Temporary credential properties
        public string? TemporaryPassword { get; set; }
        public DateTime? TempPasswordExpiry { get; set; }
    }

}
