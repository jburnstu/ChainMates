using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ChainMates.Server;
using ChainMates.Server.DTOs;
using ChainMates.Server.DTOs.Author;
using ChainMates.Server.DTOs.Segment;
using ChainMates.Server.Services;
using System.Reflection.Emit;
using System.Security.Claims;
using System.Text.Json;
using System.Xml.Linq;
using ChainMates.Server.Enums;

[ApiController]
[Route("chainmates/load")]
public class InitialLoadController : ControllerBase
    // For now, I have quite a lot of actual logic going on this controller. It just made sense to me for now for it to be done
    // here, since it pulls together so many services, but realistically I think at some point this should be moved into a
    // dedicated service, called from one endpoint in this file.
{
    private readonly CurrentUserService _currentUserService;
    private readonly InitialLoadService _initialLoadService;

    public InitialLoadController(
        CurrentUserService currentUserService,
        InitialLoadService initialLoadService
        )
    {
        _currentUserService = currentUserService;
        _initialLoadService = initialLoadService;
}


    [Authorize]
    [HttpGet]
    public async Task<IActionResult> GetInfo()
    {
        Console.WriteLine("MADE IT TO GetInfo");
        int authorId = _currentUserService.UserId ?? 0;

        if (authorId == 0)
        {
            return Unauthorized();
        }

        var data = await _initialLoadService.getInitialLoad(authorId);

        return Ok(data);

    }
}