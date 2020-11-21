using System.ComponentModel.DataAnnotations;

namespace GetMeetings.Models
{
    // Use spList_get parms: DOWID, TimeID, Town, Suspend  -- default null
    public class MeetingListModel
    {
        [Key]
        public int ListID { get; set; }
        public int DOW { get; set; }
        public string Day { get; set; }
        public int TimeID { get; set; }
        public string Time { get; set; }
        public string Town { get; set; }
        public string GroupName { get; set; }
        public string Information { get; set; }
        public string Location { get; set; }
        public string Type { get; set; }
        public bool suspend { get; set; }       
        public int district { get; set; }
    }
}
