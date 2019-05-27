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
    [View(typeof(RequestServicePopup))]
    class RequestServicePopupViewModel: PopupViewModelBase<RequestServiceResult>
    {
        public RequestServicePopupViewModel()
        {
            SignInCommand = new RelayCommand(args =>
            {
                taskCompletionSource.SetResult(RequestServiceResult.SignIn);
            });
            BuyServiceCommand = new RelayCommand(args =>
            {
                taskCompletionSource.SetResult(RequestServiceResult.BuyService);
            });
        }

        public RelayCommand SignInCommand { get; }
        public RelayCommand BuyServiceCommand { get; }
    }

    public enum RequestServiceResult
    {
        SignIn,
        BuyService
    }
}
