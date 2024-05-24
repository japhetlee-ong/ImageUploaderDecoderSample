using ImageUploaderDecoder.Models;
using ImageUploaderDecoder.Utils;
using Microsoft.AspNetCore.Mvc;

namespace ImageUploaderDecoder.Controllers
{
    public class ImageController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ImageController(IConfiguration configuration, IWebHostEnvironment webHostEnvironment) 
        {
            _configuration = configuration;
            _webHostEnvironment = webHostEnvironment;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Index(ImageUploadModel model)
        {
            if (ModelState.IsValid)
            {
                var key = _configuration["ImageEncryption:ImageKey"];
                var iv = _configuration["ImageEncryption:ImageIV"];
                string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "uploads");

                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                var res = ImageUtils.ProcessImageUpload(model.ImageFile,uploadsFolder,key!,iv!, out string imageFilePath);

                if (res)
                {
                    var imageBytes = ImageUtils.ProcessDecodeImage(imageFilePath,key!,iv!);
                    return File(imageBytes, "image/jpeg");
                }
                else
                {
                    return Content("Image upload failed");
                }
                
            }
            return View(model);
        }
    }
}
