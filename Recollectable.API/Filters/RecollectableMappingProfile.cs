using AutoMapper;
using Recollectable.API.Models.Collectables;
using Recollectable.API.Models.Collections;
using Recollectable.API.Models.Locations;
using Recollectable.API.Models.Users;
using Recollectable.Core.Entities.Collectables;
using Recollectable.Core.Entities.Collections;
using Recollectable.Core.Entities.Locations;
using Recollectable.Core.Entities.Users;

namespace Recollectable.API.Filters
{
    public class RecollectableMappingProfile : Profile
    {
        public RecollectableMappingProfile()
        {
            CreateMap<User, UserDto>().ForMember(dest => dest.Name,
                    opt => opt.MapFrom(src => $"{src.FirstName} {src.LastName}"));
            CreateMap<UserCreationDto, User>();
            CreateMap<UserUpdateDto, User>();
            CreateMap<User, UserUpdateDto>();
            CreateMap<Collection, CollectionDto>();
            CreateMap<CollectionCreationDto, Collection>();
            CreateMap<CollectionUpdateDto, Collection>();
            CreateMap<Collection, CollectionUpdateDto>();
            CreateMap<CollectionCollectable, CollectionCollectableDto>();
            CreateMap<CollectionCollectableCreationDto, CollectionCollectable>();
            CreateMap<CollectionCollectableUpdateDto, CollectionCollectable>();
            CreateMap<CollectionCollectable, CollectionCollectableUpdateDto>();
            CreateMap<Coin, CoinDto>();
            CreateMap<CoinCreationDto, Coin>();
            CreateMap<CoinUpdateDto, Coin>();
            CreateMap<Coin, CoinUpdateDto>();
            CreateMap<Banknote, BanknoteDto>();
            CreateMap<BanknoteCreationDto, Banknote>();
            CreateMap<BanknoteUpdateDto, Banknote>();
            CreateMap<Banknote, BanknoteUpdateDto>();
            CreateMap<Country, CountryDto>();
            CreateMap<CountryCreationDto, Country>();
            CreateMap<CountryUpdateDto, Country>();
            CreateMap<Country, CountryUpdateDto>();
            CreateMap<CollectorValue, CollectorValueDto>();
            CreateMap<CollectorValueCreationDto, CollectorValue>();
            CreateMap<CollectorValueUpdateDto, CollectorValue>();
            CreateMap<CollectorValue, CollectorValueUpdateDto>();
        }
    }
}