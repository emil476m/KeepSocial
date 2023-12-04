using System.ComponentModel.DataAnnotations;

namespace Infastructure;

public class FriendRequestModel
{
    [Required] public required int RequestId { get; set; }
    
    [Required] public required string RequesterName { get; set; }
    
    [Required] public required string RequesterAvatarurl { get; set; }
    
}