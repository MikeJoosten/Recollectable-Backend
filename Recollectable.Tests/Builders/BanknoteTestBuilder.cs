using Recollectable.Core.Entities.Collectables;
using System;
using System.Collections.Generic;

namespace Recollectable.Tests.Builders
{
    public class BanknoteTestBuilder
    {
        private Banknote banknote;

        public BanknoteTestBuilder()
        {
            banknote = new Banknote();
        }

        public BanknoteTestBuilder WithId(Guid id)
        {
            banknote.Id = id;
            return this;
        }

        public BanknoteTestBuilder WithType(string type)
        {
            banknote.Type = type;
            return this;
        }

        public Banknote Build()
        {
            return banknote;
        }

        public List<Banknote> Build(int count)
        {
            var banknotes = new List<Banknote>();

            for (int i = 0; i < count; i++)
            {
                banknotes.Add(banknote);
            }

            return banknotes;
        }
    }
}