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
        private readonly IMapper _mapper;


        public BackgroundCheckService(IBackgroundCheckRepository repository, ICharacterReferenceService characterReferenceService, IMapper mapper)
        {
            _repository = repository;
            _characterReferenceService = characterReferenceService;
            _mapper = mapper;
        }
        public LogContent Create(BackgroundCheckFormViewModel form)
        {
            LogContent logContent = new LogContent();

            if (!logContent.Result)
            {
                var newForm = _mapper.Map<BackgroundCheck>(form);
                newForm.AnsweredDate = DateTime.Now;
                _repository.Create(newForm);
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
