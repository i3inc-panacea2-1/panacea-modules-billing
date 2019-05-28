using Panacea.Modularity.Billing;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Panacea.Modules.Billing.Controls
{
    /// <summary>
    /// Interaction logic for Services.xaml
    /// </summary>
    public partial class ServicesControl : UserControl
    {
        public bool HasPackages
        {
            get { return (bool)GetValue(HasPackagesProperty); }
            set { SetValue(HasPackagesProperty, value); }
        }

        // Using a DependencyProperty as the backing store for HasPackages.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty HasPackagesProperty =
            DependencyProperty.Register("HasPackages", typeof(bool), typeof(ServicesControl), new PropertyMetadata(false));

        public bool HasServices
        {
            get { return (bool)GetValue(HasServicesProperty); }
            set { SetValue(HasServicesProperty, value); }
        }

        // Using a DependencyProperty as the backing store for HasServices.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty HasServicesProperty =
            DependencyProperty.Register("HasServices", typeof(bool), typeof(ServicesControl), new PropertyMetadata(false));

        public bool HasSelectedPerDay
        {
            get { return (bool)GetValue(HasSelectedPerDayProperty); }
            set { SetValue(HasSelectedPerDayProperty, value); }
        }

        // Using a DependencyProperty as the backing store for HasSelectedPerDay.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty HasSelectedPerDayProperty =
            DependencyProperty.Register("HasSelectedPerDay", typeof(bool), typeof(ServicesControl), new PropertyMetadata(false));

        public double Sum
        {
            get { return (double)GetValue(SumProperty); }
            set { SetValue(SumProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Sum.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SumProperty =
            DependencyProperty.Register("Sum", typeof(double), typeof(ServicesControl), new PropertyMetadata(0.0));

        public ObservableCollection<Service> SelectedItems
        {
            get { return (ObservableCollection<Service>)GetValue(SelectedItemsProperty); }
            set { SetValue(SelectedItemsProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SelectedItems.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SelectedItemsProperty =
            DependencyProperty.Register("SelectedItems", typeof(ObservableCollection<Service>), typeof(ServicesControl), new PropertyMetadata(null));


        public List<Package> Packages
        {
            get { return (List<Package>)GetValue(PackagesProperty); }
            set { SetValue(PackagesProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Packages.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PackagesProperty =
            DependencyProperty.Register("Packages", typeof(List<Package>), typeof(ServicesControl), new PropertyMetadata(null));


        public List<Service> Services
        {
            get { return (List<Service>)GetValue(ServicesProperty); }
            set { SetValue(ServicesProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Services.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ServicesProperty =
            DependencyProperty.Register("Services", typeof(List<Service>), typeof(ServicesControl), new PropertyMetadata(null));


        public ServicesControl()
        {
            InitializeComponent();
            SelectedItems = new ObservableCollection<Service>();
        }

        private void packagesLimited_Click(object sender, EventArgs e)
        {
            var package = sender as Service;

            if (package == null) return;
            CancelButton.Visibility = Visibility.Collapsed;
            CancelButtonServices.Visibility = Visibility.Visible;
            if (package is Package)
            {
                CancelButton.Visibility = Visibility.Visible;
                CancelButtonServices.Visibility = Visibility.Collapsed;
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
                CartBox.IsOpen = SelectedItems.Count > 0;
                HasSelectedPerDay = SelectedItems.All(i => i.IsPricePerDay);
                UpdateSum();
                TotalPanel.Visibility = SelectedItems.Any(i => i.IsPricePerDay) || SelectedItems.Count > 1
                    ? Visibility.Visible
                    : Visibility.Collapsed;
                return;
            }


            if ((package.IsPricePerDay && SelectedItems.Any(s => !s.IsPricePerDay)) ||
                (!package.IsPricePerDay && SelectedItems.Any(s => s.IsPricePerDay)))
            {

                //var warning = new ChangeServiceTypeWarning();
                //warning.Continue += (oo, ee) =>
                //{
                //    window.ThemeManager.HidePopup(warning);
                //    SelectedItems.Clear();
                //    packages.ForEach(lp => lp.IsChecked = false);
                //    services.ForEach(lp => lp.IsChecked = false);
                //    package.IsChecked = true;
                //    SelectedItems.Add(package);
                //    CartBox.IsOpen = SelectedItems.Count > 0;
                //    HasSelectedPerDay = SelectedItems.All(i => i.IsPricePerDay);
                //    UpdateSum();
                //    TotalPanel.Visibility = SelectedItems.Any(i => i.IsPricePerDay) || SelectedItems.Count > 1
                //        ? Visibility.Visible
                //        : Visibility.Collapsed;
                //};

                //warning.Cancel += (oo, ee) =>
                //{
                //    window.ThemeManager.HidePopup(warning);
                //    package.IsChecked = false;
                //    CartBox.IsOpen = SelectedItems.Count > 0;
                //};
                CartBox.IsOpen = false;

                //window.ThemeManager.ShowPopup(warning).Closed += (oo, ee) =>
                //{
                //    package.IsChecked = false;
                //};


            }
            else
            {

                SelectedItems.Add(package);
                CartBox.IsOpen = SelectedItems.Count > 0;
                HasSelectedPerDay = SelectedItems.All(i => i.IsPricePerDay);
                UpdateSum();
                TotalPanel.Visibility = SelectedItems.Any(i => i.IsPricePerDay) || SelectedItems.Count > 1
                    ? Visibility.Visible
                    : Visibility.Collapsed;
            }
        }

        void UpdateSum()
        {
            if (SelectedItems == null) return;
            Sum = SelectedItems.All(i => i.IsPricePerDay) ? SelectedItems.Sum(i => i.TotalPrice * DaysSlider.Value) : SelectedItems.Sum(s => s.TotalPrice);
        }
    }
}
