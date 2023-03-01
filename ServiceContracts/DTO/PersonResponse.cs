using System;
using Entities;
using ServiceContracts.Enums;
namespace ServiceContracts.DTO
{
	/// <summary>
	/// DTO class is used as return type for most of Persons Service method 
	/// </summary>
	public class PersonResponse
	{
		public Guid PersonId { get; set; }
		public string? PersonName { get; set; }
		public string? Email { get; set; }
		public DateTime? DateOfBirth { get; set; }
		public string? Gender { get; set; }
		public Guid? CountryID { get; set; }
		public string? Country { get; set; }
		public string? Address { get; set; }
		public bool ReciveNewsLetters { get; set; }
		public double? Age { get; set; }

		/// <summary>
		/// Compare the current object data with the parameter object
		/// </summary>
		/// <param name="obj">The PersonResponse object to compare</param>
		/// <returns>True if current object data match with parameter object, otherwise is false</returns>
		public override bool Equals(object? obj)
		{
			if (obj == null)
				return false;
			if (obj.GetType() != typeof(PersonResponse)) 
				return false;
			PersonResponse person = (PersonResponse)obj;

			return PersonId == person.PersonId && PersonName == person.PersonName && Email == person.Email && DateOfBirth == person.DateOfBirth && Gender == person.Gender && CountryID == person.CountryID && Country == person.Country && Address == person.Address && ReciveNewsLetters == person.ReciveNewsLetters && Age == person.Age;
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		public override string ToString()
		{
			return $"PersonID: {PersonId}, PersonName: {PersonName}, Email: {Email}, DateOfBirth: {DateOfBirth?.ToString("dd MMM yyyy")}, Gender: {Gender}, CountryID: {CountryID}, Country: {Country}, Address: {Address}, ReciveNewsLetters: {ReciveNewsLetters}";
		}

		public PersonUpdateRequest ToPersonUpdateRequest()
		{
			return new PersonUpdateRequest()
			{
				PersonID = PersonId,
				PersonName = PersonName,
				Email = Email,
				DateOfBirth = DateOfBirth,
				Gender = (GenderOptions)Enum.Parse(typeof(GenderOptions), Gender, true), CountryID = CountryID, Address = Address, ReciveNewsLetters= ReciveNewsLetters,
			};
		}

	}

	public static class PersonExtensions
	{
		public static PersonResponse ToPersonResponse(this Person person)
		{
			return new PersonResponse()
			{
				PersonId = person.PersonId,		
				PersonName = person.PersonName,
				Email = person.Email,
				DateOfBirth = person.DateOfBirth,
				Gender = person.Gender,
				CountryID = person.CountryID,
				Address = person.Address,
				ReciveNewsLetters = person.ReciveNewsLetters,	
				Age = (person.DateOfBirth != null) ? Math.Round((DateTime.Now - person.DateOfBirth.Value).TotalDays / 365.25) : null,
				Country = person.Country?.CountryName
			};
		}
	}
}
