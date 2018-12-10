using Recollectable.Core.Entities.Collections;
using Recollectable.Core.Entities.ResourceParameters;
using Recollectable.Core.Interfaces;
using Recollectable.Core.Shared.Entities;
using Recollectable.Core.Shared.Extensions;
using Recollectable.Core.Shared.Services;
using Recollectable.Core.Specifications.Collections;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Recollectable.Core.Services
{
    public class ConditionService : IConditionService
    {
        private readonly IUnitOfWork _unitOfWork;

        public ConditionService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<PagedList<Condition>> FindConditions(ConditionsResourceParameters resourceParameters)
        {
            var conditions = await _unitOfWork.Conditions.GetAll();

            if (!string.IsNullOrEmpty(resourceParameters.Grade))
            {
                conditions = await _unitOfWork.Conditions.GetAll(new ConditionByGrade(resourceParameters.Grade));
            }

            if (!string.IsNullOrEmpty(resourceParameters.LanguageCode))
            {
                conditions = await _unitOfWork.Conditions.GetAll(new ConditionByLanguageCode(resourceParameters.LanguageCode));
            }

            if (!string.IsNullOrEmpty(resourceParameters.Search))
            {
                conditions = await _unitOfWork.Conditions.GetAll(new ConditionBySearch(resourceParameters.Search));
            }

            conditions = conditions.OrderBy(resourceParameters.OrderBy,
                PropertyMappingService.ConditionPropertyMapping);

            return PagedList<Condition>.Create(conditions.ToList(), resourceParameters.Page, resourceParameters.PageSize);
        }

        public async Task<Condition> FindConditionById(Guid id)
        {
            return await _unitOfWork.Conditions.GetSingle(new ConditionById(id));
        }

        public async Task CreateCondition(Condition condition)
        {
            await _unitOfWork.Conditions.Add(condition);
        }

        public void UpdateCondition(Condition condition) { }

        public void RemoveCondition(Condition condition)
        {
            _unitOfWork.Conditions.Delete(condition);
        }

        public async Task<bool> ConditionExists(Guid id)
        {
            var condition = await _unitOfWork.Conditions.GetSingle(new ConditionById(id));
            return condition != null;
        }

        public async Task<bool> Save()
        {
            return await _unitOfWork.Save();
        }
    }
}