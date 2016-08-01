using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Collections;
using System.Data;

namespace pixChange
{
	class MegerTable
	{
        #region 私有字段
        private string LastErrInfo = String.Empty;  //最后一次出错信息
        #endregion

        public string GetLastErrInfo()
        {
            return LastErrInfo;
        }

        /// <summary>
        /// 将主从表进行左连接操作
        /// </summary>
        /// <param name="mainTable">主表</param>
        /// <param name="subTable">从表</param>
        /// <param name="keyFields">关联字段</param>
        /// <param name="megerFields">子表要合并的字段</param>
        /// <returns>合并后的表</returns>
        public DataTable LeftJoinTabel(DataTable mainTable, DataTable subTable, string[] keyFields, string[] megerFields)
        {
            if ((keyFields.Length == 0) || (megerFields.Length == 0))
            {
                LastErrInfo = "必须指定关联字段及要合并的字段！";
                return null;
            }

            foreach (string keyField in keyFields)
            {
                if (keyField.Length == 0)
                {
                    LastErrInfo = "关键字段的列名不允许为空！";
                    return null;
                }
                if (!mainTable.Columns.Contains(keyField))
                {
                    LastErrInfo = "主表并没有关键字段[" + keyField + "]！";
                    return null;
                }
                if (!subTable.Columns.Contains(keyField))
                {
                    LastErrInfo = "子表并没有关键字段[" + keyField + "]！";
                    return null;
                }
            }

            foreach (string megerField in megerFields)
            {
                if (megerField.Length == 0)
                {
                    LastErrInfo = "要合并字段的列名不允许为空！";
                    return null;
                }
                if (!subTable.Columns.Contains(megerField))
                {
                    LastErrInfo = "子表并没有要合并的字段[" + megerField + "]！";
                    return null;
                }
            }

            if (mainTable.Rows.Count * subTable.Rows.Count < 5000)
            {
                return NestJoin(mainTable, subTable, keyFields, megerFields);
            }
            else
            {
                if ((megerFields.Length < 5) && (subTable.Rows.Count < 10000))
                {
                    return HashJoin(mainTable, subTable, keyFields, megerFields);
                }
                else
                {
                    return IndexJoin(mainTable, subTable, keyFields, megerFields);
                }
            }
        }

        /// <summary>
        /// 嵌套循环方式。当主从表都足够小，循环总次数不超过5000时，可免去建立哈希表的开销。
        /// </summary>
        public DataTable NestJoin(DataTable mainTable, DataTable subTable, string[] keyFields, string[] megerFields)
        {
            int mainTableSrcCols = mainTable.Columns.Count;

            //添加主表列
            DataColumn newColumn;
            foreach (string colName in megerFields)
            {
                newColumn = new DataColumn(colName);
                if (mainTable.Columns.Contains(colName))
                {
                    newColumn.ColumnName = colName + "$";
                }
                mainTable.Columns.Add(newColumn);
            }

            //合并表
            bool same;
            foreach (DataRow mainRow in mainTable.Rows)
            {
                foreach (DataRow subRow in subTable.Rows)
                {
                    //比较当前行的所有关键列值是否一致
                    same = true;
                    foreach (string keyField in keyFields)
                    {
                        if (mainRow[keyField].ToString() != subRow[keyField].ToString())
                        {
                            same = false;
                            break;
                        }
                    }

                    //若一致才合并
                    if (same)
                    {
                        int newColIndex = mainTableSrcCols;
                        foreach (string megerField in megerFields)
                        {
                            mainRow[newColIndex++] = subRow[megerField].ToString();
                        }
                    }
                }
            }

            return mainTable;
        }

        /// <summary>
        /// 哈希查询方式。当从表比较小，列数少于5且记录数少于1万时，可直接把所有行存入哈希表，就无需再查询从表。
        /// </summary>
        public DataTable HashJoin(DataTable mainTable, DataTable subTable, string[] keyFields, string[] megerFields)
        {
            const string SEP = "#%";    //关键字段的列分隔符
            int mainTableSrcCols = mainTable.Columns.Count;
            int subTableSrcCols = subTable.Columns.Count;

            //将子表放入哈希表
            string key;
            string[] value;
            Hashtable hashTable = new Hashtable();
            foreach (DataRow row in subTable.Rows)
            {
                key = row[keyFields[0]].ToString();
                for (int i = 1; i < keyFields.Length; i++)
                {
                    key += SEP + row[keyFields[i]].ToString();
                }

                value = new string[megerFields.Length];
                for (int i = 0; i < megerFields.Length; i++)
                {
                    value[i] = row[megerFields[i]].ToString();
                }

                //哈希表保存主键和实际行内容
                hashTable.Add(key, value);
            }

            //添加主表列
            DataColumn newColumn;
            foreach (string colName in megerFields)
            {
                newColumn = new DataColumn(colName);
                if (mainTable.Columns.Contains(colName))
                {
                    newColumn.ColumnName = colName + "$";
                }
                mainTable.Columns.Add(newColumn);
            }

            //合并表
            foreach (DataRow row in mainTable.Rows)
            {
                key = row[keyFields[0]].ToString();
                for (int i = 1; i < keyFields.Length; i++)
                {
                    key += SEP + row[keyFields[i]].ToString();
                }

                if (hashTable.ContainsKey(key))
                {
                    value = hashTable[key] as string[];
                    for (int i = mainTableSrcCols; i < mainTable.Columns.Count; i++)
                    {
                        row[i] = value[i - mainTableSrcCols];
                    }
                }
            }

            return mainTable;
        }

        /// <summary>
        /// 哈希索引方式。当从表比较大时，只在哈希表存储索引，找根据索引到从表查询记录，可避免建立过大的哈希表。
        /// </summary>
        public DataTable IndexJoin(DataTable mainTable, DataTable subTable, string[] keyFields, string[] megerFields)
        {
            const string SEP = "#%";    //关键字段的列分隔符
            int mainTableSrcCols = mainTable.Columns.Count;
            int subTableSrcCols = subTable.Columns.Count;

            //将子表索引放入哈希表
            int rowIndex = 0;
            string key;
            Hashtable hashTable = new Hashtable();
            foreach (DataRow row in subTable.Rows)
            {
                key = row[keyFields[0]].ToString();
                for (int i = 1; i < keyFields.Length; i++)
                {
                    key += SEP + row[keyFields[i]].ToString();
                }

                //哈希表保存主键和行索引
                hashTable.Add(key, rowIndex);
                rowIndex++;
            }

            //添加主表列
            DataColumn newColumn;
            foreach (string colName in megerFields)
            {
                newColumn = new DataColumn(colName);
                if (mainTable.Columns.Contains(colName))
                {
                    newColumn.ColumnName = colName + "$";
                }
                mainTable.Columns.Add(newColumn);
            }

            //合并表
            foreach (DataRow row in mainTable.Rows)
            {
                key = row[keyFields[0]].ToString();
                for (int i = 1; i < keyFields.Length; i++)
                {
                    key += SEP + row[keyFields[i]].ToString();
                }

                if (hashTable.ContainsKey(key))
                {
                    rowIndex = (int)hashTable[key];
                    for (int i = mainTableSrcCols; i < mainTable.Columns.Count; i++)
                    {
                        row[i] = subTable.Rows[rowIndex][megerFields[i - mainTableSrcCols].ToString()];
                    }
                }
            }

            return mainTable;
        }
	}
}
