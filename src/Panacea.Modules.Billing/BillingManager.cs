using Panacea.Core;
using Panacea.Models;
using Panacea.Modularity.Billing;
using Panacea.Modularity.UiManager;
using Panacea.Modules.Billing.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Panacea.Modules.Billing
{
    class BillingManager:IBillingManager
    {
        private readonly PanaceaServices _core;

        public BillingManager(PanaceaServices core)
        {
            _core = core;
        }

        public Task<bool> ConsumeItemAsync(string pluginName, ServerItem item)
        {
            if(_core.TryGetUiManager(out IUiManager ui))
            {
                return ui.ShowPopup(new ConsumeItemPopupViewModel());
            }
            return Task.FromResult(false);
        }

        public Task<bool> ConsumeItemOrRequestServiceAsync(string message, string pluginName, ServerItem item)
        {
            return Task.FromResult(false);
        }

        public Task<bool> ConsumeQuantityAsync(string pluginName, int quantity)
        {
            return Task.FromResult(false);
        }

        public Task<bool> ConsumeQuantityOrRequestServiceAsync(string pluginName, int quantity)
        {
            return Task.FromResult(false);
        }

        public Task<Service> GetServiceForItemAsync(string pluginName, ServerItem item)
        {
            return Task.FromResult(default(Service));
        }

        public Task<Service> GetServiceForQuantityAsync(string pluginName)
        {
            return Task.FromResult(default(Service));
        }

        public bool IsPluginFree(string plugnName)
        {
            return false;
        }
    }
}
