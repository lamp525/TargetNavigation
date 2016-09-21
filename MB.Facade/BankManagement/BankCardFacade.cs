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
    public class BankCardFacade
    {
        BankCardBLL BankCardBLL = new BankCardBLL();
        /// <summary>
        /// 银行卡获取
        /// </summary>
        /// <returns></returns>
        public List<BankCardModel> GetBankCardList()
        {
            using (var db = new TargetNavigationDBEntities())
            {
                return BankCardBLL.GetBankCardList(db);
            }
        }

        /// <summary>
        /// 新增银行卡
        /// </summary>
        /// <param name="bankCardModel"></param>
        /// <returns></returns>
        public int AddNewBankCard(BankCardModel bankCardModel)
        {

            using (var db = new TargetNavigationDBEntities())
            {
                return BankCardBLL.AddNewBankCard(bankCardModel, db);
            }
        }

        /// <summary>
        /// 删除银行卡
        /// </summary>
        /// <param name="id">id列表</param>
        /// <returns></returns>
        public bool DeleteBankCard(string id)
        {

            using (var db = new TargetNavigationDBEntities())
            {
                return BankCardBLL.DeleteBankCard(id, db);
            }
        }

        /// <summary>
        /// 更新执行
        /// </summary>
        /// <param name="bankCardModel">更新列表</param>
        /// <returns></returns>
        public int Update(BankCardModel bankCardModel)
        {

            using (var db = new TargetNavigationDBEntities())
            {
                return BankCardBLL.Update(bankCardModel, db);
            }
        }
    }
}
