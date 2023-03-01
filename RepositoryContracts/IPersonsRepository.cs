using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Entities;

namespace RepositoryContracts
{
	/// <summary>
	/// Repersents data access logic for managing Person entity
	/// </summary>
	public interface IPersonsRepository
	{
		/// <summary>
		/// Adds a person object to data store
		/// </summary>
		/// <param name="person">Person object to add</param>
		/// <returns>Returns the person object after adding it to the table</returns>
		Task<Person> AddPerson(Person person);


		/// <summary>
		/// Returns all persons in the data store
		/// </summary>
		/// <returns>List of person object from table</returns>
		Task<List<Person>> GetAllPersons();


		/// <summary>
		/// Returns a person object based on the given person id 
		/// </summary>
		/// <param name="personID">PersonID (guid) to search</param>
		/// <returns>A person object or null</returns>
		Task<Person?> GetPersonByPersonID(Guid personID);


		/// <summary>
		/// Returns all person objects based on the given experssion
		/// </summary>
		/// <param name="predicate">LINQ expression to check</param>
		/// <returns>All matching persons with given condition</returns>
		Task<List<Person>> GetFillteredPersons(Expression<Func<Person, bool>> predicate);


		/// <summary>
		/// Deletes a person object based on the person id
		/// </summary>
		/// <param name="personID">Person ID (guid) to search</param>
		/// <returns>Returns true, if the deletion is successful; otherwise false</returns>
		Task<bool> DeletePersonByPersonID(Guid personID);


		/// <summary>
		/// Updates a person object (person name and others details) base on the given person id
		/// </summary>
		/// <param name="person">Person object to update</param>
		/// <returns>Return the updateed person object</returns>
		Task<Person> UpadatePerosn(Person person);
	}
}
