using LinqSpecs.Core;
using Recollectable.Core.Entities.Users;
using System;
using System.Linq.Expressions;

namespace Recollectable.Core.Specifications.Users
{
    public class UserById : Specification<User>
    {
        public Guid Id { get; }

        public UserById(Guid id)
        {
            Id = id;
        }

        public override Expression<Func<User, bool>> ToExpression()
        {
            return user => user.Id == Id;
        }
    }
}