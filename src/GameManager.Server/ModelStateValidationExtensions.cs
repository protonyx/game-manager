using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace GameManager.Server;

public static class ModelStateValidationExtensions
{
    public static void AddValidationResults(this ModelStateDictionary modelState,
        IEnumerable<ValidationResult> validationResults)
    {
        foreach (var validationResult in validationResults)
        {
            foreach (var memberName in validationResult.MemberNames)
            {
                modelState.AddModelError(memberName, validationResult.ErrorMessage ?? "An error occurred");
            }
        }
    }

    public static void AddValidationResults(this ModelStateDictionary modelState,
        FluentValidation.Results.ValidationResult validationResult)
    {
        foreach (var validationError in validationResult.Errors)
        {
            modelState.AddModelError(validationError.PropertyName, validationError.ErrorMessage);
        }
    }

}