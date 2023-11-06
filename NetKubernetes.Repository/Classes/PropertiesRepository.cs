using System.Net;
using Microsoft.AspNetCore.Identity;
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

        _context.Properties!.Add(property);
    }

    public void Delete(int id)
    {
        var property = _context.Properties!
        .FirstOrDefault(p => p.Id == id);

        _context.Properties!.Remove(property!);
    }

    public IEnumerable<Property> GetAllEntities()
    {
        return _context.Properties!.ToList();
    }

    public Property GetEntityById(int id)
    {
        return _context.Properties!
        .FirstOrDefault(p => p.Id == id)!;
    }

    public bool SaveChanges()
    {
        return _context.SaveChanges() >= 0;
    }
}