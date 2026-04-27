namespace PicGramWebApp.Services.Storage
{
    public interface IStorageProvider
    {
        Task<(string FileName, string FilePath)> SaveFileAsync(IFormFile file);
        string GetFullPath(string relativePath);
    }
}