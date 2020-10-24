using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using GetMeetings.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace GetMeetings
    .Controllers
{
    public class HomeController : Controller
    {   
        // Vars for holding information to pass to the spList_get
       char b = 'a';
        int dayId = 0;
        int timeId = 0;
        string town = "";
        public IActionResult Index()
        {
            var dlmodel = new DlViewModel()
            {
                TownModel = PopulateTowns(),
                DOWModel = PopulateDOW(),
                TimeModel = PopulateTime(),
                ListModel = PopulateList(b, dayId, timeId, town)
            };

            dlmodel.SuspendSelect = "a";

            return View(dlmodel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Index(char? SuspendSelect, int? DOWSelection, int? TimeSelection, string TownSelection)
        {


            // @DOWID INTEGER = NULL,
            // @TimeID INTEGER = NULL,
            // @Town VARCHAR(25) = NULL,
            // @Suspend BIT = NULL
            b = (char)SuspendSelect;
            //dayId = (int)DOWSelection;
            //timeId = (int)TimeSelection;
            //town = TownSelection.ToString();

            var dlmodel = new DlViewModel()
            {
                TownModel = PopulateTowns(),
                DOWModel = PopulateDOW(),
                TimeModel = PopulateTime(),
                ListModel = PopulateList(b, DOWSelection, TimeSelection, TownSelection)
            };

            return View(dlmodel);
        }

        // This should go into a separate file
        private static List<SelectListItem> PopulateTowns()
        {
            List<SelectListItem> items = new List<SelectListItem>();

            using (SqlConnection connection = new SqlConnection(Startup.cnstr))
            {
                connection.Open();
                string sql = "spTowns";

                SqlCommand cmd = new SqlCommand(sql, connection);
                cmd.CommandType = CommandType.StoredProcedure;

                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        items.Add(new SelectListItem
                        {
                            Value = dr["Town"].ToString(),
                            Text = dr["Town"].ToString()
                        });
                    }
                }
                connection.Close();
            }
            return items;
        }

        //DOW
        private static List<SelectListItem> PopulateDOW()
        {
            List<SelectListItem> items = new List<SelectListItem>();

            using (SqlConnection connection = new SqlConnection(Startup.cnstr))
            {
                connection.Open();
                string sql = "spDOW";

                SqlCommand cmd = new SqlCommand(sql, connection);
                cmd.CommandType = CommandType.StoredProcedure;

                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        items.Add(new SelectListItem
                        {
                            Value = dr["DayID"].ToString(),
                            Text = dr["DayName"].ToString()
                        });
                    }
                }
                connection.Close();
            }
            return items;
        }

        // Time
        private static List<SelectListItem> PopulateTime()
        {
            List<SelectListItem> items = new List<SelectListItem>();

            using (SqlConnection connection = new SqlConnection(Startup.cnstr))
            {
                connection.Open();
                string sql = "spTime";

                SqlCommand cmd = new SqlCommand(sql, connection);
                cmd.CommandType = CommandType.StoredProcedure;

                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        items.Add(new SelectListItem
                        {
                            Value = dr["TimeID"].ToString(),
                            Text = dr["Time"].ToString()
                        });
                    }
                }
                connection.Close();
            }
            return items;
        }

        private static List<MeetingListModel> PopulateList(char? b, int? dow, int? timeId, string town)
        {
            // @Suspend BIT = NULL
            // @DOWID INTEGER = NULL,
            // @TimeID INTEGER = NULL,
            // @Town VARCHAR(25) = NULL,

            List<MeetingListModel> meetingList = new List<MeetingListModel>();
            using (SqlConnection connection = new SqlConnection(Startup.cnstr))
            {
                connection.Open();

                string sql = "spList_get";
                SqlCommand cmd = new SqlCommand(sql, connection);
                cmd.CommandType = CommandType.StoredProcedure;

                // Add Parms
                // Suspend
                SqlParameter bsuspend = cmd.Parameters.Add("@Suspend", SqlDbType.Bit);
                if (b == '0')
                {
                    bsuspend.Value = false;
                }
                else if (b == '1')
                {
                    bsuspend.Value = true;
                }
                else
                {
                    bsuspend.Value = null;
                }

                // DOW (day of week id)
                SqlParameter dowid = cmd.Parameters.Add("@DOWID", SqlDbType.Int);

                if (dow > 0 && dow < 8)
                {
                    dowid.Value = (int)dow;
                }
                else
                {
                    dowid.Value = null;
                }

                // Time Id
                SqlParameter timeid = cmd.Parameters.Add("@TimeID", SqlDbType.Int);
                if (timeId > 0 && timeId < 370)
                {
                    timeid.Value = (int)timeId;
                }
                else
                {
                    timeid.Value = null;
                }

                // Town
                SqlParameter townname = cmd.Parameters.Add("@Town", SqlDbType.NVarChar);
                if (town.Length < 4)
                {
                    townname.Value = null;
                }
                else
                {
                    townname.Value = town.ToString();
                }


                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        MeetingListModel ml = new MeetingListModel();
                        ml.ListID = Convert.ToInt32(dr["ListID"]);
                        ml.DOW = Convert.ToInt32(dr["DOW"]);
                        ml.Day = Convert.ToString(dr["Day"]);
                        ml.TimeID = Convert.ToInt32(dr["TimeID"]);
                        ml.Time = Convert.ToString(dr["Time"]);
                        ml.Town = Convert.ToString(dr["Town"]);
                        ml.GroupName = Convert.ToString(dr["GroupName"]);
                        ml.Information = Convert.ToString(dr["Information"]);
                        ml.Location = Convert.ToString(dr["Location"]);
                        ml.Type = Convert.ToString(dr["Type"]);
                        ml.suspend = Convert.ToBoolean(dr["suspend"]);

                        meetingList.Add(ml);

                    }
                    connection.Close();
                }

                return meetingList;
            }
        }

    } // controller class
} // Namespance