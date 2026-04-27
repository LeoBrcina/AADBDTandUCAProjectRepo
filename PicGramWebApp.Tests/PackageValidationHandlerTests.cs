using Microsoft.EntityFrameworkCore;
using PicGramWebApp.Data;
using PicGramWebApp.Models;
using PicGramWebApp.Services.Packages.Validation;

namespace PicGramWebApp.Tests;

public class PackageValidationHandlerTests
{
    private static ApplicationDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new ApplicationDbContext(options);
    }

    [Fact]
    public async Task UploadCountLimitHandler_WhenMonthlyUploadLimitReached_ReturnsDeniedResult()
    {
        // Arrange
        await using var context = CreateContext();

        var user = new ApplicationUser
        {
            Id = "user-1",
            Email = "test@example.com"
        };

        var packagePlan = new PackagePlan
        {
            Name = "FREE",
            MaxUploadsPerMonth = 1,
            MaxDownloadsPerMonth = 10,
            MaxStorageBytes = 10_000_000
        };

        context.Users.Add(user);

        context.Photos.Add(new Photo
        {
            FileName = "existing.jpg",
            FilePath = "/uploads/existing.jpg",
            Description = "Existing upload test photo",
            FileSize = 1000,
            UploadedAt = DateTime.Now,
            UserId = user.Id,
            User = user
        });

        await context.SaveChangesAsync();

        var validationContext = new PackageValidationContext
        {
            User = user,
            PackagePlan = packagePlan,
            NewFileSize = 1000
        };

        var handler = new UploadCountLimitHandler(context);

        // Act
        var result = await handler.HandleAsync(validationContext);

        // Assert
        Assert.False(result.IsAllowed);
        Assert.Equal("You have reached your package upload limit.", result.ErrorMessage);
    }

    [Fact]
    public async Task StorageLimitHandler_WhenUploadWouldExceedStorageLimit_ReturnsDeniedResult()
    {
        // Arrange
        await using var context = CreateContext();

        var user = new ApplicationUser
        {
            Id = "user-2",
            Email = "storage@example.com"
        };

        var packagePlan = new PackagePlan
        {
            Name = "FREE",
            MaxUploadsPerMonth = 10,
            MaxDownloadsPerMonth = 10,
            MaxStorageBytes = 5000
        };

        context.Users.Add(user);

        context.Photos.Add(new Photo
        {
            FileName = "existing.jpg",
            FilePath = "/uploads/existing.jpg",
            Description = "Existing storage test photo",
            FileSize = 4000,
            UploadedAt = DateTime.Now,
            UserId = user.Id,
            User = user
        });

        await context.SaveChangesAsync();

        var validationContext = new PackageValidationContext
        {
            User = user,
            PackagePlan = packagePlan,
            NewFileSize = 2000
        };

        var handler = new StorageLimitHandler(context);

        // Act
        var result = await handler.HandleAsync(validationContext);

        // Assert
        Assert.False(result.IsAllowed);
        Assert.Equal("Uploading this file would exceed your package storage limit.", result.ErrorMessage);
    }

    [Fact]
    public async Task DownloadCountLimitHandler_WhenMonthlyDownloadLimitReached_ReturnsDeniedResult()
    {
        // Arrange
        await using var context = CreateContext();

        var user = new ApplicationUser
        {
            Id = "user-3",
            Email = "download@example.com"
        };

        var packagePlan = new PackagePlan
        {
            Name = "FREE",
            MaxUploadsPerMonth = 10,
            MaxDownloadsPerMonth = 1,
            MaxStorageBytes = 10_000_000
        };

        context.Users.Add(user);

        context.ActionLogs.Add(new ActionLog
        {
            UserId = user.Id,
            ActionType = "DownloadOriginal",
            CreatedAt = DateTime.Now,
            Details = "Downloaded original photo."
        });

        await context.SaveChangesAsync();

        var validationContext = new PackageValidationContext
        {
            User = user,
            PackagePlan = packagePlan,
            NewFileSize = 0
        };

        var handler = new DownloadCountLimitHandler(context);

        // Act
        var result = await handler.HandleAsync(validationContext);

        // Assert
        Assert.False(result.IsAllowed);
        Assert.Equal("You have reached your package download limit.", result.ErrorMessage);
    }
}