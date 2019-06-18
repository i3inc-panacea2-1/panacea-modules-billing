using Panacea.Core;
using Panacea.Models;
using Panacea.Modularity.Billing;
using Panacea.Modules.Billing.Models;
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
        BillingManager _manager;
        List<string> _freePlugins;
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
            var res = await _core.HttpClient.GetObjectAsync<GetFreePluginsResponse>("get_versions/", allowCache: false);
            if (res.Success)
            {
                _freePlugins = res.Result.FreePlugins;
            }
            else
            {
                throw new Exception(res.Error);
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
            return _manager = _manager ?? new BillingManager(_core, _freePlugins);
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
