using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;


namespace WebdeskEA.Domain.Service
{
    public class ImageService : IImageService
    {
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ImageService(IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
        }

        public string UploadImage(IFormFile file, string folderPath)
        {
            if (file == null || file.Length == 0)
                return null;

            string uploadFolder = Path.Combine(_webHostEnvironment.WebRootPath, folderPath);
            Directory.CreateDirectory(uploadFolder);

            string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
            string filePath = Path.Combine(uploadFolder, fileName);

            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                file.CopyTo(fileStream);
            }

            return fileName;
        }

        public bool DeleteImage(string fileName, string folderPath)
        {
            if (string.IsNullOrEmpty(fileName))
                return false;

            string filePath = Path.Combine(_webHostEnvironment.WebRootPath, folderPath, fileName);
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
                return true;
            }
            return false;
        }

        public string GetImagePath(string fileName, string folderPath)
        {
            if (string.IsNullOrEmpty(fileName))
                return null;

            return Path.Combine(folderPath, fileName);
        }
    }
}
