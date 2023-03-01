﻿using Entities;
using Microsoft.EntityFrameworkCore;
using RepositoryContracts;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace Repositories
{
	public class CountriesRepository : ICountriesRepository
	{
		private readonly ApplicationDbContext _db;

		public CountriesRepository(ApplicationDbContext db)
		{
			_db = db;
		}

		public async Task<Country> AddCountry(Country country)
		{
			_db.Countries.Add(country);
			await _db.SaveChangesAsync();
			return country;
		}

		public async Task<List<Country>> GetAllCountries()
		{
			return await _db.Countries.ToListAsync();
		}

		public async Task<Country?> GetCountryByCountryId(Guid countryID)
		{
			return await _db.Countries.FirstOrDefaultAsync(tmp => tmp.CountryId == countryID);
		}

		public async Task<Country?> GetCountryByCountryName(string countryName)
		{
			return await _db.Countries.FirstOrDefaultAsync(tmp => tmp.CountryName == countryName);
		}
	}
}