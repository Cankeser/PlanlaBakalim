using Microsoft.AspNetCore.Http;


namespace PlanlaBakalim.Utilities
{
    public class FileHelper
    {
        public static async Task<string> FileLoaderAsync(IFormFile formFile, string filePath = "/img/")
        {
            string uploadsPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", filePath.TrimStart('/'));
            if (!Directory.Exists(uploadsPath))
            {
                Directory.CreateDirectory(uploadsPath);
            }
            string extension = Path.GetExtension(formFile.FileName);
            string fileName = Guid.NewGuid().ToString("N") + extension;

            string fullPath = Path.Combine(uploadsPath, fileName);

            using var stream = new FileStream(fullPath, FileMode.Create);
            await formFile.CopyToAsync(stream);

            return filePath + fileName;

        }
        public static bool FileRemover(string fileName)
        {
            string fullPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", fileName.TrimStart('/'));
            if (File.Exists(fullPath))
            {
                File.Delete(fullPath);
                return true;
            }
            return false;
        }

    }
}
