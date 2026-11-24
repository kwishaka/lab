namespace LAB_3_assignment.Models;

public static class RentalValidation
{
    public static bool TryValidate(string startText, string endText, out int start, out int end, out string error)
    {
        start = 0;
        end = 0;
        if (!int.TryParse(startText, out start) || !int.TryParse(endText, out end))
        {
            error = "Start and end hours must be whole numbers; decimal numbers are not accepted.";
            return false;
        }
        if (start is < 0 or > 23) { error = "The start hour must be between 0 and 23."; return false; }
        if (end is < 1 or > 24) { error = "The end hour must be between 1 and 24."; return false; }
        if (start >= end) { error = "The start hour must be less than the end hour."; return false; }
        error = string.Empty;
        return true;
    }
}
