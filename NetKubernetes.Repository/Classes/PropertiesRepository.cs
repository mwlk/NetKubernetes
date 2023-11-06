using System.Net;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Netkubernetes.Repository.Interfaces;
using NetKubernetes.Middleware;
using NetKubernetes.Models;
using NetKubernetes.Persistence;
using NetKubernetes.Token.Interfaces;

namespace NetKubernetes.Repository.Classes;

public class PropertiesRepository : IPropertiesRepository
{
    private readonly AppDbContext _context;
    private readonly IUserSession _userSession;
    private readonly UserManager<User> _userManager;

    public PropertiesRepository(AppDbContext context, IUserSession userSession, UserManager<User> userManager)
    {
        _context = context;
        _userSession = userSession;
        _userManager = userManager;
    }

    public async Task AddAsync(Property property)
    {
        var user = await _userManager.FindByNameAsync(_userSession.GetUserSession());

        if (user is null)
            throw new MiddlewareException(
                HttpStatusCode.Unauthorized,
                new { message = "User Not Valid" }
            );

        if (property is null)
            throw new MiddlewareException(
                HttpStatusCode.BadRequest,
                new { message = "Property is null" }
            );

        property.CreationDate = DateTime.Now;
        property.UserId = Guid.Parse(user!.Id);

        await _context.Properties!.AddAsync(property);
    }

    public async Task Delete(int id)
    {
        var property = await _context.Properties!
        .FirstOrDefaultAsync(p => p.Id == id);

        _context.Properties!.Remove(property!);
    }

    public async Task<IEnumerable<Property>> GetAllEntities()
    {
        return await _context.Properties!.ToListAsync();
    }

    public async Task<Property> GetEntityById(int id)
    {
        return await _context.Properties!
        .FirstOrDefaultAsync(p => p.Id == id)!;
    }

    public async Task<bool> SaveChanges()
    {
        return await _context.SaveChangesAsync() >= 0;
    }
}