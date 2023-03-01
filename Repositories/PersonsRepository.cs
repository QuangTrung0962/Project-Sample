using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Entities;
using RepositoryContracts;
using Microsoft.EntityFrameworkCore;
namespace Repositories
{
	public class PersonsRepository : IPersonsRepository
	{
		private readonly ApplicationDbContext _db;

		public PersonsRepository(ApplicationDbContext db)
		{
			_db = db;
		}

		public async Task<Person> AddPerson(Person person)
		{
			_db.Persons.Add(person);
			await _db.SaveChangesAsync();
			return person;
		}

		public async Task<bool> DeletePersonByPersonID(Guid personID)
		{
			_db.Persons.RemoveRange(_db.Persons.Where(tmp => tmp.PersonId == personID));
			int rowsDelete = await _db.SaveChangesAsync();

			return rowsDelete > 0;
		}

		public async Task<List<Person>> GetAllPersons()
		{
			return await _db.Persons.Include("Country").ToListAsync();
		}

		public async Task<List<Person>> GetFillteredPersons(Expression<Func<Person, bool>> predicate)
		{
			return await _db.Persons.Include("Country").Where(predicate).ToListAsync();
		}

		public async Task<Person?> GetPersonByPersonID(Guid personID)
		{
			return await _db.Persons.Include("Country")
			.FirstOrDefaultAsync(temp => temp.PersonId == personID);
		}

		public async Task<Person> UpadatePerosn(Person person)
		{
			Person? matchingPerson = await _db.Persons.FirstOrDefaultAsync(tmp => tmp.PersonId == person.PersonId);

			if (matchingPerson == null) {
				return person;
			}

			matchingPerson.PersonName = person.PersonName;
			matchingPerson.Email = person.Email; matchingPerson.DateOfBirth = person.DateOfBirth; matchingPerson.Address = person.Address; 
			matchingPerson.Gender = person.Gender;
			matchingPerson.ReciveNewsLetters = person.ReciveNewsLetters;
			matchingPerson.CountryID = person.CountryID;
			
			int countUpdate = await _db.SaveChangesAsync();

			return matchingPerson;
		}
	}
}
