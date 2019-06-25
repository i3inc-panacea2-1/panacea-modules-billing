using Panacea.Controls;
using Panacea.Modularity.Billing;
using Panacea.Modularity.UiManager;
using Panacea.Modules.Billing.Views;
using Panacea.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Panacea.Modules.Billing.ViewModels
{
    [View(typeof(SettingsControl))]
    class SettingsControlViewModel: SettingsControlViewModelBase
    {
        public SettingsControlViewModel(IBillingManager bill)
        {
            BuyServiceCommand = new RelayCommand(args =>
            {
                OnClose();
                bill.NavigateToBuyServiceWizard();
            });
        }
        public ICommand BuyServiceCommand { get; }
    }
}
