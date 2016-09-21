using System.Collections.Generic;
using System.Linq;
using MB.DAL;
using MB.Model;

namespace MB.BLL
{
    public class BankBLL : IBankBLL
    {
        /// <summary>
        /// 银行信息查看
        /// </summary>
        /// <returns></returns>
        public List<BankModel> GetBankList()
        {
            var list = new List<BankModel>();
            using (var db = new TargetNavigationDBEntities())
            {
                list = (from p in db.tblBank
                        orderby p.bankName descending
                        select new BankModel
                        {
                            bankId = p.bankId,
                            bankName = p.bankName
                        }
                          ).ToList();
            }
            return list;
        }

        /// <summary>
        /// 银行卡添加
        /// </summary>
        /// <param name="bankCard"></param>
        /// <returns></returns>
        public bool AddBankCard(BankCardModel bankCard)
        {
            var flag = false;
            using (var db = new TargetNavigationDBEntities())
            {
                if (bankCard != null)
                {
                    var model = new tblBankCard
                    {
                        bankId = bankCard.bankId,
                        cardId = bankCard.cardId
                    };
                    db.tblBankCard.Add(model);
                }
                flag = true;
                db.SaveChanges();
            }
            return flag;
        }

        /// <summary>
        /// 删除银行信息
        /// </summary>
        /// <param name="addlist"></param>
        /// <returns></returns>
        public bool DeleteBank(List<int> addlist)
        {
            var flag = false;
            using (var db = new TargetNavigationDBEntities())
            {
                foreach (var item in addlist)
                {
                    var model = db.tblBank.Where(p => p.bankId == item).FirstOrDefault();
                    if (model != null)
                    {
                        db.tblBank.Remove(model);
                    }
                }
                db.SaveChanges();
                flag = true;
            }
            return flag;
        }
    }
}