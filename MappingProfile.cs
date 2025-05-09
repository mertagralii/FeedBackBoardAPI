using AutoMapper;
using FeedBackBoardAPI.Data.DTO.Category;
using FeedBackBoardAPI.Data.DTO.Comment;
using FeedBackBoardAPI.Data.DTO.Feedback;
using FeedBackBoardAPI.Data.DTO.Register;
using FeedBackBoardAPI.Data.DTO.Status;
using FeedBackBoardAPI.Data.Entities;
using Microsoft.AspNetCore.Identity;

namespace FeedBackBoardAPI;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // Feedback <-> FeedbackDto
        CreateMap<Feedback, FeedbackDto>().ReverseMap();
        
        // Feedback <-> AddFeedbackDto
        CreateMap<Feedback, AddFeedbackDto>().ReverseMap();
        
        //Feedback >-> DetailsFeetbackDto
        CreateMap<Feedback, DetailsFeetbackDto>()
            .ForMember(dest => dest.Comments,
                opt => opt.MapFrom(src => src.Comments
                    .Where(c => c.ParentCommentId == null)))
            .ReverseMap();
        
        // Feedback <-> UpdateFeedbackDto
        CreateMap<Feedback, UpdateFeedbackDto>().ReverseMap();
        
        //CommentOnTheCommentDto <-> Comment
        CreateMap<Comment, CommentOnTheCommentDto>().ReverseMap();

        // Category <-> CategoryDto
        CreateMap<Category, CategoryDto>().ReverseMap();
        
        // Status <-> StatusDto
        CreateMap<Status, StatusDto>().ReverseMap();
        
        // ApplicationUser <-> ApplicationUserDto
        CreateMap<ApplicationUser, ApplicationUserDto>().ReverseMap();
        
        //AplicationUser <-> IdentityUser
        CreateMap<ApplicationUser,IdentityUser>().ReverseMap();
        
        // AddCommentDto <-> Comment
        CreateMap<AddCommentDto, Comment>().ReverseMap();
        
        // Comment <-> CommentDto
        CreateMap<Comment, CommentDto>().ReverseMap();



        // Bu Ne demek ? ("Source (kaynak) nesnedeki Y property'sini al, Destination (hedef) nesnedeki X property'sine ata.")
        // Örnek Kod : 
        //CreateMap<Source, Destination>()
        //.ForMember(dest => dest.X, opt => opt.MapFrom(src => src.Y));



    }
}