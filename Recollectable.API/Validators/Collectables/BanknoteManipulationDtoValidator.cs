using AutoMapper;
using FluentValidation;
using Recollectable.API.Models.Collectables;
using Recollectable.Core.Entities.Collectables;
using Recollectable.Core.Entities.ResourceParameters;
using Recollectable.Core.Interfaces;
using Recollectable.Core.Shared.Extensions;
using System.Collections.Generic;

namespace Recollectable.API.Validators.Collectables
{
    public class BanknoteManipulationDtoValidator<T> : AbstractValidator<T>
        where T : BanknoteManipulationDto
    {
        private readonly IBanknoteService _service;
        private readonly IEqualityComparer<Currency> _comparer;
        private IMapper _mapper;

        public BanknoteManipulationDtoValidator(IBanknoteService service, IEqualityComparer<Currency> comparer, IMapper mapper)
        {
            _service = service;
            _comparer = comparer;
            _mapper = mapper;

            var banknotes = _service.FindBanknotes(new CurrenciesResourceParameters()).Result;

            RuleFor(b => b)
                .IsDuplicate(banknotes, _comparer, _mapper, "Banknote must be unique");

            RuleFor(b => b.FaceValue)
                .NotEmpty().WithMessage("Face value is a required field");

            RuleFor(b => b.Type)
                .NotEmpty().WithMessage("Type is a required field")
                .MaximumLength(100).WithMessage("Type shouldn't contain more than 100 characters");

            RuleFor(b => b.ReleaseDate)
                .NotEmpty().WithMessage("ReleaseDate is a required field")
                .MaximumLength(100).WithMessage("ReleaseDate shouldn't contain more than 100 characters");

            RuleFor(b => b.CountryId)
                .NotEmpty().WithMessage("CountryId is a required field");

            RuleFor(b => b.Color)
                .MaximumLength(250).WithMessage("Color shouldn't contain more than 250 characters");

            RuleFor(b => b.Watermark)
                .MaximumLength(100).WithMessage("Watermark shouldn't contain more than 100 characters");

            RuleFor(b => b.Signature)
                .MaximumLength(250).WithMessage("Signature shouldn't contain more than 250 characters");

            RuleFor(b => b.ObverseDescription)
                .MaximumLength(250).WithMessage("ObverseDescription shouldn't contain more than 250 characters");

            RuleFor(b => b.ReverseDescription)
                .MaximumLength(250).WithMessage("ReverseDescription shouldn't contain more than 250 characters");

            RuleFor(b => b.Designer)
                .MaximumLength(250).WithMessage("Designer shouldn't contain more than 250 characters");

            RuleFor(b => b.HeadOfState)
                .MaximumLength(250).WithMessage("HeadOfState shouldn't contain more than 250 characters");

            RuleFor(b => b.FrontImagePath)
                .MaximumLength(250).WithMessage("FrontImagePath shouldn't contain more than 250 characters");

            RuleFor(b => b.BackImagePath)
                .MaximumLength(250).WithMessage("BackImagePath shouldn't contain more than 250 characters");
        }
    }
}