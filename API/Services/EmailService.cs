using MailKit.Net.Smtp;
using MimeKit;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using API.Models;

namespace API.Services
{
    public class EmailService
    {
        // SMTP server details for Gmail
        private readonly string _smtpServer = "smtp.gmail.com";
        private readonly int _smtpPort = 587;
        private readonly string _smtpUser; // Store the SMTP username (email)
        private readonly string _smtpPass; // Store the SMTP password (app password)
        private static readonly Dictionary<string, string> _twoFactorCodes = new Dictionary<string, string>(); // To store 2FA codes temp
        private static readonly Dictionary<string, string> _passwordResetCodes = new Dictionary<string, string>(); // To store password reset codes

        // Constructor to initialize SMTP credentials
        /* Explicit login credentials (remove/fix) */
        public EmailService(string smtpUser, string smtpPass)
        {
            _smtpUser = "dccconnecttest@protonmail.com";
            _smtpPass = "T3stP@ssword322!";
        }

        /* 2FA for Auth */
        // Method to send a 2FA code to the user's email
        public async Task<string> SendTwoFactorCodeAsync(string recipientEmail)
        {
            var code = GenerateVerificationCode();

            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("DCC Connect", _smtpUser));
            message.To.Add(new MailboxAddress("", recipientEmail));
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
        public bool ValidateTwoFactorCode(string recipientEmail, string code)
        {
            if (_twoFactorCodes.ContainsKey(recipientEmail) && _twoFactorCodes[recipientEmail] == code)
            {
                _twoFactorCodes.Remove(recipientEmail);
                return true;
            }
            return false;
        }

        /* Password Reset */
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
            if (_passwordResetCodes.ContainsKey(recipientEmail) && _passwordResetCodes[recipientEmail] == code)
            {
                user.SetPassword(newPassword); // Update the user's password hash
                _passwordResetCodes.Remove(recipientEmail); // Remove the code after use
                return true;
            }
            return false;
        }

        /* Generate Verification Codes */
        // Method to generate a random 6-digit verification code
        private string GenerateVerificationCode()
        {
            var random = new Random();
            return random.Next(100000, 999999).ToString();
        }

        /* Simple Email */
        // Method to send a simple email
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
