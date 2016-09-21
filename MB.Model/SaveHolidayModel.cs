using System.Collections.Generic;

namespace MB.Model
{
    public class SaveHolidayModel
    {
        public List<string> holiday { get; set; }
        public List<string> workday { get; set; }
        public List<string> defday { get; set; }
    }
}