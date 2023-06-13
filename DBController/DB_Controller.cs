using DBController.Enums;
using DBController.Interfaces;
using MedInfoDb;
using MedInfoDb.Models.AdditionalInfo;
using MedInfoDb.Models.Patient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBController
{
    public class DB_Controller : ICRUDController<Patient, SearchMode>
    {
        #region Fields

        MedInfoDbContext m_db;

        #endregion

        #region ctor

        public DB_Controller(MedInfoDbContext db)
        {
            m_db = db;
        }

        #endregion

        public void Add(Patient item)
        {
            m_db.Add(item);

            m_db.SaveChanges();
        }

        public void Delete(Patient item)
        {
            var p = m_db.PatientNotes.Find(item.Id);

            if (p != null)
            {
                m_db.PatientNotes.Remove(p);

                m_db.SaveChanges();
            }                        
        }

        public void Edit(Patient item, Patient newItem, bool [] editArray)
        {
            var p = m_db.PatientNotes.Find(item.Id);

            if (p!= null && editArray != null)
            {
                var dest = p.GetType();
                
                var source = newItem.GetType();

                var Sourceprops = source.GetProperties();

                var Destprops = dest.GetProperties();

                if (Destprops.Length >= editArray.Length)
                {
                    for (int i = 0; i < editArray.Length; i++)
                    {
                        if (editArray[i])
                        {
                            Destprops[i].SetValue(p, Sourceprops[i].GetValue(newItem));
                        }
                    }

                    m_db.SaveChanges();
                }
            }
        }

        public IEnumerable<Patient> GetItems(object key, SearchMode mode)
        {
            IEnumerable<Patient> res = null;

            var str = key.ToString();

            try
            {
                switch (mode)
                {
                    case SearchMode.По_Прізвищу:

                        res = (from p in m_db.PatientNotes
                               where EF.Functions.Like(p.Surename, $"%{str}%")
                               select p).Include(x => x.AdditionalPatientInfo).OrderBy(x => x.Surename);

                        break;
                    case SearchMode.По_Імені:

                        res = (from p in m_db.PatientNotes
                               where p.Name.Equals(key.ToString())
                               select p).Include(x => x.AdditionalPatientInfo).OrderBy(x => x.Surename);

                        break;
                    case SearchMode.По_батькові:

                        res = (from p in m_db.PatientNotes
                               where p.Lastname.Equals(key.ToString())
                               select p).Include(x => x.AdditionalPatientInfo).OrderBy(x => x.Surename);

                        break;
                    case SearchMode.По_Номеру_Направлення:

                        res = (from p in m_db.PatientNotes
                               where p.Code.Equals(key.ToString())
                               select p).Include(x => x.AdditionalPatientInfo);

                        break;

                    case SearchMode.По_Даті_Формування:


                        var t = key as DateTime[];

                        if (t != null)
                        {
                            res = (from p in m_db.PatientNotes
                                   where p.RegisterDate >= t[0] && p.RegisterDate <= t[1]
                                   select p).Include(x => x.AdditionalPatientInfo).OrderBy(x => x.RegisterDate);
                        }



                        break;
                    default:
                        break;
                }
            }
            catch (Exception ex)
            {

                throw;
            }
           
            return res;
        }

        public IEnumerable<Patient> GetItems()
        {
            var r = (from p in m_db.PatientNotes select p).Include(x => x.AdditionalPatientInfo);

            return r;
        }
    }
}
