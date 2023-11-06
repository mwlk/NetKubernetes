using NetKubernetes.Models;

namespace Netkubernetes.Repository.Interfaces;

public interface IPropertiesRepository
{
    Task<bool> SaveChanges();

    Task<IEnumerable<Property>> GetAllEntities();

    Task<Property> GetEntityById(int id);

    Task AddAsync(Property property);

    Task Delete(int id);
}