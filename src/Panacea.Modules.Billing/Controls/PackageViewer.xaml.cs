using Panacea.Controls;
using Panacea.Modularity.Billing;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
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

        public static readonly DependencyProperty CultureProperty = DependencyProperty.Register(
            "Culture", typeof(CultureInfo), typeof(PackageViewer), new PropertyMetadata(default(CultureInfo)));

        public CultureInfo Culture
        {
            get { return (CultureInfo)GetValue(CultureProperty); }
            set { SetValue(CultureProperty, value); }
        }



        public List<Package> DemoPackages
        {
            get
            {
                return new List<Package>()
                {
                    new Package()
                    {
                        Name = "asdf asdfsdfsd fasdf",
                        Description =
                            "dfsgfj ggjidfasj gfgihj asdopghfasdoighsdfugh puio aghfuighaui hguiagh hgahfughaufgh uaigh fuagh afuhg auohg ufasdhguh puahdfgufh",
                        TotalPrice = 76,
                        Services = new List<ServicePriority>()
                        {
                            new ServicePriority()
                            {
                                Service = new Service()
                                {
                                    Name = "ASDfdsfsdfsd",
                                    Description =
                                        "iofjaisdj sdjafia sjifjsdioafhuifhusdhaupshad uashd fusdhf usdha fuhfu ashf uashfusdhaf sudfh asufh su8fh dsu8fh df",
                                        TotalPrice = 50
                                }
                            }
                        }
                    },
                    new Package()
                    {
                        Name = "asdf asdfsdfsd fasdf",
                        Description =
                            "dfsgfj ggjidfasj gfgihj asdopghfasdoighsdfugh puio aghfuighaui hguiagh hgahfughaufgh uaigh fuagh afuhg auohg ufasdhguh puahdfgufh",
                        TotalPrice = 76,
                        Services = new List<ServicePriority>()
                        {
                            new ServicePriority()
                            {
                                Service = new Service()
                                {
                                    Name = "ASDfdsfsdfsd",
                                    Description =
                                        "iofjaisdj sdjafia sjifjsdioafhuifhusdhaupshad uashd fusdhf usdha fuhfu ashf uashfusdhaf sudfh asufh su8fh dsu8fh df",
                                        TotalPrice = 50
                                }
                            }
                        }
                    }
                };
            }
            set
            {

            }
        }

        public PackageViewer()
        {
            InitializeComponent();

            if (DesignerProperties.GetIsInDesignMode(this))
            {
                Packages = DemoPackages;
            }
            Culture = Thread.CurrentThread.CurrentUICulture;

            
        }



        public ICommand SelectCommand
        {
            get { return (ICommand)GetValue(SelectCommandProperty); }
            set { SetValue(SelectCommandProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SelectCommand.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SelectCommandProperty =
            DependencyProperty.Register("SelectCommand", typeof(ICommand), typeof(PackageViewer), new PropertyMetadata(null));


        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            SelectCommand?.Execute((sender as FrameworkElement).Tag);
        }
    }
}
