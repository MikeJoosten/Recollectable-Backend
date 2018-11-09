using FluentValidation.Validators;
using Recollectable.Core.Shared.Entities;
using System.Linq;

namespace Recollectable.Core.Shared.Validators
{
    public class DuplicateValidator<T> : PropertyValidator
    {
        private readonly PagedList<T> _items;

        public DuplicateValidator(PagedList<T> items)
            : base("Object must be unique")
        {
            _items = items;
        }

        protected override bool IsValid(PropertyValidatorContext context)
        {
            var newValue = context.PropertyValue as string;
            var property = typeof(T).GetProperty(context.PropertyName);

            return _items.All(x => property?.GetValue(x).ToString() != newValue);
        }
    }
}