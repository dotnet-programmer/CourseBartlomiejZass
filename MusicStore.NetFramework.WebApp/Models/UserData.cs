﻿using System.ComponentModel.DataAnnotations;

namespace MusicStore.NetFramework.WebApp.Models
{
	public class UserData
	{
		public string FirstName { get; set; }
		public string LastName { get; set; }
		public string Address { get; set; }
		public string CodeAndCity { get; set; }

		[RegularExpression(@"(\+\d{2})*[\d\s-]+", ErrorMessage = "Błędny format numeru telefonu.")]
		public string PhoneNumber { get; set; }

		[EmailAddress(ErrorMessage = "Błędny format adresu e-mail.")]
		public string Email { get; set; }
	}
}