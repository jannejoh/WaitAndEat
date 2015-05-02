using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;
using System.Collections.ObjectModel;

namespace WaitAndEat.ViewModel
{
    class WaitingListViewModel
    {
        private ObservableCollection<Model.Party> waitingList = null;

        public ObservableCollection<Model.Party> WaitingList
        {
            get
            {
                if (waitingList == null)
                {
                    waitingList = new ObservableCollection<Model.Party>();
                }
                return waitingList;
            }
        }

    }
}
