using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace PortalKulinarny.Services
{
    public class ImagesService
    {
        private readonly IWebHostEnvironment _hostEnvironment;

        public ImagesService(IWebHostEnvironment hostEnvironmen)
        {
            _hostEnvironment = hostEnvironmen;
        }
        public async Task<string> UploadImage(IFormFile file, string recipeName)
        {
            string wwwRootPath = _hostEnvironment.WebRootPath;
            string fileName = Path.GetFileNameWithoutExtension(file.FileName);
            string extension = Path.GetExtension(file.FileName);
            var filenameCombined = recipeName + "_" + fileName + "_" + DateTime.Now.ToString("yymmssfff") + extension;
            var path = Path.Combine(wwwRootPath + "/Images/", filenameCombined);

            using (var fileStream = new FileStream(path, FileMode.Create))
            {
                await file.CopyToAsync(fileStream);
            }

            return filenameCombined;
        }

        public void DeleteImage(string name)
        {
            var imagePath = Path.Combine(_hostEnvironment.WebRootPath, "images", name);
            if (System.IO.File.Exists(imagePath))
                System.IO.File.Delete(imagePath);
        }
    }
}
