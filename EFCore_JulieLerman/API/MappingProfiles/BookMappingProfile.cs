using AutoMapper;

namespace EFCore_JulieLerman.MappingProfiles;

public class BookMappingProfile : Profile
{
    public BookMappingProfile()
    {
        CreateMap<Domain.Book, Dtos.BookDto>();
    }
}