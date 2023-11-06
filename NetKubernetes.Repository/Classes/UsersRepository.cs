using System.Net;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using NetKubernetes.DTO.Users;
using NetKubernetes.Middleware;
using NetKubernetes.Models;
using NetKubernetes.Persistence;
using NetKubernetes.Repository.Interfaces;
using NetKubernetes.Token.Interfaces;

namespace NetKubernetes.Repository.Classes;

public class UsersRepository : IUsersRepository
{
    private readonly UserManager<User> _userManager;
    private readonly SignInManager<User> _signInManager;
    private readonly IJwtGenerator _jwtGenerator;
    private readonly AppDbContext _context;
    private readonly IUserSession _userSession;

    public UsersRepository(
        UserManager<User> userManager,
        SignInManager<User> signInManager,
        IJwtGenerator jwtGenerator,
        AppDbContext context,
        IUserSession userSession)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _jwtGenerator = jwtGenerator;
        _context = context;
        _userSession = userSession;
    }

    private UserResponseDto TransformerUserToUserDto(User user)
    {
        return new UserResponseDto
        {
            Id = user.Id,
            Name = user.Name,
            Surname = user.Surname,
            Email = user.Email,
            Phone = user.Phone,
            Username = user.UserName,
            Token = _jwtGenerator.GenerateToken(user)
        };
    }

    public async Task<UserResponseDto> GetUser()
    {
        var user = await _userManager
        .FindByNameAsync(_userSession.GetUserSession());

        if (user is null)
            throw new MiddlewareException(HttpStatusCode.Unauthorized,
            new { message = "Token's User Not Found" });

        return TransformerUserToUserDto(user!);
    }

    public async Task<UserResponseDto> Login(LoginRequestDto request)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);

        if (user is null)
            throw new MiddlewareException(HttpStatusCode.Unauthorized,
            new { message = "Email Not Exist" });

        //* false for not block account
        var result = await _signInManager
        .CheckPasswordSignInAsync(user!, request.Password!, false);

        if (result.Succeeded)
        {
            return TransformerUserToUserDto(user!);
        }
        throw new MiddlewareException(
            HttpStatusCode.Unauthorized,
            new { message = "credentials not valid" }
        );
    }

    public async Task<UserResponseDto> Post(UserPostDto request)
    {
        var emailExist = await _context.Users
            .Where(p => p.Email == request.Email)
            .AnyAsync();

        if (emailExist)
            throw new MiddlewareException(
            HttpStatusCode.BadRequest,
            new { message = "Email is used" }
        );


        var usernameExist = await _context.Users
            .Where(p => p.UserName == request.Username)
            .AnyAsync();

        if (usernameExist)
            throw new MiddlewareException(
            HttpStatusCode.BadRequest,
            new { message = "Username is used" }
        );


        var user = new User
        {
            Name = request.Name,
            Surname = request.Surname,
            Phone = request.Phone,
            Email = request.Email,
            UserName = request.Username
        };

        var result = await _userManager.CreateAsync(user, request.Password!);

        if (result.Succeeded)
            return TransformerUserToUserDto(user);

        throw new Exception("cannot insert new user");
    }
}