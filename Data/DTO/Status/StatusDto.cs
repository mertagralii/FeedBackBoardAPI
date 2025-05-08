using System.ComponentModel.DataAnnotations;

namespace FeedBackBoardAPI.Data.DTO.Status;

public class StatusDto
{
    [Required]
    public string Name { get; set; }
}