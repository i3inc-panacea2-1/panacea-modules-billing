using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Panacea.Modules.Billing.Controls
{
    public class PaginatingItemsControl : ItemsControl
    {
        public PaginatingItemsControl()
        {
            NextPageCommand = new PageCommand(() =>
            {
                if ((_index + 1) * ItemsPerPage >= ItemsSource2.Cast<object>().Count()) return;
                _index++;
                ItemsSource = ItemsSource2.Cast<object>().Skip(_index * ItemsPerPage).Take(ItemsPerPage);
                HasNext = (_index + 1) * ItemsPerPage < ItemsSource2.Cast<object>().Count();
                HasPrevious = _index > 0;
            });
            PreviousPageCommand = new PageCommand(() =>
            {
                if (_index == 0) return;
                _index--;
                ItemsSource = ItemsSource2.Cast<object>().Skip(_index * ItemsPerPage).Take(ItemsPerPage);
                HasNext = (_index + 1) * ItemsPerPage < ItemsSource2.Cast<object>().Count();
                HasPrevious = _index > 0;
            });
        }

        static PaginatingItemsControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(PaginatingItemsControl),
                new FrameworkPropertyMetadata(typeof(PaginatingItemsControl)));
        }

        public bool HasNext
        {
            get { return (bool)GetValue(HasNextProperty); }
            set { SetValue(HasNextProperty, value); }
        }

        // Using a DependencyProperty as the backing store for HasNext.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty HasNextProperty =
            DependencyProperty.Register("HasNext", typeof(bool), typeof(PaginatingItemsControl), new PropertyMetadata(false));


        public bool HasPrevious
        {
            get { return (bool)GetValue(HasPreviousProperty); }
            set { SetValue(HasPreviousProperty, value); }
        }

        // Using a DependencyProperty as the backing store for HasPrevious.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty HasPreviousProperty =
            DependencyProperty.Register("HasPrevious", typeof(bool), typeof(PaginatingItemsControl), new PropertyMetadata(false));




        public IEnumerable ItemsSource2
        {
            get { return (IEnumerable)GetValue(ItemsSource2Property); }
            set { SetValue(ItemsSource2Property, value); }
        }

        // Using a DependencyProperty as the backing store for ItemsSource.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ItemsSource2Property =
            DependencyProperty.Register("ItemsSource2", typeof(IEnumerable), typeof(PaginatingItemsControl), new PropertyMetadata(null, OnItemsSource2Changed));

        private static void OnItemsSource2Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as PaginatingItemsControl;
            control._index = 0;
            control.ItemsSource = (e.NewValue as IEnumerable).Cast<object>().Take(control.ItemsPerPage);
            control.HasNext = (control._index + 1) * control.ItemsPerPage < control.ItemsSource2.Cast<object>().Count();
            control.HasPrevious = control._index > 0;
        }

        public int ItemsPerPage
        {
            get { return (int)GetValue(ItemsPerPageProperty); }
            set { SetValue(ItemsPerPageProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ItemsPerPage.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ItemsPerPageProperty =
            DependencyProperty.Register("ItemsPerPage", typeof(int), typeof(PaginatingItemsControl), new PropertyMetadata(6));

        public PageCommand NextPageCommand { get; set; }

        public PageCommand PreviousPageCommand { get; set; }

        int _index = 0;

    }

    public class PageCommand : ICommand
    {
        Action _action;
        public PageCommand(Action action)
        {
            _action = action;
        }
        public event EventHandler CanExecuteChanged
        {
            add { }
            remove { }
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            _action();
        }
    }
}
