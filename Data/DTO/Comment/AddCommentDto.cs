using System.ComponentModel.DataAnnotations;

namespace FeedBackBoardAPI.Data.DTO.Comment;

public class AddCommentDto
{
    [Required]
    public string Content { get; set; }
    [Required]
    public int FeedBackId { get; set; }
}