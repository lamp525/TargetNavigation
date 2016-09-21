using MB.DAL;
using System.Linq;

namespace MB.New.BLL
{
    public class DBUtility
    {
        /// <summary>
        /// 取得数据库表主键自增长ID
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="db"></param>
        /// <returns></returns>
        public static int GetPrimaryKeyByTableName(TargetNavigationDBEntities db, string tableName)
        {
            System.Data.Entity.Core.Objects.ObjectParameter obj = new System.Data.Entity.Core.Objects.ObjectParameter("num", -1);
            return db.prcGetPrimaryKey(tableName, obj).FirstOrDefault().Value;
        }
    }
}