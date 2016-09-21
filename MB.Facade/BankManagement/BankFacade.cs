using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MB.DAL;
using MB.New.BLL.BankManagement;
using MB.New.Model.BankManagement;

namespace MB.Facade.BankManagement
{
   public class BankFacade
    {
       BankBLL BankBLL = new BankBLL();
          /// <summary>
        /// 银行信息查看
        /// </summary>
        /// <returns></returns>
       public List<BankModel> GetBankList()
       {
           using (var db = new TargetNavigationDBEntities())
           {
              return BankBLL.GetBankList(db);
           }
       }

          /// <summary>
        /// 银行卡添加
        /// </summary>
        /// <param name="bankCard"></param>
        /// <returns></returns>
       public bool AddBankCard(BankCardModel bankCard)
       {
          
           using (var db = new TargetNavigationDBEntities())
           {
               return BankBLL.AddBankCard(bankCard,db);
           }
       }

           /// <summary>
        /// 删除银行信息
        /// </summary>
        /// <param name="addlist"></param>
        /// <returns></returns>
       public bool DeleteBank(List<int> addlist)
       {
        
           using (var db = new TargetNavigationDBEntities())
           {
               return BankBLL.DeleteBank(addlist,db);
           }
       }
    }
}
