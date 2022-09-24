using System.Linq.Expressions;

namespace ProxyPoolAPI.Repository
{
    public interface IRepository<TEntity> where TEntity : class
    {
        #region 查询
        /// <summary>
        /// 获取用于从整个表中检索实体的IQueryable
        /// </summary>
        /// <returns>可用于从数据库中选择实体</returns>
        IQueryable<TEntity> GetAll();
        /// <summary>
        /// 用于获取所有实体
        /// </summary>
        /// <returns>所有实体列表</returns>
        List<TEntity> GetAllList();
        /// <summary>
        /// 用于获取所有实体的异步实现
        /// </summary>
        /// <returns>所有实体列表</returns>
        Task<List<TEntity>> GelAllListAsync();
        /// <summary>
        /// 用于获取传入本方法的所有实体
        /// </summary>
        /// <param name="predicate">筛选实体的条件</param>
        /// <returns>所有实体列表</returns>
        List<TEntity> GetAllList(Expression<Func<TEntity, bool>> predicate);
        /// <summary>
        /// 用于获取传入本方法的所有实体
        /// </summary>
        /// <param name="predicate">筛选实体的条件</param>
        /// <returns>所有实体列表</returns>
        Task<List<TEntity>> GetAllListAsync(Expression<Func<TEntity, bool>> predicate);
        /// <summary>
        /// 通过传入的筛选条件来获取实体信息
        /// 如果查询不到返回值，则会引发异常
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        TEntity Single(Expression<Func<TEntity, bool>> predicate);
        /// <summary>
        /// 通过传入的筛选条件来获取实体信息
        /// 如果查询不到返回值，则会引发异常
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        Task<TEntity> SingleAsync(Expression<Func<TEntity, bool>> predicate);
        /// <summary>
        /// 通过传入的筛选条件查询实体信息，如果没有找到，则返回null
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        TEntity FirstOrDefault(Expression<Func<TEntity, bool>> predicate);
        /// <summary>
        /// 通过传入的筛选条件查询实体信息，如果没有找到，则返回null
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        Task<TEntity> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> predicate);
        #endregion

        #region 添加
        /// <summary>
        /// 添加一个新实体
        /// </summary>
        /// <param name="entity">被添加的实体</param>
        /// <returns></returns>
        TEntity Insert(TEntity entity);
        /// <summary>
        /// 添加一个新实体
        /// </summary>
        /// <param name="entity">被添加的实体</param>
        /// <returns></returns>
        Task<TEntity> InsertAsync(TEntity entity);

        #endregion

        #region 修改
        /// <summary>
        /// 更新现有实体
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        TEntity Update(TEntity entity);
        /// <summary>
        /// 更新现有实体
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        Task<TEntity> UpdateAsync(TEntity entity);

        #endregion

        #region 删除
        /// <summary>
        /// 删除一个实体
        /// </summary>
        /// <param name="entity">无返回值</param>
        void Delete(TEntity entity);
        /// <summary>
        /// 删除一个实体
        /// </summary>
        /// <param name="entity">无返回值</param>
        /// <returns></returns>
        Task DeleteAsync(TEntity entity);
        /// <summary>
        /// 传入的条件可删除多个实体
        /// 注意：所有符合给定条件的实体都将被检索和删除
        /// 如果条件比较多，则待删除的实体也比较多，则可能会导致主要的性能问题
        /// </summary>
        /// <param name="predicate"></param>
        void Delete(Expression<Func<TEntity, bool>> predicate);
        /// <summary>
        /// 传入的条件可删除多个实体
        /// 注意：所有符合给定条件的实体都将被检索和删除
        /// 如果条件比较多，则待删除的实体也比较多，则可能会导致主要的性能问题
        /// </summary>
        /// <param name="predicate"></param>
        Task DeleteAsync(Expression<Func<TEntity, bool>> predicate);
        #endregion

        #region 总和计算
        /// <summary>
        /// 获取此仓储中所有实体的总和
        /// </summary>
        /// <returns>实体的总数</returns>
        int Count();
        /// <summary>
        /// 获取此仓储中所有实体的总和
        /// </summary>
        /// <returns>实体的总数</returns>
        Task<int> CountAsync();
        /// <summary>
        /// 支持条件筛选计算仓储中的实体总和
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns>实体的总数</returns>
        int Count(Expression<Func<TEntity, bool>> predicate);
        /// <summary>
        /// 支持条件筛选计算仓储中的实体总和
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns>实体的总数</returns>
        Task<int> CountAsync(Expression<Func<TEntity, bool>> predicate);
        /// <summary>
        /// 获取此存储库中所有的实体的总和，如果返回值大于了Int.MaxValue值，则推荐该方法
        /// </summary>
        /// <returns>实体的总和</returns>
        long LongCount();
        /// <summary>
        /// 获取此存储库中所有的实体的总和，如果返回值大于了Int.MaxValue值，则推荐该方法
        /// </summary>
        /// <returns>实体的总和</returns>
        Task<long> LongCountAsync();
        /// <summary>
        /// 获取此存储库中所有的实体的总和，如果返回值大于了Int.MaxValue值，则推荐该方法
        /// </summary>
        /// <returns>实体的总和</returns>
        long LongCount(Expression<Func<TEntity, bool>> predicate);
        /// <summary>
        /// 获取此存储库中所有的实体的总和，如果返回值大于了Int.MaxValue值，则推荐该方法
        /// </summary>
        /// <returns>实体的总和</returns>
        Task<long> LongCountAsync(Expression<Func<TEntity, bool>> predicate);
        #endregion
    }
}
