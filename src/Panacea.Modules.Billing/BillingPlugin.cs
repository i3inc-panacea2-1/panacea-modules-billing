using Panacea.Core;
using Panacea.Models;
using Panacea.Modularity.Billing;
using Panacea.Modularity.UiManager;
using Panacea.Modules.Billing.Models;
using Panacea.Modules.Billing.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Panacea.Modules.Billing
{
    public class BillingPlugin : IBillingPlugin
    {
        private readonly PanaceaServices _core;
        private IBillingSettings _settings;
        BillingManager _manager;
        List<string> _freePlugins;
        List<string> _nonFreePlugins;
        SettingsControlViewModel _settingsControl;

        [PanaceaInject("AllFree", "Makes all plugins free", "AllFree=1")]
        protected bool AllFree { get; set; }

        public BillingPlugin(PanaceaServices core)
        {
            _core = core;
        }

        #region IPlugin
        public Task BeginInit()
        {
            _core.UserService.UserLoggedIn += UserService_UserLoggedIn;
            _core.UserService.UserLoggedOut += UserService_UserLoggedOut;
            return Task.CompletedTask;
        }

        public async Task EndInit()
        {
            var fetched = false;
            while (!fetched)
            {
                try
                {
                    var res = await _core.HttpClient.GetObjectAsync<GetFreePluginsResponse>("get_versions/", allowCache: false);
                    fetched = true;
                    if (res.Success)
                    {
                        _freePlugins = res.Result.FreePlugins;
                        _nonFreePlugins = res.Result.PaidPlugins;
                        if (AllFree)
                        {
                            _freePlugins.Add("*");
                        }
                    }
                    else
                    {
                        throw new Exception(res.Error);
                    }
                }
                catch (Exception ex)
                {
                    _core.Logger.Error(this, ex.Message);
                    await Task.Delay(5000);
                }
            }
           
            if (_core.TryGetUiManager(out IUiManager ui))
            {
                _settingsControl = new SettingsControlViewModel(GetBillingManager());
                ui.AddSettingsControl(_settingsControl);
            }
        }

        public Task Shutdown()
        {
            return Task.CompletedTask;
        }

        public void Dispose()
        {

        }
        #endregion

        #region ICallable
        public void Call()
        {

        }
        #endregion


        public IBillingManager GetBillingManager()
        {
            return _manager = _manager ?? new BillingManager(_core, _freePlugins, _nonFreePlugins);
        }

        private async Task UserService_UserLoggedOut(IUser user)
        {
            var man = GetBillingManager() as BillingManager;
            await man.GetActiveUserServicesAsync();
        }

        private async Task UserService_UserLoggedIn(IUser user)
        {
            var man = GetBillingManager() as BillingManager;
            await man.GetActiveUserServicesAsync();
        }
    }
}
