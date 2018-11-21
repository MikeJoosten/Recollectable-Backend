using LinqSpecs.Core;
using Recollectable.Core.Entities.Users;
using System;
using System.Linq.Expressions;

namespace Recollectable.Core.Specifications.Users
{
    public class UserBySearch : Specification<User>
    {
        public string Search { get; }

        public UserBySearch(string search)
        {
            Search = search.Trim().ToLowerInvariant();
        }

        public override Expression<Func<User, bool>> ToExpression()
        {
            return user => user.FirstName.ToLowerInvariant().Contains(Search)
                    || user.LastName.ToLowerInvariant().Contains(Search)
                    || user.UserName.ToLowerInvariant().Contains(Search)
                    || user.Email.ToLowerInvariant().Contains(Search);
        }
    }
}