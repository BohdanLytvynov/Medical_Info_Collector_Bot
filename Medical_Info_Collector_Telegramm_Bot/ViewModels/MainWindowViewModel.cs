using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ViewModelBaseLib.Commands;
using ViewModelBaseLib.VM;
using Collector_Bot_Api;
using System.Windows;
using DBController;
using MedInfoDb;
using Microsoft.EntityFrameworkCore;
using System.Configuration;
using System.Collections.Specialized;
using System.Text.RegularExpressions;
using MedInfoDb.Models.Patient;
using MedInfoDb.Models.Patient.Enums;

namespace Medical_Info_Collector_Telegramm_Bot.ViewModels
{
    public class MainWindowViewModel : ViewModelBaseClass
    {
        #region Fields
        CollectorBot m_colBot;

        Window m_window;

        DB_Controller m_dbController;

        MedInfoDbContext m_medInfoDB;

        Regex m_code;
       
        #endregion

        #region Properties

        #endregion

        #region Ctor
        public MainWindowViewModel(Window window)
        {
            m_code = new Regex(@"\d{4}-\d{4}-\d{4}-\d{4}");
                        
            var settings = ConfigurationManager.GetSection("appSettings") as NameValueCollection;
                
            DbContextOptionsBuilder<MedInfoDbContext> optBuilder = new DbContextOptionsBuilder<MedInfoDbContext>();

            optBuilder.UseSqlServer(settings["ConString"]);

            m_medInfoDB = new MedInfoDbContext(optBuilder);

            m_dbController = new DB_Controller(m_medInfoDB);

            m_window = window;

            m_colBot = new CollectorBot();

            m_colBot.OnUpdateRecieve += M_colBot_OnUpdateRecieve;

            var str = m_colBot.TestBotAsync();

            MessageBox.Show($"{str}");

            m_colBot.Start();
                         
        } 

        private void M_colBot_OnUpdateRecieve(IronOcr.OcrResult obj)
        {
            bool CodeCorrect = false;

            bool SNLFound = false;
            
            string[] snl = null;

            string code = String.Empty;

            if (obj != null && obj.Lines.Length > 0)
            {                                
                var Lines = obj.Lines;
               
                foreach (var item in Lines)
                {
                    if (!CodeCorrect)
                    {
                        CodeCorrect = m_code.IsMatch(item.Text);

                        if (CodeCorrect)// El refferal was found
                        {                            
                            code = item.Text;
                        }
                    }

                    if (!SNLFound)
                    {
                        SNLFound = IsValidSNL(item.Text, out snl);
                    }                                       
                }
            }

            if (CodeCorrect && SNLFound)
            {
                var p = new Patient(Guid.NewGuid(), snl[1], snl[0],
                    snl[2], code, String.Empty,
                    PatientStatus.Не_Погашено,
                    new DateTime(), DateTime.Now,
                    String.Empty, null
                   );

                m_dbController.Add(
                    p
                    );
            }
        }
        #endregion

        private bool IsValidSNL(string txt, out string[] snl)
        {
            snl = new string[3];

            if (txt != null)
            {
                var splitArray = txt.Split(' ');

                for (int i = 0; i < 3; i++)
                {
                    string first = splitArray[i][0].ToString();

                    if (!first.Equals(first.ToUpper()))
                    {
                        return false;
                    }
                    else
                    {
                        snl[i] = splitArray[i];
                    }
                }                
            }

            return true;
        }

        #region Methods
        public void StopBot()
        {
            MessageBox.Show($"Зупиняю роботу!");

            m_colBot.Stop();
        }
        
        #endregion
    }
}
