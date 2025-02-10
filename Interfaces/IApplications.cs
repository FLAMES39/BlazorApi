using BlazorApi.Models;
using Microsoft.AspNetCore.Mvc;

namespace BlazorApi.Interfaces
{
    public interface IApplications
    {
        Task<List<Applications>> GetAllApplications();
        Task<Applications> GetApplicationById(int applicationId);
        Task<bool> DeleteApplication(int applicationId);
    }
}
