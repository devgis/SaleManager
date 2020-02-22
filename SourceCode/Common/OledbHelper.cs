using DEVGIS.SaleManager.Interface;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Linq;
using System.Text;

namespace DEVGIS.Common
{
    /// <summary>
    /// MSSQLServer帮助类
    /// </summary>
    public class OledbHelper : IDBHelper
    {
        private string strCon = string.Empty;

        #region 构造方法
        public OledbHelper()
        {
            #region 初始化连接信息
            strCon = System.Configuration.ConfigurationManager.AppSettings["DBCONSTRING"].ToString();
            #endregion
        }
        #endregion

        #region 单例
        private static OledbHelper _instance = null;

        /// <summary>
        /// PGHelper的实例
        /// </summary>
        public static OledbHelper Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new OledbHelper();
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
            using (OleDbConnection conn = new OleDbConnection(strCon))
            {
                OleDbCommand cmd = new OleDbCommand(SQL, conn);
                OleDbDataAdapter da = new OleDbDataAdapter(cmd);
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
            using (OleDbConnection conn = new OleDbConnection(strCon))
            {
                conn.Open();
                try
                {
                    OleDbCommand oc = new OleDbCommand(Sql, conn);
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
            using (OleDbConnection conn = new OleDbConnection(strCon))
            {
                conn.Open();
                OleDbTransaction sqlTran = conn.BeginTransaction();
                try
                {
                    //OracleTransaction tx=conn.BeginTransaction();	
                    foreach (String sql in ListSql)
                    {
                        try
                        {
                            OleDbCommand oc = new OleDbCommand(sql, conn);
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
            return new OleDbConnection(strCon).Database;
        }

        public bool ExecuteSql(string Sql, List<KeyValuePair<string, object>> Params)
        {
            using (OleDbConnection conn = new OleDbConnection(strCon))
            {
                conn.Open();
                try
                {
                    OleDbCommand oc = new OleDbCommand(Sql, conn);
                    if (Params != null && Params.Count > 0)
                    {
                        foreach (var item in Params)
                        {
                            oc.Parameters.Add(new OleDbParameter(item.Key, item.Value));
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
            using (OleDbConnection conn = new OleDbConnection(strCon))
            {
                conn.Open();
                OleDbTransaction sqlTran = conn.BeginTransaction();
                try
                {
                    //OracleTransaction tx=conn.BeginTransaction();	
                    for (int i = 0; i < ListSql.Count; i++)
                    {
                        try
                        {
                            OleDbCommand oc = new OleDbCommand(ListSql[i], conn);
                            if (ListParams != null && ListParams[i].Count > 0)
                            {
                                foreach (var item in ListParams[i])
                                {
                                    oc.Parameters.Add(new OleDbParameter(item.Key, item.Value));
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
