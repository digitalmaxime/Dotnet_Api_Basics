using AutoMapper;

namespace EFCore_JulieLerman.MappingProfiles;

public class AuthorMappingProfile : Profile
{
    public AuthorMappingProfile()
    {
        CreateMap<Domain.Author, Dtos.AuthorDto>();
    }
}