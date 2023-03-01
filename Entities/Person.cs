﻿using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Entities
{
	/// <summary>
	/// Person domail model class
	/// </summary>
	public class Person
	{
		[Key]
		public Guid PersonId { get; set; }

		[StringLength(40)]
		public string? PersonName { get; set; }

		[StringLength(60)]
		public string? Email { get; set; }

		public DateTime? DateOfBirth { get; set; }

		[StringLength(10)]
		public string? Gender { get; set; }
		 
		public Guid? CountryID { get; set; }

		[StringLength(200)]
		public string? Address { get; set; }
		public bool ReciveNewsLetters { get; set; }

		//TIN: Tax identification number
		public string? TIN { get; set; }

		[ForeignKey("CountryId")]
		public virtual Country? Country{ get; set; }
	}
}
