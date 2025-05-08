using System.ComponentModel.DataAnnotations;
using FeedBackBoardAPI.Data.DTO.Category;
using FeedBackBoardAPI.Data.DTO.Comment;
using FeedBackBoardAPI.Data.DTO.Register;
using FeedBackBoardAPI.Data.DTO.Status;

namespace FeedBackBoardAPI.Data.DTO.Feedback;

public class DetailsFeetbackDto
{
    [Required]
    public int Id { get; set; }
    [Required]
    public ApplicationUserDto ApplicationUser { get; set; }
    [Required]
    public CategoryDto Category { get; set; }
    [Required]
    public StatusDto Status { get; set; }
    [Required]
    public string Title { get; set; }
    [Required]
    public string Detail { get; set; }
    [Required]
    public CommentDto Comment { get; set; }
    
}