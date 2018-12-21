using Recollectable.API.Models.Collectables;
using Recollectable.Core.Entities.Collectables;
using System;
using System.Collections.Generic;

namespace Recollectable.Tests.Builders
{
    public class CoinTestBuilder
    {
        private Coin coin;

        public CoinTestBuilder()
        {
            coin = new Coin();
        }

        public CoinTestBuilder WithId(Guid id)
        {
            coin.Id = id;
            return this;
        }

        public CoinTestBuilder WithType(string type)
        {
            coin.Type = type;
            return this;
        }

        public CoinTestBuilder WithNote(string note)
        {
            coin.Note = note;
            return this;
        }

        public CoinTestBuilder WithSubject(string subject)
        {
            coin.Subject = subject;
            return this;
        }

        public CoinTestBuilder WithCountryId(Guid countryId)
        {
            coin.CountryId = countryId;
            return this;
        }

        public Coin Build()
        {
            return coin;
        }

        public CoinCreationDto BuildCreationDto()
        {
            return new CoinCreationDto
            {
                Type = coin.Type,
                Note = coin.Note,
                Subject = coin.Subject,
                CountryId = coin.CountryId
            };
        }

        public CoinUpdateDto BuildUpdateDto()
        {
            return new CoinUpdateDto
            {
                Type = coin.Type,
                Note = coin.Note,
                Subject = coin.Subject,
                CountryId = coin.CountryId
            };
        }

        public List<Coin> Build(int count)
        {
            var coins = new List<Coin>();

            for (int i = 0; i < count; i++)
            {
                coins.Add(coin);
            }

            return coins;
        }
    }
}