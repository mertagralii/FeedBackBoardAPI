using System.ComponentModel.DataAnnotations;

namespace FeedBackBoardAPI.Data.DTO.Comment;

public class UpdateCommentDto
{
    [Required]
    public int Id { get; set; }

    [Required]
    public string Content { get; set; }
}