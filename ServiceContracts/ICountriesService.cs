using ServiceContracts.DTO;

namespace ServiceContracts
{
	/// <summary>
	/// Represents business logic for manipulating Country entity
	/// </summary>
	public interface ICountriesService
	{
		/// <summary>
		/// Add a country object to a list of countries
		/// </summary>
		/// <param name="countryAddRequest">Country object to add</param>
		/// <returns>Returns the country object after adding it (including newly generated country id)</returns>
		Task<CountryResponse> AddCountry(CountryAddRequest? countryAddRequest);
		
		/// <summary>
		/// Returns all countries from the list
		/// </summary>
		/// <returns>All countries from the list as List of CountryResponse</returns>
		Task<List<CountryResponse>> GetAllCountries();

		/// <summary>
		/// Returns a country object based on the given country id
		/// </summary>
		/// <param name="id">CountryID (guid) to search</param>
		/// <returns>Matching country as CountryRespond object</returns>
		Task<CountryResponse?> GetCountryById(Guid? id);
	}
}