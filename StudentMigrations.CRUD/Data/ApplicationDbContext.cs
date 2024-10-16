﻿using Microsoft.EntityFrameworkCore;
using StudentMigrations.CRUD.Models;

namespace StudentMigrations.CRUD.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {

        }

        public DbSet<Student> Students { get; set; }
        public DbSet<Movie> Movies { get; set; }
    }
}
