using System.Text.Json;
using FastEndpoints;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.JsonPatch.Operations;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using NJsonSchema.Annotations;

namespace GameManager.Server.DTO;

public abstract class PatchRequest
{
    [FromBody]
    [JsonSchemaType(typeof(Operation[]))]
    public JsonElement[] Patches { get; set; }

    public void Patch<TModel>(TModel model) where TModel : class
    {
        var doc = new JsonPatchDocument(
            operations: Patches.Select(o => JsonConvert.DeserializeObject<Operation>(o.GetRawText())).ToList(),
            contractResolver: new DefaultContractResolver());
        doc.ApplyTo(model);
    }
}