using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MB.DAL;
using MB.New.Model.BankManagement;

namespace MB.New.BLL.BankManagement
{
   public class BankCardBLL
    {
        /// <summary>
        /// 银行卡获取
        /// </summary>
        /// <returns></returns>
        public List<BankCardModel> GetBankCardList(TargetNavigationDBEntities db)
        {
         
                var cardList = (from bc in db.tblBankCard
                                join bank in db.tblBank on bc.bankId equals bank.bankId
                                orderby bc.bankId, bc.cardId
                                select new BankCardModel
                                {
                                    bankId = bc.bankId,
                                    bankName = bank.bankName,
                                    cardId = bc.cardId
                                }).ToList();

                return cardList;
            
        }

        /// <summary>
        /// 新增银行卡
        /// </summary>
        /// <param name="bankCardModel"></param>
        /// <returns></returns>
        public int AddNewBankCard(BankCardModel bankCardModel,TargetNavigationDBEntities db)
        {

            var flag = 1;
          
                var hasBankCard = db.tblBankCard.Where(p => p.cardId == bankCardModel.cardId).FirstOrDefault();
                if (hasBankCard != null)
                {
                    flag = 2;
                }
                else
                {

                    System.Data.Entity.Core.Objects.ObjectParameter obj = new System.Data.Entity.Core.Objects.ObjectParameter("num", -1);
                    var ValueIncentiveCustom = new tblBankCard
                    {
                        cardId = bankCardModel.cardId,
                        bankId = bankCardModel.bankId
                    };
                    db.tblBankCard.Add(ValueIncentiveCustom);

                    db.SaveChanges();
                }
            
            return flag;
        }

        /// <summary>
        /// 删除银行卡
        /// </summary>
        /// <param name="id">id列表</param>
        /// <returns></returns>
        public bool DeleteBankCard(string id,TargetNavigationDBEntities db)
        {
            var flag = false;
       
                var olddata = db.tblBankCard.Where(p => p.cardId == id).FirstOrDefault();
                if (olddata != null)
                {
                    db.tblBankCard.Remove(olddata);
                }

                db.SaveChanges();
                flag = true;
            
            return flag;
        }

        /// <summary>
        /// 更新执行
        /// </summary>
        /// <param name="bankCardModel">更新列表</param>
        /// <returns></returns>
        public int Update(BankCardModel bankCardModel,TargetNavigationDBEntities db)
        {
            var flag = 1;
         
                var hasBankCard = db.tblBankCard.Where(p => p.cardId == bankCardModel.cardId && p.bankId == bankCardModel.bankId).FirstOrDefault();
                if (hasBankCard != null)
                {
                    flag = 2;
                }
                else
                {
                    var model = db.tblBankCard.Where(p => p.cardId == bankCardModel.updateCardId).FirstOrDefault();
                    if (model != null)
                    {
                        db.tblBankCard.Remove(model);
                        db.SaveChanges();
                        model.cardId = bankCardModel.cardId;
                        model.bankId = bankCardModel.bankId;
                        db.tblBankCard.Add(model);
                        db.SaveChanges();
                    }

                }
            
            return flag;
        }
    }
}
