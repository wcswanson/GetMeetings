using GetMeetings.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;

namespace GetMeetings
    .Controllers
{

    //todo: Change background on buttons
    //todo: Change font size to make it smaller
    //z todo: Remove titles in cshtml to make the presentation iframe ready
    //z todo: Correct spelling in Group name in sp
    //todo: Remove empty space for first 2 columns in sp.
    //todo: bootstrap styling
    //todo: error log add
    //z todo: Suspended removed from meeting list but hook is left for sp but always false

    public class HomeController : Controller
    {
        // Vars for holding information to pass to the spList
        //char b = 'a';
        int dayId = 0;
        int timeId = 0;
        string town = "";
        int district = -1;
        static string msg = "";
        //private Stream fileStream;

        public IActionResult Index()
        {
            var dlmodel = new DlViewModel()
            {
                TownModel = PopulateTowns(),
                DOWModel = PopulateDOW(),
                TimeModel = PopulateTime(),
                DistrictModel = PopulateDistricts(),
                ListModel = PopulateList(dayId, timeId, town, district)
            };

            //  dlmodel.SuspendSelect = "a";

            return View(dlmodel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Index(char? SuspendSelect, int? DOWSelection, int? TimeSelection, string TownSelection, int? DistrictSelection)
        {


            // @DOWID INTEGER = NULL,
            // @TimeID INTEGER = NULL,
            // @Town VARCHAR(25) = NULL,
            // @Suspend BIT = NULL
            //b = (char)SuspendSelect;
            //dayId = (int)DOWSelection;
            //timeId = (int)TimeSelection;
            //town = TownSelection.ToString();

            var dlmodel = new DlViewModel()
            {
                TownModel = PopulateTowns(),
                DOWModel = PopulateDOW(),
                TimeModel = PopulateTime(),
                DistrictModel = PopulateDistricts(),
                
                ListModel = PopulateList(DOWSelection, TimeSelection, TownSelection, DistrictSelection)
            };

            return View(dlmodel);
        }

        //Get directions
        // [HttpGet("{id}")]
        public RedirectResult GetDirections(string id)
        {
            return Redirect("http://downeastintergroup.org/DirectionsToMeeting.html?" + id.ToString() + ", ME");
        }

        [HttpGet]
        public IActionResult OnLineMeetings()
        {
            return View();
        }

        // *******************************************************/
        // Export list
        // Solution: https://stackoverflow.com/questions/53491070/create-text-file-and-download-without-saving-on-server-in-asp-net-core-mvc-2-1
        public ContentResult ExportList()
        {

            string ml = ExportMeetingList();

            // To product a file name for down load.
            Response.Headers.Add("Content-Disposition", "attachment; filename=\"MeetingList.txt\"");

            return Content(ml, "text/csv");
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
                    try
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
                    catch (SqlException ex)
                    {
                        msg = msg + " spTowns: " + ex.Message.ToString();
                    }
                }
                connection.Close();
            }
            return items;
        }

        // District
        private static List<SelectListItem> PopulateDistricts()
        {
            List<SelectListItem> items = new List<SelectListItem>();

            using (SqlConnection connection = new SqlConnection(Startup.cnstr))
            {
                connection.Open();
                string sql = "spDistrict";

                SqlCommand cmd = new SqlCommand(sql, connection);
                cmd.CommandType = CommandType.StoredProcedure;

                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    try
                    {

                        while (dr.Read())
                        {
                            items.Add(new SelectListItem
                            {
                                Value = dr["district"].ToString(),
                                Text = dr["district"].ToString()
                            });
                        }
                    }
                    catch (SqlException ex)
                    {
                        msg = msg + " spDistrict: " + ex.Message.ToString();
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
                    try
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
                    catch (SqlException ex)
                    {
                        msg = msg + " spDow: " + ex.Message.ToString();
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
                    try
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
                    catch (SqlException ex)
                    {
                        msg = "sptime: " + ex.Message.ToString();
                    }

                }
                connection.Close();
            }
            return items;
        }

        // private static List<MeetingListModel> PopulateList(char? b, int? dow, int? timeId, string town)
        private static List<MeetingListModel> PopulateList(int? dow, int? timeId, string town, int? district)
        {
            // @Suspend BIT = NULL
            // @DOWID INTEGER = NULL,
            // @TimeID INTEGER = NULL,
            // @Town VARCHAR(25) = NULL,

            List<MeetingListModel> meetingList = new List<MeetingListModel>();
            using (SqlConnection connection = new SqlConnection(Startup.cnstr))
            {
                connection.Open();

                string sql = "spList";
                SqlCommand cmd = new SqlCommand(sql, connection);
                cmd.CommandType = CommandType.StoredProcedure;

                // Add Parms
                // Suspend -- not shown in list display
                SqlParameter bsuspend = cmd.Parameters.Add("@Suspend", SqlDbType.Bit);
                bsuspend.Value = null;
                
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
                if (String.IsNullOrEmpty(town))
                {
                    town = "";
                }

                SqlParameter townname = cmd.Parameters.Add("@Town", SqlDbType.VarChar);
                if (town.Length < 4)
                {
                    townname.Value = null;
                }
                else
                {
                    townname.Value = town.ToString();
                }

                // District
                // DOW (day of week id)
                SqlParameter districtnumber = cmd.Parameters.Add("@District", SqlDbType.Int);

                if (district > 0)
                {
                    districtnumber.Value = (int)district;
                }
                else
                {
                    districtnumber.Value = null;
                }

                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    try
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
                            ml.district = Convert.ToInt32(dr["district"]);

                            meetingList.Add(ml);
                        }
                    }
                    catch (SqlException ex)
                    {
                        msg = msg + " spList: " + ex.Message.ToString();
                    }
                    connection.Close();
                }

                return meetingList;
            }
        }

        private static string ExportMeetingList()
        {
            string comma = ", ";

            using (SqlConnection connection = new SqlConnection(Startup.cnstr))
            {

                connection.Open();
                string sql = "GetMeetingList";

                SqlCommand cmd = new SqlCommand(sql, connection);
                cmd.CommandType = CommandType.StoredProcedure;
                SqlDataAdapter sqlDataAdapter = new SqlDataAdapter();
                cmd.Connection = connection;
                sqlDataAdapter.SelectCommand = cmd;
                DataTable dataTable = new DataTable();
                try
                {
                    sqlDataAdapter.Fill(dataTable);
                }
                catch (InvalidCastException ex)
                {
                    msg = ex.ToString();
                }
                StringBuilder stringBuilder = new StringBuilder();
                foreach (DataColumn column in dataTable.Columns)
                {
                    stringBuilder.Append(column.ColumnName + comma);
                }
                stringBuilder.AppendLine();
                foreach (DataRow row in dataTable.Rows)
                {
                    foreach (DataColumn column2 in dataTable.Columns)
                    {
                        stringBuilder.Append(row[column2.ColumnName].ToString() + comma);
                    }
                    stringBuilder.AppendLine();
                }

                return stringBuilder.ToString();
            }
        }


    } // controller class
} // Namespance