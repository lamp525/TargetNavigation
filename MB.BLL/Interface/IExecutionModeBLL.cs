using System.Collections.Generic;
using MB.Model;

namespace MB.BLL
{
    public interface IExecutionModeBLL
    {
        List<ExecutionMode> GetExecutionList();

        int AddNewExecution(ExecutionMode list);

        bool DeleteExection(int id);

        int Update(ExecutionMode list);
    }
}