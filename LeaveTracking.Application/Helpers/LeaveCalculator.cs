namespace LeaveTracking.Application.Helpers
{
    public static class LeaveCalculator
    {
        public static int CalculateLeaveDate(DateTime startDate, DateTime endDate)
        {
            var totalDays = 0;
            for (var date = startDate.Date; date <= endDate.Date; date = date.AddDays(1))
            {
                if (!(IsWeekend(date)))
                    totalDays++;
            }

            return totalDays;
        }

        private static bool IsWeekend(DateTime date)
        {
            return date.DayOfWeek == DayOfWeek.Saturday || date.DayOfWeek == DayOfWeek.Sunday;
        }
    }
}