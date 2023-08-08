using Basecode.Data.Dto;

namespace Basecode.Services.Interfaces;

public interface ICalendarService
{
    Task<string> CreateEvent(CalendarEvent calendarEvent, string email);
}