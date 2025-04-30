using FluentValidation;
using FluentValidation.Results;
using OpcUaClient.Domain.Models;

namespace OpcUaClient.Controllers;

public abstract class BaseEntityController<T> : BaseController where T : Entity
{
    protected async Task ValidateAndChangeModelStateAsync(
        IValidator<T> validator,
        T instance,
        CancellationToken cancellationToken = default)
    {
        var validationResult = await validator
            .ValidateAsync(instance, cancellationToken);
            
        if (!validationResult.IsValid)
            ChangeModelState(validationResult.Errors);
    }

    private void ChangeModelState(IEnumerable<ValidationFailure> errors)
    {
        foreach (var item in errors)
            ModelState.AddModelError(
                item.ErrorCode,
                item.ErrorMessage);
    }
}