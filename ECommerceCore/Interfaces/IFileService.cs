using Microsoft.AspNetCore.Http;
using System;
using System.IO;
using System.Threading.Tasks;

namespace ECommerceCore.Interfaces
{
    public interface IFileService
    {
        Task<string> SaveFileAsync(IFormFile file, string folderName);
        void DeleteFile(string fileName, string folderName);
    }

    public class FileService : IFileService
    {
        public async Task<string> SaveFileAsync(IFormFile file, string folderName)
        {
            // Get the project root directory path (assuming the project is ECommerceAPI)
            var projectRoot = Directory.GetCurrentDirectory();

            // Define the path relative to your project root (ECommerceAPI/files/images)
            var folderPath = Path.Combine(projectRoot, "files", folderName);

            // Create the directory if it doesn't exist
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }

            // Generate a unique file name
            var fileName = $"{Guid.NewGuid()}_{file.FileName}";
            var filePath = Path.Combine(folderPath, fileName);

            // Save the file to the file system
            var stream = new FileStream(filePath, FileMode.Create);
            
                await file.CopyToAsync(stream);
            

            // Return the relative path to the saved file
            return fileName;
        }
        public void DeleteFile(string fileName, string folderName)
        {
            string filePath = Path.Combine(Directory.GetCurrentDirectory(), "files", folderName, fileName);

            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
        }
    }

   
}
