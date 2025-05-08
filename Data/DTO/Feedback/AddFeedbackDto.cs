using System.ComponentModel.DataAnnotations;

namespace FeedBackBoardAPI.Data.DTO.Feedback;

public class AddFeedbackDto
{
    [Required]
    public int CategoryId { get; set; }
    [Required]
    public int StatusId { get; set; }
    [Required]
    public string Title { get; set; }
    [Required]
    public string Detail { get; set; }
}