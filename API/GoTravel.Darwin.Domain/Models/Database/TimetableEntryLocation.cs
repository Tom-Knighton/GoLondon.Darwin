namespace GoTravel.Darwin.Domain.Models.Database;

public class TimetableEntryLocation
{
    public int Id { get; set; }
    public string RID { get; set; }
    public string Location { get; set; }
    public DateTime? PredictedArrival { get; set; }
    public DateTime? PredictedDeparture { get; set; }
    public DateTime? ScheduledArrival { get; set; }
    public DateTime? ScheduledDeparture { get; set; }
    public string ActivityType { get; set; }
    public string? Platform { get; set; }
    public bool Cancelled { get; set; }
    public bool Delayed { get; set; }
    
    public virtual TimetableEntry TimetableEntry { get; set; }

}