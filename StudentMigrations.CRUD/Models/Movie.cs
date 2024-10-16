using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace StudentMigrations.CRUD.Models
{
	public class Movie
	{
		public int Id { get; set; }
				
		public string? Title { get; set; }
		public string? Director { get; set; }
		public int ProductionYear { get; set; }
		public string? Description { get; set; }
		
		[Display(Name ="Poster Image")]		
		public string? PosterUrl { get; set; }
	}
}
