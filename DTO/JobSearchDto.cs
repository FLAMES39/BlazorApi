namespace BlazorApi.DTO
{
    public class JobSearchDto
    {
        public string? JobName { get; set; }

        public int? CompanyId { get; set; }

        public bool? IsDeleted { get; set; }

        public string? JobType { get; set; }
    }
}