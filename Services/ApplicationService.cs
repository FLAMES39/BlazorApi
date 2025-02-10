using BlazorApi.Data;
using BlazorApi.Interfaces;
using BlazorApi.Models;
using Microsoft.EntityFrameworkCore;

namespace BlazorApi.Services
{
    public class ApplicationService : IApplications
    {
        private readonly DataContext _context;

        public ApplicationService(DataContext context)
        {
            _context = context;
        }

        public async Task<List<Applications>> GetAllApplications()
        {
            var application = await _context.Applications
                .Where(a => a.IsDelete == a.IsDelete)
                .ToListAsync();

            if (application is null) 
            {
                Console.WriteLine("No Application Available at the Moment");
            }
            return application;
        }

        public async Task<Applications> GetApplicationById(int ApplicationId)
        {
            var sigleApplication = await _context.Applications.FirstOrDefaultAsync(a => a.ApplicationId == ApplicationId);
            if (sigleApplication is null) 
            {
                Console.WriteLine("No ApplicationFound");
            }
            return sigleApplication;
        }

        public async Task<bool> DeleteApplication(int ApplicationId)
        {
            var application = await _context.Applications.FirstOrDefaultAsync(a => a.ApplicationId == ApplicationId);
            if (application is null)
            {
                return false;
            }
            application.IsDelete = true;
            application.DeleteAt = DateTime.Now;

            _context.Applications.Remove(application);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
