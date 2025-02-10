namespace BlazorApi.DTO
{
    public class ApplicationApplyDto
    {
        public int JobId { get; set; }

        public  int UserId { get; set; }

        public string Email { get; set; }

        public DateTime ApplicationDate { get; set; } = DateTime.Now;

        public string ResumePath { get; set; }

        public string CoverLetter { get; set; }

        public string City { get; set; }

        public string Stret { get; set; }

        public string PhoneNumber { get; set; }

        public string PostalCode { get; set; }

    }
}
