using System.ComponentModel.DataAnnotations;

namespace FeedBackBoardAPI.Data.DTO.Register;

public class LoginDto
{
    [Required]
    public string Email { get; set; }
    [Required]
    public string Password { get; set; }

}