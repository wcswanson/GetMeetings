using System.ComponentModel.DataAnnotations;

namespace GetMeetings.Models
{
    public class TownModel
    {
        [Key]
        public string Town { get; set; }
      //  public int district { get; set; }
    }
}
