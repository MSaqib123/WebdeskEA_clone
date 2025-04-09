using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebdeskEA.Domain.Service
{
    public interface IImageService
    {
        string UploadImage(IFormFile file, string folderPath);
        bool DeleteImage(string fileName, string folderPath);
        string GetImagePath(string fileName, string folderPath);
    }
}
