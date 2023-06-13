using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
using ViewModelBaseLib.VM;
using System.Collections.ObjectModel;
using ItemViewModels.Patient;
using System.Windows.Input;
using PatientRep.ViewModelBase.Commands;
using DBController.Enums;
using ItemViewModels;

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

        ObservableCollection<PatientVM> m_patients;

        string m_SearchKey;

        DateTime m_start;

        DateTime m_end;

        SearchMode m_sMode;

        #region Input Visibility

        Visibility m_TetxBoxVisibility;

        Visibility m_DatePickerVisibility;

        int m_Count;

        #endregion

        #endregion

        #region Properties

        #region Input Visibility

        public Visibility TextBoxVisibility { get=> m_TetxBoxVisibility; 
            set=> Set(ref m_TetxBoxVisibility, value, nameof(TextBoxVisibility)); }

        public Visibility DatePickerVisibility 
        { get=> m_DatePickerVisibility; 
            set=> Set(ref m_DatePickerVisibility, value, nameof(DatePickerVisibility)); }

        #endregion

        public ObservableCollection<PatientVM> Patients 
        {
            get=> m_patients;
            
            set=> Set(ref m_patients, value, nameof(Patients));
        }

        public string SearchKey 
        {
            get=> m_SearchKey;
            
            set=> Set(ref m_SearchKey, value, nameof(SearchKey)); 
        }

        public SearchMode SearchModeProp 
        {
            get=> m_sMode;

            set
            {
                Set(ref m_sMode, value, nameof(SearchModeProp));

                switch (SearchModeProp)
                {
                    case SearchMode.По_Прізвищу:
                        
                    case SearchMode.По_Імені:
                        
                    case SearchMode.По_батькові:
                        
                    case SearchMode.По_Номеру_Направлення:

                        TextBoxVisibility = Visibility.Visible;

                        DatePickerVisibility = Visibility.Hidden;

                        break;
                    case SearchMode.По_Даті_Формування:

                        TextBoxVisibility = Visibility.Hidden;

                        DatePickerVisibility = Visibility.Visible;

                        break;
                    default:
                        break;
                }
            }
                    
        }

        public DateTime Start { get=> m_start; set=> Set(ref m_start, value, nameof(Start)); }

        public DateTime End { get=> m_end; set=> Set(ref m_end, value, nameof(End)); }

        public int Count { get=> m_Count; set=> Set(ref m_Count, value, nameof(Count)); }

        #endregion


        #region Commands

        public ICommand OnSearchButtonPressed { get;}

        public ICommand OnClearFieldsButtonPressed { get; }

        #endregion

        #region IDataErrorInfo

        public override string this[string columnName]
        {
            get
            {
                string error = String.Empty;

                switch (columnName)
                {
                    case nameof(SearchKey):

                        switch (SearchModeProp)
                        {
                            case SearchMode.По_Прізвищу:                                

                            case SearchMode.По_Імені:
                                                               
                            case SearchMode.По_батькові:

                                m_ValidationArray[0] = Validation.ValidateText(SearchKey.ToString(), Validation.Restricted, out error);

                                break;
                            case SearchMode.По_Номеру_Направлення:

                                m_ValidationArray[0] = Validation.ValidateCode(SearchKey.ToString(), out error);

                                break;
                            case SearchMode.По_Даті_Формування:

                                m_ValidationArray[0] = true;

                                break;
                           
                        }

                        break;                    
                }

                return error;
            }
        }

        #endregion

        #region Ctor
        public MainWindowViewModel(Window window)
        {
            #region InitFields

            m_patients = new ObservableCollection<PatientVM>();

            m_start = DateTime.Now;

            m_end = DateTime.Now;

            m_sMode = SearchMode.По_Прізвищу;

            m_TetxBoxVisibility = Visibility.Visible;

            m_DatePickerVisibility = Visibility.Hidden;

            m_SearchKey = String.Empty;

            m_ValidationArray = new bool[1];

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

            MessageBox.Show($"{str}", "MedInfoCollectorBot", MessageBoxButton.OK, MessageBoxImage.Information);

            #endregion

            #region Init Commands

            OnSearchButtonPressed = new LambdaCommand
                (
                    OnSearchButtonPressedExecute,
                    CanOnSearchButtonPressedExecute
                );

            OnClearFieldsButtonPressed = new LambdaCommand
                (
                    OnClearFieldsButtonPressedExecute,
                    CanOnClearFieldsButtonPressedExecute
                );

            #endregion

            m_colBot.Start();
                         
        }


        #region Methods

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

                Patients.Add(new PatientVM(Guid.NewGuid(), snl[0], snl[1],
                    snl[2], code, String.Empty,
                    PatientStatus.Не_Погашено,
                    new DateTime(), DateTime.Now,
                    String.Empty, null));
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


        public void StopBot()
        {
            MessageBox.Show($"Зупиняю роботу!", "MedInfoCollectorBot", MessageBoxButton.OK, MessageBoxImage.Information);

            m_colBot.Stop();
        }

        #region On Search Button Pressed

        private bool CanOnSearchButtonPressedExecute(object p)
        {
            return CheckValidArray(0, 1);
        }

        private void OnSearchButtonPressedExecute(object p)
        {
            m_patients.Clear();

            Task<IEnumerable<Patient>> t = new Task<IEnumerable<Patient>>(() =>
            {
                IEnumerable<Patient> res = null;

                switch (SearchModeProp)
                {
                    case SearchMode.По_Прізвищу:
                        
                    case SearchMode.По_Імені:
                       
                    case SearchMode.По_батькові:
                      
                    case SearchMode.По_Номеру_Направлення:

                        res = m_dbController.GetItems(SearchKey, SearchModeProp);

                        break;
                    case SearchMode.По_Даті_Формування:

                        DateTime[] temp = new DateTime[2] { Start, End };

                        res = m_dbController.GetItems(temp, SearchModeProp);

                        break;
                    
                }

                return res;                
            });

            t.ContinueWith((t) => {

                var r = t.Result;

                foreach (var item in r)
                {
                    var addInfo = new List<string>();

                    foreach (var Info in item.AdditionalPatientInfo)
                    {
                        addInfo.Add(Info.Value);
                    }

                    m_window.Dispatcher.Invoke(() =>
                    {
                        Patients.Add(new PatientVM(item.Id, item.Surename, item.Name,
                        item.Lastname, item.Code, item.Diagnosis, item.Status, item.RegisterDate,
                        item.InvestigationDate, item.Center, addInfo));
                    });                    
                }

                Count = Patients.Count;

            });
        }

        #endregion

        #region On Clear Fields Button Pressed

        private bool CanOnClearFieldsButtonPressedExecute(object p) => true;

        private void OnClearFieldsButtonPressedExecute(object p)
        {
            SearchKey = String.Empty;
        }
        #endregion

        #endregion
    }
}
