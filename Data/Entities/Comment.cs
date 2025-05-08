namespace FeedBackBoardAPI.Data.Entities;

public class Comment
{
    public int Id { get; set; }
    public string Content { get; set; }
    public DateTime DateCreated { get; set; } = DateTime.Now;
    public string ApplicationUserId  { get; set; }
    public ApplicationUser ApplicationUser { get; set; }
    public int? ParentCommentId { get; set; }
    public Comment? ParentComment { get; set; }
    public ICollection<Comment>? Replies { get; set; }
}