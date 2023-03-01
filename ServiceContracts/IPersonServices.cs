﻿using System;
using ServiceContracts.DTO;
using ServiceContracts.Enums;
namespace ServiceContracts
{
	/// <summary>
	/// Represents business logic for manipulating Person entity
	/// </summary>
	public interface IPersonServices
	{
		/// <summary>
		/// Add person into list of persons
		/// </summary>
		/// <param name="personAddRequest"></param>
		/// <returns>Returns the person object after adding it (including newly generated person id)</returns>
		Task<PersonResponse> AddPerson(PersonAddRequest? personAddRequest);

		/// <summary>
		/// Returns all persons
		/// </summary>
		/// <returns>Return a list of object of PersonResponse type</returns>
		Task<List<PersonResponse>> GetAllPersons();

		/// <summary>
		/// Return the person object base on the given person id
		/// </summary>
		/// <param name="personID">Person id to search </param>
		/// <returns>Return matching person object</returns>
		Task<PersonResponse?> GetPersonByPersonID(Guid? personID);

		/// <summary>
		/// Returns all person objects that matches with the given searchj field and search string
		/// </summary>
		/// <param name="SearchBy">Search field to search</param>
		/// <param name="searchString">Search string to search</param>
		/// <returns>Returns all matching persons based on the given search field an and string</returns>
		Task<List<PersonResponse>> GetFilteredPersons(string SearchBy, string? searchString);
		

		/// <summary>
		/// Returns sorted list of persons
		/// </summary>
		/// <param name="allPersons">Represents list of person to sort</param>
		/// <param name="sortBy">Name of the porperty (key), base on which the person should be sorted</param>
		/// <param name="sortOrder"></param>
		/// <returns>Returns sorted persons as PersonResponse list</returns>
		Task<List<PersonResponse>> GetSortedPersons(List<PersonResponse> allPersons, string sortBy, SortOrderOptions sortOrder);

		/// <summary>
		/// Updates the specified person details based on the given person id
		/// </summary>
		/// <param name="personUpdateRequest">Person details to update including perosn id</param>
		/// <returns>Returns the person response object after updation</returns>
		Task<PersonResponse> UpdatePerson(PersonUpdateRequest? personUpdateRequest);

		/// <summary>
		/// Deletes a person based on the given person id
		/// </summary>
		/// <param name="personID">Person id to delete</param>
		/// <returns>Return true, if the delete is succesful; otherwise return false</returns>
		Task<bool> DeletePerson(Guid? personID);
	}
}