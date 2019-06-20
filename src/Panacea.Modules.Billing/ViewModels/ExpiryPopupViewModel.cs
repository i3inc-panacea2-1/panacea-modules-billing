using Panacea.Controls;
using Panacea.Modularity.Billing;
using Panacea.Modularity.UiManager;
using Panacea.Modules.Billing.Views;
using Panacea.Multilinguality;
using Panacea.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Input;
using System.Windows.Threading;

namespace Panacea.Modules.Billing.ViewModels
{
    [View(typeof(ExpiryPopup))]
    public class ExpiryPopupViewModel : PopupViewModelBase<object>
    {
        readonly Translator _translator = new Translator("Billing");
        private readonly Timer _timer = new Timer() { Interval = 10000 };
        public ExpiryPopupViewModel(IBillingManager billing, DateTime time)
        {
            Time = time;
            _timer.Elapsed += _timer_Tick;
            BuyCommand = new RelayCommand(args =>
            {
                Close();
                billing.NavigateToBuyServiceWizard();
            });
        }

        public ICommand BuyCommand { get; }

        public DateTime Time { get; set; }


        string _text;
        public string Text
        {
            get => _text;
            set
            {
                _text = value;
                OnPropertyChanged();
            }
        }

        private void _timer_Tick(object sender, EventArgs e)
        {
            if (Time == null) return;
            var now = DateTime.Now;
            if (Time <= now)
            {
                Text = _translator.Translate("Your service has expired.");
                _timer.Stop();
                return;
            }
            var span = Time.Subtract(now);
            if (span.Hours > 1)
            {
                Text = _translator.Translate("Your service expires in {0} hours and {0} minutes.", span.Hours, span.Minutes);
            }
            else if (span.Hours == 1)
            {
                Text = _translator.Translate("Your service expires in 1 hour and {0} minutes.", span.Minutes);
            }
            else
            {
                Text = _translator.Translate("Your service expires in {0} minutes.", Math.Ceiling(span.TotalMinutes));
            }
        }

        public override void Activate()
        {
            _timer.Start();
            _timer_Tick(null, null);
        }
    }
}
