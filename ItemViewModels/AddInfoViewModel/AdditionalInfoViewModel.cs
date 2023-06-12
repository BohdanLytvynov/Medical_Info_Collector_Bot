using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ViewModelBaseLib.VM;

namespace ItemViewModels.AddInfoViewModel
{
    public class AdditionalInfoViewModel : ViewModelBaseClass
    {
        #region fields

        int m_ShowNumber;

        string m_Value;

        #endregion

        #region Properties

        public int ShowNumber
        {
            get => m_ShowNumber;
            set => Set<int>(ref m_ShowNumber, value, nameof(ShowNumber));
        }

        public string Value
        {
            get => m_Value;
            set => Set<string>(ref m_Value, value, nameof(Value));
        }

        #endregion

        #region Ctor

        public AdditionalInfoViewModel(int number, string value)
        {
            m_ShowNumber = number;

            m_Value = value;
        }

        #endregion
    }
}
