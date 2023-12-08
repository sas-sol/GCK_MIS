using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MeherEstateDevelopers.Models
{
    public static class WebAppEnvironment
    {
        public static DateTime? LastAttendanceSync;
        public static int Attendance_Machine_Conn_Check_Count = 0;
        public static int Attendance_Machine_Conn_Check_Count_Notify_Max_Limit = 0;
        public static List<string> AttendanceNoticingBodies = new List<string>{
            "info@meharestate.com"
        };
    }
}