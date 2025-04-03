using FluentValidation;
using OpcUaClient.Domain.Models;

namespace OpcUaClient.Domain.Validation;

public class TagValidator : AbstractValidator<Tag>
{
    public TagValidator()
    {
        RuleFor(tag => tag.Name).NotEmpty().WithMessage($"{nameof(Tag)} name is required.");
        RuleFor(tag => tag.Name).MaximumLength(Constants.TagMaxNameLength).WithMessage($"{nameof(Tag)} name too long.");
    }
}