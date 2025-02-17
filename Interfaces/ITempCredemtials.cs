using BlazorApi.DTO;

namespace BlazorApi.Interfaces
{
    public interface ITempCredemtials
    {

        Task<TemporaryCredentialsDto> GenerateTemporaryCredentials(int applicantId);
    }
}
