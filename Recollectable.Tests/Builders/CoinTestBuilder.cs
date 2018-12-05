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

        public Coin Build()
        {
            return coin;
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