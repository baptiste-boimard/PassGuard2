using Blazored.LocalStorage;
using Bunit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MudBlazor.Services;
using PassGuard.Shared.State;
using PassGuard.Web.Client.Components.Data;
using Tests.MockService;

namespace Tests;

public class SubmitFormEnterDataComposantTest : TestContext
{
    private readonly MockPasswordState _mockPasswordState;
    private readonly MockLocalStorage _mockLocalStorage;
    
    public SubmitFormEnterDataComposantTest()
    {
        var httpClient = new HttpClient();
        
        _mockPasswordState = new MockPasswordState(httpClient);
        _mockLocalStorage = new MockLocalStorage();
        
        Services.AddMudServices();
        Services.AddScoped<ILocalStorageService>(sp => _mockLocalStorage);
        Services.AddScoped<PasswordState>(sp => _mockPasswordState);
    }

    [Fact]
    public async Task Adding_Data_In_Inputs_And_Sends_CorrectData_WhenSubmissionIsSuccessful()
    {
        //! Arrange
        var cut = Render<EnterData>();

        //* Remplissage des inputs
        cut.Find("[data-testid='site-input']").Change("TestSite");
        cut.Find("[data-testid='username-input']").Change("TestUsername");
        cut.Find("[data-testid='password-input']").Change("TestPassword");
        cut.Find("[data-testid='category-input']").Change("TestCategory");
        
        //! Act
        cut.Find("[data-testid='add-button']").Click();
        await Task.Delay(500);

        //! Assert 1
        var received = _mockPasswordState.ReceivedPasswordForm;
        Assert.NotNull(received);
        Assert.Equal("TestSite", received.Site);
        Assert.Equal("TestUsername", received.Username);
        Assert.Equal("TestPassword", received.Password);
        Assert.Equal("TestCategory", received.Category);
        
        //! Assert 2
        Assert.Null(cut.Find("[data-testid='site-input']").GetAttribute("value"));
        Assert.Null(cut.Find("[data-testid='username-input']").GetAttribute("value"));
        Assert.Null(cut.Find("[data-testid='password-input']").GetAttribute("value"));
        Assert.Null(cut.Find("[data-testid='category-input']").GetAttribute("value"));
    }
}