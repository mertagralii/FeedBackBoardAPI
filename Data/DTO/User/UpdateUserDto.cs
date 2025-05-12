using System.ComponentModel.DataAnnotations;

namespace FeedBackBoardAPI.Data.DTO.Register;

public class UpdateUserDto
{
    [Required]
    public string FirstName { get; set; }
    [Required]
    public string LastName { get; set; }
    
}