using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MusicStore.NetFramework.WebApp.Models
{
	public class Album
	{
        public int AlbumId { get; set; }
        public string AlbumTitle { get; set; }
        public string ArtistName { get; set; }
        public DateTime DateAdded { get; set; }
        public string CoverFileName { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public bool IsBestseller { get; set; }
        public bool IsHidden { get; set; }

        public int GenreId { get; set; }
        public Genre Genre { get; set; }
    }
}