using AutoMapper;
using DishAppPluralsight.Entities;
using DishAppPluralsight.Models;

namespace DishAppPluralsight.MappingProfiles;

public class IngredientProfile : Profile
{
    public IngredientProfile()
    {
        CreateMap<Ingredient, IngredientDto>()
            .ForMember(
                dest => dest.DishId,
                o => o.MapFrom(src => src.Dishes.First().Id));
    }
}