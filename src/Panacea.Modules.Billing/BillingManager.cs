using Panacea.Core;
using Panacea.Models;
using Panacea.Modularity.Billing;
using Panacea.Modularity.UserAccount;
using Panacea.Modularity.UiManager;
using Panacea.Modules.Billing.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Panacea.Modules.Billing
{
    class BillingManager : IBillingManager
    {
        private readonly PanaceaServices _core;
        private List<Service> _services;

        public BillingManager(PanaceaServices core)
        {
            _core = core;
        }

        public async Task<Service> GetServiceForItemAsync(string text, string pluginName, ServerItem item)
        {
            if (_core.TryGetUiManager(out IUiManager ui))
            {
                if (_core.UserService.User.Id == null)
                {
                    await ui.ShowPopup(new RequestServicePopupViewModel());
                    if (_core.TryGetUserAccountManager(out IUserAccountManager userManager))
                    {
                        if (await userManager.LoginAsync())
                        {

                        }
                    }
                }

                //return ui.ShowPopup(new ConsumeItemPopupViewModel());

            }
            return null;
            //return Task.FromResult(false);
        }



        public Task<Service> GetServiceForQuantityAsync(string text, string pluginName, int quantity)
        {
            return Task.FromResult(default(Service));
        }

        public async Task<Service> GetServiceAsync(string text, string pluginName)
        {
            if (await EnsureLoggedIn())
            {
                
            }
            return null;
        }

        protected async Task<bool> EnsureLoggedIn()
        {
            if (_core.UserService.User.Id != null) return true;
            if (_core.TryGetUiManager(out IUiManager ui))
            {
                var res = await ui.ShowPopup(new RequestServicePopupViewModel());
                if (res == RequestServiceResult.SignIn)
                {
                    if (_core.TryGetUserAccountManager(out IUserAccountManager userManager))
                    {
                        return await userManager.LoginAsync();
                        
                    }
                }
                else
                {
                    ui.Navigate(new ServiceWizardViewModel(_core));
                }
            }
            return false;
        }

        public bool IsPluginFree(string plugnName)
        {
            return false;
        }

        protected async Task GetUserServicesAsync()
        {
            var servicesResponse =
                await _core.HttpClient.GetObjectAsync<List<Service>>("billing/get_user_services/", allowCache: false);
            if (servicesResponse.Success)
            {
                _services = servicesResponse.Result;
            }
        }
    }
}
