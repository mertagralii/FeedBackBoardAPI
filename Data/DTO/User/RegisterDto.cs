using System.ComponentModel.DataAnnotations;

namespace FeedBackBoardAPI.Data.DTO.Register;

public class RegisterDto
{
    [Required]
    public string FirstName { get; set; }
    [Required]
    public string LastName { get; set; }
    [Required]
    public string Email { get; set; }
    [Required]
    public string Password { get; set; }
    
}