namespace BlazorApi.DTO
{
    public class UpdateUserDto
    {
        public int Id { get; set; }
        public string Names { get; set; }

        public string Description { get; set; } = string.Empty;

        public string Email { get; set; }

        public long PhoneNumber { get; set; }
    }
}
