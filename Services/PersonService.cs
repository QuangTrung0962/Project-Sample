using System;
using Entities;
using ServiceContracts.DTO;
using ServiceContracts;
using System.ComponentModel.DataAnnotations;
using Services.Helpers;
using ServiceContracts.Enums;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using RepositoryContracts;
namespace Services
{
	public class PersonService : IPersonServices
	{
		//Tuong trung cho 1 database cua 1 list cac Person
		private readonly IPersonsRepository _personsRepository;

		private readonly ICountriesService _countriesService;
		//Contructor
		public PersonService(IPersonsRepository personsRepository, ICountriesService countriesService)
		{
			_personsRepository = personsRepository;
			_countriesService = countriesService;

		}

		public async Task<PersonResponse> AddPerson(PersonAddRequest? personAddRequest)
		{
			//Check if personAddRequest is not null
			if (personAddRequest == null)
			{
				throw new ArgumentNullException(nameof(personAddRequest));
			}

			//Model validations
			ValidationHelper.ModelValidation(personAddRequest);


			//Convert personAddRequest into Person type
			Person person = personAddRequest.ToPerson();
			//Adding PersonID
			person.PersonId = Guid.NewGuid();
			
			//Adding new person into person list(database) 
			await _personsRepository.AddPerson(person);

			return person.ToPersonResponse();
		}

		public async Task<List<PersonResponse>> GetAllPersons()
		{
			//Select * from Perons == _db.Persons.ToList()
			//var persons = await _personsRepository.Persons.Include("Country").ToListAsync();

			//return persons
			//	.Select(temp => temp.ToPersonResponse()).ToList();

			//Use sql procedure
			//return _db.sp_GetAllPersons()
			//	Select(temp => temp.ToPersonResponse()).ToList();

			List<Person> persons = await _personsRepository.GetAllPersons();
			return persons.Select(tmp => tmp.ToPersonResponse()).ToList();  
		}

		public async Task<PersonResponse?> GetPersonByPersonID(Guid? personID)
		{
			if (personID == null)
				return null;

			Person? person = await _personsRepository.GetPersonByPersonID(personID.Value);

			if (person == null)
				return null;

			return person.ToPersonResponse();
		}

		public async Task<List<PersonResponse>> GetFilteredPersons(string SearchBy, string? searchString)
		{
			//Có 1 cách tốt hơn cách này được viết rõ trong github sau này có thể lấy dungg
			List<PersonResponse> allPersons = await GetAllPersons();
			List<PersonResponse> matchingPersons = allPersons;

			//If seatchString == null, return all person
			if (string.IsNullOrEmpty(searchString) || string.IsNullOrEmpty(SearchBy))
				return matchingPersons;

			switch (SearchBy)
			{
				case nameof(PersonResponse.PersonName):
					matchingPersons = allPersons.Where(temp => (!string.IsNullOrEmpty(temp.PersonName)? temp.PersonName.Contains(searchString, StringComparison.OrdinalIgnoreCase) : true)).ToList();
					break;

				case nameof(PersonResponse.Email):
					matchingPersons = allPersons.Where(temp => (!string.IsNullOrEmpty(temp.Email) ? temp.Email.Contains(searchString, StringComparison.OrdinalIgnoreCase) : true)).ToList();
					break;

				case nameof(PersonResponse.DateOfBirth):
					matchingPersons = allPersons.Where(temp => (temp.DateOfBirth != null) ? temp.DateOfBirth.Value.ToString("dd MMMM yyyy").Contains(searchString, StringComparison.OrdinalIgnoreCase) : true).ToList();
					break;

				case nameof(PersonResponse.Gender):
					matchingPersons = allPersons.Where(temp => (temp.Gender != null) ? temp.Gender.ToString().Contains(searchString, StringComparison.OrdinalIgnoreCase) : true).ToList();
					break;

				case nameof(PersonResponse.CountryID):
					matchingPersons = allPersons.Where(temp => (temp.Country != null) ? temp.Country.ToString().Contains(searchString, StringComparison.OrdinalIgnoreCase) : true).ToList();
					break;

				case nameof(PersonResponse.Address):
					matchingPersons = allPersons.Where(temp => (temp.Address != null) ? temp.Address.ToString().Contains(searchString, StringComparison.OrdinalIgnoreCase) : true).ToList();
					break;

				default:
					matchingPersons = allPersons;
					break;
			}

			return matchingPersons;
		}

		public async Task<List<PersonResponse>> GetSortedPersons(List<PersonResponse> allPersons, string sortBy, SortOrderOptions sortOrder)
		{
			if (sortBy == null)
				return allPersons;

			List<PersonResponse> sortedPersons = (sortBy, sortOrder) switch
			{
				//For PersonName
				(nameof(Person.PersonName), SortOrderOptions.ASC) => allPersons.OrderBy(temp => temp.PersonName, StringComparer.OrdinalIgnoreCase).ToList(),

				(nameof(Person.PersonName), SortOrderOptions.DESC) => allPersons.OrderByDescending(temp => temp.PersonName, StringComparer.OrdinalIgnoreCase).ToList(),

				//For Email
				(nameof(Person.Email), SortOrderOptions.ASC) => allPersons.OrderBy(temp => temp.Email, StringComparer.OrdinalIgnoreCase).ToList(),

				(nameof(Person.Email), SortOrderOptions.DESC) => allPersons.OrderByDescending(temp => temp.Email, StringComparer.OrdinalIgnoreCase).ToList(),

				// For DateOfBirth
				(nameof(Person.DateOfBirth), SortOrderOptions.ASC) => allPersons.OrderBy(temp => temp.DateOfBirth).ToList(),

				(nameof(Person.DateOfBirth), SortOrderOptions.DESC) => allPersons.OrderByDescending(temp => temp.DateOfBirth).ToList(),

				// For Age
				(nameof(PersonResponse.Age), SortOrderOptions.ASC) => allPersons.OrderBy(temp => temp.Age).ToList(),

				(nameof(PersonResponse.Age), SortOrderOptions.DESC) => allPersons.OrderByDescending(temp => temp.Age).ToList(),

				//For Gender
				(nameof(Person.Gender), SortOrderOptions.ASC) => allPersons.OrderBy(temp => temp.Gender, StringComparer.OrdinalIgnoreCase).ToList(),

				(nameof(Person.Gender), SortOrderOptions.DESC) => allPersons.OrderByDescending(temp => temp.Gender, StringComparer.OrdinalIgnoreCase).ToList(),

				//For Country
				(nameof(PersonResponse.Country), SortOrderOptions.ASC) => allPersons.OrderBy(temp => temp.Country, StringComparer.OrdinalIgnoreCase).ToList(),

				(nameof(PersonResponse.Country), SortOrderOptions.DESC) => allPersons.OrderByDescending(temp => temp.Country, StringComparer.OrdinalIgnoreCase).ToList(),

				//For Address
				(nameof(PersonResponse.Address), SortOrderOptions.ASC) => allPersons.OrderBy(temp => temp.Address, StringComparer.OrdinalIgnoreCase).ToList(),

				(nameof(PersonResponse.Address), SortOrderOptions.DESC) => allPersons.OrderByDescending(temp => temp.Address, StringComparer.OrdinalIgnoreCase).ToList(),

				//For ReciveNewsLetters
				(nameof(PersonResponse.ReciveNewsLetters), SortOrderOptions.ASC) => allPersons.OrderBy(temp => temp.ReciveNewsLetters).ToList(),

				(nameof(PersonResponse.ReciveNewsLetters), SortOrderOptions.DESC) => allPersons.OrderByDescending(temp => temp.ReciveNewsLetters).ToList(),

				//"_" tương đương với default case
				_ => allPersons
			} ;

			return sortedPersons;
		}

		public async Task<PersonResponse> UpdatePerson(PersonUpdateRequest? personUpdateRequest)
		{
			if (personUpdateRequest == null)
				throw new ArgumentNullException(nameof(personUpdateRequest));

			////Validation
			ValidationHelper.ModelValidation(personUpdateRequest);

			//Get maching person object to update
			Person? machingPerson = await _personsRepository.GetPersonByPersonID(personUpdateRequest.PersonID);

			if (machingPerson == null)
			{
				throw new ArgumentException("Given person id dosen't exist abc");
			}

			////Update all details
			await _personsRepository.UpadatePerosn(personUpdateRequest.ToPerson());

			return machingPerson.ToPersonResponse();
		}


		public async Task<bool> DeletePerson(Guid? personID)
		{
			if (personID == null)
			{
				throw new ArgumentNullException(nameof(personID));
			}

			Person? person = await _personsRepository.GetPersonByPersonID(personID.Value);

			if (person == null)
			{
				return false;
			}

			await _personsRepository.DeletePersonByPersonID(person.PersonId);

			return true;
		}
	}
}
