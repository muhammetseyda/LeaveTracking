using LeaveTracking.Domain.Enums;

namespace LeaveTracking.Application.Helpers
{
    public static class LeaveTypeHelpers
    {
        public static string GetLeaveTypeName(int leaveTypeId)
        {
            return leaveTypeId switch
            {
                1 => "Yıllık İzin",
                2 => "Hastalık İzni",
                3 => "Doğum İzni",
                4 => "Ölüm İzni",
                5 => "Evlilik İzni",
                _ => leaveTypeId.ToString()
            };
        }

        public static string GetLeaveTypeName(LeaveType leaveType)
        {
            return GetLeaveTypeName((int)leaveType);
        }

        public static LeaveType? GetLeaveTypeFromName(string name)
        {
            return name switch
            {
                "Yıllık İzin" => LeaveType.Annual,
                "Hastalık İzni" => LeaveType.Sick,
                "Doğum İzni" => LeaveType.Maternity,
                "Ölüm İzni" => LeaveType.Bereavement,
                "Evlilik İzni" => LeaveType.Marriage,
                _ => null
            };
        }
    }
}