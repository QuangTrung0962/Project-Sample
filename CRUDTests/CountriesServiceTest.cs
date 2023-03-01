using System;
using ServiceContracts;
using ServiceContracts.DTO;
using Services;
using Entities;
using Microsoft.EntityFrameworkCore;
using Moq;
using EntityFrameworkCoreMock;
using AutoFixture;

namespace CRUDTests
{
	public class CountriesServiceTest
	{
		private readonly ICountriesService _countriesService;
		private readonly IFixture _fixture;

		//Contructor
		public CountriesServiceTest()
		{
			var dbContextOption = new DbContextOptionsBuilder<ApplicationDbContext>().Options;

			//Mock the dbContext		
			DbContextMock<ApplicationDbContext> dbContextMock = new DbContextMock<ApplicationDbContext>(dbContextOption);
			var initialCountry = new List<Country>() { };

			//Mock the DbSet
			var dbCountrysSetMock = dbContextMock.CreateDbSetMock(temp => temp.Countries, initialCountry);

			//Create service instance with mocked DbContext
			ApplicationDbContext dbContext = dbContextMock.Object;

			_countriesService = new CountriesService(null);

			_fixture = new Fixture();
		}

		#region AddCountry
		//When CountryAddRequest is null, it show throw ArgumentNullException
		[Fact]
		public async Task AddCountry_NullCountry()//return Task == void
		{
			//Arrange
			CountryAddRequest? request = null;

			//Assert
			await Assert.ThrowsAsync<ArgumentNullException>(async () =>
			{
				//Act
				await _countriesService.AddCountry(request);
			});
		}


		//When the ContryName is null, it should throw ArgumentException
		[Fact]
		public async Task AddCountry_CountyNameIsNull()
		{
			//Arrange
			CountryAddRequest? request = new CountryAddRequest 
			{ 
				CountryName = null
			};

			//Assert
			await Assert.ThrowsAsync<ArgumentException>(async () =>
			{
				//Act
				await _countriesService.AddCountry(request);
			});
		}

		//When the CountryName is duplicate it should throw ArgumentException
		[Fact]
		public async Task AddCountry_DuplicateCountryName()
		{
			//Arrange
			CountryAddRequest? request1 = new CountryAddRequest
			{
				CountryName = "USA"
			};
			CountryAddRequest? request2 = new CountryAddRequest
			{
				CountryName = "USA"
			};

			//Assert
			await Assert.ThrowsAsync<ArgumentException>( async() =>
			{
				//Act
				await _countriesService.AddCountry(request1);
				await _countriesService.AddCountry(request2);
			});
		}


		//When you supply proper country name, it should insert(add) the country to the existing list of countries
		[Fact]
		public async Task AddCountry_PoperCountryDetails()
		{
			//Arrange
			CountryAddRequest? request = new CountryAddRequest
			{
				CountryName = "VIETNAM"
			};

			//Act
			CountryResponse response = await _countriesService.AddCountry(request);
			List<CountryResponse> countries_from_getAllCountries = await _countriesService.GetAllCountries();

			//Assert
			Assert.True(response.CountryId != Guid.Empty);
			//Before compare must override Equal method
			Assert.Contains(response, countries_from_getAllCountries);

		}
		#endregion

		#region GetAllCountries
		//The list of countries should be empty by default (before adding any countries)
		[Fact]
		public async Task GetAllCountries_EmptyList()
		{
			//Act
			List<CountryResponse> actual_country_response_list = await _countriesService.GetAllCountries();

			//Assert
			Assert.Empty(actual_country_response_list);
		}

		[Fact]
		public async Task GetAllCountries_AddFewCountries()
		{
			//Act 
			List<CountryAddRequest> countries_request_list = new List<CountryAddRequest>
			{
				new CountryAddRequest {CountryName = "USA"},
				new CountryAddRequest {CountryName = "Japan"},
				new CountryAddRequest {CountryName = "VietNam"}
			};

			List<CountryResponse> countries_list_from_AddCountry = new List<CountryResponse>();

			foreach (CountryAddRequest country in countries_request_list)
			{
				countries_list_from_AddCountry.Add(await _countriesService.AddCountry(country));
			}

			List<CountryResponse> actualCountryResponseList = new List<CountryResponse>();

			actualCountryResponseList = await _countriesService.GetAllCountries();


			//Read each element from countries_list_from_AddCountry
			foreach (CountryResponse countryResponse in countries_list_from_AddCountry)
			{
				//Kiểm tra xem đất nước add vào = _countriesService.AddCountry có tồn tại trong _countriesService.GetAllCountries() không;
				Assert.Contains(countryResponse, actualCountryResponseList);
			}
		}

		#endregion

		#region GetCountryByCountryId

		//If supply a null country id, it response a null CountryResponse
		[Fact]
		public async Task GetCountryByCountryID_NullCountryID()
		{
			//Arrange
			Guid? id = null;

			//Act
			CountryResponse? country_from_get_method = await _countriesService.GetCountryById(id);

			//Assert
			Assert.Null(country_from_get_method);
		}

		//If we supply a valid country id, it should return the matching country details as CountryResponse object
		[Fact]
		public async void GetCountryByID_ValidCountryID()
		{
			//Arrange
			CountryAddRequest country = new CountryAddRequest()
			{
				CountryName = "USA"
			};
			CountryResponse? country_response_add_request =	await	_countriesService.AddCountry(country);

			//Act
			CountryResponse? country_response_from_get = await _countriesService.GetCountryById(country_response_add_request.CountryId);

			//Assert
			Assert.Equal(country_response_add_request, country_response_from_get);
		}

		#endregion
	}
}
