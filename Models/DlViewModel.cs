using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace GetMeetings.Models
{
    // Define models -- in seperate files
    // Except for Suspend -- define it here
    // Follow the pattern for public properties: 
    //  1. Singular item var to put the selected value in: 'TownSelection'
    //  2. Plural items model name: TownModel


    public class DlViewModel
    {
        // Suspend
        public string SuspendSelect { get; set; }
        // Plural
        public List<SelectListItem> Suspended = new List<SelectListItem>
        {
            new SelectListItem { Value = "a", Text = "All" },
            new SelectListItem { Value = "1", Text = "Suspended" },
            new SelectListItem { Value = "0", Text = "Not suspended"  },
        };

        // Day of Week (DOW)
        public string DOWSelection { get; set; }
        public IEnumerable<SelectListItem> DOWModel { get; set; }

        // Time
        public string TimeSelection { get; set; }
        public IEnumerable<SelectListItem> TimeModel { get; set; }

        // Towns
        public string TownSelection { get; set; }
        public IEnumerable<SelectListItem> TownModel { get; set; }

        // Meeting list
       public IEnumerable< MeetingListModel> ListModel { get; set; }

    }
}
