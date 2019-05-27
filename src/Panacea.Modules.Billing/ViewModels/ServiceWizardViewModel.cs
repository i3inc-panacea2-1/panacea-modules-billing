using Panacea.Core;
using Panacea.Modularity.Billing;
using Panacea.Modules.Billing.Models;
using Panacea.Modules.Billing.Views;
using Panacea.Multilinguality;
using Panacea.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Panacea.Modules.Billing.ViewModels
{
    [View(typeof(ServiceWizard))]
    class ServiceWizardViewModel : ViewModelBase
    {
        private readonly PanaceaServices _core;

        public ServiceWizardViewModel(PanaceaServices core)
        {
            _core = core;
        }

        List<Package> _packages;
        public List<Package> Packages
        {
            get => _packages;
            set
            {
                _packages = value;
                OnPropertyChanged();
            }
        }


        List<Service> _services;
        public List<Service> Services
        {
            get => _services;
            set
            {
                _services = value;
                OnPropertyChanged();
            }
        }

        bool _hasPackages;
        public bool HasPackages
        {
            get => _hasPackages;
            set
            {
                _hasPackages = value;
                OnPropertyChanged();
            }
        }

        bool _hasServices;
        public bool HasServices
        {
            get => _hasServices;
            set
            {
                _hasServices = value;
                OnPropertyChanged();
            }
        }

        public override async void Activate()
        {
            await LoadPackages();
        }

        public async Task LoadPackages()
        {
            try
            {
                var response = await _core.HttpClient.GetObjectAsync<GetServicesAndPackagesResponse>(
                    "billing/get_services_and_packages/"
                    );
                if (response.Success)
                {
                    var dyn = response.Result;
                    Packages = dyn.Packages.Billing
                        .BillingPackages
                        .GroupBy(s => s.Id)
                        .Select(g=>g.First())
                        .ToList();
                    Services = dyn.Packages.Billing.BillingPackages.Any(p => p.StandAlone)
                        ? dyn.Packages.Billing.BillingPackages.Where(p => p.StandAlone)
                            .SelectMany(p => p.Services)
                            .Select(p => p.Service)
                            .GroupBy(s => s.Id)
                            .Select(g => g.First())
                            .ToList()
                        : new List<Service>();
                    HasPackages = Packages.Count > 0;
                    HasServices = Services.Count > 0;

                    if (Packages.Count == 0 && Services.Count == 0)
                    {
                        //todo NoServices?.Invoke(null, null);
                        return;
                    }
                    if (Packages.Count == 0)
                    {
                        //todo limitedTabs.SelectedIndex = 1;
                    }
                }
                else
                {
                    //todo Error?.Invoke(this, new Translator("core").Translate("Connection error. Please try again."));
                }
            }
            catch
            {
                //todo Error?.Invoke(this, new Translator("core").Translate("Connection error. Please try again."));
            }

        }
    }
}
