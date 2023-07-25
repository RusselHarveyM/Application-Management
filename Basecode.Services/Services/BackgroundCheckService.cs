using AutoMapper;
using Basecode.Data.Interfaces;
using Basecode.Data.Models;
using Basecode.Data.ViewModels;
using Basecode.Services.Interfaces;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Basecode.Services.Services
{
    public class BackgroundCheckService : ErrorHandling, IBackgroundCheckService
    {
        private readonly IBackgroundCheckRepository _repository;
        private readonly ICharacterReferenceService _characterReferenceService;
        private readonly ITrackService _trackService;
        private readonly IApplicantService _applicantService;
        private readonly IMapper _mapper;

        public BackgroundCheckService(IBackgroundCheckRepository repository, ICharacterReferenceService characterReferenceService, IMapper mapper, ITrackService trackService, IApplicantService applicantService)
        {
            _repository = repository;
            _characterReferenceService = characterReferenceService;
            _mapper = mapper;
            _trackService = trackService;
            _applicantService = applicantService;
        }

        public async Task<LogContent> Create(BackgroundCheckFormViewModel form)
        {
            LogContent logContent = new LogContent();

            logContent = CheckBackground(form);
            if (!logContent.Result)
            {
                var newForm = _mapper.Map<BackgroundCheck>(form);
                newForm.AnsweredDate = DateTime.Now;

                var backgroundId = _repository.Create(newForm);

                var result = _repository.GetById(backgroundId);
                var applicant = _applicantService.GetApplicantById(result.CharacterReference.ApplicantId);

                await _trackService.GratitudeNotification(applicant, result);
            }
            return logContent;
        }

        public BackgroundCheck GetBackgroundByCharacterRefId(int characterRefId)
        {
            var fetchData = _repository.GetAll().Where(m => m.CharReferenceId == characterRefId).FirstOrDefault();

            if (fetchData != null)
            {
                fetchData.CharacterReference = _characterReferenceService.GetCharacterReferenceById(characterRefId);
            }

            return fetchData;
        }
    }
}
