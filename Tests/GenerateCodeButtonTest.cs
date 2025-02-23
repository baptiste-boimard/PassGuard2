using System.ComponentModel.Design;
using Blazored.LocalStorage;
using Bunit;
using Microsoft.Extensions.DependencyInjection;
using PassGuard.Shared.State;
using PassGuard.Web.Client.Components.Data;

namespace Tests.MockService;

public class GenerateCodeButtonTest : TestContext
{
    public GenerateCodeButtonTest()
    {
        Services.AddScoped<ILocalStorageService, MockLocalStorage>();
        Services.AddScoped<UserState>();
    }
    
    [Fact]
    public void GenerateCode_ShouldProduceValidPassword()
    {
        //! Arrange
        //* Instantiation du composant
        var cut = Render<Menu>();

        //! Act
        var generateButton = cut.Find("[data-testid='generate-button']");
        generateButton.Click();
        
        //! Assert
        cut.WaitForAssertion(() =>
        {
            var generatedTextElement = cut.Find("[data-testid='generated-text']");
            var generatedPassword = generatedTextElement.TextContent.Trim();
            string specialChars = "!@#$%^&*()-_+=<>?";

            Assert.False(string.IsNullOrWhiteSpace(generatedPassword));
            Assert.Equal(18, generatedPassword.Length);
            Assert.True(generatedPassword.Any(char.IsUpper));
            Assert.True(generatedPassword.Any(char.IsLower));
            Assert.True(generatedPassword.Any(char.IsDigit));
            Assert.True(generatedPassword.Any(c => specialChars.Contains(c)));
        });
    }
}