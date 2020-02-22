using DEVGIS.SaleManager.Interface;
using Oracle.ManagedDataAccess.Client;
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
    public class OracleHelper : IDBHelper
    {
        private string strCon = string.Empty;

        #region 构造方法
        public OracleHelper()
        {
            #region 初始化连接信息
            string strCon = System.Configuration.ConfigurationManager.AppSettings["DBCONSTRING"].ToString();
            #endregion
        }
        #endregion

        #region 单例
        private static OracleHelper _instance = null;

        /// <summary>
        /// PGHelper的实例
        /// </summary>
        public static OracleHelper Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new OracleHelper();
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
            using (OracleConnection conn = new OracleConnection(strCon))
            {
                OracleCommand cmd = new OracleCommand(SQL, conn);
                OracleDataAdapter da = new OracleDataAdapter(cmd);
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
            using (OracleConnection conn = new OracleConnection(strCon))
            {
                conn.Open();
                try
                {
                    OracleCommand oc = new OracleCommand(Sql, conn);
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
            using (OracleConnection conn = new OracleConnection(strCon))
            {
                conn.Open();
                OracleTransaction sqlTran = conn.BeginTransaction();
                try
                {
                    //OracleTransaction tx=conn.BeginTransaction();	
                    foreach (String sql in ListSql)
                    {
                        try
                        {
                            OracleCommand oc = new OracleCommand(sql, conn);
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
            using (OracleConnection conn = new OracleConnection(strCon))
            {
                return conn.Database;
            }
        }

        public bool ExecuteSql(string Sql, List<KeyValuePair<string, object>> Params)
        {
            using (OracleConnection conn = new OracleConnection(strCon))
            {
                conn.Open();
                try
                {
                    OracleCommand oc = new OracleCommand(Sql, conn);
                    if (Params != null && Params.Count > 0)
                    {
                        foreach (var item in Params)
                        {
                            oc.Parameters.Add(new OracleParameter(item.Key, item.Value));
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
            using (OracleConnection conn = new OracleConnection(strCon))
            {
                conn.Open();
                OracleTransaction sqlTran = conn.BeginTransaction();
                try
                {
                    //OracleTransaction tx=conn.BeginTransaction();	
                    for (int i = 0; i < ListSql.Count; i++)
                    {
                        try
                        {
                            OracleCommand oc = new OracleCommand(ListSql[i], conn);
                            if (ListParams != null && ListParams[i].Count > 0)
                            {
                                foreach (var item in ListParams[i])
                                {
                                    oc.Parameters.Add(new OracleParameter(item.Key, item.Value));
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
