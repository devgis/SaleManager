using DEVGIS.Common;
using DEVGIS.SaleManager.Interface;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace DEVGIS.Common
{
    /// <summary>
    /// 公共类
    /// </summary>
    public class PublicClass
    {
        /// <summary>
        /// 数据库类型
        /// </summary>
        public static string DBType
        {
            get;
            set;
        }

        /// <summary>
        /// 数据库帮助类
        /// </summary>
        public static IDBHelper DBHelper
        {
            get;
            set;
        }

        public static string GetRowJson(DataRow dataRow)
        {
            StringBuilder strBuilder = new StringBuilder();
            strBuilder.Append("{");
            bool bFirst=true;
            foreach (DataColumn column in dataRow.Table.Columns)
            {
                if(bFirst)
                {
                    strBuilder.Append(string.Format("\"{0}\":\"{1}\"", column.ColumnName, dataRow[column.ColumnName].ToString()));
                    bFirst = false;
                }
                else
                {
                    strBuilder.Append(string.Format(",\"{0}\":\"{1}\"", column.ColumnName, dataRow[column.ColumnName].ToString()));
                }
            }
            strBuilder.Append("}");
            return strBuilder.ToString();
        }

        public static string GetDataTableJson(DataTable dataTable)
        {
            #region stringBuilder方式
            StringBuilder strBuilder = new StringBuilder();
            strBuilder.Append("[");
            bool bFirst = true;
            foreach (DataRow row in dataTable.Rows)
            {
                if (bFirst)
                {
                    strBuilder.Append(GetRowJson(row));
                    bFirst = false;
                }
                else
                {
                    strBuilder.Append("," + GetRowJson(row));
                }
            }
            strBuilder.Append("]");
            return strBuilder.ToString();
            #endregion

            #region string方式
            //string sTemp = "[";
            //bool bFirst = true;
            //foreach (DataRow row in dataTable.Rows)
            //{
            //    if (bFirst)
            //    {
            //        sTemp += GetRowJson(row);
            //        bFirst = false;
            //    }
            //    else
            //    {
            //        sTemp +=(","+ GetRowJson(row));
            //    }
            //}
            //sTemp += "]";
            //return sTemp;
            #endregion
        }
    }
}
