namespace Basecode.Data.Dto;

public class CalendarEvent
{
    public CalendarEvent()
    {
        Body = new Body()
        {
            ContentType = "html"
        };
        Start = new EventDateTime()
        {
            TimeZone = "Pacific Standard Time"
        };
        End = new EventDateTime()
        {
            TimeZone = "Pacific Standard Time"
        };
        IsOnlineMeeting = true;
        OnlineMeetingProvider = "TeamsForBusiness";
    }
    
    public string Subject { get; set; }
    
    public Body Body { get; set; }
    
    public EventDateTime Start { get; set; }
    
    public EventDateTime End { get; set; }
    public string OnlineMeetingProvider { get; set; }
    public bool IsOnlineMeeting { get; set; }
}

public class Body
{
    public string ContentType { get; set; }
    public string Content { get; set; }
}

public class EventDateTime
{
    public DateTime DateTime { get; set; }
    public string TimeZone { get; set; }
}
