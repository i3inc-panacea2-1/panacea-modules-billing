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
                    await ui.ShowPopup(new RequestServicePopupViewModel(_core.UserService.User, text));
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
            if (_core.TryGetUiManager(out IUiManager ui))
            {
                if (_core.UserService.User.Id == null)
                {
                    var res = await ui.ShowPopup(new RequestServicePopupViewModel(_core.UserService.User, text));
                    if (res == RequestServiceResult.SignIn && _core.TryGetUserAccountManager(out IUserAccountManager userManager))
                    {
                        if (!await userManager.LoginAsync()) return null;
                    }
                    else if(res == RequestServiceResult.BuyService)
                    {
                        if(await ShowBuyServiceWizard())
                        {
                            var service2 = await GetServiceAsync(pluginName);
                            return service2;
                        }
                    }
                    else if (res == RequestServiceResult.None)
                    {
                        return null;
                    }
                }

                var service = await GetServiceAsync(pluginName);
                if (service != null) return service;
                var res2 = await ui.ShowPopup(new RequestServicePopupViewModel(_core.UserService.User, text));
                if (res2 == RequestServiceResult.BuyService && await ShowBuyServiceWizard())
                {
                    service = await GetServiceAsync(pluginName);
                    if (service != null) return service;
                }

            }
            return null;
        }

        internal async Task<Service> GetServiceAsync(string pluginName)
        {
            await GetUserServicesAsync();
            var pluginServices = GetPluginActiveServices(pluginName);
            if (pluginServices != null)
            {
                pluginServices = pluginServices.OrderBy(s => s.Duration).ThenBy(x => x.RestDuration).ToList();
                if (pluginServices.Any(s => s.Duration != -1))
                {
                    var limitedServices = pluginServices.Where(s => s.Duration != -1).ToList();
                    var service = FilterServices(limitedServices);
                    if (service != null) return service;
                }
            
                pluginServices = pluginServices.Where(s => s.Duration == -1).ToList();
                var serv = FilterServices(pluginServices);
                if (serv != null) return serv;
            }
            return null;
        }

        internal Service FilterServices(List<Service> services)
        {
            var unlimitedService = services.FirstOrDefault(HasUnlimitedService);
            if (unlimitedService != null)
            {
                return unlimitedService;
            }
            var limitedPerDay = services.FirstOrDefault(HasLimitedPerDayService);
            if (limitedPerDay != null)
            {
                return limitedPerDay;
            }
            var limited = services.FirstOrDefault(HasLimitedService);
            if (limited != null)
            {
                return limited;
            }
            return null;
        }

        internal bool HasUnlimitedService(Service s) => s.Quantity == -1;

        internal bool HasLimitedPerDayService(Service s) => s.IsQuantityPerDay && s.ServiceHistory != null &&
                    s.ServiceHistory.Count(h => h.Timestamp.Date == DateTime.Now.Date) < s.Quantity;

        internal bool HasLimitedService(Service s) => s.Quantity > 0 && !s.IsQuantityPerDay;


        internal async Task<bool> ShowBuyServiceWizard()
        {
            if (_core.TryGetUiManager(out IUiManager ui))
            {
                var source = new TaskCompletionSource<bool>();
                ui.Navigate(new ServiceWizardViewModel(_core, source));
                var res = await source.Task;
                ui.GoBack();
                return res;
            }
            return false;
        }

        protected List<Service> GetPluginActiveServices(string pluginName)
        {
            if (_services == null) return null;
            if (_services.All(s => s.Plugin != pluginName)) return null;

            //find services for the specified plugin
            List<Service> pluginServices = _services.Where(s => s.Plugin == pluginName && (s.RestDuration > 0 || s.Duration == -1.0)).ToList();
            return pluginServices;
        }


        public bool IsPluginFree(string plugnName)
        {
            return false;
        }

        protected Task GetUserServicesAsync()
        {
            return DoWhileBusy(async () =>
            {
                var servicesResponse =
                await _core.HttpClient.GetObjectAsync<List<Service>>("billing/get_user_services/", allowCache: false);
                if (servicesResponse.Success)
                {
                    _services = servicesResponse.Result;
                }
            });

        }

        private Task<T> DoWhileBusy<T>(Func<Task<T>> act)
        {
            if (_core.TryGetUiManager(out IUiManager ui))
            {
                return ui.DoWhileBusy(act);
            }
            else
            {
                return act();
            }
        }

        private Task DoWhileBusy(Func<Task> act)
        {
            if (_core.TryGetUiManager(out IUiManager ui))
            {
                return ui.DoWhileBusy(act);
            }
            else
            {
                return act();
            }
        }
    }
}
