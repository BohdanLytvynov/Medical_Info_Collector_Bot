using MedInfoDb.Models.Patient;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace MedInfoDb.Models.AdditionalInfo
{
    [Table("PatientInfo")]
    public class AdditionalInfo<T>
    {
        #region Nav Properties

        public Guid PatientId { get; set; }

        public Patient.Patient Patient { get; set; }

        #endregion

        #region Properties
        [Key]
        public Guid Id { get; set; }
       
        public T Value
        {
            get;
            set;
        }

        #endregion

        #region Ctor

        public AdditionalInfo(Guid id, T value)
        {
            Id = id;

            Value = value;
        }

        public AdditionalInfo()
        {

        }

        #endregion
    }
}
