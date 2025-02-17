using BlazorApi.Data;
using BlazorApi.DTO;
using BlazorApi.Interfaces;
using MailKit.Net.Smtp;
using MimeKit;
using Microsoft.EntityFrameworkCore;



namespace BlazorApi.Services
{
    public class TemporaryCredentialsService : ITempCredemtials
    {
        private readonly DataContext _context;

        public TemporaryCredentialsService(DataContext context)
        {
            _context = context;
        }

        public async Task<TemporaryCredentialsDto> GenerateTemporaryCredentials(int applicantId)
        {
            // Fetch the application using UserId (which matches applicantId)
            var application = await _context.Applications.FirstOrDefaultAsync(a => a.UserId == applicantId);

            if (application == null)
            {
                throw new Exception("Applicant not found.");
            }

            // Generate a temporary password
            string tempPassword = GenerateRandomPassword();
            application.TemporaryPassword = tempPassword;
            application.TempPasswordExpiry = DateTime.UtcNow.AddMinutes(2); // Ensure expiration is correctly set
            application.TemporaryEmail = application.Email;

            // Save changes to the database
            await _context.SaveChangesAsync();

            // Send temporary credentials email
            await SendTemporaryCredentialsEmail(application.Email, tempPassword);

            return new TemporaryCredentialsDto
            {
                UserId = application.UserId,
                Email = application.Email,
                TemporaryPassword = tempPassword,
                ExpiryDate = application.TempPasswordExpiry.Value
            };
        }


        private string GenerateRandomPassword()
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            return new string(Enumerable.Repeat(chars, 8).Select(s => s[new Random().Next(s.Length)]).ToArray());
        }

        private async Task SendTemporaryCredentialsEmail(string email, string tempPassword)
        {
            try
            {
                var message = new MimeMessage();
                message.From.Add(new MailboxAddress("christian ", "christianabiodun2020@gmail.com"));
                message.To.Add(new MailboxAddress("", email));
                message.Subject = "Your Temporary Credentials for SunPro Jobs";
                message.Body = new TextPart("plain")
                {
                    Text = $"Dear Applicant,\n\nHere are your temporary credentials:\n\nPassword: {tempPassword}\n\nPlease use this password within the next hour.\n\nBest Regards,\nSunPro Jobs Team"
                };

                using (var client = new SmtpClient())
                {
                    await client.ConnectAsync("smtp.gmail.com", 587, MailKit.Security.SecureSocketOptions.StartTls);
                    await client.AuthenticateAsync("christianabiodun2020@gmail.com", "ayfl nvzf jycu peyv");
                    await client.SendAsync(message);
                    await client.DisconnectAsync(true);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error sending email: {ex.Message}");
                throw new Exception("Failed to send temporary credentials email.");
            }
        }
    }
}
