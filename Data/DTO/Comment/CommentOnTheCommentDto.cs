namespace FeedBackBoardAPI.Data.DTO.Comment;

public class CommentOnTheCommentDto
{
    public string Content { get; set; }
    public int? ParentCommentId { get; set; }
}