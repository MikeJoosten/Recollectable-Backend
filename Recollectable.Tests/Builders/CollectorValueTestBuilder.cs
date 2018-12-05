using Recollectable.API.Models.Collectables;
using Recollectable.Core.Entities.Collectables;
using System;
using System.Collections.Generic;

namespace Recollectable.Tests.Builders
{
    public class CollectorValueTestBuilder
    {
        private CollectorValue collectorValue;

        public CollectorValueTestBuilder()
        {
            collectorValue = new CollectorValue();
        }

        public CollectorValueTestBuilder WithId(Guid id)
        {
            collectorValue.Id = id;
            return this;
        }

        public CollectorValueTestBuilder WithG4(double g4)
        {
            collectorValue.G4 = g4;
            return this;
        }

        public CollectorValue Build()
        {
            return collectorValue;
        }

        public CollectorValueCreationDto BuildCreationDto()
        {
            return new CollectorValueCreationDto
            {
                G4 = collectorValue.G4
            };
        }

        public CollectorValueUpdateDto BuildUpdateDto()
        {
            return new CollectorValueUpdateDto
            {
                G4 = collectorValue.G4
            };
        }

        public List<CollectorValue> Build(int count)
        {
            var collectorValues = new List<CollectorValue>();

            for (int i = 0; i < count; i++)
            {
                collectorValues.Add(collectorValue);
            }

            return collectorValues;
        }
    }
}