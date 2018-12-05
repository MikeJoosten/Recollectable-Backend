using Recollectable.Core.Entities.Collections;
using System;
using System.Collections.Generic;

namespace Recollectable.Tests.Builders
{
    public class CollectionTestBuilder
    {
        private Collection collection;

        public CollectionTestBuilder()
        {
            collection = new Collection();
        }

        public CollectionTestBuilder WithId(Guid id)
        {
            collection.Id = id;
            return this;
        }

        public CollectionTestBuilder WithType(string type)
        {
            collection.Type = type;
            return this;
        }

        public Collection Build()
        {
            return collection;
        }

        public List<Collection> Build(int count)
        {
            var collection = new List<Collection>();

            for (int i = 0; i < count; i++)
            {
                collection.Add(this.collection);
            }

            return collection;
        }
    }
}