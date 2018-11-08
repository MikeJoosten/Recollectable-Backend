using FluentValidation;
using Recollectable.Core.Shared.Entities;
using Recollectable.Core.Shared.Validators;

namespace Recollectable.Core.Shared.Extensions
{
    public static class ValidatorExtensions
    {
        public static IRuleBuilderOptions<T, string> IsUnique<T, TProperty>
        (this IRuleBuilder<T, string> ruleBuilder, PagedList<TProperty> items)
            where TProperty : class
        {
            return ruleBuilder.SetValidator(new UniqueValidator<TProperty>(items));
        }
    }
}