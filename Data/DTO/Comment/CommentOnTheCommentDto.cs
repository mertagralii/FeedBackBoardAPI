using System.ComponentModel.DataAnnotations;

namespace FeedBackBoardAPI.Data.DTO.Comment;

public class CommentOnTheCommentDto
{
    [Required]
    public string Content { get; set; }

    // Hangi yorumun alt yorumu olduğu
    [Required]
    public int ParentCommentId { get; set; }
}