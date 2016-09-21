using System.Collections.Generic;
using MB.Model;

namespace MB.BLL
{
    public interface IBankBLL
    {
        List<BankModel> GetBankList();

        bool DeleteBank(List<int> addlist);
    }
}