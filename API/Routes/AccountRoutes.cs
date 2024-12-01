using Carter;
using Microsoft.AspNetCore.Mvc;
using API.Services;
using API.Constants;
using System.Security.Cryptography;
using MongoDB.Bson;
using API.Models.SignIn;
using API.Errors;
using Amazon.Runtime;
using API.Services.Authentication;
using API.Models.Users;
using API.Utils;

namespace API.Routes
{
    public class AccountRoutes(ILogger<AccountRoutes> logger, ILoginService loginService, IUserRegisterService userRegisterService, IUserService userService) : CarterModule
    {
        private readonly ILoginService _loginService = loginService;
        private readonly IUserRegisterService _userRegisterService = userRegisterService;
        private readonly ILogger<AccountRoutes> _logger = logger;
        private readonly IUserService _userService = userService;

        public override void AddRoutes(IEndpointRouteBuilder app)
        {
            //Create new user
            app.MapPost(RouteConstants.RegisterUserRoute, RegisterUser).RequireAuthorization(PolicyConstants.ManagerPolicy);

            //Signing in routes
            app.MapPost(RouteConstants.LoginUserRoute, LoginUser); // Login route
            app.MapPost(RouteConstants.Validate2FACodeRoute, VerifyMFA); // MFA route
            app.MapGet(RouteConstants.SignedInUserRoute, (HttpRequest request) =>
            {
                var claims = AuthUtils.GetClaims(request);
                var userWithoutPassword = new UserInfo(_userService.GetUserById(claims.UserID));

                return Results.Ok(userWithoutPassword);

            }).RequireAuthorization(PolicyConstants.EmployeePolicy);
            // User CRUD routes
            //app.MapGet(RouteConstants.GetUserRoute, Get);
            //app.MapGet(RouteConstants.GetUserByIdRoute, GetUserById);
            //app.MapGet(RouteConstants.GetUserRoleRoute, GetUserRole);
            //app.MapPut(RouteConstants.UpdateUserRoute, UpdateUser);
            //app.MapDelete(RouteConstants.DeleteUserRoute, DeleteUser);
        }

        // Register new user
        public async Task<IResult> RegisterUser(UserRegistrationInfo userInfo)
        {
            try
            {
                var user = await _userRegisterService.RegisterUser(userInfo);
                return Results.Ok("Successfully created user!");
            }
            catch (DCCApiException e)
            {
                return Results.Problem(e.Message);
            }
            catch (Exception e)
            {
                return Results.Problem("Error registering user: " + e.Message);
            }
        }

        /// <summary>
        /// Attempts to sign in a user with the provided credentials.
        /// </summary>
        /// <param name="credentials">The users email and password</param>
        /// <param name="request"></param>
        /// <returns> Problem when invalid credentials, success when valid</returns>
        public async Task<IResult> LoginUser(LoginRequest credentials, HttpRequest request)
        {
            try
            {
                bool sentMfaCode = _loginService.StartSession(credentials);
                if (!sentMfaCode)
                {
                    return Results.Unauthorized();
                }
                return Results.Ok("MFA Code sent, awaiting input."); // Return true if login is successful
            }
            catch (Exception e)
            {
                return Results.Problem("Error logging in: " + e.Message);
            }
        }
        /// <summary>
        /// Validates an MFA code with an email and generates a JWT token if successful
        /// </summary>
        /// <param name="credentials"> The email and MFA code sent.</param>
        /// <returns></returns>
        public async Task<IResult> VerifyMFA(LoginRequest credentials, HttpResponse response)
        {
            try
            {
                var jwtToken = _loginService.ValidateMFA(credentials);
                response.Headers.Append("Authorization", "Bearer " + jwtToken);
                // Set the JWT token in the HTTP-only cookie
                response.Cookies.Append(
                    "sessionid",                 // Cookie name
                    jwtToken,                        // JWT token value
                    new CookieOptions
                    {
                        HttpOnly = true,             // Prevent access to the cookie via JavaScript (XSS protection)
                        Secure = true,               // Ensure the cookie is sent only over HTTPS
                        SameSite = SameSiteMode.None, // Control cross-origin requests (adjust based on your needs)
                        Expires = DateTime.UtcNow.AddHours(1) // Set expiration time (e.g., 1 hour)
                    });

                return Results.Ok("Sign in successful.");

            }
            catch (Exception e)
            {
                return Results.Problem("Error verifying MFA: " + e.Message);
            }
        }
    }
}
