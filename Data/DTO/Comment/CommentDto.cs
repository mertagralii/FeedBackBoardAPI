using System.ComponentModel.DataAnnotations;
using FeedBackBoardAPI.Data.DTO.Feedback;
using FeedBackBoardAPI.Data.DTO.Register;

namespace FeedBackBoardAPI.Data.DTO.Comment;

public class CommentDto
{
    [Required]
    public int Id { get; set; }
    [Required]
    public string Content { get; set; }
    [Required]
    public DateTime DateCreated { get; set; } = DateTime.Now;
    [Required]
    public string UserId { get; set; }
    [Required]
    public ApplicationUserDto ApplicationUser { get; set; }
    [Required]
    public int FeedBackId { get; set; }
    [Required]
    public FeedbackDto FeedBack { get; set; }
    
    public int? ParentCommentId { get; set; }
    public CommentDto ParentComment { get; set; }
    public ICollection<CommentDto> Replies { get; set; }
}