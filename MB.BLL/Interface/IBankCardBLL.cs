using System.Collections.Generic;
using MB.Model;

namespace MB.BLL
{
    public interface IBankCardBLL
    {
        List<BankCardModel> GetBankCardList();

        int AddNewBankCard(BankCardModel list);

        bool DeleteBankCard(string id);

        int Update(BankCardModel list);
    }
}