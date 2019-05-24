using Panacea.Modularity.Billing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Panacea.Modules.Billing.Controls
{
    public class PriceTag : Control
    {
        static PriceTag()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(PriceTag), new FrameworkPropertyMetadata(typeof(PriceTag)));
        }

        public Service Service
        {
            get { return (Service)GetValue(ServiceProperty); }
            set { SetValue(ServiceProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Service.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ServiceProperty =
            DependencyProperty.Register("Service", typeof(Service), typeof(PriceTag), new PropertyMetadata(null));


    }
}
