using System;
using System.Collections.Generic;
using Entities;
using ServiceContracts;
using ServiceContracts.DTO;
using ServiceContracts.Enums;
using Services;
using Xunit;
using Xunit.Abstractions;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using EntityFrameworkCoreMock;
using AutoFixture;
using FluentAssertions;

namespace CRUDTests
{
	public class PersonsServiceTest
	{
		private readonly IPersonServices _personServices;
		private readonly ICountriesService _countriesService;
		private readonly ITestOutputHelper _testOutputHelper;
		private readonly IFixture _fixture;
		//Contructor
		public PersonsServiceTest(ITestOutputHelper testOutputHelper)
		{
			var dbContextOption = new DbContextOptionsBuilder<ApplicationDbContext>().Options;

			//Mock the dbContext		
			DbContextMock<ApplicationDbContext> dbContextMock = new DbContextMock<ApplicationDbContext>(dbContextOption);
			var initialCountry = new List<Country>() { };
			var initialPerson = new List<Person>() { };
			//Mock the DbSet
			var dbCountrysSetMock = dbContextMock.CreateDbSetMock(temp => temp.Countries, initialCountry);
			var dbPesonSetMock = dbContextMock.CreateDbSetMock(temp => temp.Persons, initialPerson);

			//Create service instance with mocked DbContext
			ApplicationDbContext dbContext = dbContextMock.Object;
			_countriesService = new CountriesService(null);
			
			_personServices = new PersonService(null, _countriesService);

			_testOutputHelper = testOutputHelper;


			_fixture = new Fixture();
		}
		
		#region AddPerson
		//When we supply null value as PersonAddRequest, it show thro ArgumentNullException
		[Fact]
		public async Task AddPerson_NullPerson ()
		{
			//Arrage
			PersonAddRequest? request = null;

			//Assert
			//await Assert.ThrowsAsync<ArgumentNullException>(async () =>
			//{
			//	//Act
			//	await _personServices.AddPerson(request);
			//});
			Func<Task> action = async () =>
			{
				await _personServices.AddPerson(request);
			};
			await action.Should().ThrowAsync<ArgumentNullException>();

		}

		//When we supply null value as PersonName, it show throw ArgumentException
		[Fact]
		public async Task AddPerson_NullPersonName()
		{
			//Arrage
			PersonAddRequest request = _fixture.Build<PersonAddRequest>()
				.With(temp => temp.PersonName, null as string).Create();
			//Assert
			Func<Task> action = async () =>
			{
				await _personServices.AddPerson(request);
			};
			await action.Should().ThrowAsync<ArgumentException>();
		}

		//When we supply proper person details, it should insert person into the person list, it should return an object of PersonRespond, which includes with the newly generated person id
		[Fact]
		public async Task AddPerson_PorperPersonDetails()
		{
			//Arrage
			PersonAddRequest request = _fixture.Build<PersonAddRequest>()
				.With(temp => temp.Email, "sample@gmail.com")
				.Create();


			//Act
			PersonResponse person_reponse_from_add = await _personServices.AddPerson(request);
			List<PersonResponse> person_list = await _personServices.GetAllPersons();


			//Assert
			//Assert.True(person_reponse_from_add.PersonId != Guid.Empty);
			person_reponse_from_add.Should().NotBe(Guid.Empty);

			//Assert.Contains(person_reponse_from_add, person_list);
			person_list.Should().Contain(person_reponse_from_add);

		}
		#endregion

		#region GetPersonByPersonID

		//If we supply null as PersonID, it should return null as PersonRespond
		[Fact]
		public async Task GetPersonByPersonID_NullPersonID()
		{
			//Arrange
			Guid? personID = null;

			//Act
			PersonResponse? person_response_from_get = await _personServices.GetPersonByPersonID(personID);

			//Assert
			//Assert.Null(person_response_from_get);
			person_response_from_get.Should().BeNull();

		}

		//If we supply a valid person id, it should return the valid person details as PersonResponse object
		[Fact]
		public async Task GetPersonByPersonID_ProperPersonID ()
		{
			//Arange
			PersonAddRequest person_request = _fixture.Build<PersonAddRequest>()
				.With(temp => temp.Email, "sample@gmail.com")
				.Create();

			//Act
			PersonResponse person_response_from_add = await _personServices.AddPerson(person_request);

			PersonResponse? person_response_from_get = await _personServices.GetPersonByPersonID(person_response_from_add.PersonId);

			//Assert
			//Assert.Equal(person_response_from_add, person_response_from_get);
			person_response_from_get.Should().Be(person_response_from_add);
		}
		#endregion

		#region GetAllPersons

		//The GetAllPersons() should return empty list by default 
		[Fact]
		public async Task GetAllPersons_EmptyList()
		{
			//Act
			List<PersonResponse> persons_from_get = await _personServices.GetAllPersons();

			//Assert
			Assert.Empty(persons_from_get);
			persons_from_get.Should().BeEmpty();
		}

		//We should recive the same person that were added
		[Fact]
		public async Task GetAllPersons_AddFewPersons() 
		{
			List<PersonAddRequest> persons_list = new List<PersonAddRequest>()
			{
				_fixture.Build<PersonAddRequest>()
				.With(temp => temp.Email,"sample@gmail.com")
				.Create(),
				_fixture.Build<PersonAddRequest>()
				.With(temp => temp.Email,"sample2@gmail.com")
				.Create(),
			};

			List<PersonResponse> peron_respond_list_from_add = new List<PersonResponse>();
			foreach (PersonAddRequest person in persons_list)
			{
				PersonResponse person_response = await _personServices.AddPerson(person);
				peron_respond_list_from_add.Add(person_response);
			}

			//print peron_respond_list_from_add 
			_testOutputHelper.WriteLine("Expected");
			foreach (PersonResponse person in peron_respond_list_from_add)
			{
				_testOutputHelper.WriteLine(person.ToString());
			}


			//Act
			List<PersonResponse> peron_respond_list_from_get = await _personServices.GetAllPersons();

			//print peron_respond_list_from_get
		
			_testOutputHelper.WriteLine("Actual");
			foreach (PersonResponse person in peron_respond_list_from_get)
			{
				_testOutputHelper.WriteLine(person.ToString());
			}

			//Asset
			foreach (PersonResponse peron_respond_from_add in  peron_respond_list_from_add)
			{
				Assert.Contains(peron_respond_from_add, peron_respond_list_from_get);
			}
			peron_respond_list_from_get.Should().BeEquivalentTo(peron_respond_list_from_add);
		}

		#endregion

		#region GetFilteredPerson

		//If the search text is empty and search by is "PersonName", it should return all persons
		[Fact]
		public async Task GetFilteredPersons_EmptySearchText()
		{
			List<PersonAddRequest> persons_list = new List<PersonAddRequest>()
			{
				_fixture.Build<PersonAddRequest>()
				.With(temp => temp.Email,"sample@gmail.com")
				.Create(),
				_fixture.Build<PersonAddRequest>()
				.With(temp => temp.Email,"sample2@gmail.com")
				.Create()
			};

			List<PersonResponse> peron_respond_list_from_add = new List<PersonResponse>();
			foreach (PersonAddRequest person in persons_list)
			{
				PersonResponse person_response = await _personServices.AddPerson(person);
				peron_respond_list_from_add.Add(person_response);
			}

			//print peron_respond_list_from_add 
			_testOutputHelper.WriteLine("Expected");
			foreach (PersonResponse person in peron_respond_list_from_add)
			{
				_testOutputHelper.WriteLine(person.ToString());
			}


			//Act
			List<PersonResponse> peron_respond_list_from_search = await _personServices.GetFilteredPersons(nameof(Person.PersonName), "");

			//print peron_respond_list_from_search

			_testOutputHelper.WriteLine("Actual");
			foreach (PersonResponse person in peron_respond_list_from_search)
			{
				_testOutputHelper.WriteLine(person.ToString());
			}

			//Asset
			peron_respond_list_from_search.Should().BeEquivalentTo(peron_respond_list_from_add);
		}

		//First we will add few persons, and then we will search based on person name with same string. It should return the matching persons
		[Fact]
		public async Task GetFilteredPersons_SearchByPersonName()
		{
			List<PersonAddRequest> persons_list = new List<PersonAddRequest>()
			{
				_fixture.Build<PersonAddRequest>()
				.With(temp => temp.Email,"sample@gmail.com")
				.With(temp => temp.PersonName, "Trung")
				.Create(),
				_fixture.Build<PersonAddRequest>()
				.With(temp => temp.Email,"sample2@gmail.com")
				.With(temp => temp.PersonName, "Nguyen")
				.Create(),
			};

			List<PersonResponse> peron_respond_list_from_add = new List<PersonResponse>();
			foreach (PersonAddRequest person in persons_list)
			{
				PersonResponse person_response = await _personServices.AddPerson(person);
				peron_respond_list_from_add.Add(person_response);
			}

			//print peron_respond_list_from_add 
			_testOutputHelper.WriteLine("All person in the list");
			foreach (PersonResponse person in peron_respond_list_from_add)
			{
				_testOutputHelper.WriteLine(person.ToString());
			}


			//Act
			List<PersonResponse> peron_respond_list_from_search = await _personServices.GetFilteredPersons(nameof(Person.PersonName), "ng");

			//print peron_respond_list_from_search

			_testOutputHelper.WriteLine("Actual");
			foreach (PersonResponse person in peron_respond_list_from_search)
			{
				_testOutputHelper.WriteLine(person.ToString());
			}

			//Asset
			//foreach (PersonResponse peron_respond_from_add in peron_respond_list_from_add)
			//{
			//	if (peron_respond_from_add.PersonName != null)
			//	{
			//		//Check xem peron_respond_from_add co PersonName chứa kí tự "ng" không(kể cả có viết hoa "NG" hay "Ng")
			//		if (peron_respond_from_add.PersonName.Contains("ng", StringComparison.OrdinalIgnoreCase))
			//		{
			//			Assert.Contains(peron_respond_from_add, peron_respond_list_from_search);
			//		}
			//	}
			//}

			peron_respond_list_from_search.Should().OnlyContain(tmp => tmp.PersonName.Contains("ng", StringComparison.OrdinalIgnoreCase));
		}
		#endregion 

		#region GetSortedPersons

		//When we sort based in PersonName is DESC, it should rerturn list in descending on PersonName
		[Fact]
		public async Task GetSortedPersonss()
		{
			List<PersonAddRequest> persons_list = new List<PersonAddRequest>()
			{
				_fixture.Build<PersonAddRequest>()
				.With(temp => temp.Email,"sample@gmail.com")
				.With(temp => temp.PersonName, "Trung")
				.Create(),
				_fixture.Build<PersonAddRequest>()
				.With(temp => temp.Email,"sample2@gmail.com")
				.With(temp => temp.PersonName, "Nguyen")
				.Create(),
			};

			List<PersonResponse> peron_respond_list_from_add = new List<PersonResponse>();
			foreach (PersonAddRequest person in persons_list)
			{
				PersonResponse person_response = await _personServices.AddPerson(person);
				peron_respond_list_from_add.Add(person_response);
			}

			//print peron_respond_list_from_add 
			_testOutputHelper.WriteLine("All person in the list");
			foreach (PersonResponse person in peron_respond_list_from_add)
			{
				_testOutputHelper.WriteLine(person.ToString());
			}
			List<PersonResponse> allPersons = await _personServices.GetAllPersons();

			//Act
			//Sort list PersonResponse by PersonName
			List<PersonResponse> peron_respond_list_from_sort = await _personServices.GetSortedPersons(allPersons, nameof(Person.PersonName), SortOrderOptions.DESC);

			//print peron_respond_list_from_sort
			_testOutputHelper.WriteLine("Actual");
			foreach (PersonResponse person in peron_respond_list_from_sort)
			{
				_testOutputHelper.WriteLine(person.ToString());
			}

			//Sort the added list persons
			//peron_respond_list_from_add = peron_respond_list_from_add.OrderByDescending(x => x.PersonName).ToList();

			//Asset
			peron_respond_list_from_sort.Should().BeInDescendingOrder(temp => temp.PersonName);
		}

		#endregion

		#region UpdatePerson

		//When we supply null as PersonUpdateRequest it should throw ArgumentNullException
		[Fact]
		public async Task UpdatePerson_NullPerson()
		{
			PersonUpdateRequest? personUpdateRequest = null; 

			//Act
			Func<Task> action = (async() => 
			{
				await _personServices.UpdatePerson(personUpdateRequest); 
			});
			//Assert
			await action.Should().ThrowAsync<ArgumentNullException>();
		}

		//When we supply invalid person id it should throw ArgumentException
		[Fact]
		public async Task UpdatePerson_InvalidPersonID()
		{
			PersonUpdateRequest? personUpdateRequest = _fixture.Create<PersonUpdateRequest>();

			//Assert
			//Act
			Func<Task> action = (async () =>
			{
				await _personServices.UpdatePerson(personUpdateRequest);
			});
			//Assert
			await action.Should().ThrowAsync<ArgumentException>();
		}
		//First, add a new person and try to update the person name and email
		[Fact]
		public async Task UpdatePerson_PersonFullDetailsUpdation()
		{
			//Arrange
			PersonAddRequest personAddRequest = _fixture.Build<PersonAddRequest>()
				.With(tmp => tmp.PersonName, "trung")
				.With(tmp => tmp.Email, "trung@gmail.com")
				.With(tmp => tmp.Address, "")
				.Create();				
			
			PersonResponse person_response_from_add = await _personServices.AddPerson(personAddRequest);

			PersonUpdateRequest personUpdateRequest = person_response_from_add.ToPersonUpdateRequest();
			personUpdateRequest.PersonName = "Phu";
			personUpdateRequest.Email = "abc@gmail.com";

			//Act
			PersonResponse person_response_from_uppdate = await _personServices.UpdatePerson(personUpdateRequest);

			PersonResponse? person_response_from_get = await _personServices.GetPersonByPersonID(person_response_from_uppdate.PersonId);

			//Assert
			person_response_from_uppdate.Should().Be(person_response_from_get);
		}


		//When PersonName is null it should throw ArgumentException
		[Fact]
		public async Task UpdatePerson_PersonNameIsNull()
		{
			PersonUpdateRequest? personUpdateRequest = new PersonUpdateRequest() { PersonName = null };

			//Assert
			Func<Task> action = (async () =>
			{
				await _personServices.UpdatePerson(personUpdateRequest);

			});
			await action.Should().ThrowAsync<ArgumentException>();
		}
		#endregion

		#region DeletePerson

		//If supply an valid PerosnID it should return true
		[Fact]
		public async Task DeletePerson_ValidPersonID ()
		{
			//Arrange

			PersonAddRequest personAddRequest = _fixture.Build<PersonAddRequest>()
				.With(tmp => tmp.Email, "sample@gmail.com")
				.Create();

			PersonResponse person_response_from_add = await _personServices.AddPerson(personAddRequest);

			//Act
			bool isDeleted = await _personServices.DeletePerson(person_response_from_add.PersonId);

			//Assert 
			//Assert.True(isDeleted);
			isDeleted.Should().BeTrue();
		}

		//If supply an invalid PerosnID it should return true
		[Fact]
		public async Task DeletePerson_InValidPersonID()
		{
			//Arrange
			
			//Act
			bool isDeleted = await _personServices.DeletePerson(Guid.NewGuid());

			//Assert 
			//Assert.False(isDeleted);
			isDeleted.Should().BeFalse();
		}

		#endregion

	}
}
