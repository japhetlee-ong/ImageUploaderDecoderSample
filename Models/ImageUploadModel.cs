using System.ComponentModel.DataAnnotations;

namespace ImageUploaderDecoder.Models
{
    public class ImageUploadModel
    {
        [Required]
        [Display(Name = "Upload Image")]
        public IFormFile ImageFile { get; set; }
    }
}
