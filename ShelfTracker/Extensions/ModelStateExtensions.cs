using Microsoft.AspNetCore.Mvc.ModelBinding;
using ShelfTracker.Middleware;

namespace ShelfTracker.Extensions;

public static class ModelStateExtensions
{
    public static void ThrowIfInvalid(this ModelStateDictionary modelState)
    {
        if (!modelState.IsValid)
        {
            var errors = modelState
                .SelectMany(x => x.Value!.Errors)
                .Select(x => x.ErrorMessage)
                .ToList();
            
            throw new ValidationException(errors);
        }
    }
}