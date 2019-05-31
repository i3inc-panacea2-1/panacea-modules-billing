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
using Panacea.Modules.Billing.Models;

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
                    var res = await ui.ShowPopup(new RequestServicePopupViewModel(_core.UserService.User, text));
                    if (res == RequestServiceResult.SignIn && _core.TryGetUserAccountManager(out IUserAccountManager userManager))
                    {
                        if (!await userManager.LoginAsync()) return null;
                    }
                    else if (res == RequestServiceResult.BuyService)
                    {
                        if (await ShowBuyServiceWizard())
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

                var service = await GetServiceForItemAsync(pluginName, item);
                if (service != null) return service;
                var res2 = await ui.ShowPopup(new RequestServicePopupViewModel(_core.UserService.User, text));
                if (res2 == RequestServiceResult.BuyService && await ShowBuyServiceWizard())
                {
                    service = await GetServiceForItemAsync(pluginName, item);
                    if (service != null) return service;
                }

            }
            return null;
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
            await UpdateUserServicesAsync();
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

        internal async Task<Service> GetServiceForItemAsync(string pluginName, ServerItem item)
        {
            await UpdateUserServicesAsync();
            var pluginServices = GetPluginActiveServices(pluginName);
            if (pluginServices != null)
            {
                pluginServices = pluginServices.OrderBy(s => s.Duration).ThenBy(x => x.RestDuration).ToList();
                if (pluginServices.Any(s => s.Duration != -1))
                {
                    var limitedServices = pluginServices.Where(s => s.Duration != -1).ToList();
                    var service = FilterServicesForItem(limitedServices, item);
                    if (service != null) return service;
                }

                pluginServices = pluginServices.Where(s => s.Duration == -1).ToList();
                var serv = FilterServicesForItem(pluginServices, item);
                if (serv != null) return serv;
            }
            return null;
        }

        internal Service FilterServicesForItem(List<Service> services, ServerItem item)
        {
            var alreadyBought = services.FirstOrDefault(s => AlreadyBoughtItem(s, item));
            if (alreadyBought != null)
            {
                return alreadyBought;
            }
            var itemInCategory = services.FirstOrDefault(s => ItemInCategory(s, item));
            if (itemInCategory != null)
            {
                return itemInCategory;
            }
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

        internal bool AlreadyBoughtItem(Service s, ServerItem item) => s.ServiceHistory != null && s.ServiceHistory.Any(sh => sh.Item == item.Id);

        internal bool ItemInCategory(Service s, ServerItem item) =>
            s.Categories == null 
            || s.Categories.Count == 0 
            || (item.Categories != null && s.Categories.Any(c => item.Categories.Contains(c))) 
            || s.Items == null || s.Items.Count == 0 || s.Items.Contains(item.Id);

        internal bool HasUnlimitedService(Service s) => s.Quantity == -1;

        internal bool HasLimitedPerDayService(Service s) => s.IsQuantityPerDay && s.ServiceHistory != null &&
                    s.ServiceHistory.Count(h => h.Timestamp.Date == DateTime.Now.Date) < s.Quantity;

        internal bool HasLimitedService(Service s) => s.Quantity > 0 && !s.IsQuantityPerDay;


        internal async Task<bool> ShowBuyServiceWizard()
        {
            if (_core.TryGetUiManager(out IUiManager ui))
            {
                var source = new TaskCompletionSource<bool>();
                var vm = new ServiceWizardViewModel(_core, source);
                ui.Navigate(vm);
                var res = await source.Task;
                if(ui.CurrentPage == vm)
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

        protected Task UpdateUserServicesAsync()
        {
            return DoWhileBusy(async () =>
            {
                await GetActiveUserServicesAsync();
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

        IBillingSettings _settings;
        public async Task<IBillingSettings> GetSettingsAsync()
        {
            if (_settings != null) return _settings;
            var resp = await _core.HttpClient.GetObjectAsync<GetCurrencySettingsResponse>("billing/get_currency_settings/");
            if (resp.Success)
            {
                _settings = resp.Result;
                return _settings;
            }
            throw new Exception(resp.Error);
        }

        public async Task<List<Service>> GetActiveUserServicesAsync()
        {
            var servicesResponse = await _core.HttpClient.GetObjectAsync<List<Service>>("billing/get_user_services/", allowCache: false);
            if (servicesResponse.Success)
            {
                _services = servicesResponse.Result;
            }
            if (_services == null) return null;
            //find services for the specified plugin
            var services = _services.Where(s =>s.RestDuration > 0 || s.Duration == -1.0).ToList();
            return services;
        }

        public async Task<List<Ledger>> GetUserPurchaseHistoryAsync()
        {
            var response = await _core.HttpClient.GetObjectAsync<List<Ledger>>("billing/get_payment_history/");
            if (response.Success)
            {
                var ret = response.Result;
                foreach (var a in ret)
                {
                    if (a.PackageItem == null)
                    {
                        a.PackageItem = new Package()
                        {
                            Name = string.Join(", ", a.Services.Select(s => s.Name).ToList())
                        };
                    }
                }
                return ret.OrderByDescending(a => a.Timestamp).ToList();
            }
            else
            {
                throw new Exception(response.Error);
            }
        }

        public async void NavigateToBuyServiceWizard()
        {
            await ShowBuyServiceWizard();
        }
    }
}
