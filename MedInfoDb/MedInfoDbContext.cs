using MedInfoDb.Models.AdditionalInfo;
using MedInfoDb.Models.Patient;
using Microsoft.EntityFrameworkCore;

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

        public MedInfoDbContext()
        {

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