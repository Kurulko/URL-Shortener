using AutoMapper;
using URL_ShortenerAPI.Data.Models;
using URL_ShortenerAPI.Data.DTOs;
using URL_ShortenerAPI.Extentions;

namespace URL_ShortenerAPI.Profiles;

public class ShortUrlProfile : Profile
{
    public ShortUrlProfile()
    {
        CreateMap<ShortUrl, ShortUrlDTO>()
            .ForMember(
                dest => dest.IsCreatedByYou, 
                opt => opt.MapFrom<IsCreatedByYouResolver>()
            )
            .ReverseMap();
    }
}

public class IsCreatedByYouResolver : IValueResolver<ShortUrl, ShortUrlDTO, bool>
{
    readonly IHttpContextAccessor httpContextAccessor;
    public IsCreatedByYouResolver(IHttpContextAccessor httpContextAccessor)
        => this.httpContextAccessor = httpContextAccessor;

    public bool Resolve(ShortUrl source, ShortUrlDTO destination, bool destMember, ResolutionContext context)
    {
        return source.UserId == httpContextAccessor.GetUserId()!;
    }
}