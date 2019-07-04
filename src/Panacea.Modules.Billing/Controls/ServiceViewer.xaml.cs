using Panacea.Modularity.Billing;
using System;
using System.Collections.Generic;
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
    /// Interaction logic for ServiceViewer.xaml
    /// </summary>
    public partial class ServiceViewer : UserControl
    {

        public ServiceViewer()
        {
            InitializeComponent();
        }

        public static readonly DependencyProperty ServicesProperty =
             DependencyProperty.Register("Services", typeof(List<Service>),
             typeof(ServiceViewer), new FrameworkPropertyMetadata(null));

        // .NET Property wrapper
        public List<Service> Services
        {
            get { return (List<Service>)GetValue(ServicesProperty); }
            set { SetValue(ServicesProperty, value); }
        }



        public ICommand SelectCommand
        {
            get { return (ICommand)GetValue(SelectCommandProperty); }
            set { SetValue(SelectCommandProperty, value); }
        }

  
      public static readonly DependencyProperty SelectCommandProperty =
            DependencyProperty.Register("SelectCommand", typeof(ICommand), typeof(ServiceViewer), new PropertyMetadata(null));



        private void Grid_PreviewMouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            object package = null;
            var el = sender as FrameworkElement;
            package = el.Tag;
            if (!(package as Service).IsMore)
                (package as Service).IsChecked = !(package as Service).IsChecked;
            SelectCommand?.Execute(package);
        }

        public List<Service> GetSelectedServices()
        {
            try
            {
                List<Service> serv = itemsControl.ItemsSource as List<Service>;

                return serv.Where(s => s.IsChecked).ToList();
            }
            catch
            {
            }
            return null;
        }

        public void Reset()
        {

        }
    }
}
