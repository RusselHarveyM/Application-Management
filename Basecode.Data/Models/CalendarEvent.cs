namespace Basecode.Data.Dto;

public class CalendarEvent
{
    public string Subject { get; set; }
    
    public Body Body { get; set; }
    
    public EventDateTime Start { get; set; }
    
    public EventDateTime End { get; set; }
    public string OnlineMeetingProvider { get; set; } = "TeamsForBusiness";
    public bool IsOnlineMeeting { get; set; } = true;
}

public class Body
{
    public string ContentType { get; set; } = "html";
    public string? Content { get; set; }
}

public class EventDateTime
{
    public DateTime DateTime { get; set; }
    public string TimeZone { get; set; } = "Asia/Singapore";
}
