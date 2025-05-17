using Microsoft.Build.Framework;

namespace FeedBackBoardAPI.Data.DTO.Register;

public class ImageUpdateUserDto
{
    [Required]
    public IFormFile ImageFile { get; set; } // Dosya ile gönderim
}