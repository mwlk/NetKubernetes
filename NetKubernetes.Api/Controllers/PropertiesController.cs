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
    public async Task<IActionResult> GetPropertiesAsync(CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var entities = await _repository.GetAllEntities();

        return Ok(_mapper.Map<IEnumerable<PropertyResponseDto>>(entities));
    }

    [HttpGet("{id}", Name = "GetPropertyById")]
    public async Task<IActionResult> GetPropertyByIdAsync(int id, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var entity = await _repository.
        GetEntityById(id);

        if (entity is null)
            throw new MiddlewareException(HttpStatusCode.NotFound,
            new { message = $"Property id {id} not found" }
            );

        return Ok(_mapper.Map<PropertyResponseDto>(entity));
    }


    [HttpPost]
    public async Task<IActionResult> Save([FromBody] PropertyRequestDto request, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var propertyModel = _mapper.Map<Property>(request);

        await _repository.AddAsync(propertyModel);
        await _repository.SaveChanges();

        var response = _mapper.Map<PropertyResponseDto>(propertyModel);

        return CreatedAtRoute(nameof(GetPropertyByIdAsync), new { response.Id });
    }

    [HttpDelete]
    public async Task<IActionResult> Delete(int id)
    {
        await _repository.Delete(id);
        await _repository.SaveChanges();
        return Ok();
    }
}