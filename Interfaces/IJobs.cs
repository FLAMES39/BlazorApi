using BlazorApi.DTO;
using BlazorApi.Models;
using Microsoft.AspNetCore.Mvc;

namespace BlazorApi.Interfaces
{
    public interface IJobs
    {
        Task<ActionResult<Jobs>> PostJob(JobsDtocs jobsDtocs);
    }

}
