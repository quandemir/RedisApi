using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Text;

namespace RedisExampleApı.Cache
{
    public class RedisService
    {
        private readonly IConnectionMultiplexer _connectionMultiplexer;

        public RedisService(string url)
        {
            _connectionMultiplexer =ConnectionMultiplexer.Connect(url);
        }
        //Method specifying which database we want to use
        public IDatabase GetDb(int dbIndex)
        {
            return _connectionMultiplexer.GetDatabase(dbIndex);
        }
    }
}
