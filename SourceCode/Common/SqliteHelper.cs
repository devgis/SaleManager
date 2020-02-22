using DEVGIS.SaleManager.Interface;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Linq;
using System.Text;

namespace DEVGIS.Common
{
    /// <summary>
    /// MSSQLServer帮助类
    /// </summary>
    public class SqliteHelper : IDBHelper
    {
        private string strCon = string.Empty;

        #region 构造方法
        public SqliteHelper()
        {
            #region 初始化连接信息
            strCon = System.Configuration.ConfigurationManager.AppSettings["DBCONSTRING"].ToString();
            #endregion
        }
        #endregion

        #region 单例
        private static SqliteHelper _instance = null;

        /// <summary>
        /// PGHelper的实例
        /// </summary>
        public static SqliteHelper Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new SqliteHelper();
                }
                return _instance;
            }
        }
        #endregion

        #region 公有方法
        /// <summary>
        /// 从型号基础库获取数据
        /// </summary>
        /// <param name="SQL">查询的SQL语句</param>
        /// <returns>查询的结果</returns>
        public DataTable GetDataTable(String SQL)
        {
            using (SQLiteConnection conn = new SQLiteConnection(strCon))
            {
                SQLiteCommand cmd = new SQLiteCommand(SQL, conn);
                SQLiteDataAdapter da = new SQLiteDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);
                return dt;
            }
        }

        /// <summary>
        /// 执行单个SQL
        /// </summary>
        /// <param name="Sql">需要执行的SQL</param>
        /// <returns>执行结果</returns>
        public bool ExecuteSql(String Sql)
        {
            using (SQLiteConnection conn = new SQLiteConnection(strCon))
            {
                conn.Open();
                try
                {
                    SQLiteCommand oc = new SQLiteCommand(Sql, conn);
                    if (oc.ExecuteNonQuery() > 0)
                    {
                        return true;
                    }
                }
                catch
                {
                }
                finally
                {
                    conn.Close();
                }
                return false;
            }
        }

        /// <summary>
        /// 执行多条SQL语句，实现数据库事务。
        /// </summary>
        /// <param name="ListSql">SQL语句集合</param>
        /// <returns>执行结果</returns>
        public bool ExecuteSqlTran(List<String> ListSql)
        {
            using (SQLiteConnection conn = new SQLiteConnection(strCon))
            {
                conn.Open();
                SQLiteTransaction sqlTran = conn.BeginTransaction();
                try
                {
                    //OracleTransaction tx=conn.BeginTransaction();	
                    foreach (String sql in ListSql)
                    {
                        try
                        {
                            SQLiteCommand oc = new SQLiteCommand(sql, conn);
                            oc.Transaction = sqlTran;
                            oc.ExecuteNonQuery();
                        }
                        catch (System.Data.SqlClient.SqlException E)
                        {
                            sqlTran.Rollback();
                            throw new Exception(E.Message);
                        }

                    }
                    sqlTran.Commit();
                    return true;
                }
                catch
                {
                    sqlTran.Rollback();
                }
                finally
                {
                    conn.Close();
                }
                return false;
            }
        }

        public string GetDbName()
        {
            using (SQLiteConnection conn = new SQLiteConnection(strCon))
            {
                return conn.Database;
            }
        }

        public bool ExecuteSql(string Sql, List<KeyValuePair<string, object>> Params)
        {
            using (SQLiteConnection conn = new SQLiteConnection(strCon))
            {
                conn.Open();
                try
                {
                    SQLiteCommand oc = new SQLiteCommand(Sql, conn);
                    if (Params != null && Params.Count > 0)
                    {
                        foreach (var item in Params)
                        {
                            oc.Parameters.Add(new SQLiteParameter(item.Key, item.Value));
                        }
                    }
                    if (oc.ExecuteNonQuery() > 0)
                    {
                        return true;
                    }
                }
                catch
                {
                }
                finally
                {
                    conn.Close();
                }
                return false;
            }
        }

        public bool ExecuteSqlTran(List<string> ListSql, List<List<KeyValuePair<string, object>>> ListParams)
        {
            if (
                ListSql == null
                || ListParams == null
                || ListSql.Count <= 0
                || ListSql.Count != ListParams.Count)
            {
                throw new Exception("参数不正确或者SQL为空");
            }
            using (SQLiteConnection conn = new SQLiteConnection(strCon))
            {
                conn.Open();
                SQLiteTransaction sqlTran = conn.BeginTransaction();
                try
                {
                    //OracleTransaction tx=conn.BeginTransaction();	
                    for (int i = 0; i < ListSql.Count; i++)
                    {
                        try
                        {
                            SQLiteCommand oc = new SQLiteCommand(ListSql[i], conn);
                            if (ListParams != null && ListParams[i].Count > 0)
                            {
                                foreach (var item in ListParams[i])
                                {
                                    oc.Parameters.Add(new SQLiteParameter(item.Key, item.Value));
                                }
                            }
                            oc.Transaction = sqlTran;
                            oc.ExecuteNonQuery();
                        }
                        catch (System.Data.SqlClient.SqlException E)
                        {
                            sqlTran.Rollback();
                            throw new Exception(E.Message);
                        }

                    }
                    sqlTran.Commit();
                    return true;
                }
                catch
                {
                    sqlTran.Rollback();
                }
                finally
                {
                    conn.Close();
                }
                return false;
            }
        }
        #endregion

    }
}
