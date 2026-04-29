using Microsoft.Playwright;

namespace PicGramWebApp.UiTests;

public class BasicUiTests : IAsyncLifetime
{
    private IPlaywright _playwright = null!;
    private IBrowser _browser = null!;

    public async Task InitializeAsync()
    {
        _playwright = await Playwright.CreateAsync();
        _browser = await _playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
        {
            Headless = true
        });
    }

    public async Task DisposeAsync()
    {
        await _browser.DisposeAsync();
        _playwright.Dispose();
    }

    [Fact]
    public async Task HomePage_LoadsSuccessfully()
    {
        var page = await _browser.NewPageAsync();

        await page.GotoAsync("https://localhost:7089/");

        var title = await page.TitleAsync();

        Assert.False(string.IsNullOrWhiteSpace(title));
    }

    [Fact]
    public async Task SearchPage_LoadsSuccessfully()
    {
        var page = await _browser.NewPageAsync();

        await page.GotoAsync("https://localhost:7089/Photo/Search");

        var content = await page.ContentAsync();

        Assert.Contains("Search", content);
    }

    [Fact]
    public async Task UploadPage_WhenAnonymous_RedirectsToLogin()
    {
        var page = await _browser.NewPageAsync();

        await page.GotoAsync("https://localhost:7089/Photo/Upload");

        Assert.Contains("/Identity/Account/Login", page.Url);
    }
}