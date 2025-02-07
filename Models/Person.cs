namespace BlazorApi.Models
{
    public class Person
    {
        public int Id { get; set; }
        public string Names { get; set; }

        public string Password { get; set; }

        public string Description { get; set; }

        public string Email { get; set; }

        public string Role { get; set; } = "User";

        public int PhoneNumber { get; set; }


    }
}
