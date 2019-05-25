using Panacea.Controls;
using Panacea.Modularity.UiManager;
using Panacea.Modules.Billing.Views;
using Panacea.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Panacea.Modules.Billing.ViewModels
{
    [View(typeof(ConsumeItemPopup))]
    class ConsumeItemPopupViewModel:PopupViewModelBase<bool>
    {
        public ConsumeItemPopupViewModel()
        {
            YesCommand = new RelayCommand(args => taskCompletionSource.SetResult(true));
            NoCommand = new RelayCommand(args => taskCompletionSource.SetResult(false));
        }

        public RelayCommand YesCommand { get; }

        public RelayCommand NoCommand { get; }
    }
}
