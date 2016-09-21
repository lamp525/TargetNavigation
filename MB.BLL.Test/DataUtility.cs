using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;

namespace MB.BLL.Test
{
    class DataUtility
    {
        public static void InsertDataBase(string fileName, string testCaseId = null)
        {
            //Process myProcess = new Process();

            SqlConnection connection = null;
            SqlTransaction trans = null;

            string currentSql = null;

            try
            {
                string filePath = System.Environment.CurrentDirectory;
                filePath = filePath.Substring(0, filePath.IndexOf("bin"));

                fileName = filePath + "\\TestData\\" + fileName;

                //ProcessStartInfo myProcessStartInfo = new ProcessStartInfo(filePath + "ImportDataTool\\ImportDataTool.exe", fileName + " " + testCaseId);

                //myProcess.StartInfo = myProcessStartInfo;

                //myProcess.Start();

                ExcelHelper excelHelper = new ExcelHelper(fileName);

                List<string> sqlList = excelHelper.ExcelToSql(testCaseId);

                if (sqlList.Count == 0)
                {
                    return;
                }

                connection = DataAccess.DbConn();

                trans = connection.BeginTransaction();

                foreach (string sql in sqlList)
                {
                    currentSql = sql;
                    SqlHelper.ExecuteNonQuery(trans, CommandType.Text, sql);
                }

                trans.Commit();

            }
            catch (Exception ex)
            {
                DataAccess.Rollback(trans);
                throw ex;
            }
            finally
            {
                DataAccess.DbClose(connection);
            }
        }
    }
}
