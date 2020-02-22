using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace DEVGIS.SaleManager.Interface
{
    public interface IDBHelper
    {
        #region 公有方法
        /// <summary>
        /// 从型号基础库获取数据
        /// </summary>
        /// <param name="SQL">查询的SQL语句</param>
        /// <returns>查询的结果</returns>
        DataTable GetDataTable(String SQL);

        /// <summary>
        /// 执行单个SQL
        /// </summary>
        /// <param name="Sql">需要执行的SQL</param>
        /// <returns>执行结果</returns>
        bool ExecuteSql(String Sql);

        /// <summary>
        /// 执行带参数的单个SQL
        /// </summary>
        /// <param name="Sql">需要执行的SQL</param>
        /// <param name="Params">参数列表</param>
        /// <returns>是否执行成功</returns>
        bool ExecuteSql(String Sql,List<KeyValuePair<string,object>> Params);

        /// <summary>
        /// 执行多条SQL语句，实现数据库事务。
        /// </summary>
        /// <param name="ListSql">SQL语句集合</param>
        /// <returns>执行结果</returns>
        bool ExecuteSqlTran(List<String> ListSql);

        /// <summary>
        /// 执行多条带参数SQL语句，实现数据库事务。
        /// </summary>
        /// <param name="ListSql">Sql语句列表</param>
        /// <param name="ListParams">参数列表</param>
        /// <returns>执行结果</returns>
        bool ExecuteSqlTran(List<String> ListSql,List<List<KeyValuePair<string, object>>> ListParams);

        /// <summary>
        /// 獲取數據庫實例名稱
        /// </summary>
        /// <returns>返回數據庫實例名稱</returns>
        string GetDbName();



        #endregion
    }
}
