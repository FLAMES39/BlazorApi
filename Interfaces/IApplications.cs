using BlazorApi.DTO;
using BlazorApi.Models;
using Microsoft.AspNetCore.Mvc;

namespace BlazorApi.Interfaces
{
    public interface IApplications
    {
        Task<List<Applications>> GetAllApplications();
        Task<Applications> GetApplicationById(int applicationId);
        Task<bool> DeleteApplication(int applicationId);
        Task<Applications> ApplyJob (ApplicationApplyDto applicationApplyDto); 
        Task<string> uploadFiles (IFormFile formFile);
        Task<Applications> EditApplication(int applicationId, ApplicationApplyDto ApplicationApplyDto);
    }
}
