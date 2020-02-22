using DEVGIS.SaleManager.Interface;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace DEVGIS.Common
{
    /// <summary>
    /// MSSQLServer帮助类
    /// </summary>
    public class MSSQLHelper : IDBHelper
    {
        private string strCon=string.Empty;

        #region 构造方法
        public MSSQLHelper()
        {
            #region 初始化连接信息
            strCon = System.Configuration.ConfigurationManager.AppSettings["DBCONSTRING"].ToString();
            #endregion
        }
        #endregion

        #region 单例
        private static MSSQLHelper _instance = null;

        /// <summary>
        /// PGHelper的实例
        /// </summary>
        public static MSSQLHelper Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new MSSQLHelper();
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
            using (SqlConnection conn = new SqlConnection(strCon))
            {
                SqlCommand cmd = new SqlCommand(SQL, conn);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
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
            using (SqlConnection conn = new SqlConnection(strCon))
            {
                conn.Open();
                try
                {
                    SqlCommand oc = new SqlCommand(Sql, conn);
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
            using (SqlConnection conn = new SqlConnection(strCon))
            { 
                conn.Open();
            SqlTransaction sqlTran = conn.BeginTransaction();
            try
            {
                //OracleTransaction tx=conn.BeginTransaction();	
                foreach (String sql in ListSql)
                {
                    try
                    {
                        SqlCommand oc = new SqlCommand(sql, conn);
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
            return new SqlConnection(strCon).Database;
        }

        public bool ExecuteSql(string Sql, List<KeyValuePair<string, object>> Params)
        {
            using (SqlConnection conn = new SqlConnection(strCon))
            {
                conn.Open();
                try
                {
                    SqlCommand oc = new SqlCommand(Sql, conn);
                    if (Params != null && Params.Count > 0)
                    {
                        foreach (var item in Params)
                        {
                            oc.Parameters.Add(new SqlParameter(item.Key, item.Value));
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
            if(
                ListSql==null
                || ListParams==null
                ||ListSql.Count<=0
                ||ListSql.Count!=ListParams.Count)
            {
                throw new Exception("参数不正确或者SQL为空");
            }
            using (SqlConnection conn = new SqlConnection(strCon))
            {
                conn.Open();
                SqlTransaction sqlTran = conn.BeginTransaction();
                try
                {
                    //OracleTransaction tx=conn.BeginTransaction();	
                    for (int i= 0;i < ListSql.Count;i++)
                    {
                        try
                        {
                            SqlCommand oc = new SqlCommand(ListSql[i], conn);
                            if (ListParams != null && ListParams[i].Count > 0)
                            {
                                foreach (var item in ListParams[i])
                                {
                                    oc.Parameters.Add(new SqlParameter(item.Key, item.Value));
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
