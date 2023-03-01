using Entities;
using Microsoft.EntityFrameworkCore;
using ServiceContracts;
using ServiceContracts.DTO;
using RepositoryContracts;
namespace Services
{
	public class CountriesService : ICountriesService
	{
		private readonly ICountriesRepository _countriesRepository;
		
		//Contructor
		public CountriesService(ICountriesRepository countriesRepository)
		{
			_countriesRepository = countriesRepository;	
		}

		public async Task<CountryResponse> AddCountry(CountryAddRequest? countryAddRequest)
		{
			//Validation: countryAddRequest can't be null
			if (countryAddRequest == null)
			{
				throw new ArgumentNullException(nameof(countryAddRequest));
			}

			//Validation: CountryName can't be null
			if (countryAddRequest.CountryName == null)
			{
				throw new ArgumentException(nameof(countryAddRequest.CountryName));
			}

			//Validation: CountryName can't be duplicate
			if (await _countriesRepository.GetCountryByCountryName(countryAddRequest.CountryName) != null)
			{
				throw new ArgumentException("Given country name alaredy exists");
			}


			//Convert object from CountryAddRequest to Country Type
			Country country = countryAddRequest.ToCountry();

			//Generate CountryId
			country.CountryId = Guid.NewGuid();

			//Add country object into Countries
			await _countriesRepository.AddCountry(country);
			
			return country.ToCountryResponse();
		}

		public async Task<List<CountryResponse>> GetAllCountries()
		{
			List<Country> countries = await _countriesRepository.GetAllCountries();
			return countries.Select(temp => temp.ToCountryResponse()).ToList();	   
		}

		public async Task<CountryResponse?> GetCountryById(Guid? countryID)
		{
			if (countryID == null)
				return null;

			Country? country_response_from_list = await _countriesRepository.GetCountryByCountryId(countryID.Value);

			if (country_response_from_list == null)
				return null;

			return country_response_from_list.ToCountryResponse();		
		}
	}
}