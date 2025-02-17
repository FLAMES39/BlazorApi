namespace BlazorApi.DTO
{
    public class TemporaryCredentialsDto
    {
        public int UserId { get; set; }
        public string Email { get; set; }

        public string TemporaryPassword { get; set; }
        public DateTime ExpiryDate { get; set; }

    }
}
