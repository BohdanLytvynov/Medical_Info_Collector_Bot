using ItemViewModels.AddInfoViewModel;
using MedInfoDb.Models.Patient.Enums;
using PatientRep.ViewModelBase.Commands;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using ViewModelBaseLib.VM;

namespace ItemViewModels.Patient
{
    public class PatientVM : ViewModelBaseClass
    {
        #region Events

        public event Action<PatientVM> OnSaveChangesButtonPressed;

        public event Action<PatientVM> OnRemoveButtonPressed;

        #endregion

        #region Fields

        int m_SelectedAddInfoIndex;

        ObservableCollection<AdditionalInfoViewModel> m_addInfoVM;

        bool m_IsRemoved;

        string m_name;

        string m_surename;

        string m_lastname;

        string m_code;

        int m_number;

        string m_diagnosis;

        PatientStatus m_status;

        DateTime m_InvestDate;

        DateTime m_RegisterDate;

        public bool IsInvestDateSet { get; set; }

        string m_Center;

        #endregion

        #region Properties

        public int SelectedAddInfoIndex
        {
            get => m_SelectedAddInfoIndex;
            set => Set(ref m_SelectedAddInfoIndex, value, nameof(SelectedAddInfoIndex));
        }

        public ObservableCollection<AdditionalInfoViewModel> AddInfoVMCollection
        {
            get => m_addInfoVM;
            set => m_addInfoVM = value;
        }


        public bool IsRemoved
        {
            get => m_IsRemoved;

            set => Set(ref m_IsRemoved, value, nameof(IsRemoved));
        }

        public Guid Id { get; }

        public int Number { get => m_number; set => Set(ref m_number, value, nameof(Number)); }

        public string Surename { get => m_surename; set => Set(ref m_surename, value, nameof(Surename)); }

        public string Name { get => m_name; set => Set(ref m_name, value, nameof(Name)); }

        public string Lastname { get => m_lastname; set => Set(ref m_lastname, value, nameof(Lastname)); }

        public string Code { get => m_code; set => Set(ref m_code, value, nameof(Code)); }

        public string Diagnosis { get => m_diagnosis; set => Set(ref m_diagnosis, value, nameof(Diagnosis)); }

        public PatientStatus Status { get => m_status; set => Set(ref m_status, value, nameof(Status)); }

        public DateTime InvestigationDate
        {
            get => m_InvestDate;
            set => Set(ref m_InvestDate, value, nameof(InvestigationDate));
        }

        public DateTime RegisterDate
        {
            get => m_RegisterDate;
            set => Set(ref m_RegisterDate, value, nameof(RegisterDate));
        }

        public string Center { get => m_Center; set => Set(ref m_Center, value, nameof(Center)); }

        #endregion

        #region IDataErrorInfo

        public override string this[string columnName]
        {
            get
            {
                string error = string.Empty;

                switch (columnName)
                {
                    case nameof(Surename):

                        m_ValidationArray[0] = Validation.ValidateText(Surename, Validation.Restricted, out error);

                        break;

                    case nameof(Name):

                        m_ValidationArray[1] = Validation.ValidateText(Name, Validation.Restricted, out error);

                        break;

                    case nameof(Lastname):

                        m_ValidationArray[2] = Validation.ValidateText(Lastname, Validation.Restricted, out error);

                        break;

                    case nameof(Code):

                        m_ValidationArray[3] = Validation.ValidateCode(Code, out error);

                        break;
                    case nameof(RegisterDate):

                        m_ValidationArray[4] = Validation.ValidateDateTime(RegisterDate, out error);

                        break;
                    case nameof(InvestigationDate):

                        m_ValidationArray[5] = Validation.ValidateDateTime(InvestigationDate, out error);

                        break;

                    case nameof(Center):

                        m_ValidationArray[6] = Validation.ValidateNumber(Center, out error, true);

                        break;
                }

                return error;
            }
        }

        #endregion

        #region Commands

        public ICommand SaveChangesButtonPressed { get; }

        public ICommand RemoveButtonPressed { get; }

        public ICommand OnAddAdditionalInfoButtonPressed { get; }

        public ICommand OnRemoveAdditionalInfoButtonPressed { get; }

        public ICommand OnSetInvestigationDatePressed { get; set; }

        #endregion

        #region ctor

        public PatientVM(Guid id, string surename, string name, string lastname, string code, string diagnosis, PatientStatus status,
            DateTime registerDate, DateTime InvestigationDate, string center, List<string> addInfoCol)
        {
            #region Init Fields

            m_SelectedAddInfoIndex = -1;
            IsInvestDateSet = false;
            m_surename = surename;
            m_name = name;
            m_lastname = lastname;
            m_code = code;
            m_diagnosis = diagnosis;
            m_status = status;
            Id = id;

            m_ValidationArray = new bool[7];

            m_RegisterDate = registerDate;

            m_InvestDate = InvestigationDate;

            m_IsRemoved = false;

            m_addInfoVM = new ObservableCollection<AdditionalInfoViewModel>();

            if (this.InvestigationDate != default)
            {
                IsInvestDateSet = true;
            }

            if (center == null)
            {
                m_Center = String.Empty;
            }
            else
            {
                m_Center = center;
            }

            if (addInfoCol != null)
            {
                if (addInfoCol.Count > 0)
                {
                    foreach (var item in addInfoCol)
                    {
                        m_addInfoVM.Add(new AdditionalInfoViewModel(m_addInfoVM.Count + 1, item));
                    }
                }
            }

            #endregion

            #region Init Commands

            SaveChangesButtonPressed = new LambdaCommand(
                OnEditButtonPressedExecute,
                CanEditButtonPressedExecute
                );

            RemoveButtonPressed = new LambdaCommand(
                OnRemoveButtonPressedExecute,
                CanRemoveButtonPressedExecute
                );

            OnAddAdditionalInfoButtonPressed = new LambdaCommand(
                OnAddAdditionalInfoButtonPressedExecute,
                CanOnAddAdditionalInfoButtonPressedExecute
                );

            OnRemoveAdditionalInfoButtonPressed = new LambdaCommand(
                OnRemoveAdditionalInfoButtonPressedExecute,
                CanOnRemoveAdditionalInfoButtonPressedExecute
                );

            OnSetInvestigationDatePressed = new LambdaCommand(
                OnSetInvestigationDatePressedExecute,
                CanOnSetInvestigationDatePressedExecute
                );


            #endregion


        }

        public PatientVM()
        {

        }

        #endregion

        #region Methods

        #region On edit button pressed

        private bool CanEditButtonPressedExecute(object p) => CheckValidArray(0, m_ValidationArray.Length);

        private void OnEditButtonPressedExecute(object p)
        {
            OnSaveChangesButtonPressed.Invoke(this);
        }

        #endregion

        #region On Remove Button Pressed

        private bool CanRemoveButtonPressedExecute(object p) => true;

        private void OnRemoveButtonPressedExecute(object p)
        {
            OnRemoveButtonPressed.Invoke(this);
        }

        #endregion

        #region On Add Additional Info Button Pressed

        private bool CanOnAddAdditionalInfoButtonPressedExecute(object p) => true;

        private void OnAddAdditionalInfoButtonPressedExecute(object p)
        {
            AddInfoVMCollection.Add(new AdditionalInfoViewModel(AddInfoVMCollection.Count + 1, "    "));
        }

        #endregion

        #region On Remove Additional Info Button Pressed

        private bool CanOnRemoveAdditionalInfoButtonPressedExecute(object p) => SelectedAddInfoIndex >= 0;

        private void OnRemoveAdditionalInfoButtonPressedExecute(object p)
        {
            AddInfoVMCollection.RemoveAt(SelectedAddInfoIndex);
        }

        #endregion

        #region On Set Investigation date Pressed

        private bool CanOnSetInvestigationDatePressedExecute(object p) => !IsInvestDateSet;

        private void OnSetInvestigationDatePressedExecute(object p)
        {
            InvestigationDate = DateTime.Now;

            IsInvestDateSet = true;
        }

        #endregion

       

        #endregion
    }
}
