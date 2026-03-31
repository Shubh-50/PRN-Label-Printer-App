using BarcodeBartenderApp;

public static class ShiftHelper
{
    public static string GetCurrentShift()
    {
        try
        {
            var shifts = DatabaseHelper.GetShifts();
            TimeSpan now = DateTime.Now.TimeOfDay;

            foreach (var s in shifts)
            {
                if (s.start < s.end)
                {
                    if (now >= s.start && now < s.end)
                        return s.shift;
                }
                else
                {
                    if (now >= s.start || now < s.end)
                        return s.shift;
                }
            }
        }
        catch { }
        return "Unknown";
    }
}