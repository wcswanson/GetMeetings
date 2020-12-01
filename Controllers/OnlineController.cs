using GetMeetings.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace GetMeetings.Controllers
{
    public class OnlineController : Controller
    {
        const string SPGETONLINE = "spGetOnlineList";

        // ZoomId and TempData["id"] is used to pass the id from controller function to controller function
        // But not the helper functions
        // int ZoomId = 0;
        int dayId = 0;
        int timeId = 0;
        static string msg = "";
        int district = -1;
        //string sp = "";

#nullable enable
        [HttpGet]
        public IActionResult Index()
        {
            var doViewmodel = new DoViewModel()
            {
                DOWModel = PopulateDOW(),
                TimeModel = PopulateTime(),
                DistrictModel = PopulateDistricts(),
                OnlineListModel = (IEnumerable<OnlineMeetingsModel>)PopulateOnlineList(dayId, timeId, district)
            };

            return View(doViewmodel);
        }

        // Index Post
        [HttpPost]
        // [ValidateAntiForgeryToken]
        public IActionResult Index(int? DOWSelect, int? TimeSelect, int? DistrictSelect)
        {

            var doViewmodel = new DoViewModel()
            {
                DOWModel = PopulateDOW(),
                TimeModel = PopulateTime(),
                DistrictModel = PopulateDistricts(),
                OnlineListModel = (IEnumerable<OnlineMeetingsModel>)PopulateOnlineList(DOWSelect, TimeSelect, DistrictSelect)
            };

            return View(doViewmodel);
        }

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
                        msg = msg + $" spDow: {ex.Message.ToString()} ";
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
                        msg = $"spTime: {ex.Message.ToString()}";
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


        // Populate the online list
        private static List<OnlineMeetingsModel> PopulateOnlineList(int? DayId, int? TimeId, int? district)
        {
            List<OnlineMeetingsModel> onlineList = new List<OnlineMeetingsModel>();

            using (SqlConnection connection = new SqlConnection(Startup.cnstr))
            {
                connection.Open();

                SqlCommand cmd = new SqlCommand(SPGETONLINE, connection);
                cmd.CommandType = CommandType.StoredProcedure;

                // Add Parms
                // DayId
                SqlParameter dayid = cmd.Parameters.Add("@DayId", SqlDbType.Int);
                if (DayId == 0 || DayId == 8)
                {
                    dayid.Value = null;
                    DayId = 0;
                }
                else
                {
                    dayid.Value = DayId;
                }

                // TimeId
                SqlParameter timeid = cmd.Parameters.Add("@TimeId", SqlDbType.Int);
                if (TimeId == 0)
                {
                    timeid.Value = null;
                }
                else
                {
                    timeid.Value = TimeId;
                }

                // District
                SqlParameter distrinctnumber = cmd.Parameters.Add("@District", SqlDbType.Int);
                if (district == 0 || district == null)
                {
                    distrinctnumber.Value = null;
                }
                else
                {
                    distrinctnumber.Value = district;
                }

                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    try
                    {
                        while (dr.Read())
                        {
                            OnlineMeetingsModel ol = new OnlineMeetingsModel();
                            ol.zoomid = Convert.ToInt32(dr["zoomid"]);
                            ol.dayid = Convert.ToInt32(dr["dayid"]);
                            ol.day = Convert.ToString(dr["DayName"]);
                            ol.timeid = Convert.ToInt32(dr["timeid"]);
                            ol.time = Convert.ToString(dr["Time"]);
                            ol.meetingid = Convert.ToString(dr["meetingid"]);
                            // "password" is not allowed for it throws an out of range exception.
                            ol.pswd = Convert.ToString(dr["pswd"]);
                            ol.telephone = Convert.ToString(dr["telephone"]);
                            ol.groupname = Convert.ToString(dr["groupname"]);
                            ol.notes = Convert.ToString(dr["notes"]);
                            ol.district = Convert.ToInt32(dr["district"]);

                            onlineList.Add(ol);
                        }
                    }
                    catch (SqlException ex)
                    {
                        msg = msg + $" onlineList: {ex.Message.ToString()}";
                    }
                    connection.Close();
                }

                return onlineList;
            }
        }
    }
}
