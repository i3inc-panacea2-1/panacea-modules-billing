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
        }
    }
}
