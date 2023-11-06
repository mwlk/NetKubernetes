using NetKubernetes.Models;

namespace Netkubernetes.Repository.Interfaces;

public interface IPropertiesRepository
{
    bool SaveChanges();

    IEnumerable<Property> GetAllEntities();

    Property GetEntityById(int id);

    Task AddAsync(Property property);

    void Delete(int id);
}