using System;
using System.Configuration;
using StackExchange.Redis;

namespace MB.Common
{
    public static class TagRedis
    {
        private static string _redisConn = ConfigurationManager.ConnectionStrings["TargetNavigationRedisEntities"].ConnectionString;

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