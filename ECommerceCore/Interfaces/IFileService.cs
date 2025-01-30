using Microsoft.AspNetCore.Http;
using System;
using System.IO;
using System.Threading.Tasks;

namespace ECommerceCore.Interfaces
{
    public interface IFileService
    {
        Task<string> UploadFileAsync(IFormFile file, string folderName);
        void DeleteFile(string fileName, string folderName);
    }

    public class FileService : IFileService
    {
        private static readonly string[] AllowedExtensions = { ".jpg", ".jpeg", ".png", ".webp" };

        public async Task<string> UploadFileAsync(IFormFile file, string folderName)
        {
            var projectRoot = Directory.GetCurrentDirectory();
            var folderPath = Path.Combine(projectRoot, "files", folderName);

            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }

            var fileExtension = Path.GetExtension(file.FileName).ToLower();
            if (!Array.Exists(AllowedExtensions, ext => ext == fileExtension))
            {
                throw new InvalidOperationException("يرجى تحميل صورة بتنسيق JPG أو PNGأو jpegأو webp فقط");
            }

            var fileName = $"{Guid.NewGuid()}_{file.FileName}";
            var filePath = Path.Combine(folderPath, fileName);

            try
            {
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }
            }
            catch (IOException ex)
            {
                throw new Exception("Error saving file", ex);
            }

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