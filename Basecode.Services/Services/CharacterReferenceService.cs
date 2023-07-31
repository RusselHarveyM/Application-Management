using AutoMapper;
using Basecode.Data.Interfaces;
using Basecode.Data.Models;
using Basecode.Data.ViewModels;
using Basecode.Services.Interfaces;
using static Basecode.Services.Services.ErrorHandling;

namespace Basecode.Services.Services
{
    public class CharacterReferenceService : ICharacterReferenceService
    {
        private readonly ICharacterReferenceRepository _repository;
        private readonly IMapper _mapper;

        /// <summary>
        /// Initializes a new instance of the <see cref="CharacterReferenceService"/> class.
        /// </summary>
        /// <param name="repository">The character reference repository.</param>
        /// <param name="mapper">The mapper for object mapping.</param>
        public CharacterReferenceService(ICharacterReferenceRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        /// <summary>
        /// Creates a new character reference for the specified applicant.
        /// </summary>
        /// <param name="characterReference">The CharacterReferenceViewModel object containing the character reference data.</param>
        /// <param name="applicantId">The ID of the associated applicant.</param>
        /// <returns>A LogContent object representing the result of the operation.</returns>
        public LogContent Create(CharacterReferenceViewModel characterReference, int applicantId)
        {
            LogContent logContent = new LogContent();

            logContent = CheckCharacterReference(characterReference);
            if (logContent.Result == false)
            {
                var characterModel = _mapper.Map<CharacterReference>(characterReference);
                characterModel.ApplicantId = applicantId;
                _repository.CreateReference(characterModel);
            }

            return logContent;
        }

        /// <summary>
        /// Retrieves a list of character references associated with the specified applicant ID.
        /// </summary>
        /// <param name="applicantId">The unique identifier of the applicant whose character references are to be retrieved.</param>
        /// <returns>A list of CharacterReference objects representing the character references related to the specified applicant.</returns>
        public List<CharacterReference> GetReferencesByApplicantId(int applicantId) 
        {
            var data = _repository.GetAll()
                .Where(m => m.ApplicantId == applicantId)
                .Select(m => new CharacterReference
                {
                    Id = m.Id,
                    Name = m.Name,
                    Address = m.Address,
                    Email = m.Email,
                    ApplicantId = m.ApplicantId,
                }).ToList();

            return data;
        }

        /// <summary>
        /// Gets the character reference by identifier.
        /// </summary>
        /// <param name="characterReferenceId">The character reference identifier.</param>
        /// <returns></returns>
        public CharacterReference GetCharacterReferenceById(int characterReferenceId)
        {
            return _repository.GetCharacterReferenceById(characterReferenceId) ?? throw new Exception("No data found");
        }

        /// <summary>
        /// Gets the character reference applicant identifier.
        /// </summary>
        /// <param name="characterReferenceId">The character reference identifier.</param>
        /// <returns></returns>
        public int GetApplicantIdByCharacterReferenceId(int characterReferenceId)
        {
            return _repository.GetAll().Where(m => m.Id == characterReferenceId).Select(m => m.ApplicantId).SingleOrDefault();
        }
    }
}
