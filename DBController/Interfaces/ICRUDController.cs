using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBController.Interfaces
{
    public interface ICRUDController<TItem, TItemSearchMode>
    {
        void Add(TItem item);

        void Delete(TItem item);

        void Edit(TItem item, TItem newItem, bool[] editArray);

        IEnumerable<TItem> GetItems();

        IEnumerable<TItem> GetItems(object key, TItemSearchMode sm);
    }
}
