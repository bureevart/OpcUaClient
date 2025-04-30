using FluentValidation;
using OpcUaClient.Domain.Models;

namespace OpcUaClient.Domain.Validation;

public class ServerValidator : AbstractValidator<Server>
{
    public ServerValidator()
    {
        RuleFor(tag => tag.ApplicationName).NotEmpty().WithMessage($"{nameof(Server)} name is required.");
        RuleFor(tag => tag.ApplicationName).MaximumLength(Constants.DefaultMaxNameLength).WithMessage($"{nameof(Server)} name too long.");
    }
}