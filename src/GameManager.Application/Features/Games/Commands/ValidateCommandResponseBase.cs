using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace GameManager.Application.Features.Games.Commands;

public class ValidateCommandResponseBase
{
    [JsonIgnore]
    public ICollection<ValidationResult> ValidationResults { get; set; } = new List<ValidationResult>();

}