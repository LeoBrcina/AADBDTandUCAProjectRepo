namespace PicGramWebApp.Services.Storage
{
    public class LocalStorageProvider : IStorageProvider
    {
        public async Task<(string FileName, string FilePath)> SaveFileAsync(IFormFile file)
        {
            var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads");

            if (!Directory.Exists(uploadsFolder))
                Directory.CreateDirectory(uploadsFolder);

            var fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
            var fullPath = Path.Combine(uploadsFolder, fileName);

            using (var stream = new FileStream(fullPath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            var relativePath = "/uploads/" + fileName;

            return (fileName, relativePath);
        }

        public string GetFullPath(string relativePath)
        {
            return Path.Combine(
                Directory.GetCurrentDirectory(),
                "wwwroot",
                relativePath.TrimStart('/').Replace("/", Path.DirectorySeparatorChar.ToString()));
        }
    }
}