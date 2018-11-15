using AutoMapper;
using FluentValidation.Validators;
using Recollectable.Core.Shared.Entities;
using System.Collections.Generic;
using System.Linq;

namespace Recollectable.Core.Shared.Validators
{
    public class DuplicateValidator<T> : PropertyValidator
    {
        private readonly PagedList<T> _items;
        private readonly IEqualityComparer<T> _comparer;

        public DuplicateValidator(PagedList<T> items, IEqualityComparer<T> comparer)
            : base("Object must be unique")
        {
            _items = items;
            _comparer = comparer;
        }

        protected override bool IsValid(PropertyValidatorContext context)
        {
            var newItem = Mapper.Map<T>(context.PropertyValue);
            return _items.Contains(newItem, _comparer) ? false : true;
        }
    }
}