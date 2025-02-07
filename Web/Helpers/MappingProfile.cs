using Application.DTOs;
using AutoMapper;
using Web.Models;

namespace Web.Helpers;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<BookSessionViewModel, BookSessionDto>()
            .ForMember<object>(dest => dest.SelectedSeats, opt => opt.MapFrom(src => src.SelectedSeats));
        
        CreateMap<BookSessionDto, BookSessionViewModel>();
        CreateMap<SeatDto, SeatViewModel>();
    }
}