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
    /// Interaction logic for PackageViewer.xaml
    /// </summary>
    public partial class PackageViewer : UserControl
    {
        public static readonly DependencyProperty PackagesProperty =
            DependencyProperty.Register("Packages", typeof(List<Package>),
            typeof(PackageViewer), new FrameworkPropertyMetadata(null));

        // .NET Property wrapper
        public List<Package> Packages
        {
            get { return (List<Package>)GetValue(PackagesProperty); }
            set { SetValue(PackagesProperty, value); }
        }


        public ObservableCollection<Package> SelectedPackages
        {
            get { return (ObservableCollection<Package>)GetValue(SelectedPackagesProperty); }
            set { SetValue(SelectedPackagesProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SelectedPackages.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SelectedPackagesProperty =
            DependencyProperty.Register("SelectedPackages", typeof(ObservableCollection<Package>), typeof(PackageViewer), new PropertyMetadata(null));



        public PackageViewer()
        {
            InitializeComponent();
            SelectedPackages = new ObservableCollection<Package>();
        }

        private void ItemsControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SelectedPackages.Clear();
            foreach (var item in (sender as ListBox).SelectedItems)
            {
                SelectedPackages.Add(item as Package);

            }
        }
    }
}
