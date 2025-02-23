using PassGuard.Shared.Models;
using PassGuard.Shared.State;
using PassGuard.Web.Client.Pages;

namespace Tests.MockService;

public class MockPasswordState : PasswordState
{
    public MockPasswordState(HttpClient httpClient) : base(httpClient)
    {
    }

    public ObjectPasswordForm? ReceivedPasswordForm { get; private set; }

    public override Task<bool> CreatePassword(string? token, ObjectPasswordForm passwordForm)
    {
        ReceivedPasswordForm = passwordForm;
        return Task.FromResult(true);
    } 
}