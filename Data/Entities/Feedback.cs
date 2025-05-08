namespace FeedBackBoardAPI.Data.Entities;

public class Feedback
{
    public int Id { get; set; }
    public string ApplicationUserId { get; set; }
    public ApplicationUser ApplicationUser { get; set; }
    public int CategoryId { get; set; }
    public Category Category { get; set; }
    public int StatusId { get; set; }
    public Status Status { get; set; }
    public string Title { get; set; }
    public string Detail { get; set; }
    public int? CommentId { get; set; }
    public Comment? Comment { get; set; }
}