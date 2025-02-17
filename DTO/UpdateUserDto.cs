namespace BlazorApi.DTO
{
    public class UpdateUserDto
    {
        public int UserId { get; set; }
        public string Names { get; set; }

        public string Description { get; set; } = string.Empty;

        public string Email { get; set; }

        public string PhoneNumber { get; set; }
    }
}
