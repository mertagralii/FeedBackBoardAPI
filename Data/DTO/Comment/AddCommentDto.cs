using System.ComponentModel.DataAnnotations;

namespace FeedBackBoardAPI.Data.DTO.Comment;

public class AddCommentDto
{
    [Required]
    public string Content { get; set; }

    [Required]
    public int FeedBackId { get; set; }   // “FeedbackId” ile birebir eşleşiyor

    // Alt yorum ise:
    public int? ParentCommentId { get; set; }
}