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
using System.Timers;
using System.Windows.Threading;

namespace Panacea.Modules.Billing
{
    class ServiceMonitor:IServiceMonitor
    {
        private readonly PanaceaServices _core;
        private readonly IBillingManager _billing;
        private Timer _timer;

        public Service CurrentService { get; private set; }
        ServerItem _item;
        public event EventHandler<Service> ServiceExpired;
        private bool _n1Hour, _n30min;
        private ExpiryPopupViewModel _popup;

        public ServiceMonitor(PanaceaServices core, IBillingManager billing)
        {
            _core = core;
            _billing = billing;
            _timer = new Timer() { Interval = 10000 };
            _timer.Elapsed += _timer_Elapsed;
        }

        private Service _previousService;

        public void Monitor(Service service, ServerItem item)
        {
            //_n30min = _n1Hour = false;
            _timer.Stop();
            _item = item;
            CurrentService = service;
            if (service == null) return;
            if (_previousService?.Plugin != CurrentService?.Plugin)
            {
                _n1Hour = _n30min = false;
            }
            _previousService = service;
            _timer.Start();
        }

        public void Monitor(Service service)
        {
            //_n30min = _n1Hour = false;
            _timer.Stop();
            _item = null;
            CurrentService = service;
            if (service == null) return;
            if (_previousService?.Plugin != CurrentService?.Plugin)
            {
                _n1Hour = _n30min = false;
            }
            _previousService = service;
            _timer.Start();
        }

        public void StopMonitor()
        {
            _timer.Stop();
            CurrentService = null;
        }

        void ShowPopup(DateTime time)
        {
            if(_core.TryGetUiManager(out IUiManager ui))
            {
                if (_popup != null) ui.HidePopup(_popup);
                _popup = new ExpiryPopupViewModel(_billing, time);
                ui.ShowPopup(_popup);
            }
            
        }

        async void _timer_Elapsed(object sender, EventArgs e)
        {

            if (CurrentService == null || CurrentService.ExpirationDate == default(DateTime)) return;

            CurrentService.RestDuration -= _timer.Interval/1000.0 / 60.0;

            var diff = CurrentService.ExpirationDate.Subtract(DateTime.Now);
            var otherServices = _billing.GetActiveUserServices()
                .Where(s => s != CurrentService && s.Plugin == CurrentService.Plugin && (s.RestDuration > 0 || s.Duration == -1));

            if (CurrentService.RestDuration <= 0)
            {
                _timer.Stop();
                try
                {
                    if (_item != null)
                    {
                        var s = await _billing.GetServiceForItemAsync(CurrentService.Plugin, _item);
                        if (s != null)
                        {
                            CurrentService = s;
                            _timer.Start();
                            return;
                        }
                    }
                    else
                    {
                        var s2 = await _billing.GetServiceAsync(CurrentService.Plugin);
                        if (s2 != null)
                        {
                            CurrentService = s2;
                            _timer.Start();
                            return;
                        }
                    }
                    var h = ServiceExpired;
                    h?.Invoke(this, CurrentService);
                }
                catch
                {
                    _timer.Start();
                }
                return;
            }

            if (otherServices.Any()) return;


            if (CurrentService.RestDuration <= 20)
            {
                if (_n30min) return;
                ShowPopup(CurrentService.ExpirationDate);
                _n30min = true;
            }
            else if (CurrentService.RestDuration <= 120)
            {
                if (_n1Hour) return;
                ShowPopup(CurrentService.ExpirationDate);
                _n1Hour = true;
            }
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }
    }
}
