using System;
using System.Data.Entity;
using Models;
namespace DatabaseConnection
{
    public class EntryContexst : DbContext
    {
        public DbSet<EntryDB> Entries { get; set; }
        public EntryContexst()
            : base(@"data source=.\SQLEXPRESS; initial catalog=MikolajP158074; integrated security=SSPI")
        {

        }
    }
}
