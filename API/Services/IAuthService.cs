using System;
using API.Models;
using MongoDB.Driver;

namespace API.Services
{
    public interface IAuthService
    {
        string AuthenticateAndGenerateSession(string email, string code);
        bool ValidateSession(string token);
        void DeleteSession(string token);
    }

    public class AuthService : IAuthService
    {
        private readonly ICollectionsProvider _collectionsProvider;
        private readonly IEmailService _emailService;

        public AuthService(ICollectionsProvider collectionsProvider, IEmailService emailService)
        {
            _collectionsProvider = collectionsProvider ?? throw new ArgumentNullException(nameof(collectionsProvider));
            _emailService = emailService ?? throw new ArgumentNullException(nameof(emailService));
        }

        // Authenticates a user and generates a session token
        public string AuthenticateAndGenerateSession(string email, string code)
        {
            var user = _collectionsProvider.Users.Find(e => e.Email == email).FirstOrDefault();

            if (user == null || !_emailService.ValidateTwoFactorCode(email, code))
            {
                throw new ArgumentException("Invalid email or 2FA code.");
            }

            var session = new SessionToken(email);
            _collectionsProvider.Sessions.InsertOne(session);

            return session.Token;
        }

        // Validates a session token and removes it if expired
        public bool ValidateSession(string token)
        {
            var session = _collectionsProvider.Sessions.Find(s => s.Token == token).FirstOrDefault();

            if (session == null || !session.IsValid())
            {
                _collectionsProvider.Sessions.DeleteOne(s => s.Token == token);
                return false;
            }

            return true;
        }

        public void DeleteSession(string token)
        {
            // Find and delete the session associated with the token
            var session = _collectionsProvider.Sessions.Find(s => s.Token == token).FirstOrDefault();

            if (session != null)
            {
                // Delete the session from the sessions collection
                _collectionsProvider.Sessions.DeleteOne(s => s.Token == token);
            }
        }

    }
}
