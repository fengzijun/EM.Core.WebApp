using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EM.Caching
{
    public interface IRedis : IDisposable
    {
        /// <summary>
        /// Gets or sets the value associated with the specified key.
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <param name="key">The key of the value to get.</param>
        /// <returns>The value associated with the specified key.</returns>
        T Get<T>(string key);

        /// <summary>
        /// Adds the specified key and object to the cache.
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="data">Data</param>
        /// <param name="cacheTime">Cache time</param>
        void Set(string key, object data, int cacheTime);

        /// <summary>
        ///
        /// </summary>
        /// <param name="keys"></param>
        /// <param name="objs"></param>
        void BatchSetAuthorInfo(List<string> keys, List<string> objs);

        /// <summary>
        ///
        /// </summary>
        /// <param name="key"></param>
        /// <param name="obj"></param>
        void SetAuthorInfo(string key, string obj);

        /// <summary>
        ///
        /// </summary>
        /// <param name="keys"></param>
        /// <param name="objs"></param>
        void BatchSetArticleInfo(List<string> keys, List<string> objs);

        /// <summary>
        /// Gets a value indicating whether the value associated with the specified key is cached
        /// </summary>
        /// <param name="key">key</param>
        /// <returns>Result</returns>
        bool IsSet(string key);

        /// <summary>
        /// Removes the value with the specified key from the cache
        /// </summary>
        /// <param name="key">/key</param>
        void Remove(string key);

        /// <summary>
        /// Removes items by pattern
        /// </summary>
        /// <param name="pattern">pattern</param>
        void RemoveByPattern(string pattern);

        /// <summary>
        /// Clear all cache data
        /// </summary>
        void Clear();

        /// <summary>
        ///
        /// </summary>
        /// <param name="type"></param>
        /// <param name="id"></param>
        /// <param name="accouttype"></param>
        void Incr(string type, string id, string accouttype);

        /// <summary>
        /// 获取计数
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        string GetCount(string key);

        /// <summary>
        /// 批量获取计数
        /// </summary>
        /// <param name="type">Type是0财富号,1文章id,2话题id</param>
        /// <param name="pageindex"></param>
        /// <param name="pagesize"></param>
        /// <param name="pagesize">0 asc 1 desc</param>
        /// <returns></returns>
        Dictionary<string, double> GetCountPageList(string type, int pageindex, int pagesize, int order);

        /// <summary>
        ///
        /// </summary>
        /// <param name="ids"></param>
        /// <param name="type"></param>
        /// <param name="pageindex"></param>
        /// <param name="pagesize"></param>
        /// <param name="order">0 asc 1 desc</param>
        /// <returns></returns>
        Dictionary<string, double> GetCountPageList(string[] ids, string type, int pageindex, int pagesize, int order);

        /// <summary>
        /// 获取话题文章数
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        Dictionary<string, int> GetTopicCount(string[] key);

        /// <summary>
        /// 清理REDIS 数据
        /// </summary>
        /// <param name="type">Type是0财富号,1文章id,2话题id</param>
        /// <param name="isweek">是否一周 TRUE 一周内数据 FALSE 72小时数据</param>
        void ClearTimeouotData(string type, bool isweek);

        /// <summary>
        /// 恢复数据
        /// </summary>
        /// <param name="type"></param>
        /// <param name="id"></param>
        void ReSotreListData(string type, string id);

        /// <summary>
        /// 清除列表数据
        /// </summary>
        /// <param name="type"></param>
        /// <param name="id"></param>
        void ClearListData(string type, string id);

        /// <summary>
        /// 获取点击数
        /// </summary>
        /// <param name="type"></param>
        /// <param name="isweek"></param>
        /// <returns></returns>
        Dictionary<string, long> GetClickNum(string type, bool isweek);

        /// <summary>
        /// 获取用户点击数排序
        /// </summary>
        /// <param name="accounttype"></param>
        /// <param name="pageindex"></param>
        /// <param name="pagesize"></param>
        /// <returns></returns>
        Dictionary<string, double> GetAccountPageList(string accounttype, int pageindex, int pagesize);
    }
}