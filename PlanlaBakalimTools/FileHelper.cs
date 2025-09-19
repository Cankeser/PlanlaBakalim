using Microsoft.AspNetCore.Http;


namespace PlanlaBakalim.Utilities
{
    public class FileHelper
    {
        public static async Task<string> FileLoaderAsync(IFormFile formFile, string filePath = "/img/")
        {
            string fileName = "";
            if (formFile != null && formFile.Length > 0)
            {
                fileName = formFile.FileName.ToLower();
                string directory = Directory.GetCurrentDirectory() + "/wwwroot" + filePath + fileName;
                using var stream = new FileStream(directory, FileMode.Create);
                await formFile.CopyToAsync(stream);
            }
            return filePath+fileName;
        }
        public static bool FileRemover(string fileName)
        {
            string directory = Directory.GetCurrentDirectory() + "/wwwroot" + fileName;
            if (File.Exists(directory))
            {
                File.Delete(directory);
                return true;
            }
            return false;
        }
    }
}
