using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace GameManager.Server;

public static class ModelStateValidationExtensions
{
    public static void AddValidationResults(this ModelStateDictionary modelState,
        IEnumerable<ValidationResult> validationResults)
    {
        foreach (var validationResult in validationResults)
        {
            modelState.AddModelError(validationResult.MemberNames.First(), validationResult.ErrorMessage ?? "An error occurred");
        }
    }

}