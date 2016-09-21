using MB.DAL;

namespace MB.Model
{
    public class PlanViewModel
    {
        public tblPlan plan
        {
            get;
            set;
        }

        public tblProject project
        {
            get;
            set;
        }

        public tblOrganization organization
        {
            get;
            set;
        }

        public tblExecutionMode executionMode
        {
            get;
            set;
        }

        public tblUser user
        {
            get;
            set;
        }
    }
}