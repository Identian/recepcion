using System;
using System.Collections.Generic;
using Domain.Entity;
using Domain.Entity.Store_Procedure;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.DataBase;

public partial class ProvidersContext : DbContext
{
    public ProvidersContext()
    {
    }

    public ProvidersContext(DbContextOptions<ProvidersContext> options)
        : base(options)
    {
    }

    public virtual DbSet<SpPaginateProviders> SpPaginateProviders { get; set; }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<SpPaginateProviders>().HasKey(s => s.IdReceptor);
    }
}
