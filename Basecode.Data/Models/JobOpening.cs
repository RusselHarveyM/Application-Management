﻿namespace Basecode.Data.Models;

public class JobOpening
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string EmploymentType { get; set; }
    public string WorkSetup { get; set; }
    public string Location { get; set; }
    public string Tag { get; set; }
    public string CreatedBy { get; set; }

    public DateTime CreatedTime { get; set; }

    public string UpdatedBy { get; set; }

    public DateTime UpdatedTime { get; set; }

    public List<Qualification> Qualifications { get; set; }
    public List<Responsibility> Responsibilities { get; set; }
    public ICollection<Application> Applications { get; } = new List<Application>();
    public ICollection<User> Users { get; } = new List<User>();
}