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
using Panacea.Multilinguality;

namespace Panacea.Modules.Billing
{
    class BillingManager : IBillingManager
    {
        private readonly PanaceaServices _core;
        private readonly List<string> _paidPlugins;
        private readonly List<string> _freePlugins;
        private List<Service> _services;
        IBillingSettings _settings;

        public BillingManager(PanaceaServices core, List<string> freePlugins, List<string> paidPlugins)
        {
            _core = core;
            _paidPlugins = paidPlugins;
            _freePlugins = freePlugins;
        }

        public async Task<Service> GetOrRequestServiceForItemAsync(string text, string pluginName, ServerItem item)
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
                var service = await DoWhileBusy(async () =>
                {
                    var service3 = await GetServiceForItemAsync(pluginName, item);
                    return service3;
                });
                if (service != null) return service;
                var res2 = await ui.ShowPopup(new RequestServicePopupViewModel(_core.UserService.User, text));
                if (res2 == RequestServiceResult.BuyService && await ShowBuyServiceWizard())
                {
                    service = await DoWhileBusy(async () =>
                    {
                        var service3 = await GetServiceForItemAsync(pluginName, item);
                        return service3;
                    });
                    if (service != null) return service;
                }

            }
            return null;
        }

        public async Task<Service> GetServiceForQuantityAsync(string pluginName)
        {
            if (_core.UserService.User.Id == null) return null;

            await GetActiveUserServicesAsync();

            var pluginServices = GetPluginActiveServices(pluginName);
            if (pluginServices == null) return null;

            //check if a service has unlimited qunatity and not for a specific category
            if (pluginServices.Any(s => (s.Quantity == -1) && (s.Categories == null || s.Categories.Count == 0)))
            {
                return
                    pluginServices.First(
                        s => (s.Quantity == -1) && (s.Categories == null || s.Categories.Count == 0));
            }
            if (pluginServices.Any(s => (s.Quantity > 0 && !s.IsQuantityPerDay)))
                return pluginServices.First(s => s.Quantity > 0);

            return null;
        }

        public Task<Service> GetOrRequestServiceForQuantityAsync(string text, string pluginName)
        {
            return Task.FromResult(default(Service));
        }

        public async Task<Service> GetOrRequestServiceAsync(string text, string pluginName)
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

        public async Task<Service> GetServiceAsync(string pluginName)
        {
            if (_core.UserService.User.Id == null) return null;
            await GetActiveUserServicesAsync();
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

        public async Task<Service> GetServiceForItemAsync(string pluginName, ServerItem item)
        {
            if (_core.UserService.User.Id == null) return null;
            await GetActiveUserServicesAsync();
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
                if (ui.CurrentPage == vm)
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


        ConsumeItemPopupViewModel _consumeItemPopup;
        public async Task<bool> RequestServiceAndConsumeItemAsync(string text, string pluginName, ServerItem item)
        {
            if (IsPluginFree(pluginName))
            {
                return true;
            }
            if (_core.UserService.User.Id == null)
            {
                if (await GetOrRequestServiceForItemAsync(text, pluginName, item) != null)
                {
                    return await RequestServiceAndConsumeItemAsync(text, pluginName, item);
                }
                return false;
            }
            try
            {
                var s = await GetServiceForItemAsync(pluginName, item);
                if (s != null)
                {
                    if (s.Quantity == -1 || s.ServiceHistory.Any(c => c.Item == item.Id))
                    {
                        return true;
                    }
                    else
                    {
                        if (_core.TryGetUiManager(out IUiManager ui))
                        {
                            ui.HidePopup(_consumeItemPopup);
                            _consumeItemPopup = new ConsumeItemPopupViewModel(item, s);

                            if (await ui.ShowPopup(_consumeItemPopup, null, PopupType.Information))
                            {
                                return await ConsumeItemAsync(pluginName, item);
                            }
                        }
                        return false;
                    }
                }
                else
                {
                    if (await GetOrRequestServiceForItemAsync(text, pluginName, item) != null)
                    {
                        return await RequestServiceAndConsumeItemAsync(text, pluginName, item);
                    }
                    return false;
                }
            }
            catch
            {
                new Translator("Billing").Translate(
                    "Unable to access service information due to network issues. Please try again later.");
                return false;
            }
        }

        public async Task<bool> ConsumeQuantityAsync(string pluginName, int quantity)
        {
            return await DoWhileBusy(async () =>
            {
                try
                {
                    var s = await GetServiceForQuantityAsync(pluginName);
                    if (s != null)
                    {
                        if (s.Quantity > 0)
                        {
                            var response = await _core.HttpClient.GetObjectAsync<object>(
                                "billing/update_user_service/",
                                postData: new List<KeyValuePair<string, string>>()
                                {
                                    new KeyValuePair<string, string>("quantity", quantity.ToString()),
                                    new KeyValuePair<string, string>("serviceID", s.Id),
                                    new KeyValuePair<string, string>("itemID", "")
                                }
                            );
                            return response.Success;
                        }
                        return s.Quantity == -1;
                    }
                }
                catch
                {

                }
                return false;
            });

        }

        public async Task<bool> ConsumeItemAsync(string pluginName, ServerItem item)
        {
            return await DoWhileBusy(async () =>
            {
                try
                {
                    var s = await GetServiceForItemAsync(pluginName, item);
                    if (s == null) return false;

                    var response = await _core.HttpClient.GetObjectAsync<object>(
                        "billing/update_user_service/",
                        postData: new List<KeyValuePair<string, string>>()
                        {
                                    new KeyValuePair<string, string>("quantity", "1"),
                                    new KeyValuePair<string, string>("serviceID", s.Id),
                                    new KeyValuePair<string, string>("itemID", item.Id)
                        }
                    );
                    return response.Success;
                }
                catch
                {

                }
                return false;
            });
        }

        public bool IsPluginFree(string plugnName)
        {
            return _freePlugins.Any(p => p == plugnName || p == "*") || !_paidPlugins.Any(p => p == plugnName);
        }

        protected Task UpdateUserServicesWithUiAsync()
        {
            return DoWhileBusy(async () =>
            {
                await GetActiveUserServicesAsync();
            });

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
            if (_core.UserService.User.Id != null)
            {
                var servicesResponse = await _core.HttpClient.GetObjectAsync<List<Service>>("billing/get_user_services/", allowCache: false);
                if (servicesResponse.Success)
                {
                    _services = servicesResponse.Result;
                }
                if (_services == null) return null;
                //find services for the specified plugin
                var services = _services.Where(s => s.RestDuration > 0 || s.Duration == -1.0).ToList();
                return services;
            }
            else
            {
                _services = new List<Service>();
                return new List<Service>();
            }

        }

        public async Task<List<Ledger>> GetUserPurchaseHistoryAsync()
        {
            if (_core.UserService.User.Id == null) new List<Ledger>();
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

        public List<Service> GetActiveUserServices()
        {
            if (_core.UserService.User.Id == null) return new List<Service>();
            return _services.Where(s => s.RestDuration > 0 || s.Duration == -1.0).ToList();
        }

        public IServiceMonitor CreateServiceMonitor()
        {
            return new ServiceMonitor(_core, this);
        }
    }
}
