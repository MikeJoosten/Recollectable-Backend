using Recollectable.Core.Entities.Collections;
using System;
using System.Collections.Generic;

namespace Recollectable.Tests.Builders
{
    public class ConditionTestBuilder
    {
        private Condition condition;

        public ConditionTestBuilder()
        {
            condition = new Condition();
        }

        public ConditionTestBuilder WithId(Guid id)
        {
            condition.Id = id;
            return this;
        }

        public ConditionTestBuilder WithGrade(string grade)
        {
            condition.Grade = grade;
            return this;
        }

        public Condition Build()
        {
            return condition;
        }

        /*public ConditionCreationDto BuildCreationDto()
        {
            return new ConditionCreationDto
            {
                Grade = condition.Grade
            };
        }

        public ConditionUpdateDto BuildUpdateDto()
        {
            return new ConditionUpdateDto
            {
                Name = condition.Name
            };
        }*/

        public List<Condition> Build(int count)
        {
            var conditions = new List<Condition>();

            for (int i = 0; i < count; i++)
            {
                conditions.Add(condition);
            }

            return conditions;
        }
    }
}