using System.ComponentModel.DataAnnotations;
using FeedBackBoardAPI.Data.DTO.Category;
using FeedBackBoardAPI.Data.DTO.Register;
using FeedBackBoardAPI.Data.DTO.Status;

namespace FeedBackBoardAPI.Data.DTO.Feedback;

public class UpdateFeedbackDto
{   
    [Required]
    public int Id { get; set; }
    [Required]
    public int CategoryId { get; set; }
    [Required]
    public int StatusId { get; set; }
    [Required]
    public string Title { get; set; }
    [Required]
    public string Detail { get; set; }
}