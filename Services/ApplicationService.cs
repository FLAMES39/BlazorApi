﻿using BlazorApi.Data;
using BlazorApi.DTO;
using BlazorApi.Interfaces;
using BlazorApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MailKit.Net.Smtp;
using MimeKit;

namespace BlazorApi.Services
{
    public class ApplicationService : IApplications
    {
        private readonly DataContext _context;


        private string coverLetterPath;
        private string resumePath;
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
                Console.WriteLine("No Application Found");

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

        public async Task<Applications> ApplyJob(ApplicationApplyDto applicationApplyDto)
        {
            var applyJob = await _context.Applications.AnyAsync(a => a.UserId == applicationApplyDto.UserId && a.UserName == applicationApplyDto.UserName);
            if (!applyJob)
            {
                Console.WriteLine("You have Already Applied For this Job Position");
            }

            if (applicationApplyDto.CoverLetter != null && applicationApplyDto.ResumePath != null)
            {

                coverLetterPath = await uploadFiles(applicationApplyDto.CoverLetter);
                resumePath = await uploadFiles(applicationApplyDto.ResumePath);
            }
            if (coverLetterPath == null && resumePath == null)
            {
                return null;
            }

            var application = new Applications
            {
                JobId = applicationApplyDto.JobId,
                UserId = applicationApplyDto.UserId,
                UserName = applicationApplyDto.UserName,
                Email = applicationApplyDto.Email,
                ApplicationDate = DateTime.Now,
                ResumePath = resumePath,  // Save the file path in the database
                CoverLetter = coverLetterPath,
                City = applicationApplyDto.City,
                Street = applicationApplyDto.Street,
                PhoneNumber = applicationApplyDto.PhoneNumber,
                PostalCode = applicationApplyDto.PostalCode,
            };
            Console.WriteLine("Application not Successful");
            await _context.Applications.AddAsync(application);
            await _context.SaveChangesAsync();
            await SendEmail("mashadachris85@gmail.com", "JOB APPLIED", "Application Received");
            return application;
        }

        public async Task<string> uploadFiles(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return null;
            }

            var uploadsFolderPath = Path.Combine(Directory.GetCurrentDirectory(), "Uploads");

            if (!Directory.Exists(uploadsFolderPath))
            {
                Directory.CreateDirectory(uploadsFolderPath);
            }
            var uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
            var filePath = Path.Combine(uploadsFolderPath, uniqueFileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            // Save the relative file path (optional)
            var relativePath = Path.Combine("Uploads", uniqueFileName);
            return relativePath;
        }

        static async Task SendEmail(string to, string subject, string body)
        {
            try
            {
                var message = new MimeMessage();
                message.From.Add(new MailboxAddress("chtistian ", "christianabiodun2020@gmail.com"));
                message.To.Add(new MailboxAddress("", to));
                message.Subject = subject;
                message.Body = new TextPart("html") { Text = body };
                using (var client = new SmtpClient())
                {
                    await client.ConnectAsync("smtp.gmail.com", 587, MailKit.Security.SecureSocketOptions.StartTls);
                    await client.AuthenticateAsync("christianabiodun2020@gmail.com", "ayfl nvzf jycu peyv");
                    await client.SendAsync(message);
                    await client.DisconnectAsync(true);
                }
                Console.WriteLine("Email sent successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }

        }
       public async Task<Applications> EditApplication(int ApplicationId, ApplicationApplyDto applicationApplyDto)
        {
            var application = await _context.Applications.FirstOrDefaultAsync(a => a.ApplicationId == ApplicationId && a.JobId == applicationApplyDto.JobId);
            if (application == null) 
            {
                return null;
            }

            application.JobId = applicationApplyDto.JobId;
            application.UserId = applicationApplyDto.UserId;
            application.UserName = applicationApplyDto.UserName;
            application.Email = applicationApplyDto.Email;
            application.ApplicationDate = DateTime.Now;  // Update the application date to current time
            application.City = applicationApplyDto.City;
            application.Street = applicationApplyDto.Street;
            application.PhoneNumber = applicationApplyDto.PhoneNumber;
            application.PostalCode = applicationApplyDto.PostalCode;


            await _context.SaveChangesAsync();
            return application;
        }
    }

}
