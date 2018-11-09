using FluentValidation;
using Recollectable.Core.Models.Collectables;

namespace Recollectable.API.Validators.Collectables
{
    public class CoinManipulationDtoValidator<T> : AbstractValidator<T>
        where T : CoinManipulationDto
    {
        public CoinManipulationDtoValidator()
        {
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