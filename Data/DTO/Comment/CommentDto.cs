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
    public DateTime DateCreated { get; set; }

    [Required]
    public string ApplicationUserId { get; set; }

    [Required]
    public ApplicationUserDto ApplicationUser { get; set; }

    [Required]
    public int FeedbackId { get; set; }

    // Eğer bu bir alt yorumsa, ParentComment bilgisi taşıyacağız
    public int? ParentCommentId { get; set; }

    public CommentDto? ParentComment { get; set; }

    // Bir yorumun birden çok cevabı (Replies) olabilir
    public ICollection<CommentDto>? Replies { get; set; }
}