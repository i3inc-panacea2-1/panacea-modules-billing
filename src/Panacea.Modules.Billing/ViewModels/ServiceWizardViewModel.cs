using Panacea.Controls;
using Panacea.Core;
using Panacea.Modularity.Billing;
using Panacea.Modularity.UiManager;
using Panacea.Modularity.UserAccount;
using Panacea.Modules.Billing.Models;
using Panacea.Modules.Billing.Views;
using Panacea.Multilinguality;
using Panacea.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms.Integration;
using System.Windows.Input;

namespace Panacea.Modules.Billing.ViewModels
{
    [View(typeof(ServiceWizard))]
    class ServiceWizardViewModel : ViewModelBase
    {
        private readonly PanaceaServices _core;
        private readonly TaskCompletionSource<bool> _source;
        bool _waitingForAnotherTask = false;
        public ServiceWizardViewModel(PanaceaServices core, TaskCompletionSource<bool> source)
        {
            _core = core;
            _source = source;
            SelectedItems = new ObservableCollection<Service>();
            RemoveServiceCommand = new RelayCommand(args =>
            {
                SelectedItems.Remove(args as Service);
                if (!SelectedItems.Any())
                {
                    CartBoxIsOpen = false;
                }
            });
            SwitchToServicesCommand = new RelayCommand(args =>
            {
                if (SelectedIndex == 0)
                    SelectedIndex = 1;
                else SelectedIndex = 0;
            });
            BuyServiceCommand = new RelayCommand(arg =>
            {
                SelectedItems.Clear();
                TabsSelectedIndex = 0;
            });
            CompleteCommand = new RelayCommand(arg =>
            {
                source.TrySetResult(true);
            });
            ToggleCartBoxCommand = new RelayCommand(arg =>
            {
                CartBoxIsOpen = !CartBoxIsOpen;
            });
            CheckoutCommand = new RelayCommand(async args =>
            {
                if (Packages != null && Packages.Any(p => p.IsChecked) ||
                       Services != null && Services.Any(s => s.IsChecked))
                {
                    CartBoxIsOpen = false;
                    if (core.UserService.User.Id == null)
                    {
                        if (core.TryGetUserAccountManager(out IUserAccountManager account))
                        {
                            _waitingForAnotherTask = true;
                            if (await account.RequestLoginAsync("In order to continue, you need to sign in with your user account"))
                            {
                                _waitingForAnotherTask = false;
                                TabsSelectedIndex = 1;
                                CreateWebBrowser();
                                BuyService();
                            }
                        }
                        //todo TabsSelectedIndex = 1;
                    }
                    else
                    {
                        var pop = new UserConfirmationViewModel(_core.UserService.User);
                        if (_core.TryGetUiManager(out IUiManager ui))
                        {
                            var res = await ui.ShowPopup(pop);
                            if (res == UserConfirmationResult.Confirm)
                            {
                                TabsSelectedIndex = 1;
                                CreateWebBrowser();
                                BuyService();
                                //CheckUserService(() =>
                                //{

                                // });
                            }
                            else if (res == UserConfirmationResult.NotMe)
                            {
                                if (!(_core.TryGetUserAccountManager(out IUserAccountManager user) && await user.LogoutAsync()))
                                {
                                    await _core.UserService.LogoutAsync(); //todo (true)
                                }
                                CheckoutCommand.Execute(null);
                            }
                            else
                            {
                                CartBoxIsOpen = true;
                            }
                        }

                    }
                }
                else
                {
                    if (core.TryGetUiManager(out IUiManager ui))
                    {
                        await ui.ShowPopup(new NoServiceSelectedPopupViewModel());
                    }
                }
            });

            SelectCommand = new RelayCommand(args =>
            {
                var package = args as Service;

                if (package == null) return;
                CancelButtonVisibility = Visibility.Collapsed;
                CancelButtonServicesVisibility = Visibility.Visible;
                if (package is Package)
                {
                    CancelButtonVisibility = Visibility.Visible;
                    CancelButtonServicesVisibility = Visibility.Collapsed;
                    SelectedItems.Clear();
                    Packages.ForEach(lp => lp.IsChecked = false);
                    Services.ForEach(lp => lp.IsChecked = false);
                    package.IsChecked = true;
                }
                else if (SelectedItems.Any(i => i is Package))
                {
                    SelectedItems.Clear();
                    Packages.ForEach(lp => lp.IsChecked = false);
                    Services.ForEach(lp => lp.IsChecked = false);
                    package.IsChecked = true;
                }
                if (SelectedItems.Contains(package))
                {
                    SelectedItems.Remove(package);
                    package.IsChecked = false;
                    CartBoxIsOpen = SelectedItems.Count > 0;
                    HasSelectedPerDay = SelectedItems.All(i => i.IsPricePerDay);
                    UpdateSum();
                    TotalPanelVisibility = SelectedItems.Any(i => i.IsPricePerDay) || SelectedItems.Count > 1
                        ? Visibility.Visible
                        : Visibility.Collapsed;
                    return;
                }


                if ((package.IsPricePerDay && SelectedItems.Any(s => !s.IsPricePerDay)) ||
                    (!package.IsPricePerDay && SelectedItems.Any(s => s.IsPricePerDay)))
                {
                    /*todo
                    var warning = new ChangeServiceTypeWarning();
                    warning.Continue += (oo, ee) =>
                    {
                        window.ThemeManager.HidePopup(warning);
                        SelectedItems.Clear();
                        packages.ForEach(lp => lp.IsChecked = false);
                        services.ForEach(lp => lp.IsChecked = false);
                        package.IsChecked = true;
                        SelectedItems.Add(package);
                        CartBox.IsOpen = SelectedItems.Count > 0;
                        HasSelectedPerDay = SelectedItems.All(i => i.IsPricePerDay);
                        UpdateSum();
                        TotalPanel.Visibility = SelectedItems.Any(i => i.IsPricePerDay) || SelectedItems.Count > 1
                            ? Visibility.Visible
                            : Visibility.Collapsed;
                    };

                    warning.Cancel += (oo, ee) =>
                    {
                        window.ThemeManager.HidePopup(warning);
                        package.IsChecked = false;
                        CartBox.IsOpen = SelectedItems.Count > 0;
                    };
                    CartBoxIsOpen = false;

                    window.ThemeManager.ShowPopup(warning).Closed += (oo, ee) =>
                    {
                        package.IsChecked = false;
                    };
                    */

                }
                else
                {

                    SelectedItems.Add(package);
                    CartBoxIsOpen = SelectedItems.Count > 0;
                    HasSelectedPerDay = SelectedItems.All(i => i.IsPricePerDay);
                    UpdateSum();
                    TotalPanelVisibility = SelectedItems.Any(i => i.IsPricePerDay) || SelectedItems.Count > 1
                        ? Visibility.Visible
                        : Visibility.Collapsed;
                }
            });

        }

        private async Task CheckUserService(Action continuation)
        {
            //if (_core.UserService.User.Id == null) return;
            //try
            //{
            //    var selectedPackage = Services.GetSelectedPackage();
            //    var selectedServices = services.GetSelectedServices();
            //    var alreadyOwnedCriteria = new Func<Service, Func<Service, bool>>(serv =>
            //    {
            //        return s => s.Plugin == serv.Plugin &&
            //                    (s.ExpirationDate > DateTime.Now || s.RestDuration == -1.0) &&
            //                    (s.Quantity > 0 || s.Quantity == -1);
            //    });
            //    var listOfServicesToExamine = selectedPackage?.Services.Select(s => s.Service).ToList() ??
            //                                  selectedServices;

            //    var owned = new List<Service>();
            //    foreach (var serv in listOfServicesToExamine)
            //    {
            //        var criteria = alreadyOwnedCriteria(serv);
            //        if (!UserManager.User.Services.Any(criteria)) continue;
            //        owned.AddRange(UserManager.User.Services.Where(criteria));
            //    }
            //    if (owned.Count > 0)
            //    {
            //        ShowAlreadyOwnedPopup(owned, continuation);
            //    }
            //    else
            //    {
            //        continuation();
            //    }
            //}
            //catch
            //{
            //    continuation?.Invoke();
            //}
        }

        void UpdateSum()
        {
            if (SelectedItems == null) return;
            Sum = SelectedItems.All(i => i.IsPricePerDay) ? SelectedItems.Sum(i => i.TotalPrice * DaysSliderValue) : SelectedItems.Sum(s => s.TotalPrice);
        }

        void CreateWebBrowser()
        {
            _webBrowser = new System.Windows.Forms.WebBrowser();
            _webBrowser.ScriptErrorsSuppressed = true;
            _host = new WindowsFormsHost()
            {
                Child = _webBrowser
            };
            _webBrowser.Navigating += _webBrowser_Navigating;
            _webBrowser.Navigated += _webBrowser_Navigated;
            _webBrowser.DocumentCompleted += _webBrowser_DocumentCompleted;
            WebBrowserControl = _host;
        }

        private void _webBrowser_DocumentCompleted(object sender, System.Windows.Forms.WebBrowserDocumentCompletedEventArgs e)
        {
            var regex = new Regex(@"billing\/([^\/]*)\/([^\/]*)\/");
            var match = regex.Match(e.Url.ToString().ToLower());
            if (match.Success && match.Groups.Count >= 2)
            {
                if (match.Groups[1].Value.Contains("payment"))
                {
                    if (match.Groups[2].Value == "completed" || match.Groups[2].Value == "rejected")
                    {
                        Success = match.Groups[2].Value == "completed";
                        TabsSelectedIndex = 2;
                        if (_core.TryGetUiManager(out IUiManager ui))
                        {
                            ui.Back -= Ui_Back;
                        }
                    }
                }
               
            }
        }

        private void _webBrowser_Navigated(object sender, System.Windows.Forms.WebBrowserNavigatedEventArgs e)
        {
            BrowserVisible = true;
        }

        private void _webBrowser_Navigating(object sender, System.Windows.Forms.WebBrowserNavigatingEventArgs e)
        {
            BrowserVisible = false;
        }

        #region Properties

        public RelayCommand CompleteCommand { get; }
        public RelayCommand BuyServiceCommand { get; }
        public int DaysSliderValue { get; set; }

        public ObservableCollection<Service> SelectedItems { get; set; }

        Visibility _totalPanelVisibility;
        public Visibility TotalPanelVisibility
        {
            get => _totalPanelVisibility;
            set
            {
                _totalPanelVisibility = value;
                OnPropertyChanged();
            }
        }

        public bool PackagesTextVisible { get; set; } = true;

        Visibility _cancelButtonVisibility;
        public Visibility CancelButtonVisibility
        {
            get => _cancelButtonVisibility;
            set
            {
                _cancelButtonVisibility = value;
                OnPropertyChanged();
            }
        }

        Visibility _cancelButtonServicesVisibility;
        public Visibility CancelButtonServicesVisibility
        {
            get => _cancelButtonServicesVisibility;
            set
            {
                _cancelButtonServicesVisibility = value;
                OnPropertyChanged();
            }
        }

        bool _success;
        public bool Success
        {
            get => _success;
            set
            {
                _success = value;
                OnPropertyChanged();
            }
        }

        bool _browservisible;
        public bool BrowserVisible
        {
            get => _browservisible;
            set
            {
                _browservisible = value;
                OnPropertyChanged();
            }
        }

        bool _cartBoxIsOpen;
        public bool CartBoxIsOpen
        {
            get => _cartBoxIsOpen;
            set
            {
                if (!SelectedItems.Any())
                {
                    _cartBoxIsOpen = false;
                    OnPropertyChanged();
                    return;
                }
                _cartBoxIsOpen = value;
                OnPropertyChanged();
            }
        }


        bool _hasSelectedPerDay;
        public bool HasSelectedPerDay
        {
            get => _hasSelectedPerDay;
            set
            {
                _hasSelectedPerDay = value;
                OnPropertyChanged();
            }
        }


        double _sum;
        public double Sum
        {
            get => _sum;
            set
            {
                _sum = value;
                OnPropertyChanged();
            }
        }

        int _selectedIndex = 0;
        public int SelectedIndex
        {
            get => _selectedIndex;
            set
            {
                _selectedIndex = value;

                PackagesTextVisible = value == 0;
                OnPropertyChanged(nameof(PackagesTextVisible));
                OnPropertyChanged();
            }
        }

        double _tabsSelectedIndex = 0;
        public double TabsSelectedIndex
        {
            get => _tabsSelectedIndex;
            set
            {
                _tabsSelectedIndex = value;
                OnPropertyChanged();
            }
        }


        public bool HasSelectedAnything { get; set; }

        public RelayCommand CheckoutCommand { get; }

        public RelayCommand SelectCommand { get; }

        public RelayCommand ToggleCartBoxCommand { get; }

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
        #endregion properties

        public override async void Activate()
        {
            if (_core.TryGetUiManager(out IUiManager ui))
            {
                ui.Back += Ui_Back;
            }
            await LoadPackages();
        }

        private void Ui_Back(object sender, BeforeNavigateEventArgs e)
        {
            if (TabsSelectedIndex > 0)
            {
                TabsSelectedIndex--;
                e.Cancel = true;
            }

        }

        public override void Deactivate()
        {
            if (_core.TryGetUiManager(out IUiManager ui))
            {
                ui.Back -= Ui_Back;
            }
            WebBrowserControl = null;
            if (_webBrowser != null)
            {
                _webBrowser.Navigating -= _webBrowser_Navigating;
                _webBrowser.Navigated -= _webBrowser_Navigated;
                _webBrowser.DocumentCompleted -= _webBrowser_DocumentCompleted;
                _webBrowser?.Dispose();
            }

            _host?.Dispose();
            if (!_waitingForAnotherTask)
                _source.TrySetResult(false);

        }

        System.Windows.Forms.WebBrowser _webBrowser;
        WindowsFormsHost _host;
        FrameworkElement _webBrowserControl;
        public FrameworkElement WebBrowserControl
        {
            get => _webBrowserControl;
            set
            {
                _webBrowserControl = value;
                OnPropertyChanged();
            }
        }

        public async Task LoadPackages()
        {
            try
            {
                if (_core.TryGetUiManager(out IUiManager ui))
                {
                    await ui.DoWhileBusy(async () =>
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
                                .Select(g => g.First())
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
                    });

                }
            }
            catch
            {
                //todo Error?.Invoke(this, new Translator("core").Translate("Connection error. Please try again."));
            }

        }

        protected void BuyService()
        {
            if (_webBrowser.IsDisposed) return;

            var package = Packages.FirstOrDefault(s => s.IsChecked);
            var services = Services.Where(s => s.IsChecked).ToList();
            string postData = null;
            var culture = CultureInfo.CreateSpecificCulture("en-US");
            const string additionalHeaders = "Content-Type: application/x-www-form-urlencoded";
            string url = null;

            if (package != null)
            {
                url = _core.HttpClient.GetApiEndpoint("billing/buy_package/");
                if (package.IsPricePerDay)
                {
                    var duration = DaysSliderValue;
                    postData =
                        string.Format("user={0}&packageID={1}&amountWithTaxes={2}&transactionType=purchase&method=cash",
                            _core.UserService.User.Id, package.Id, (package.TotalPrice * duration).ToString("#0.00", culture));
                    postData += "&duration=" + duration;

                }
                else
                {
                    postData =
                        string.Format("user={0}&packageID={1}&amountWithTaxes={2}&transactionType=purchase&method=cash",
                            _core.UserService.User.Id, package.Id, package.TotalPrice.ToString("#0.00", culture));
                }
            }
            else
            {
                url = _core.HttpClient.GetApiEndpoint("billing/buy_services/");
                if (services[0].IsPricePerDay)
                {
                    var duration = DaysSliderValue;
                    postData =
                        string.Format("user={0}&amountWithTaxes={1}&transactionType=purchase&method=cash",
                            _core.UserService.User.Id,
                            services.Sum(s => s.TotalPrice * duration).ToString("#0.00", culture));
                    for (var i = 0; i < services.Count; i++)
                    {
                        postData += "&serviceIDs[" + i + "]=" + services[i].Id;
                    }
                    postData += "&duration=" + duration;
                }
                else
                {
                    postData =
                        string.Format(
                            "user={0}&amountWithTaxes={1}&transactionType=purchase&method=cash",
                            _core.UserService.User.Id,
                            services.Sum(s => s.TotalPrice).ToString("#0.00", culture));
                    for (var i = 0; i < services.Count; i++)
                    {
                        postData += "&serviceIDs[" + i + "]=" + services[i].Id;
                    }
                }
            }

            var post = Encoding.UTF8.GetBytes(postData);
            if (_webBrowser.IsDisposed) return;

            _webBrowser.Navigate(url, null, post, additionalHeaders);
        }

        public ICommand SwitchToServicesCommand { get; }

        public ICommand RemoveServiceCommand { get; }
    }
}
