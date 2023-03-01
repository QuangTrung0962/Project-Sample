using System;
using System.Collections.Generic;
using Entities;
using ServiceContracts.Enums;
using System.ComponentModel.DataAnnotations;

namespace ServiceContracts.DTO
{
	/// <summary>
	/// Acts as a DTO for adding a new person
	/// </summary>
	public class PersonAddRequest
	{
		[Required (ErrorMessage = "PersonName can't be blank")]
		public string? PersonName { get; set; }

		[Required(ErrorMessage = "Email can't be blank")]
		[EmailAddress(ErrorMessage = "Email value should be a valid email")]
		public string? Email { get; set; }
		public DateTime? DateOfBirth { get; set; }
		public GenderOptions? Gender { get; set; }

		[Required(ErrorMessage = "Country can't be blank")]
		public Guid? CountryID { get; set; }
		public string? Address { get; set; }
		public bool ReciveNewsLetters { get; set; }

		/// <summary>
		/// Convert the PersonAddRequest object to a Person object
		/// </summary>
		/// <returns>A person object</returns>
		public Person ToPerson() 
		{
			return new Person {
				PersonName = this.PersonName,
				Email = this.Email,
				DateOfBirth = this.DateOfBirth,
				//?
				Gender = this.Gender.ToString(),
				CountryID = this.CountryID,
				Address = this.Address,
				ReciveNewsLetters = this.ReciveNewsLetters,
			};
		}
	}
}
