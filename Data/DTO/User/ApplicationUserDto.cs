using System.ComponentModel.DataAnnotations;

namespace FeedBackBoardAPI.Data.DTO.Register;

public class ApplicationUserDto
{
    [Required]
    public string Id { get; set; }
    [Required]
    public string FirstName { get; set; }
    [Required]
    public string LastName { get; set; }
}