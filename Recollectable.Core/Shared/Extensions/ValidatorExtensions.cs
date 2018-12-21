using FluentValidation;
using Recollectable.Core.Shared.Entities;
using Recollectable.Core.Shared.Validators;
using System.Collections.Generic;

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

        public static IRuleBuilder<T, T> IsDuplicate<T, TProperty>
        (this IRuleBuilder<T, T> ruleBuilder, PagedList<TProperty> items, IEqualityComparer<TProperty> comparer, string message)
            where TProperty : class
        {
            return ruleBuilder.SetValidator(new DuplicateValidator<TProperty>(items, comparer)).WithMessage(message);
        }
    }
}