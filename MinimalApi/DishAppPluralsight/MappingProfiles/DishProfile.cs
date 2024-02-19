using AutoMapper;
using DishAppPluralsight.Entities;
using DishAppPluralsight.Models;

namespace DishAppPluralsight.MappingProfiles;

public class DishProfile : Profile
{
    public DishProfile()
    {
        CreateMap<Dish, DishDto>();
        CreateMap<DishForCreationDto, Dish>();
        CreateMap<DishForUpdateDto, Dish>();
    }
}