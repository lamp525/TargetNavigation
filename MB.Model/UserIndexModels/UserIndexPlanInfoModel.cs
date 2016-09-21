using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MB.Model.UserIndexModels
{
    public  class UserIndexPlanInfoModel
    {
        public int planId { get; set; }
        public int loopId { get; set; }
        public bool isLoop { get; set; }
        public int status { get; set; }
        public string eventOutput { get; set; }
        public int responsibleUser { get; set; }
        public string responsibleUserName { get; set; }
        public int confirmUser { get; set; }
        public string confirmUserName { get; set; }
        public int importance { get; set; }
        public int urgency { get; set; }
        public int difficulty { get; set; }
        public int progress { get; set; }

    }
}
