using Panacea.Core;
using Panacea.Models;
using Panacea.Modularity.Billing;
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
        public BillingPlugin(PanaceaServices core)
        {
            _core = core;
        }

        #region IPlugin
        public Task BeginInit()
        {
            return Task.CompletedTask;
        }

        public Task EndInit()
        {
            return Task.CompletedTask;
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
            return _manager = _manager ?? new BillingManager(_core);
        }
    }
}
