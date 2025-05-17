using Microsoft.AspNetCore.Identity;

namespace FeedBackBoardAPI.Data.Entities;

public class ApplicationUser : IdentityUser
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    
    public string? ProfileImagePath { get; set; } // İsteğe bağlı profil fotoğraf yolu
    public ICollection<Feedback> FeedBack { get; set; }
    public ICollection<Comment> Comment { get; set; }
}