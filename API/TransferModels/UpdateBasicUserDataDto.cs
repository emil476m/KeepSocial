using System.ComponentModel.DataAnnotations;

namespace API.TransferModels;

public class UpdateBasicUserDataDto
{
    public required string updatedValueName { get; set; }
    public required string updatedValue { get; set; }
}