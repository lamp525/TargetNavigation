using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using NPOI.HSSF.UserModel;
using NPOI.HSSF.Util;

namespace MB.BLL.Test
{
    public class ExcelHelper : IDisposable
    {
        private string fileName = null; //文件名
        private IWorkbook workbook = null;
        private FileStream fs = null;
        private bool disposed;

        public ExcelHelper(string fileName)
        {
            this.fileName = fileName;
            disposed = false;
        }

        /// <summary>
        /// 将excel数据读入输出SQL文
        /// </summary>
        /// <param name="operateRow">操作方式所在行</param>
        /// <param name="operateColumn">操作方式所在列</param>
        /// <param name="columnRow">列名所在行</param>
        /// <param name="columnColumn">列名所在列</param>
        /// <returns></returns>
        public List<string> ExcelToSql(string testCaseId = null)
        {
            ISheet sheet = null;
            List<string> sqlList = new List<string>();

            fs = new FileStream(fileName, FileMode.Open, FileAccess.Read);
            if (fileName.IndexOf(".xlsx") > 0) // 2007版本
                workbook = new XSSFWorkbook(fs);
            else if (fileName.IndexOf(".xls") > 0) // 2003版本
                workbook = new HSSFWorkbook(fs);

            for (int i = 0; i < workbook.NumberOfSheets; i++)
            {
                sheet = workbook.GetSheetAt(i);

                if (sheet != null)
                {
                    string table = workbook.GetSheetName(i);
                    int operateType = 0;

                    int.TryParse(sheet.GetRow(1).GetCell(0).ToString(), out operateType);

                    switch (operateType)
                    {
                        // 删除原有数据
                        case 0:
                            sqlList.Add(string.Format("DELETE FROM {0}", table));
                            break;
                        default:
                            break;
                    }

                    StringBuilder sqlFront = new StringBuilder();

                    sqlFront.Append("INSERT INTO ");
                    sqlFront.Append(table);
                    sqlFront.Append("(");

                    IRow columnTemplateRow = sheet.GetRow(2);
                    int? primary1 = null;
                    int? primary2 = null;
                    int? primary3 = null;

                    for (int j = 1; j < columnTemplateRow.LastCellNum; j++)
                    {
                        // 判断背景色记录主键
                        if (columnTemplateRow.GetCell(j).CellStyle.BorderTop == BorderStyle.Double && columnTemplateRow.GetCell(j).CellStyle.BorderBottom == BorderStyle.Double)
                        {
                            if (primary1 == null)
                            {
                                primary1 = j;
                            }
                            else if (primary2 == null)
                            {
                                primary2 = j;
                            }
                            else if (primary3 == null)
                            {
                                primary3 = j;
                            }
                        }
                        sqlFront.Append(columnTemplateRow.GetCell(j).ToString());
                        sqlFront.Append(",");
                    }

                    sqlFront.Remove(sqlFront.Length - 1, 1);
                    sqlFront.Append(") VALUES (");

                    for (int j = 3; j <= sheet.LastRowNum; j++)
                    {
                        IRow dataRow = sheet.GetRow(j);

                        if (!string.IsNullOrEmpty(testCaseId) && !testCaseId.Equals(dataRow.GetCell(0).ToString().Trim()))
                        {
                            continue;
                        }

                        #region 判断主键是否为空，主键为空则不处理
                        if (primary1 != null && string.IsNullOrEmpty(dataRow.GetCell(primary1.Value).ToString()))
                        {
                            //LogHelper.Output("", string.Format("表名：{2}，行号：{0}， 列号：{1}, 主键为空", j + 1, primary1.Value + 1, table));
                            continue;
                        }
                        if (primary2 != null && string.IsNullOrEmpty(dataRow.GetCell(primary2.Value).ToString()))
                        {
                            //LogHelper.Output("", string.Format("表名：{2}，行号：{0}， 列号：{1}, 主键为空", j + 1, primary2.Value + 1, table));
                            continue;
                        }
                        if (primary3 != null && string.IsNullOrEmpty(dataRow.GetCell(primary3.Value).ToString()))
                        {
                            //LogHelper.Output("", string.Format("表名：{2}，行号：{0}， 列号：{1}, 主键为空", j + 1, primary3.Value + 1, table));
                            continue;
                        }
                        #endregion

                        StringBuilder sql = new StringBuilder(sqlFront.ToString());

                        IRow typeRow = sheet.GetRow(1);

                        for (int k = 1; k < dataRow.LastCellNum; k++)
                        {
                            if (dataRow.GetCell(k).ToString().ToLower() == "null")
                            {
                                sql.Append(dataRow.GetCell(k).ToString());
                            }
                            else if (typeRow.GetCell(k).ToString().ToLower() == "int" || typeRow.GetCell(k).ToString().ToLower() == "decimal")
                            {
                                if (string.IsNullOrEmpty(dataRow.GetCell(k).ToString()))
                                {
                                    sql.Append("NULL");
                                }
                                else
                                {
                                    sql.Append(dataRow.GetCell(k).ToString());
                                }
                            }
                            else if (typeRow.GetCell(k).ToString().ToLower() == "bit")
                            {
                                if (string.IsNullOrEmpty(dataRow.GetCell(k).ToString()))
                                {
                                    sql.Append("NULL");
                                }
                                else
                                {
                                    int bit = 0;

                                    if (dataRow.GetCell(k).ToString().ToLower() == "true" || dataRow.GetCell(k).ToString().ToLower() == "false")
                                    {
                                        bit = Convert.ToInt32(bool.Parse(dataRow.GetCell(k).ToString()));
                                    }
                                    else
                                    {
                                        bit = Convert.ToInt32(dataRow.GetCell(k).ToString());
                                    }

                                    sql.Append(bit);
                                }
                            }
                            else if (typeRow.GetCell(k).ToString().ToLower() == "datetime")
                            {
                                if (string.IsNullOrEmpty(dataRow.GetCell(k).ToString()))
                                {
                                    sql.Append("NULL");
                                }
                                else
                                {
                                    sql.Append("'");
                                    sql.Append(DateTime.Parse(dataRow.GetCell(k).ToString()).ToString("yyyy-MM-dd HH:mm:ss"));
                                    sql.Append("'");
                                }
                            }
                            else
                            {
                                sql.Append("'");
                                sql.Append(dataRow.GetCell(k).ToString().Replace("'", "''"));
                                sql.Append("'");
                            }

                            sql.Append(",");
                        }

                        sql.Remove(sql.Length - 1, 1);
                        sql.Append(")");

                        sqlList.Add(sql.ToString());
                        sql.Clear();
                    }
                }
            }

            return sqlList;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    if (fs != null)
                        fs.Close();
                }

                fs = null;
                disposed = true;
            }
        }

    }
}
