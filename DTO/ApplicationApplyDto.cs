namespace BlazorApi.DTO
{
    public class ApplicationApplyDto
    {
        public int JobId { get; set; }

        public  int UserId { get; set; }

        public string UserName { get; set; }

        public string Email { get; set; }

        public DateTime ApplicationDate { get; set; } = DateTime.Now;

        public IFormFile ResumePath { get; set; }

        public IFormFile CoverLetter { get; set; }

        public string City { get; set; }

        public string Street { get; set; }

        public string PhoneNumber { get; set; }

        public string PostalCode { get; set; }

        public string? TemporaryPassword { get; set; }

        public DateTime? TempPasswordExpiry { get; set; }

        public string? TemporaryEmail { get; set; }

    }
}
