using System.Net;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Netkubernetes.Repository.Interfaces;
using NetKubernetes.DTO.Properties;
using NetKubernetes.Middleware;
using NetKubernetes.Models;

namespace NetKubernetes.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PropertiesController : ControllerBase
{
    private readonly IPropertiesRepository _repository;
    private IMapper _mapper;

    public PropertiesController(IPropertiesRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    [HttpGet]
    public IActionResult GetProperties(CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var entities = _repository.GetAllEntities();

        return Ok(_mapper.Map<IEnumerable<PropertyResponseDto>>(entities));
    }

    [HttpGet("{id}", Name = "GetPropertyById")]
    public IActionResult GetPropertyById(int id, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var entity = _repository.
        GetEntityById(id);

        if (entity is null)
            throw new MiddlewareException(HttpStatusCode.NotFound,
            new { message = $"Property id {id} not found" }
            );

        return Ok(_mapper.Map<PropertyResponseDto>(entity));
    }


    [HttpPost]
    public IActionResult Save([FromBody] PropertyRequestDto request, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var propertyModel = _mapper.Map<Property>(request);

        _repository.AddAsync(propertyModel);
        _repository.SaveChanges();

        var response = _mapper.Map<PropertyResponseDto>(propertyModel);

        return CreatedAtRoute(nameof(GetPropertyById), new { response.Id });
    }

    [HttpDelete]
    public IActionResult Delete(int id)
    {
        _repository.Delete(id);
        _repository.SaveChanges();
        return Ok();
    }
}