using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBController.Enums
{
    public enum SearchMode : byte
    {
        По_Прізвищу = 0, По_Імені, По_батькові, По_Номеру_Направлення, По_Даті_Формування
    }
}
