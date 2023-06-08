using MedInfoDb.Models.Patient.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MedInfoDb.Models.AdditionalInfo;

namespace MedInfoDb.Models.Patient
{
    [Table("Patients")]
    public class Patient
    {
        #region Fields

        #endregion

        #region Properties

        [Key]
        public Guid Id { get; set; }

        public string Name { get; set; }

        public string Surename { get; set; }

        public string Lastname { get; set; }

        public string Code { get; set; }

        public string Diagnosis { get; set; }

        public PatientStatus Status { get; set; }

        public DateTime InvestigationDate { get; set; }

        public DateTime RegisterDate { get; set; }

        public List<AdditionalInfo<string>> AdditionalPatientInfo { get; set; }

        public string Center { get; set; }
        
        #endregion

        #region Ctor
        public Patient()
        {

        }

        public Patient(Guid id, string name, string surename, string lastname, string code,
            string diagnosis, PatientStatus status, DateTime investDate, DateTime regDate, string Center,
            List<string> addInfo)
        {
            Id = id;

            Name = name;

            Surename = surename;

            Lastname = lastname;

            Code = code;

            AdditionalPatientInfo = new List<AdditionalInfo<string>>();

            if (addInfo != null && addInfo.Count > 0)
            {
                foreach (var item in addInfo)
                {
                    AdditionalPatientInfo.Add(new AdditionalInfo<string>());
                }
            }
        }

        #endregion

        #region Methods

        public override string ToString()
        {
            return $"{Surename} {Name} {Lastname} {Code}";
        }

       

        #endregion
    }
}
