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

namespace Panacea.Modules.Billing.ViewModels
{
    [View(typeof(UserConfirmationPopup))]
    class UserConfirmationViewModel:PopupViewModelBase<UserConfirmationResult>
    {
        public UserConfirmationViewModel(IUser user)
        {
            FirstName = user.FirstName;
            LastName = user.LastName;
            ConfirmCommand = new RelayCommand(arg =>
            {
                SetResult(UserConfirmationResult.Confirm);
            });

            NotMeCommand = new RelayCommand(arg =>
            {
                SetResult(UserConfirmationResult.NotMe);
            });
        }
        public RelayCommand ConfirmCommand { get; }

        public RelayCommand NotMeCommand { get; }

        public string FirstName { get; }

        public string LastName { get; }
    }

    public enum UserConfirmationResult
    {
        None,
        Confirm,
        NotMe
    }
}
