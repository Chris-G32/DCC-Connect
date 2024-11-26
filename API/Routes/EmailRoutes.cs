using Carter;
using Microsoft.AspNetCore.Mvc;
using API.Services;
using API.Constants;
using API.Models;

namespace API.Routes;

public class EmailRoutes : CarterModule
{
    private readonly IEmailService _emailService;

    public EmailRoutes(IEmailService emailService)
    {
        _emailService = emailService;
    }

    public override void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost(RouteConstants.SendPasswordResetRoute, SendPasswordResetCode);
        app.MapPost(RouteConstants.ResetPasswordRoute, ResetPassword);
    }

    public async Task<IResult> SendTwoFactorCode(string recipientEmail)
    {
        var code = await _emailService.SendTwoFactorCodeAsync(recipientEmail);
        return !string.IsNullOrEmpty(code) ? Results.Ok("2FA code sent!") : Results.Problem("Error sending 2FA code");
    }

    public async Task<IResult> SendPasswordResetCode(string recipientEmail)
    {
        var code = await _emailService.SendPasswordResetCodeAsync(recipientEmail);
        return !string.IsNullOrEmpty(code) ? Results.Ok("Password reset code sent!") : Results.Problem("Error sending password reset code");
    }

    public IResult ResetPassword(string recipientEmail, string code, User user, string newPassword)
    {
        return _emailService.ResetPassword(recipientEmail, code, user, newPassword) ? Results.Ok("Password reset successful") : Results.Problem("Invalid reset code");
    }
}
