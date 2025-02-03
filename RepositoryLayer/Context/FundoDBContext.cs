using Microsoft.EntityFrameworkCore;
using RepositoryLayer.Entity;
using System;
using System.Collections.Generic;
using System.Text;

namespace RepositoryLayer.Context
{
    public class FundoDBContext : DbContext
    {
        public FundoDBContext(DbContextOptions options):base(options) { }

        public DbSet<User> User { get; set; }

        public DbSet<NotesEntity> Notestable { get; set; }

        public DbSet<LabelEntity> Labeltable { get; set; }

        public DbSet<CollaboratorEntity> Collaboratortable { get; set; }
    }
}
