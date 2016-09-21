using StackExchange.Redis;
using System;
using System.Configuration;

namespace MB.New.Common
{
    public static class RedisHelper
    {
        private static readonly string _redisConn = ConfigurationManager.ConnectionStrings["TargetNavigationRedisEntities"].ConnectionString;

        private static Lazy<ConnectionMultiplexer> _lazyConnection = new Lazy<ConnectionMultiplexer>(() =>
        {
            return ConnectionMultiplexer.Connect(_redisConn);
        });

        public static ConnectionMultiplexer redis
        {
            get
            {
                return _lazyConnection.Value;
            }
        }
    }
}