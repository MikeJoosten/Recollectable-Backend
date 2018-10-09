using AutoMapper;
using Recollectable.Core.Entities.Collectables;
using Recollectable.Core.Entities.Collections;
using Recollectable.Core.Entities.Locations;
using Recollectable.Core.Entities.Users;
using Recollectable.Core.Models.Collectables;
using Recollectable.Core.Models.Collections;
using Recollectable.Core.Models.Locations;
using Recollectable.Core.Models.Users;

namespace Recollectable.Core.Shared.Entities
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
            CreateMap<CollectionCollectable, CollectableDto>();
            CreateMap<CollectableCreationDto, CollectionCollectable>();
            CreateMap<CollectableUpdateDto, CollectionCollectable>();
            CreateMap<CollectionCollectable, CollectableUpdateDto>();
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