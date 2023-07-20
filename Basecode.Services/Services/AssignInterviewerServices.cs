using AutoMapper;
using Basecode.Data.Interfaces;
using Basecode.Data.Models;
using Basecode.Data.Repositories;
using Basecode.Data.ViewModels;
using Basecode.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Basecode.Services.Services
{
    public class AssignInterviewerService : IAssignInterviewerService
    {
        private readonly IAssignInterviewerRepository _assignRepository;
        private readonly IMapper _mapper;


        public AssignInterviewerService(IAssignInterviewerRepository assignRepository, IMapper mapper)
        {
            _assignRepository = assignRepository;
            _mapper = mapper;
        }

        public List<AssignInterviewerViewModel> GetJobs()
        {
            var data = _assignRepository.GetAll()
                .Select(m => _mapper.Map<AssignInterviewerViewModel>(m))
                .ToList();

            return data;
        }
        public void Create(string JobPosition, string Email)
        {
            AssignInterviewer assign = new AssignInterviewer();
            assign.JobPosition = JobPosition;
            assign.Email = Email;
              _assignRepository.AddAssign(assign);
           
        }

        public AssignInterviewerViewModel GetById(int id)
        {

            var data = _assignRepository.GetAll()
                .Where(m => m.Id == id)
                .Select(m => new AssignInterviewerViewModel
                {
                    Id = m.Id,
                    JobPosition = m.JobPosition,
                    Email = m.Email,
                })
                .FirstOrDefault();

            return data;
        }

    }
}
