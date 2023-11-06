using System.Diagnostics.Contracts;
using AutoMapper;
using NetKubernetes.DTO.Properties;
using NetKubernetes.Models;

namespace NetKubernetes.Profiles;

public class PropertyProfile : Profile
{
    public PropertyProfile()
    {
        CreateMap<Property, PropertyResponseDto>();

        CreateMap<PropertyResponseDto, Property>();
    }
}