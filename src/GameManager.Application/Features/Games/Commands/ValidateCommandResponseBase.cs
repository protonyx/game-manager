using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace GameManager.Application.Features.Games.Commands;

public class ValidateCommandResponseBase
{
    [JsonIgnore]
    public ICollection<ValidationResult> ValidationResults { get; set; } = new List<ValidationResult>();

}