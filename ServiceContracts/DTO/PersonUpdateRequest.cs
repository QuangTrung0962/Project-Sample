using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entities;
using ServiceContracts.Enums;

namespace ServiceContracts.DTO
{
	/// <summary>
	/// Represents the DTO class that contains the person details to update
	/// </summary>
	public class PersonUpdateRequest
	{
		[Required(ErrorMessage = "PersonID can't be blank")]
		public Guid PersonID { get; set; }

		[Required(ErrorMessage = "PersonName can't be blank")]
		public string? PersonName { get; set; }
		[Required(ErrorMessage = "Email can't be blank")]
		[EmailAddress(ErrorMessage = "Email value should be a valid email")]
		public string? Email { get; set; }
		public DateTime? DateOfBirth { get; set; }
		public GenderOptions? Gender { get; set; }
		public Guid? CountryID { get; set; }
		public string? Address { get; set; }
		public bool ReciveNewsLetters { get; set; }

		/// <summary>
		/// Convert the PersonAddRequest object to a Person object
		/// </summary>
		/// <returns>Return a person object</returns>
		public Person ToPerson()
		{
			return new Person
			{
				PersonId = PersonID,

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
