using System;

namespace SGBL.Web.Options
{
    public class LoanReminderOptions
    {
        public bool Enabled { get; set; } = true;
        public int DaysBeforeDueDate { get; set; } = 2;
        public string DailyRunTime { get; set; } = "08:00";

        public TimeSpan GetDailyRunTime()
        {
            if (TimeSpan.TryParse(DailyRunTime, out var result))
            {
                return result;
            }

            return TimeSpan.FromHours(8);
        }
    }
}