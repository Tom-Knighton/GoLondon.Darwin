namespace GoTravel.Darwin.Domain.Models.Database;

public class TimetableEntry
{
    public string RID { get; set; }
    public DateTime StartDate { get; set; }
    public string TrainHeadcode { get; set; }
    public string Operator { get; set; }
    public int? LateReason { get; set; }
    public int? CancelledReason { get; set; }
    public bool Cancelled { get; set;  }
    
    public virtual ICollection<TimetableEntryLocation> Locations { get; set; }
}