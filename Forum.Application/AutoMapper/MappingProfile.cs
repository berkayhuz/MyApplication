using AutoMapper;
using Forum.Api.Controllers;
using Forum.Application.Aggregates;
using Forum.Application.Commands.FeedCommands;
using Forum.Application.Commands.IdentityCommands;
using Forum.Application.Commands.ProfileCommands;
using Forum.Application.Common.DTOs;
using Forum.Application.DTOs;
using Forum.Application.Requests.Feed;
using Forum.Application.Requests.Identity;
using Forum.Application.Requests.Member;
using Forum.Domain.Entities.FeedEntities;
using Forum.Domain.Entities.User;
using Forum.Domain.Entities.UserEntities.UserProfileEntities;
using Forum.Domain.ValueObjects;

namespace Forum.Application.AutoMapper
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            #region Feed Mapping
            CreateMap<UserProfileAggregate, UserpostDto>()
            .ForMember(dest => dest.ProfileImageUrl, opt => opt.MapFrom(src => src.ProfileImageUrl));
            CreateMap<UserProfile, UserpostDto>();

            CreateMap<UserAggregate, UserpostDto>()
                .ForMember(dest => dest.Username, opt => opt.MapFrom(src => src.Username))
                .ForMember(dest => dest.FirstName, opt => opt.MapFrom(src => src.FirstName))
                .ForMember(dest => dest.LastName, opt => opt.MapFrom(src => src.LastName));
            CreateMap<UserProfile, UserProfileDto>();
            CreateMap<User, UserpostDto>();
            CreateMap<UserProfileAggregate, UserpostDto>();
            CreateMap<PostEntity, PostDto>();
            CreateMap<CreatePostCommand, PostEntity>();
            CreateMap<CreatePostRequest, CreatePostCommand>();
            CreateMap<CategoryEntity, CategoryDto>();
            CreateMap<CreateCategoryRequest, CategoryEntity>()
                .ForMember(dest => dest.ImageUrl, opt => opt.Ignore());
            CreateMap<UpdateCategoryRequest, CategoryEntity>()
                .ForMember(dest => dest.ImageUrl, opt => opt.Ignore());
            #endregion

            #region Member Mapping
            #region CreateSocialMediaLinkCommand => UserSocialMedia
            CreateMap<CreateSocialMediaLinkCommand, UserSocialMedia>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => Guid.NewGuid()))
            .ForMember(dest => dest.ProfileId, opt => opt.Ignore());
            #endregion
            #region CreateSocialLinksRequest => CreateSocialMediaLinkCommand
            CreateMap<CreateSocialLinksRequest, CreateSocialMediaLinkCommand>();
            #endregion
            #region CreateHobbyRequest => CreateHobbyCommand
            CreateMap<CreateHobbyRequest, CreateHobbyCommand>();
            #endregion
            #region CreateHobby => UserHobby
            CreateMap<CreateHobbyCommand, UserHobby>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => Guid.NewGuid()))
            .ForMember(dest => dest.ProfileId, opt => opt.MapFrom(src => src.UserId));
            #endregion
            #region CreateUserDTO => UserAggregate
            CreateMap<CreateUserDTO, UserAggregate>()
            .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => true))
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.UtcNow));
            #endregion
            #region CreateUserCommand => ProfileUpdateDTO
            CreateMap<CreateUserCommand, ProfileUpdateDTO>();
            #endregion
            #region CreateUserCommand => CreateUserDTO
            CreateMap<CreateUserCommand, CreateUserDTO>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => Guid.NewGuid()))
            .ForMember(dest => dest.PasswordHash, opt => opt.MapFrom(src => src.Password));
            #endregion
            #region UpdateProfileCommand => ProfileUpdateDTO
            CreateMap<UpdateProfileCommand, ProfileUpdateDTO>()
            .ForMember(dest => dest.ProfileImageUrl, opt => opt.MapFrom(src => src.ProfileImage ?? null))
            .ForMember(dest => dest.CoverImageUrl, opt => opt.MapFrom(src => src.CoverImage ?? null))
            .ForMember(dest => dest.Bio, opt => opt.MapFrom(src => src.Bio ?? null))
            .ForMember(dest => dest.Content, opt => opt.MapFrom(src => src.Content ?? null))
            .ForMember(dest => dest.CustomStatus, opt => opt.MapFrom(src => src.CustomStatus ?? null))
            .ForMember(dest => dest.Gender, opt => opt.MapFrom(src => src.Gender ?? null))
            .ForMember(dest => dest.BirthDate, opt => opt.MapFrom(src => src.BirthDate ?? null))
            .ForMember(dest => dest.Location, opt => opt.MapFrom(src => src.Location ?? null))
            .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.PhoneNumber ?? null))
            .ForMember(dest => dest.Website, opt => opt.MapFrom(src => src.Website ?? null));
            #endregion
            #region UpdateProfileRequest => UpdateProfileCommand
            CreateMap<UpdateProfileRequest, UpdateProfileCommand>()
            .ForMember(dest => dest.UserId, opt => opt.Ignore());
            #endregion
            #region VerifyEmailRequest => VerifyEmailCommand
            CreateMap<VerifyEmailRequest, VerifyEmailCommand>();
            #endregion
            #region RegisterRequest => CreateUserCommand
            CreateMap<RegisterRequest, CreateUserCommand>();
            #endregion
            #region LoginRequest => LoginCommand
            CreateMap<LoginRequest, LoginCommand>();
            #endregion
            #region ResetPasswordRequest => ResetPasswordCommand
            CreateMap<ResetPasswordRequest, ResetPasswordCommand>();
            #endregion
            #region ForgotPasswordRequest => ForgotPasswordCommand
            CreateMap<ForgotPasswordRequest, ForgotPasswordCommand>();
            #endregion
            #region UserHobby => UserHobbyDTO
            CreateMap<UserHobby, UserHobbyDto>().ReverseMap();
            #endregion
            #region UserSocialMedia => UserSocialMediaDTO
            CreateMap<UserSocialMedia, UserSocialMediaDto>().ReverseMap();
            #endregion
            #region UserProfileAggregate => UserProfile
            CreateMap<UserProfileAggregate, UserProfile>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.UserId))
            .ForMember(dest => dest.ProfileImageUrl, opt => opt.MapFrom(src => src.ProfileImageUrl))
            .ForMember(dest => dest.CoverImageUrl, opt => opt.MapFrom(src => src.CoverImageUrl))
            .ForMember(dest => dest.Bio, opt => opt.MapFrom(src => src.Bio))
            .ForMember(dest => dest.Content, opt => opt.MapFrom(src => src.Content))
            .ForMember(dest => dest.CustomStatus, opt => opt.MapFrom(src => src.CustomStatus))
            .ForMember(dest => dest.Gender, opt => opt.MapFrom(src => src.Gender))
            .ForMember(dest => dest.BirthDate, opt => opt.MapFrom(src => src.BirthDate))
            .ForMember(dest => dest.Location, opt => opt.MapFrom(src => src.Location))
            .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.PhoneNumber))
            .ForMember(dest => dest.Website, opt => opt.MapFrom(src => src.Website))
            .ForMember(dest => dest.SocialMediaLinks, opt => opt.MapFrom(src => src.SocialMediaLinks))
            .ForMember(dest => dest.Hobbies, opt => opt.MapFrom(src => src.Hobbies))
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedAt))
            .ForMember(dest => dest.LastActiveAt, opt => opt.MapFrom(src => src.LastActiveAt))
            .ForMember(dest => dest.ModifiedAt, opt => opt.MapFrom(src => src.ModifiedAt))
            .ForMember(dest => dest.DeletedAt, opt => opt.MapFrom(src => src.DeletedAt))
            .ForMember(dest => dest.PostCount, opt => opt.MapFrom(src => src.PostCount))
            .ForMember(dest => dest.CommentCount, opt => opt.MapFrom(src => src.CommentCount))
            .ForMember(dest => dest.VisitCount, opt => opt.MapFrom(src => src.VisitCount))
            .ForMember(dest => dest.IsAnonymous, opt => opt.MapFrom(src => src.IsAnonymous))
            .ForMember(dest => dest.IsEmailNotifications, opt => opt.MapFrom(src => src.IsEmailNotifications))
            .ForMember(dest => dest.ThemePreference, opt => opt.MapFrom(src => src.ThemePreference))
            .ForMember(dest => dest.IsPrivateProfile, opt => opt.MapFrom(src => src.IsPrivateProfile))
            .ForMember(dest => dest.ProfileVisibility, opt => opt.MapFrom(src => src.ProfileVisibility))
            .ForMember(dest => dest.IsEmailVisible, opt => opt.MapFrom(src => src.IsEmailVisible))
            .ForMember(dest => dest.IsPhoneNumberVisible, opt => opt.MapFrom(src => src.IsPhoneNumberVisible));
            #endregion
            #region UserProfile => UserProfileAggregate
            CreateMap<UserProfile, UserProfileAggregate>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.UserId))
                .ForMember(dest => dest.ProfileImageUrl, opt => opt.MapFrom(src => src.ProfileImageUrl))
                .ForMember(dest => dest.CoverImageUrl, opt => opt.MapFrom(src => src.CoverImageUrl))
                .ForMember(dest => dest.Bio, opt => opt.MapFrom(src => src.Bio))
                .ForMember(dest => dest.Content, opt => opt.MapFrom(src => src.Content))
                .ForMember(dest => dest.CustomStatus, opt => opt.MapFrom(src => src.CustomStatus))
                .ForMember(dest => dest.Gender, opt => opt.MapFrom(src => src.Gender))
                .ForMember(dest => dest.BirthDate, opt => opt.MapFrom(src => src.BirthDate))
                .ForMember(dest => dest.Location, opt => opt.MapFrom(src => src.Location))
                .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.PhoneNumber))
                .ForMember(dest => dest.Website, opt => opt.MapFrom(src => src.Website))
                .ForMember(dest => dest.SocialMediaLinks, opt => opt.MapFrom(src => src.SocialMediaLinks))
                .ForMember(dest => dest.Hobbies, opt => opt.MapFrom(src => src.Hobbies))
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedAt))
                .ForMember(dest => dest.LastActiveAt, opt => opt.MapFrom(src => src.LastActiveAt))
                .ForMember(dest => dest.ModifiedAt, opt => opt.MapFrom(src => src.ModifiedAt))
                .ForMember(dest => dest.DeletedAt, opt => opt.MapFrom(src => src.DeletedAt))
                .ForMember(dest => dest.PostCount, opt => opt.MapFrom(src => src.PostCount))
                .ForMember(dest => dest.CommentCount, opt => opt.MapFrom(src => src.CommentCount))
                .ForMember(dest => dest.VisitCount, opt => opt.MapFrom(src => src.VisitCount))
                .ForMember(dest => dest.IsAnonymous, opt => opt.MapFrom(src => src.IsAnonymous))
                .ForMember(dest => dest.IsEmailNotifications, opt => opt.MapFrom(src => src.IsEmailNotifications))
                .ForMember(dest => dest.ThemePreference, opt => opt.MapFrom(src => src.ThemePreference))
                .ForMember(dest => dest.IsPrivateProfile, opt => opt.MapFrom(src => src.IsPrivateProfile))
                .ForMember(dest => dest.ProfileVisibility, opt => opt.MapFrom(src => src.ProfileVisibility))
                .ForMember(dest => dest.IsEmailVisible, opt => opt.MapFrom(src => src.IsEmailVisible))
                .ForMember(dest => dest.IsPhoneNumberVisible, opt => opt.MapFrom(src => src.IsPhoneNumberVisible));
            #endregion
            #region User => UserAggregate
            CreateMap<User, UserAggregate>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Username, opt => opt.MapFrom(src => src.UserName))
                .ForMember(dest => dest.FirstName, opt => opt.MapFrom(src => src.FirstName))
                .ForMember(dest => dest.LastName, opt => opt.MapFrom(src => src.LastName))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => new EmailAddress(src.Email)))
                .ForMember(dest => dest.PasswordHash, opt => opt.MapFrom(src => src.PasswordHash))
                .ForMember(dest => dest.EmailConfirmed, opt => opt.MapFrom(src => src.EmailConfirmed))
                .ForMember(dest => dest.AccessFailedCount, opt => opt.MapFrom(src => src.AccessFailedCount))
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedAt ?? DateTime.UtcNow))
                .ForMember(dest => dest.LockoutEnd, opt => opt.MapFrom(src => src.LockoutEnd));
            #endregion
            #region UserAggregate => User
            CreateMap<UserAggregate, User>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.EmailConfirmed, opt => opt.MapFrom(src => src.EmailConfirmed))
                .ForMember(dest => dest.AccessFailedCount, opt => opt.MapFrom(src => src.AccessFailedCount))
                .ForMember(dest => dest.PasswordHash, opt => opt.MapFrom(src => src.PasswordHash))
                .ForMember(dest => dest.LockoutEnd, opt => opt.MapFrom(src => src.LockoutEnd));
            #endregion
            #region UserProfileAggregate => UserProfileDto
            CreateMap<UserProfileAggregate, UserProfileDto>()
                .ForMember(dest => dest.ProfileImageUrl, opt => opt.MapFrom(src => src.ProfileImageUrl))
                .ForMember(dest => dest.CoverImageUrl, opt => opt.MapFrom(src => src.CoverImageUrl))
                .ForMember(dest => dest.Bio, opt => opt.MapFrom(src => src.Bio))
                .ForMember(dest => dest.Content, opt => opt.MapFrom(src => src.Content))
                .ForMember(dest => dest.CustomStatus, opt => opt.MapFrom(src => src.CustomStatus))
                .ForMember(dest => dest.Gender, opt => opt.MapFrom(src => src.Gender))
                .ForMember(dest => dest.BirthDate, opt => opt.MapFrom(src => src.BirthDate))
                .ForMember(dest => dest.Location, opt => opt.MapFrom(src => src.Location))
                .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.PhoneNumber))
                .ForMember(dest => dest.Website, opt => opt.MapFrom(src => src.Website))
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedAt))
                .ForMember(dest => dest.LastActiveAt, opt => opt.MapFrom(src => src.LastActiveAt))
                .ForMember(dest => dest.ModifiedAt, opt => opt.MapFrom(src => src.ModifiedAt))
                .ForMember(dest => dest.PostCount, opt => opt.MapFrom(src => src.PostCount))
                .ForMember(dest => dest.CommentCount, opt => opt.MapFrom(src => src.CommentCount))
                .ForMember(dest => dest.VisitCount, opt => opt.MapFrom(src => src.VisitCount))
                 .ForMember(dest => dest.Username, opt => opt.Ignore())
                .ForMember(dest => dest.Email, opt => opt.Ignore())
                .ForMember(dest => dest.FirstName, opt => opt.Ignore())
                .ForMember(dest => dest.LastName, opt => opt.Ignore());
            #endregion
            #region UserProfile => UserProfileDto
            CreateMap<UserProfile, UserProfileDto>()
           .ForMember(dest => dest.SocialMediaLinks, opt => opt.MapFrom(src => src.SocialMediaLinks))
           .ForMember(dest => dest.Hobbies, opt => opt.MapFrom(src => src.Hobbies));
            #endregion
            #region UserHobby => UserHobbyDto
            CreateMap<UserHobby, UserHobbyDto>();
            #endregion
            #endregion
        }
    }
}
