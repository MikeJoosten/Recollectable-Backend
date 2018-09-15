﻿using Microsoft.EntityFrameworkCore;
using Recollectable.Core.DTOs.Collectables;
using Recollectable.Core.Entities.Collectables;
using Recollectable.Core.Entities.Common;
using Recollectable.Core.Entities.ResourceParameters;
using Recollectable.Core.Extensions;
using System;
using System.Linq;

namespace Recollectable.Infrastructure.Data.Repositories
{
    public class BanknoteRepository : BaseRepository<Banknote, CurrenciesResourceParameters>
    {
        private RecollectableContext _context;

        public BanknoteRepository(RecollectableContext context)
        {
            _context = context;
        }

        public override PagedList<Banknote> Get(CurrenciesResourceParameters resourceParameters)
        {
            var banknotes = _context.Banknotes
                .Include(c => c.Country)
                .Include(c => c.CollectorValue)
                .ApplySort(resourceParameters.OrderBy,
                    _propertyMappingService.GetPropertyMapping<BanknoteDto, Banknote>());

            if (!string.IsNullOrEmpty(resourceParameters.Type))
            {
                var type = resourceParameters.Type.Trim().ToLowerInvariant();
                banknotes = banknotes.Where(b => b.Type.ToLowerInvariant() == type);
            }

            if (!string.IsNullOrEmpty(resourceParameters.Country))
            {
                var country = resourceParameters.Country.Trim().ToLowerInvariant();
                banknotes = banknotes.Where(b => b.Country.Name.ToLowerInvariant() == country);
            }

            if (!string.IsNullOrEmpty(resourceParameters.Search))
            {
                var search = resourceParameters.Search.Trim().ToLowerInvariant();
                banknotes = banknotes.Where(b => b.Country.Name.ToLowerInvariant().Contains(search)
                    || b.Type.ToLowerInvariant().Contains(search)
                    || b.ReleaseDate.ToLowerInvariant().Contains(search)
                    || b.Color.ToLowerInvariant().Contains(search));
            }

            return PagedList<Banknote>.Create(banknotes,
                resourceParameters.Page,
                resourceParameters.PageSize);
        }

        public override Banknote GetById(Guid banknoteId)
        {
            return _context.Banknotes
                .Include(b => b.Country)
                .Include(b => b.CollectorValue)
                .FirstOrDefault(b => b.Id == banknoteId);
        }

        public override void Add(Banknote banknote)
        {
            if (banknote.Id == Guid.Empty)
            {
                banknote.Id = Guid.NewGuid();
            }

            if (banknote.CountryId == Guid.Empty)
            {
                banknote.CountryId = Guid.NewGuid();
            }

            if (banknote.CollectorValueId == Guid.Empty)
            {
                banknote.CollectorValueId = Guid.NewGuid();
            }

            _context.Banknotes.Add(banknote);
        }

        public override void Update(Banknote banknote) { }

        public override void Delete(Banknote banknote)
        {
            _context.Banknotes.Remove(banknote);
        }

        public override bool Exists(Guid banknoteId)
        {
            return _context.Banknotes.Any(b => b.Id == banknoteId);
        }
    }
}