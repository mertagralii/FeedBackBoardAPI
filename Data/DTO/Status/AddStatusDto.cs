using System.ComponentModel.DataAnnotations;

namespace FeedBackBoardAPI.Data.DTO.Status;

public class AddStatusDto
{
    [Required]
    public string Name { get; set; }
}