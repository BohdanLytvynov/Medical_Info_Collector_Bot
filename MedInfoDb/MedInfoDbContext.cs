using MedInfoDb.Models.AdditionalInfo;
using MedInfoDb.Models.Patient;
using Microsoft.EntityFrameworkCore;
using System.Configuration;

namespace MedInfoDb
{
    public class MedInfoDbContext : DbContext
    {
        #region Properties

        #region DB Sets

        public DbSet<Patient> PatientNotes { get; set; }

        public DbSet<AdditionalInfo<string>> PatientsAddInfo { get; set; }

        #endregion

        #endregion

        #region Ctor

        public MedInfoDbContext(DbContextOptionsBuilder optbuilder) : base(optbuilder.Options)
        {

        }

        public MedInfoDbContext():base()
        {

        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(
                "Data Source=BohdanPC\\SQLEXPRESS2019;Initial Catalog=Med_Info_Tel_Bot;Integrated Security=True;TrustServerCertificate=True");

            base.OnConfiguring(optionsBuilder);
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Patient>().HasMany(x => x.AdditionalPatientInfo).WithOne(p => p.Patient)
                .HasForeignKey(fk => fk.PatientId);

            base.OnModelCreating(modelBuilder);
        }

        #endregion
    }
}