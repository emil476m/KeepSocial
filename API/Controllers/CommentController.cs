using Microsoft.AspNetCore.Mvc;
using Service;

namespace API.Controllers;


[ApiController]
public class CommentController: ControllerBase
{
    private readonly CommentService _commentService;
}