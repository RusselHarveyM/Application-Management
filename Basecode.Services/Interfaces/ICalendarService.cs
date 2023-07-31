using Basecode.Data.Dto;

namespace Basecode.Services.Interfaces;

public interface ICalendarService
{
    string CreateEvent(CalendarEvent calendarEvent, string email);
}