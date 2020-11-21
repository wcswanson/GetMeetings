// using DeigCrud.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace GetMeetings.Models
{
    // Deig online View Model
    public class DoViewModel
    {
        //todo: Add models to collect:
        //todo: DOW
        //todo: Time
        // Online list

        // Day of Week (DOW)
        public string DOWSelect { get; set; }
        public IEnumerable<SelectListItem> DOWModel { get; set; }

        // Time
        public string TimeSelect { get; set; }
        public IEnumerable<SelectListItem> TimeModel { get; set; }

        // To get the rest of the online meeting data:
        public int zoomidSelect { get; set; }
        [DisplayFormat(DataFormatString = "{####-###-###}", ApplyFormatInEditMode = true)]
        public string meetingidSelect { get; set; }
        public string pswdSelect { get; set; }
        public string telephoneSelect { get; set; }
        public string groupnameSelect { get; set; }
        public string notesSelect { get; set; }

        // District
        public int DistrictSelection { get; set; }

        // To get the onling meeting model
        public IEnumerable<OnlineMeetingsModel> OnlineListModel { get; set; }

    }
}
