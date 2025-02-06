using FluentValidation;
using Forum.Domain.Entities.FeedEntities;

namespace Forum.Domain.Validators.FeedValidators
{
    public class CategoryValidator : AbstractValidator<CategoryEntity>
    {
        public CategoryValidator()
        {
            RuleFor(category => category.Name)
                .NotEmpty().WithMessage("Name is required.")
                .Length(2, 24).WithMessage("Name must be between 2 and 24 characters.")
                .Matches(@"^[a-zA-Z\s]+$").WithMessage("Name must contain only letters and spaces.");

            RuleFor(category => category.Description)
                .NotEmpty().WithMessage("Description is required.")
                .Length(5, 75).WithMessage("Description must be between 5 and 75 characters.");

            RuleFor(category => category.ImageUrl)
                .Matches(@"\.(jpg|png|jpeg|svg|webp)$").WithMessage("Image URL must end with jpg, png, JPEG, svg, or webp.");

            RuleFor(category => category.MetaTitle)
                .NotEmpty().WithMessage("Meta title is required.")
                .Length(2, 24).WithMessage("Meta title must be between 2 and 24 characters.")
                .Matches(@"^[a-zA-Z\s]+$").WithMessage("Meta title must contain only letters and spaces.");

            RuleFor(category => category.MetaDescription)
                .NotEmpty().WithMessage("Meta description is required.")
                .Length(5, 75).WithMessage("Meta description must be between 5 and 75 characters.");
        }
    }
}