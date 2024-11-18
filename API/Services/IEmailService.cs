using MailKit.Net.Smtp;
using MimeKit;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using API.Models;

namespace API.Services
{
    // Interface for the Email Service, providing methods for sending emails and managing 2FA and password resets
    public interface IEmailService
    {
        Task<string> SendTwoFactorCodeAsync(string recipientEmail);
        bool ValidateTwoFactorCode(string recipientEmail, string code);
        Task<string> SendPasswordResetCodeAsync(string recipientEmail);
        bool ResetPassword(string recipientEmail, string code, User user, string newPassword);
        Task SendEmailAsync(string recipientEmail, string subject, string body);
    }

    public class EmailService : IEmailService
    {
        // SMTP server details for Gmail
        private readonly string _smtpServer = "smtp.gmail.com";
        private readonly int _smtpPort = 587;
        private readonly string _smtpUser; // Store the SMTP username (email)
        private readonly string _smtpPass; // Store the SMTP password (app password)
        
        // Dictionary to store temporary 2FA codes for each user
        private static readonly Dictionary<string, string> _twoFactorCodes = new Dictionary<string, string>();
        
        // Dictionary to store temporary password reset codes for each user
        private static readonly Dictionary<string, string> _passwordResetCodes = new Dictionary<string, string>();

        // Constructor to initialize SMTP credentials
        public EmailService()
        {
            _smtpUser = "dccconnectnoreply@gmail.com";
            _smtpPass = "onrg uzke kjrw kjfa";
        }

        // Method to send a 2FA code to the user's email
        public async Task<string> SendTwoFactorCodeAsync(string recipientEmail)
        {
            var code = GenerateVerificationCode();

            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("DCC Connect", _smtpUser));
            message.To.Add(MailboxAddress.Parse(recipientEmail));
            message.Subject = "Your Two-Factor Authentication Code";
            message.Body = new TextPart("plain")
            {
                Text = $"Your authentication code is: {code}"
            };

            using var client = new SmtpClient();
            try
            {
                await client.ConnectAsync(_smtpServer, _smtpPort, MailKit.Security.SecureSocketOptions.StartTls);
                await client.AuthenticateAsync(_smtpUser, _smtpPass);
                await client.SendAsync(message);
                Console.WriteLine("2FA code sent!");

                // Store the generated code for validation later
                _twoFactorCodes[recipientEmail] = code;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error sending 2FA code: {ex.Message}");
            }
            finally
            {
                await client.DisconnectAsync(true);
                client.Dispose();
            }

            return code;
        }

        // Method to validate the 2FA code entered by the user

        /* This will end up leaking data long term - FIX! *put onto database as collection* */
        public bool ValidateTwoFactorCode(string recipientEmail, string code)
        {
            // Check if the code matches the one stored, then remove it
            if (_twoFactorCodes.ContainsKey(recipientEmail) && _twoFactorCodes[recipientEmail] == code)
            {
                _twoFactorCodes.Remove(recipientEmail);
                return true;
            }
            return false;
        }

        // Method to send a password reset code to the user's email
        public async Task<string> SendPasswordResetCodeAsync(string recipientEmail)
        {
            var code = GenerateVerificationCode();

            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("DCC Connect", _smtpUser));
            message.To.Add(new MailboxAddress("", recipientEmail));
            message.Subject = "Your Password Reset Code";
            message.Body = new TextPart("plain")
            {
                Text = $"Your password reset code is: {code}"
            };

            using var client = new SmtpClient();
            try
            {
                await client.ConnectAsync(_smtpServer, _smtpPort, MailKit.Security.SecureSocketOptions.StartTls);
                await client.AuthenticateAsync(_smtpUser, _smtpPass);
                await client.SendAsync(message);
                Console.WriteLine("Password reset code sent!");

                // Store the generated code for validation later
                _passwordResetCodes[recipientEmail] = code;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error sending password reset code: {ex.Message}");
            }
            finally
            {
                await client.DisconnectAsync(true);
                client.Dispose();
            }

            return code;
        }

        // Method to validate the reset code and reset the password if valid
        public bool ResetPassword(string recipientEmail, string code, User user, string newPassword)
        {
            // Check if the code matches the one stored for this email
            if (_passwordResetCodes.ContainsKey(recipientEmail) && _passwordResetCodes[recipientEmail] == code)
            {
                user.SetPassword(newPassword); // Update the user's password hash
                _passwordResetCodes.Remove(recipientEmail); // Remove the code after use
                return true;
            }
            return false;
        }

        // Method to generate a random 6-digit verification code
        private string GenerateVerificationCode()
        {
            var random = new Random();
            return random.Next(100000, 999999).ToString();
        }

        // Method to send a simple email with a subject and body
        public async Task SendEmailAsync(string recipientEmail, string subject, string body)
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("DCC Connect", _smtpUser));
            message.To.Add(new MailboxAddress("", recipientEmail));
            message.Subject = subject;
            message.Body = new TextPart("plain")
            {
                Text = body
            };

            using var client = new SmtpClient();
            try
            {
                await client.ConnectAsync(_smtpServer, _smtpPort, MailKit.Security.SecureSocketOptions.StartTls);
                await client.AuthenticateAsync(_smtpUser, _smtpPass);
                await client.SendAsync(message);
                Console.WriteLine("Email sent successfully!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error sending email: {ex.Message}");
            }
            finally
            {
                await client.DisconnectAsync(true);
                client.Dispose();
            }
        }
    }
}
