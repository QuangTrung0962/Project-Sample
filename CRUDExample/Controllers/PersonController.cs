using Microsoft.AspNetCore.Mvc;
using ServiceContracts.DTO;
using ServiceContracts;
using Services;
using ServiceContracts.Enums;
using Rotativa.AspNetCore;
using Rotativa.AspNetCore.Options;

namespace CRUDExample.Controllers
{
	[Route("[Controller]")]
	public class PersonsController : Controller
	{
		//private fields
		private readonly IPersonServices _personService;
		private readonly ICountriesService _countriesService;

		//Contructor
		public PersonsController(IPersonServices personService, ICountriesService countriesService) 
		{
			_personService = personService;
			_countriesService = countriesService;	
		}

		[Route("/")]
		[Route("[action]")]
		public async Task<IActionResult> Index(string searchBy, string? searchString,string sortBy = nameof(PersonResponse.PersonName) , SortOrderOptions sortOrder = SortOrderOptions.ASC)
		{
			//Search
			ViewBag.Search = new Dictionary<string, string>()
			{
				{ nameof(PersonResponse.PersonName), "Person Name" },
				{ nameof(PersonResponse.Email), "Email" },
				{ nameof(PersonResponse.DateOfBirth), "Date of Birth" },
				{ nameof(PersonResponse.Gender), "Gender" },
				{ nameof(PersonResponse.CountryID), "Country" },
				{ nameof(PersonResponse.Address), "Address" },
			};

			//Get filtered pesons
			List<PersonResponse> persons = await _personService.GetFilteredPersons(searchBy, searchString);

			//Preserve the search filed in View
			ViewBag.CurrentSearchBy = searchBy;
			ViewBag.CurrentSearchString = searchString;

			//Sort
			List<PersonResponse> sortedPersons = await _personService.GetSortedPersons(persons, sortBy, sortOrder);
			ViewBag.CurrentSortBy = sortBy;
			ViewBag.CurrentSortOrder = sortOrder;


			//Strongly typed view
			return View(sortedPersons);
		}

		//Execute when the user click on "Create Person" hyperlink (while opening the create view)
		[Route("create")]
		[HttpGet]//This action method only for invoke the Create View
		public async Task<IActionResult> Create()
		{
			List<CountryResponse> countries = await _countriesService.GetAllCountries();
			ViewBag.Countries = countries;
			return View();
		}

		[HttpPost] //This action method for add new Persons into data 
		//url: persons/create
		[Route("[action]")]
		public async Task<IActionResult> Create(PersonAddRequest personAddRequest)
		{
			if (!ModelState.IsValid)
			{
				List<CountryResponse> countries = await _countriesService.GetAllCountries();
				ViewBag.Countries = countries;

				ViewBag.Errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
				return View();
			}
			//Add new person
			PersonResponse personResponse = await _personService.AddPerson(personAddRequest);
			
			//Navigate to Index() action method (it makes another get request to "persons/index"
			return RedirectToAction("Index", "Persons");
		}

		[HttpGet]
		[Route("[action]/{personID}")] //Eg: persons/edit/1
		public async Task<IActionResult> Edit(Guid personID)
		{
			PersonResponse? personResponse = await _personService.GetPersonByPersonID(personID);
			if (personResponse == null)
			{
				return RedirectToAction("Index");
			}

			PersonUpdateRequest personUpdateRequest = personResponse.ToPersonUpdateRequest();
			List<CountryResponse> countries = await _countriesService.GetAllCountries();
			ViewBag.Countries = countries;

			return View(personUpdateRequest);
		}

		[HttpPost]
		[Route("[action]/{personID}")]
		public async Task<IActionResult> Edit(PersonUpdateRequest personUpdateRequest) 
		{
			PersonResponse? personResponse = await _personService.GetPersonByPersonID(personUpdateRequest.PersonID);
			if (personResponse == null)
			{
				return RedirectToAction("Index");
			}

			if (ModelState.IsValid)
			{
				PersonResponse updatePerson = await _personService.UpdatePerson(personUpdateRequest);
				return RedirectToAction("Index");
			}
			else
			{
				List<CountryResponse> countries = await _countriesService.GetAllCountries();
				ViewBag.Countries = countries;

				ViewBag.Errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
				return View(personResponse.ToPersonUpdateRequest());
			}
		}


		[HttpGet]
		[Route("[action]/{personID}")]
		public async Task<IActionResult> Delete(Guid personID)
		{
			PersonResponse? personResponse = await _personService.GetPersonByPersonID(personID);
			if (personResponse == null)
			{
				return RedirectToAction("Index");
			}

			return View(personResponse);
		}

		[HttpPost]
		[Route("[action]/{personID}")]
		public async Task<IActionResult> Delete(PersonUpdateRequest personUpdate) 
		{ 
			PersonResponse? personResponse = await _personService.GetPersonByPersonID(personUpdate.PersonID);
			if (personResponse == null)
				return RedirectToAction("Index");

			await _personService.DeletePerson(personUpdate.PersonID);
			return RedirectToAction("Index");
		}

		[Route("PersonsPDF")]
		public async Task<IActionResult> PersonsPDF()
		{
			List<PersonResponse> persons = await _personService.GetAllPersons();

			//Return View as pdf
			return new ViewAsPdf("PersonsPDF", persons, ViewData)
			{
				PageMargins = new Margins() { Top = 1, Right = 2, Bottom = 3, Left = 4 },
				PageOrientation = Orientation.Landscape
			};

		}
	}
}
