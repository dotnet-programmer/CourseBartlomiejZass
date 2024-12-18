﻿using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace MusicStore.NetFramework.WebApp.Models
{
	public class Genre
	{
		public int GenreId { get; set; }
		public string Name { get; set; }
		public string Description { get; set; }
		public string IconFileName { get; set; }

		public ICollection<Album> Albums { get; set; } = new Collection<Album>();
	}
}