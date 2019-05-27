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
    [View(typeof(RequestServicePopup))]
    class RequestServicePopupViewModel: PopupViewModelBase<bool>
    {
    }
}
