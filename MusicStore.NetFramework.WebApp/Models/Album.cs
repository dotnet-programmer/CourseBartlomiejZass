﻿using System;
using System.ComponentModel.DataAnnotations;

namespace MusicStore.NetFramework.WebApp.Models
{
	public class Album
	{
		public int AlbumId { get; set; }

		[Required]
		[Display(Name = "Tytuł")]
		public string AlbumTitle { get; set; }

		[Required]
		[Display(Name = "Artysta")]
		[DataType(DataType.Text)]
		public string ArtistName { get; set; }

		public DateTime DateAdded { get; set; }
		public string CoverFileName { get; set; }
		public string Description { get; set; }
		public decimal Price { get; set; }
		public bool IsBestseller { get; set; }
		public bool IsHidden { get; set; }

		public int GenreId { get; set; }
		public virtual Genre Genre { get; set; }
	}
}