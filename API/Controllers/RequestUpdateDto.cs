﻿using System.ComponentModel.DataAnnotations;

namespace API.Controllers;

public class RequestUpdateDto
{
    [Required] public required int RequesterId { get; set; }
    
    [Required] public required int RequestId { get; set; }
    
    [Required] public required bool Response { get; set; }
}