using MailKit.Net.Smtp;
using MimeKit;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;

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

        // Constructor to initialize SMTP credentials
        public EmailService(string smtpUser, string smtpPass)
        {
            _smtpUser = smtpUser;
            _smtpPass = smtpPass;
        }

        // Method to send a simple email
        public async Task SendEmailAsync(string recipientEmail, string subject, string body)
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("DCC Connect", _smtpUser)); // Sender email
            message.To.Add(new MailboxAddress("", recipientEmail)); // Recipient email
            message.Subject = subject; // Email subject

            message.Body = new TextPart("plain") // Email body content in plain text
            {
                Text = body
            };

            // Sending the email asynchronously
            using var client = new SmtpClient();
            try
            {
                await client.ConnectAsync(_smtpServer, _smtpPort, MailKit.Security.SecureSocketOptions.StartTls); // Connect to SMTP server securely
                await client.AuthenticateAsync(_smtpUser, _smtpPass); // Authenticate with SMTP credentials
                await client.SendAsync(message); // Send the email
                Console.WriteLine("Email sent successfully!"); // Confirmation message
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error sending email: {ex.Message}"); // Error handling if email fails
            }
            finally
            {
                await client.DisconnectAsync(true); // Disconnect from the SMTP server
                client.Dispose(); // Clean up the client
            }
        }

        // Method to send a 2FA code to the user's email
        public async Task<string> SendTwoFactorCodeAsync(string recipientEmail)
        {
            var code = GenerateVerificationCode(); // Generate a random 2FA code

            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("DCC Connect", _smtpUser)); // Sender email
            message.To.Add(new MailboxAddress("", recipientEmail)); // Recipient email
            message.Subject = "Your Two-Factor Authentication Code"; // Subject for 2FA email
            message.Body = new TextPart("plain") // Email body for 2FA code
            {
                Text = $"Your authentication code is: {code}"
            };

            // Sending the 2FA email asynchronously
            using var client = new SmtpClient();
            try
            {
                await client.ConnectAsync(_smtpServer, _smtpPort, MailKit.Security.SecureSocketOptions.StartTls); // Connect securely to SMTP server
                await client.AuthenticateAsync(_smtpUser, _smtpPass); // Authenticate with SMTP credentials (we provide)
                await client.SendAsync(message); // Send the email
                Console.WriteLine("2FA code sent!"); // Confirmation message

                // Store the generated 2FA code for later validation
                _twoFactorCodes[recipientEmail] = code;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error sending 2FA code: {ex.Message}"); // Error handling if sending fails
            }
            finally
            {
                await client.DisconnectAsync(true); // Disconnect from the SMTP server
                client.Dispose(); // Clean up the client
            }

            return code; // Return the generated code
        }

        // Method to validate the 2FA code entered by the user
        public bool ValidateTwoFactorCode(string recipientEmail, string code)
        {
            // Check if the provided code matches the stored code
            if (_twoFactorCodes.ContainsKey(recipientEmail) && _twoFactorCodes[recipientEmail] == code)
            {
                // Optionally, remove the code after successful validation
                _twoFactorCodes.Remove(recipientEmail);
                return true; // Code is valid
            }
            return false; // Code is invalid
        }

        // Method to generate a random 6-digit verification code
        private string GenerateVerificationCode()
        {
            var random = new Random();
            return random.Next(100000, 999999).ToString(); // Return a random 6-digit number
        }
    }
}
