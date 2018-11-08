using FluentValidation;
using Recollectable.Core.Entities.Locations;
using Recollectable.Core.Entities.ResourceParameters;
using Recollectable.Core.Interfaces;
using Recollectable.Core.Models.Locations;
using Recollectable.Core.Shared.Extensions;

namespace Recollectable.API.Validators.Location
{
    public class CountryManipulationDtoValidator<T> : AbstractValidator<T>
        where T : CountryManipulationDto
    {
        private readonly IUnitOfWork _unitOfWork;

        public CountryManipulationDtoValidator(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;

            var countries = _unitOfWork.CountryRepository.Get(new CountriesResourceParameters()).Result;

            RuleFor(c => c.Name)
                .NotEmpty().WithMessage("Name is a required field")
                .MaximumLength(100).WithMessage("Name shouldn't contain more than 100 characters")
                .IsUnique(countries).WithMessage("Name must be unique");
        }
    }
}