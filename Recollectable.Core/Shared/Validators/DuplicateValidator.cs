using AutoMapper;
using FluentValidation.Validators;
using Recollectable.Core.Shared.Entities;

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
            var newItem = Mapper.Map<T>(context.PropertyValue);

            foreach (T item in _items)
            {
                if (item.Equals(newItem))
                {
                    return false;
                }
            }

            return true;
        }
    }
}