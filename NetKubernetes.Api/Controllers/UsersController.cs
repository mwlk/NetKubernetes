using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NetKubernetes.DTO.Users;
using NetKubernetes.Repository.Interfaces;

namespace NetKubernetes.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class UsersController : ControllerBase
{
    private readonly IUsersRepository _repository;

    public UsersController(IUsersRepository repository)
    {
        _repository = repository;
    }

    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequestDto request, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return Ok(await _repository.Login(request));
    }

    [AllowAnonymous]
    [HttpPost("new")]
    public async Task<IActionResult> Post([FromBody] UserPostDto request, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return Ok(await _repository.Post(request));
    }

    [HttpGet]
    public async Task<IActionResult> GetUserSession(CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return Ok(await _repository.GetUser());
    }
}