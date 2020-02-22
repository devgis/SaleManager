using DEVGIS.SaleManager.Interface;
using MySql.Data.MySqlClient;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace DEVGIS.Common
{
    /// <summary>
    /// MSSQLServer帮助类
    /// </summary>
    public class PostgresqlHelper : IDBHelper
    {
        private string strCon = string.Empty;

        #region 构造方法
        public PostgresqlHelper()
        {
            #region 初始化连接信息
            strCon= System.Configuration.ConfigurationManager.AppSettings["DBCONSTRING"].ToString();
            #endregion
        }
        #endregion

        #region 单例
        private static PostgresqlHelper _instance = null;

        /// <summary>
        /// PGHelper的实例
        /// </summary>
        public static PostgresqlHelper Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new PostgresqlHelper();
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
            using (NpgsqlConnection conn = new NpgsqlConnection(strCon))
            {
                NpgsqlCommand cmd = new NpgsqlCommand(SQL, conn);
                NpgsqlDataAdapter da = new NpgsqlDataAdapter(cmd);
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
            using (NpgsqlConnection conn = new NpgsqlConnection(strCon))
            {
                conn.Open();
                try
                {
                    NpgsqlCommand oc = new NpgsqlCommand(Sql, conn);
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
            using (NpgsqlConnection conn = new NpgsqlConnection(strCon))
            {
                conn.Open();
                NpgsqlTransaction sqlTran = conn.BeginTransaction();
                try
                {
                    //OracleTransaction tx=conn.BeginTransaction();	
                    foreach (String sql in ListSql)
                    {
                        try
                        {
                            NpgsqlCommand oc = new NpgsqlCommand(sql, conn);
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
            using (NpgsqlConnection conn = new NpgsqlConnection(strCon))
            {
                return conn.Database;
            }
        }

        public bool ExecuteSql(string Sql, List<KeyValuePair<string, object>> Params)
        {
            using (NpgsqlConnection conn = new NpgsqlConnection(strCon))
            {
                conn.Open();
                try
                {
                    NpgsqlCommand oc = new NpgsqlCommand(Sql, conn);
                    if (Params != null && Params.Count > 0)
                    {
                        foreach (var item in Params)
                        {
                            oc.Parameters.Add(new NpgsqlParameter(item.Key, item.Value));
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
            using (NpgsqlConnection conn = new NpgsqlConnection(strCon))
            {
                conn.Open();
                NpgsqlTransaction sqlTran = conn.BeginTransaction();
                try
                {
                    //OracleTransaction tx=conn.BeginTransaction();	
                    for (int i = 0; i < ListSql.Count; i++)
                    {
                        try
                        {
                            NpgsqlCommand oc = new NpgsqlCommand(ListSql[i], conn);
                            if (ListParams != null && ListParams[i].Count > 0)
                            {
                                foreach (var item in ListParams[i])
                                {
                                    oc.Parameters.Add(new NpgsqlParameter(item.Key, item.Value));
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
