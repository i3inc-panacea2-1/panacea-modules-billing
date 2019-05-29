using Panacea.Controls;
using Panacea.Core;
using Panacea.Modularity.UiManager;
using Panacea.Modules.Billing.Views;
using Panacea.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Panacea.Modules.Billing.ViewModels
{
    [View(typeof(RequestServicePopup))]
    class RequestServicePopupViewModel: PopupViewModelBase<RequestServiceResult>
    {
        public RequestServicePopupViewModel(IUser user, string text)
        {
            Text = text;
            SignedIn = user.Id != null;
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

        public string Text { get; set; }

        public bool SignedIn { get; set; }

        
    }

    public enum RequestServiceResult
    {
        None,
        SignIn,
        BuyService
    }
}
