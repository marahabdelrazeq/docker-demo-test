using CommonRepo.Domain.Entities.Subscriptions;
using CommonRepo.Infrastructure.Caching.Models;
using docker_demo.Models;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace CommonRepo.Persistence.Context
{
    public class ApplicationDbContext : DbContext
    {
         

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        { 
        }

        public virtual DbSet<Subscription> Subscriptions { get; set; }

        public virtual DbSet<SubscriptionsView> SubscriptionsViews { get; set; }

        public DbSet<TablesWithViews> TablesWiths { get; set; }
        public DbSet<KeyViews> KeyViews { get; set; }

        public DbSet<COLUMN_NAME> COLUMN_NAME { get; set; }

        public async Task<long> GetNextSequenceValueAsync(string sequenceName)
        {
            var parameter = new SqlParameter("@SequenceName", sequenceName);

            parameter.Direction = ParameterDirection.Input;

            var outputParameter = new SqlParameter("@NextValue", SqlDbType.BigInt);

            outputParameter.Direction = ParameterDirection.Output;

            await Database.ExecuteSqlRawAsync("EXECUTE dbo.usp_ExecuteSequence @SequenceName, @NextValue OUTPUT", parameter, outputParameter);

            return (long)outputParameter.Value;

        }
    }


}
