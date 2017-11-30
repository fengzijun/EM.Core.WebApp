using EM.Configuration;
using EM.Infrastructure;
using Newtonsoft.Json;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EM.Caching
{
    /// <summary>
    ///redis 缓存
    /// </summary>
    public partial class RedisCacheManager : IRedis
    {
        #region Fields

        //private readonly ConnectionMultiplexer _muxer = null;
        private IDatabase _db;

        //private readonly IRedis _perRequestCacheManager;
        private readonly ConfigurationOptions redisconfig;

        private ILogger logger;

        #endregion Fields

        #region Ctor

        public RedisCacheManager(SystemConfig config)
        {
            if (String.IsNullOrEmpty(config.RedisCachingConnectionString))
                throw new Exception("Redis connection string is empty");

            if (redisconfig == null)
            {
                redisconfig = new ConfigurationOptions();
                redisconfig.ConnectTimeout = 5000;
                redisconfig.ResponseTimeout = 5000;
                redisconfig.SyncTimeout = 5000;
                redisconfig.EndPoints.Add(config.RedisCachingConnectionString, int.Parse(config.RedisPort));
                //if (!config.SystemSetting.IsDev)
                //    redisconfig.CommandMap = CommandMap.Sentinel;
            }

            logger = EngineContext.Current.Resolve<ILogger>();
            //this._muxer = ConnectionMultiplexer.Connect(redisconfig);

            //this._db = _muxer.GetDatabase();
            //this._perRequestCacheManager = EngineContext.Current.Resolve<IRedis>();
        }

        #endregion Ctor

        #region Utilities

        protected virtual byte[] Serialize(object item)
        {
            var jsonString = JsonConvert.SerializeObject(item);
            return Encoding.UTF8.GetBytes(jsonString);
        }

        protected virtual T Deserialize<T>(byte[] serializedObject)
        {
            if (serializedObject == null)
                return default(T);

            var jsonString = Encoding.UTF8.GetString(serializedObject);
            return JsonConvert.DeserializeObject<T>(jsonString);
        }

        #endregion Utilities

        #region Methods

        /// <summary>
        /// Gets or sets the value associated with the specified key.
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <param name="key">The key of the value to get.</param>
        /// <returns>The value associated with the specified key.</returns>
        public virtual T Get<T>(string key)
        {
            //little performance workaround here:
            //we use "PerRequestCacheManager" to cache a loaded object in memory for the current HTTP request.
            //this way we won't connect to Redis server 500 times per HTTP request (e.g. each time to load a locale or setting)
            try
            {
                using (ConnectionMultiplexer mux = ConnectionMultiplexer.Connect(redisconfig))
                {
                    this._db = mux.GetDatabase();
                    if (_db.KeyExists(key))
                    {
                        RedisValue result = _db.StringGet(key);
                        if (result.HasValue)
                            return Deserialize<T>(result);
                    }

                    return default(T);

                    //var rValue = _db.StringGet(key);
                    //if (!rValue.HasValue)
                    //    return default(T);
                    //var result = Deserialize<T>(rValue);

                    //_perRequestCacheManager.Set(key, result, 0);
                    //return result;
                }
            }
            catch (Exception ex)
            {
                if (logger != null)
                    logger.WriteErrorLog(ex);

                return default(T);
            }
        }

        /// <summary>
        /// Adds the specified key and object to the cache.
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="data">Data</param>
        /// <param name="cacheTime">Cache time</param>
        public virtual void Set(string key, object data, int cacheTime)
        {
            try
            {
                if (data == null)
                    return;

                var entryBytes = Serialize(data);
                var expiresIn = TimeSpan.FromMinutes(cacheTime);
                using (ConnectionMultiplexer mux = ConnectionMultiplexer.Connect(redisconfig))
                {
                    this._db = mux.GetDatabase();
                    _db.StringSet(key, entryBytes, expiresIn);
                }
            }
            catch (Exception ex)
            {
                if (logger != null)
                    logger.WriteErrorLog(ex);
            }
        }

        /// <summary>
        /// batchset authorinfo
        /// </summary>
        /// <param name="keys"></param>
        /// <param name="objs"></param>
        public virtual void BatchSetAuthorInfo(List<string> keys, List<string> objs)
        {
            try
            {
                if (keys.Count != objs.Count)
                    return;
                using (ConnectionMultiplexer mux = ConnectionMultiplexer.Connect(redisconfig))
                {
                    this._db = mux.GetDatabase();
                    for (int i = 0; i < keys.Count; i++)
                    {
                        _db.StringSet("AuthorInfo_" + keys[i], objs[i]);
                    }
                }
            }
            catch (Exception ex)
            {
                if (logger != null)
                    logger.WriteErrorLog(ex);

                Console.WriteLine(ex.Message);
            }
        }

        /// <summary>
        /// batchset authorinfo
        /// </summary>
        /// <param name="keys"></param>
        /// <param name="objs"></param>
        public virtual void SetAuthorInfo(string key, string obj)
        {
            try
            {
                using (ConnectionMultiplexer mux = ConnectionMultiplexer.Connect(redisconfig))
                {
                    this._db = mux.GetDatabase();
                    _db.StringSet("AuthorInfo_" + key, obj);
                }
            }
            catch (Exception ex)
            {
                if (logger != null)
                    logger.WriteErrorLog(ex);

                Console.WriteLine(ex.Message);
            }
        }

        /// <summary>
        /// batch set articleinfo
        /// </summary>
        /// <param name="keys"></param>
        /// <param name="objs"></param>
        public virtual void BatchSetArticleInfo(List<string> keys, List<string> objs)
        {
            try
            {
                if (keys.Count != objs.Count)
                    return;
                using (ConnectionMultiplexer mux = ConnectionMultiplexer.Connect(redisconfig))
                {
                    this._db = mux.GetDatabase();
                    for (int i = 0; i < keys.Count; i++)
                    {
                        _db.StringSet("Article_" + keys[i], objs[i]);
                    }
                }
            }
            catch (Exception ex)
            {
                if (logger != null)
                    logger.WriteErrorLog(ex);
            }
        }

        public virtual void Incr(string type, string id, string accouttype)
        {
            try
            {
                using (ConnectionMultiplexer mux = ConnectionMultiplexer.Connect(redisconfig))
                {
                    this._db = mux.GetDatabase();
                    _db.StringIncrement(type + "_" + id);
                    _db.SortedSetIncrement("click_" + type, id, 1);
                    _db.HashSet("ClickHours_7_" + type, id, DateTime.Now.ToString());//一周排行
                    _db.HashSet("ClickHours_3_" + type, id, DateTime.Now.ToString());//72小时排行
                    if (!string.IsNullOrEmpty(accouttype))
                    {
                        string[] typeitems = accouttype.Split(new char[] { ',' });
                        foreach (var item in typeitems)
                        {
                            _db.SortedSetIncrement("click_" + type + "_" + item, id, 1);
                        }
                    }
                    //财富号栏目排行
                    //_db.SortedSetAdd("click_topic", key, 1);
                }
            }
            catch (Exception ex)
            {
                if (logger != null)
                    logger.WriteErrorLog(ex);
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="type"></param>
        /// <param name="isweek"></param>
        public virtual void ClearTimeouotData(string type, bool isweek)
        {
            try
            {
                using (ConnectionMultiplexer mux = ConnectionMultiplexer.Connect(redisconfig))
                {
                    this._db = mux.GetDatabase();
                    if (!isweek)
                    {
                        var list = _db.HashGetAll("ClickHours_3_" + type);
                        foreach (var item in list)
                        {
                            if (DateTime.Now.Subtract(DateTime.Parse(item.Value)).TotalHours > 72)
                                _db.HashDelete("ClickHours_3_" + type, item.Name);
                        }
                    }
                    else
                    {
                        var list = _db.HashGetAll("ClickHours_7_" + type);
                        foreach (var item in list)
                        {
                            if (DateTime.Now.Subtract(DateTime.Parse(item.Value)).TotalDays > 7)
                                _db.HashDelete("ClickHours_7_" + type, item.Name);
                        }
                    }

                    //_db.SortedSetAdd("click_topic", key, 1);
                }
            }
            catch (Exception ex)
            {
                if (logger != null)
                    logger.WriteErrorLog(ex);
            }
        }

        public virtual void ClearListData(string type, string id)
        {
            try
            {
                using (ConnectionMultiplexer mux = ConnectionMultiplexer.Connect(redisconfig))
                {
                    this._db = mux.GetDatabase();
                    _db.SortedSetRemove("click_" + type, id);
                    _db.HashDelete("ClickHours_3_" + type, id);
                    _db.HashDelete("ClickHours_7_" + type, id);
                }
            }
            catch (Exception ex)
            {
                if (logger != null)
                    logger.WriteErrorLog(ex);
            }
        }

        public virtual void ReSotreListData(string type, string id)
        {
            try
            {
                using (ConnectionMultiplexer mux = ConnectionMultiplexer.Connect(redisconfig))
                {
                    this._db = mux.GetDatabase();
                    string key = type + "_" + id;
                    RedisValue obj = _db.StringGet(key);
                    _db.SortedSetIncrement("click_" + type, id, long.Parse(obj.ToString()));
                    //_db.HashSet("ClickHours_7_" + type, id, DateTime.Now.ToString());//一周排行
                    //_db.HashSet("ClickHours_3_" + type, id, DateTime.Now.ToString());//72小时排行
                }
            }
            catch (Exception ex)
            {
                if (logger != null)
                    logger.WriteErrorLog(ex);
            }
        }

        public Dictionary<string, long> GetClickNum(string type, bool isweek)
        {
            Dictionary<string, long> dic = new Dictionary<string, long>();
            try
            {
                using (ConnectionMultiplexer mux = ConnectionMultiplexer.Connect(redisconfig))
                {
                    this._db = mux.GetDatabase();
                    HashEntry[] list;
                    if (isweek)
                        list = _db.HashGetAll("ClickHours_7_" + type);
                    else
                        list = _db.HashGetAll("ClickHours_3_" + type);

                    var ids = list.Select(s => s.Name).ToArray();
                    foreach (var id in ids)
                    {
                        RedisValue clickcount = _db.StringGet(type + "_" + id);
                        if (clickcount.HasValue)
                            dic.Add(id, long.Parse(clickcount.ToString()));
                        else
                            dic.Add(id, 0);
                    }

                    return dic;
                }
            }
            catch (Exception ex)
            {
                if (logger != null)
                    logger.WriteErrorLog(ex);
                return dic;
            }
        }

        public virtual string GetCount(string key)
        {
            try
            {
                using (ConnectionMultiplexer mux = ConnectionMultiplexer.Connect(redisconfig))
                {
                    this._db = mux.GetDatabase();
                    RedisValue obj = _db.StringGet(key);
                    return obj.ToString();
                }
            }
            catch (Exception ex)
            {
                if (logger != null)
                    logger.WriteErrorLog(ex);
                return "0";
            }
        }

        public virtual Dictionary<string, double> GetCountPageList(string type, int pageindex, int pagesize, int order)
        {
            try
            {
                using (ConnectionMultiplexer mux = ConnectionMultiplexer.Connect(redisconfig))
                {
                    this._db = mux.GetDatabase();
                    SortedSetEntry[] obj = _db.SortedSetRangeByRankWithScores("click_" + type, (pageindex - 1) * pagesize, (pageindex * pagesize) - 1, order == 0 ? Order.Ascending : Order.Descending);
                    //_db.SortedSetScan()
                    return obj.ToStringDictionary();
                }
            }
            catch (Exception ex)
            {
                if (logger != null)
                    logger.WriteErrorLog(ex);

                return new Dictionary<string, double>();
            }
        }

        public virtual Dictionary<string, double> GetAccountPageList(string accounttype, int pageindex, int pagesize)
        {
            try
            {
                using (ConnectionMultiplexer mux = ConnectionMultiplexer.Connect(redisconfig))
                {
                    this._db = mux.GetDatabase();
                    SortedSetEntry[] obj = _db.SortedSetRangeByRankWithScores("click_0_" + accounttype, (pageindex - 1) * pagesize, (pageindex * pagesize) - 1, Order.Descending);
                    //_db.SortedSetScan()
                    return obj.ToStringDictionary();
                }
            }
            catch (Exception ex)
            {
                if (logger != null)
                    logger.WriteErrorLog(ex);

                return new Dictionary<string, double>();
            }
        }

        public virtual Dictionary<string, double> GetCountPageList(string[] ids, string type, int pageindex, int pagesize, int order)
        {
            try
            {
                if (ids.Length == 0)
                    return new Dictionary<string, double>();
                var result = new Dictionary<string, double>();
                using (ConnectionMultiplexer mux = ConnectionMultiplexer.Connect(redisconfig))
                {
                    this._db = mux.GetDatabase();
                    //SortedSetEntry[] obj = _db.SortedSetRangeByRankWithScores("click_" + type, (pageindex - 1) * pagesize, pageindex * pagesize, order == 0 ? Order.Ascending : Order.Descending);
                    foreach (var id in ids)
                    {
                        RedisValue obj = _db.StringGet(type + "_" + id);
                        if (obj.HasValue)
                            result.Add(id, double.Parse(obj.ToString()));
                        else
                            result.Add(id, Convert.ToInt64(0));
                    }

                    var sortedDict = order == 1 ? result.OrderByDescending(x => x.Value) : result.OrderBy(x => x.Value);

                    return sortedDict.Skip((pageindex - 1) * pagesize).Take(pagesize).ToDictionary(x => x.Key, x => x.Value);
                }
            }
            catch (Exception ex)
            {
                if (logger != null)
                    logger.WriteErrorLog(ex);
                return new Dictionary<string, double>();
            }
        }

        public virtual Dictionary<string, int> GetTopicCount(string[] key)
        {
            Dictionary<string, int> dic = new Dictionary<string, int>();

            try
            {
                using (ConnectionMultiplexer mux = ConnectionMultiplexer.Connect(redisconfig))
                {
                    this._db = mux.GetDatabase(2);
                    if (key != null && key.Length > 0)
                    {
                        foreach (var item in key)
                        {
                            RedisValue obj = _db.StringGet(item);
                            if (obj.HasValue)
                            {
                                int out_num;
                                if (int.TryParse(obj.ToString(), out out_num))
                                {
                                    dic[item] = out_num;
                                }
                                else
                                {
                                    dic[item] = -1;
                                }
                            }
                            else
                            {
                                dic[item] = -1;
                            }
                        }
                    }
                }
                return dic;
            }
            catch (Exception ex)
            {
                if (logger != null)
                    logger.WriteErrorLog(ex);

                return new Dictionary<string, int>();
            }
        }

        /// <summary>
        /// Gets a value indicating whether the value associated with the specified key is cached
        /// </summary>
        /// <param name="key">key</param>
        /// <returns>Result</returns>
        public virtual bool IsSet(string key)
        {
            //little performance workaround here:
            //we use "PerRequestCacheManager" to cache a loaded object in memory for the current HTTP request.
            //this way we won't connect to Redis server 500 times per HTTP request (e.g. each time to load a locale or setting)
            //if (_perRequestCacheManager.IsSet(key))
            //    return true;

            try
            {
                using (ConnectionMultiplexer mux = ConnectionMultiplexer.Connect(redisconfig))
                {
                    this._db = mux.GetDatabase();
                    return _db.KeyExists(key);
                }
            }
            catch (Exception ex)
            {
                if (logger != null)
                    logger.WriteErrorLog(ex);
                return false;
            }
        }

        /// <summary>
        /// Removes the value with the specified key from the cache
        /// </summary>
        /// <param name="key">/key</param>
        public virtual void Remove(string key)
        {
            try
            {
                using (ConnectionMultiplexer mux = ConnectionMultiplexer.Connect(redisconfig))
                {
                    this._db = mux.GetDatabase();
                    _db.KeyDelete(key);
                }
            }
            catch (Exception ex)
            {
                if (logger != null)
                    logger.WriteErrorLog(ex);
            }
        }

        /// <summary>
        /// Removes items by pattern
        /// </summary>
        /// <param name="pattern">pattern</param>
        public virtual void RemoveByPattern(string pattern)
        {
            try
            {
                using (ConnectionMultiplexer mux = ConnectionMultiplexer.Connect(redisconfig))
                {
                    this._db = mux.GetDatabase();
                    foreach (var ep in mux.GetEndPoints())
                    {
                        var server = mux.GetServer(ep);
                        var keys = server.Keys(pattern: "*" + pattern + "*");
                        foreach (var key in keys)
                            _db.KeyDelete(key);
                    }
                }
            }
            catch (Exception ex)
            {
                if (logger != null)
                    logger.WriteErrorLog(ex);
            }
        }

        /// <summary>
        /// Clear all cache data
        /// </summary>
        public virtual void Clear()
        {
            try
            {
                using (ConnectionMultiplexer mux = ConnectionMultiplexer.Connect(redisconfig))
                {
                    this._db = mux.GetDatabase();
                    foreach (var ep in mux.GetEndPoints())
                    {
                        var server = mux.GetServer(ep);
                        //we can use the code belwo (commented)
                        //but it requires administration permission - ",allowAdmin=true"
                        //server.FlushDatabase();

                        //that's why we simply interate through all elements now
                        var keys = server.Keys();
                        foreach (var key in keys)
                            _db.KeyDelete(key);
                    }
                }
            }
            catch (Exception ex)
            {
                if (logger != null)
                    logger.WriteErrorLog(ex);
            }
        }

        public virtual void SetHash()
        {
            try
            {
                using (ConnectionMultiplexer mux = ConnectionMultiplexer.Connect(redisconfig))
                {
                    this._db = mux.GetDatabase();
                    foreach (var ep in mux.GetEndPoints())
                    {
                        var server = mux.GetServer(ep);
                        //we can use the code belwo (commented)
                        //but it requires administration permission - ",allowAdmin=true"
                        //server.FlushDatabase();

                        //that's why we simply interate through all elements now
                        var keys = server.Keys();
                        foreach (var key in keys)
                            _db.KeyDelete(key);
                    }
                }
            }
            catch (Exception ex)
            {
                if (logger != null)
                    logger.WriteErrorLog(ex);
            }
        }

        /// <summary>
        /// Dispose
        /// </summary>
        public virtual void Dispose()
        {
            //if (_muxer != null)
            //    _muxer.Dispose();
        }

        #endregion Methods
    }
}