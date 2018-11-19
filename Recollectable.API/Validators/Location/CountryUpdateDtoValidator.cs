﻿using Recollectable.Core.Interfaces.Data;
using Recollectable.Core.Models.Locations;

namespace Recollectable.API.Validators.Location
{
    public class CountryUpdateDtoValidator : CountryManipulationDtoValidator<CountryUpdateDto>
    {
        public CountryUpdateDtoValidator(ICountryRepository repository)
            : base(repository)
        {
        }
    }
}