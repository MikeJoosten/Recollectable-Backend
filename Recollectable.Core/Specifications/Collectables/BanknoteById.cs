using LinqSpecs.Core;
using Recollectable.Core.Entities.Collectables;
using System;
using System.Linq.Expressions;

namespace Recollectable.Core.Specifications.Collectables
{
    public class BanknoteById : Specification<Banknote>
    {
        public Guid Id { get; }

        public BanknoteById(Guid id)
        {
            Id = id;
        }

        public override Expression<Func<Banknote, bool>> ToExpression()
        {
            return banknote => banknote.Id == Id;
        }
    }
}