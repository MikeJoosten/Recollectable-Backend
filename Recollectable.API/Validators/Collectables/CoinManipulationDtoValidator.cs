using FluentValidation;
using Recollectable.API.Models.Collectables;
using Recollectable.Core.Comparers;
using Recollectable.Core.Entities.ResourceParameters;
using Recollectable.Core.Interfaces;
using Recollectable.Core.Shared.Extensions;

namespace Recollectable.API.Validators.Collectables
{
    public class CoinManipulationDtoValidator<T> : AbstractValidator<T>
        where T : CoinManipulationDto
    {
        private readonly ICoinService _service;

        public CoinManipulationDtoValidator(ICoinService service)
        {
            _service = service;

            var coins = _service.FindCoins(new CurrenciesResourceParameters()).Result;

            RuleFor(b => b)
                .IsDuplicate(coins, new CurrencyComparer(), "Coin must be unique");

            RuleFor(c => c.FaceValue)
                .NotEmpty().WithMessage("Face value is a required field");

            RuleFor(c => c.Type)
                .NotEmpty().WithMessage("Type is a required field")
                .MaximumLength(100).WithMessage("Type shouldn't contain more than 100 characters");

            RuleFor(c => c.ReleaseDate)
                .NotEmpty().WithMessage("ReleaseDate is a required field")
                .MaximumLength(100).WithMessage("ReleaseDate shouldn't contain more than 100 characters");

            RuleFor(c => c.CountryId)
                .NotEmpty().WithMessage("CountryId is a required field");

            RuleFor(c => c.Weight)
                .MaximumLength(25).WithMessage("Weight shouldn't contain more than 25 characters");

            RuleFor(c => c.Size)
                .MaximumLength(25).WithMessage("Size shouldn't contain more than 25 characters");

            RuleFor(c => c.Metal)
                .MaximumLength(100).WithMessage("Metal shouldn't contain more than 100 characters");

            RuleFor(c => c.Note)
                .MaximumLength(250).WithMessage("Note shouldn't contain more than 250 characters");

            RuleFor(c => c.Subject)
                .MaximumLength(250).WithMessage("Subject shouldn't contain more than 250 characters");

            RuleFor(c => c.ObverseDescription)
                .MaximumLength(250).WithMessage("ObverseDescription shouldn't contain more than 250 characters");

            RuleFor(c => c.ObverseInscription)
                .MaximumLength(100).WithMessage("ObverseInscription shouldn't contain more than 100 characters");

            RuleFor(c => c.ObverseLegend)
                .MaximumLength(100).WithMessage("ObverseLegend shouldn't contain more than 100 characters");

            RuleFor(c => c.ReverseDescription)
                .MaximumLength(250).WithMessage("ReverseDescription shouldn't contain more than 250 characters");

            RuleFor(c => c.ReverseInscription)
                .MaximumLength(100).WithMessage("ReverseInscription shouldn't contain more than 100 characters");

            RuleFor(c => c.ReverseLegend)
                .MaximumLength(100).WithMessage("ReverseLegend shouldn't contain more than 100 characters");

            RuleFor(c => c.EdgeType)
                .MaximumLength(50).WithMessage("EdgeType shouldn't contain more than 50 characters");

            RuleFor(c => c.EdgeLegend)
                .MaximumLength(100).WithMessage("EdgeLegend shouldn't contain more than 100 characters");

            RuleFor(c => c.Designer)
                .MaximumLength(250).WithMessage("Designer shouldn't contain more than 250 characters");

            RuleFor(c => c.HeadOfState)
                .MaximumLength(250).WithMessage("HeadOfState shouldn't contain more than 250 characters");

            RuleFor(c => c.MintMark)
                .MaximumLength(50).WithMessage("MintMark shouldn't contain more than 50 characters");

            RuleFor(c => c.FrontImagePath)
                .MaximumLength(250).WithMessage("FrontImagePath shouldn't contain more than 250 characters");

            RuleFor(c => c.BackImagePath)
                .MaximumLength(250).WithMessage("BackImagePath shouldn't contain more than 250 characters");
        }
    }
}