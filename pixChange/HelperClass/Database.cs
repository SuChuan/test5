using System;
using System.Collections.Generic;
using System.Text;
using System.Data.OleDb;
using System.Data;
using System.Windows.Forms;
using System.IO;

namespace WinInrush.Class
{
    class Database
    {
        #region 主数据库操作变量
        private static string StartPath = Application.StartupPath + "\\DBPoint.mdb";
        private static string ConnStr = "Provider=Microsoft.Jet.OLEDB.4.0; ";
        private string ConnectStr = ConnStr + "Data Source=" + StartPath;
        private OleDbDataAdapter DataAdapter;
        private OleDbConnection DataConnection;
        private DataSet DataSet;
        #endregion

        #region 获得数据库数据
		public DataSet GetDataFromDB(string sqlstr)
		{
			try 
            {
				DataConnection = new OleDbConnection();
				DataConnection.ConnectionString = ConnectStr;
				DataAdapter = new OleDbDataAdapter(sqlstr, DataConnection);
				DataSet = new DataSet();
				DataSet.Clear();
				DataAdapter.Fill(DataSet);
				DataConnection.Close();
			}
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "错误提示");
			}

			if (DataSet != null)
			{
				if (DataSet.Tables[0].Rows.Count > 0)
				{
					return DataSet;
				}
				else
				{
					return null;
				}
			}
			return null;
		}
		#endregion

		#region 更新数据库数据
		public bool UpdateDataBase(string sqlstr)
		{
			OleDbConnection sqlconn = new OleDbConnection(ConnectStr);
			try
            {
				OleDbCommand cmdTable = new OleDbCommand(sqlstr, sqlconn);
				cmdTable.CommandType = CommandType.Text;
				sqlconn.Open();
				cmdTable.ExecuteNonQuery();
				sqlconn.Close();
			}
			catch (Exception ex ) 
            {
				MessageBox.Show(ex.Message);
				return false;
			}
			return true;
		}
		#endregion

        /// <summary>
        /// 保存图片到表中
        /// </summary>
        /// <param name="strTblName"></param>表名称
        /// <param name="strid"></param>ID号字段名称
        /// <param name="strFileFieldName"></param>图片字段名称
        /// <param name="filePath"></param>图片保存路径
        /// <returns></returns>
        public bool SavePicToDatabase(string strTbleName, string stridName, string stridValue, string strFileFieldName, string filePath)
        {
            OleDbConnection sqlconn = new OleDbConnection(ConnectStr);

                FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read);
                BinaryReader br = new BinaryReader(fs);
                byte[] photo = br.ReadBytes((int)fs.Length);
                br.Close();
                fs.Close();

                //System.IO.FileStream fs = new System.IO.FileStream(RefSaveFilePath, IO.FileMode.Open, IO.FileAccess.Read);
                //byte[] imgData = new byte[fs.Length];
                //fs.Read(imgData, 0, fs.Length - 1);
                //fs.Close();

                string sqlstr = "SELECT * FROM " + strTbleName + " WHERE " + stridName + "='" + stridValue + "'";

                DataConnection = new OleDbConnection();
                DataConnection.ConnectionString = ConnectStr;

                OleDbDataAdapter tempAdapter = new OleDbDataAdapter(sqlstr, DataConnection);
                DataSet tempDataSet = new DataSet(); 
                tempAdapter.Fill(tempDataSet);
          
                OleDbCommandBuilder cb = new OleDbCommandBuilder(tempAdapter);
                tempDataSet = new DataSet();
                tempDataSet.Clear();
                tempAdapter.Fill(tempDataSet);

                string strId = "";
                string strguyangxian = "";
                string strtuyouqi = "";
                string strshixiaqu = "";
                string strbaiyunqu = "";
                string strdamaoqi = "";
                string strmakeyear = "";
                string strmakemonth = "";
                string strmakeday = "";
                string strmakehour = "";
                string strmakeminute = "";
                string strmaker = "";
                string strdiscribe = "";
                string strtransdiscribe = "";

                if (tempDataSet.Tables[0].Rows.Count == 1)
                {
                    strId = tempDataSet.Tables[0].Rows[0]["ID"].ToString();
                    strguyangxian = tempDataSet.Tables[0].Rows[0]["guyangxian"].ToString();
                    strtuyouqi = tempDataSet.Tables[0].Rows[0]["tuyouqi"].ToString();
                    strshixiaqu = tempDataSet.Tables[0].Rows[0]["shixiaqu"].ToString(); 
                    strbaiyunqu = tempDataSet.Tables[0].Rows[0]["baiyunqu"].ToString(); 
                    strdamaoqi = tempDataSet.Tables[0].Rows[0]["damaoqi"].ToString(); 
                   
                    strmakeyear = tempDataSet.Tables[0].Rows[0]["makeyear"].ToString(); 
                    strmakemonth = tempDataSet.Tables[0].Rows[0]["makemonth"].ToString(); 
                    strmakeday = tempDataSet.Tables[0].Rows[0]["makeday"].ToString();
                    strmakehour = tempDataSet.Tables[0].Rows[0]["makehour"].ToString();
                    strmakeminute = tempDataSet.Tables[0].Rows[0]["makeminute"].ToString(); 

                    strmaker = tempDataSet.Tables[0].Rows[0]["maker"].ToString(); 
                    strdiscribe = tempDataSet.Tables[0].Rows[0]["discribe"].ToString();
                    strtransdiscribe = tempDataSet.Tables[0].Rows[0]["transdiscribe"].ToString(); 
                    // MsgBox("gfd")
                }

                tempDataSet.Tables[0].Rows[0]["ID"] = strId;
                tempDataSet.Tables[0].Rows[0]["guyangxian"] = strguyangxian;
                tempDataSet.Tables[0].Rows[0]["tuyouqi"] = strtuyouqi;
                tempDataSet.Tables[0].Rows[0]["shixiaqu"] = strshixiaqu;
                tempDataSet.Tables[0].Rows[0]["baiyunqu"] = strbaiyunqu;
                tempDataSet.Tables[0].Rows[0]["damaoqi"] = strdamaoqi;

                tempDataSet.Tables[0].Rows[0]["makeyear"] = strmakeyear;
                tempDataSet.Tables[0].Rows[0]["makemonth"] = strmakemonth;
                tempDataSet.Tables[0].Rows[0]["makeday"] = strmakeday;
                tempDataSet.Tables[0].Rows[0]["makehour"] = strmakehour;
                tempDataSet.Tables[0].Rows[0]["makeminute"] = strmakeminute;

                tempDataSet.Tables[0].Rows[0]["maker"] = strmaker;
                tempDataSet.Tables[0].Rows[0]["discribe"] = strdiscribe;
                tempDataSet.Tables[0].Rows[0]["transdiscribe"] = strtransdiscribe;

                tempDataSet.Tables[0].Rows[0][strFileFieldName] = photo;
                try
                {
                    tempAdapter.Update(tempDataSet);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                    return false;
                }
                sqlconn.Close();
                return true;

        }

        #region 关掉word进程
        //关掉word进程
        public void killAllProcess() // 杀掉所有winword.exe进程 
        {
            System.Diagnostics.Process[] myPs;
            myPs = System.Diagnostics.Process.GetProcesses();

            foreach (System.Diagnostics.Process p in myPs)
            {
                if (p.Id != 0)
                {
                    string myS = "WINWORD.EXE" + p.ProcessName + " ID:" + p.Id.ToString();
                    try
                    {
                        if (p.Modules != null)
                        {
                            if (p.Modules.Count > 0)
                            {
                                System.Diagnostics.ProcessModule pm = p.Modules[0];
                                myS += "\n Modules[0].FileName:" + pm.FileName;
                                myS += "\n Modules[0].ModuleName:" + pm.ModuleName;
                                myS += "\n Modules[0].FileVersionInfo:\n" + pm.FileVersionInfo.ToString();

                                if (pm.ModuleName.ToLower() == "winword.exe")
                                {
                                    p.Kill();
                                }
                            }
                        }
                    }
                    catch (System.Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                }
            }
        }
        #endregion

        /// <summary>
        /// 返回得到下一个要保存的ID号
        /// </summary>
        /// <param name="strTblName"></param>表名称
        /// <param name="strKeyFieldName"></param>ID号字段名称
        /// <returns></returns>
        public int GetNewID(string strTblName, string strKeyFieldName)
        {
            int i;
            int newID = 1;
            int tmpID;
            int RecCount;
            string sqlstr;
            sqlstr = "SELECT * FROM " + strTblName + " ORDER BY " + strKeyFieldName;
            DataSet myDs = new DataSet();
            myDs = GetDataFromDB(sqlstr);
            bool BlnFind;
            BlnFind = false;
            //得到记录数

            if (myDs == null)
            {
                newID = 1;
            } 
            else
            {
                RecCount = myDs.Tables[0].Rows.Count;
                if (RecCount <= 0)
                {
                    newID = 1;
                }
                else
                {
                    for (i = 1; i < RecCount + 1; i++)
                    {
                        tmpID = Convert.ToInt32(myDs.Tables[0].Rows[i - 1][strKeyFieldName].ToString());
                        if (tmpID != i)
                        {
                            newID = i;
                            BlnFind = true;
                            break;
                        }
                    }
                    if (BlnFind == false)
                    {
                        newID = RecCount + 1;
                    }
                }
            }
            return newID;
        }

    }
}
