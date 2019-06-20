using Panacea.Controls;
using Panacea.Models;
using Panacea.Modularity.Billing;
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
        public Service Service { get; }
        public ServerItem Item { get; }
        public ConsumeItemPopupViewModel(ServerItem item, Service service)
        {
            Service = service;
            Item = item;
            YesCommand = new RelayCommand(args => taskCompletionSource.SetResult(true));
            NoCommand = new RelayCommand(args => taskCompletionSource.SetResult(false));
        }

        public RelayCommand YesCommand { get; }

        public RelayCommand NoCommand { get; }
    }
}
