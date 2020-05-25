using System.Windows;
using System.Windows.Controls;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;


namespace EquipmentManager
{
    /// <summary>
    /// Interaction logic for FilterListControl.xaml
    /// </summary>
    public partial class FilterListControl : UserControl, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged(string info)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(info));
            }
        }

        public FilterListControl()
        {
            InitializeComponent();         
        }

        #region Dependency Property Declarations

     //   public static readonly DependencyProperty ListItemsProperty = DependencyProperty.Register("ListItems", typeof(ObservableCollection<FilterListItem>), typeof(FilterListControl), new FrameworkPropertyMetadata(new PropertyChangedCallback(ListChanged)));
        public static readonly DependencyProperty ListItemsProperty = DependencyProperty.Register("ListItems", typeof(ObservableCollection<FilterListItem>), typeof(FilterListControl));

        public static readonly DependencyProperty MoreTextProperty = DependencyProperty.Register("MoreText", typeof(string), typeof(FilterListControl));
        public static readonly DependencyProperty LessTextProperty = DependencyProperty.Register("LessText", typeof(string), typeof(FilterListControl));
        public static readonly DependencyProperty HeadingProperty = DependencyProperty.Register("Heading", typeof(string), typeof(FilterListControl));
        public static readonly DependencyProperty SelectedItemsProperty = DependencyProperty.Register("SelectedItems", typeof(string), typeof(FilterListControl));
        public static readonly DependencyProperty ToggleButtonVisibilityProperty = DependencyProperty.Register("ToggleButtonVisibility", typeof(Visibility), typeof(FilterListControl));
        public static readonly DependencyProperty ShowItemsProperty = DependencyProperty.Register("ShowItems", typeof(bool), typeof(FilterListControl));
       
        //commands
        //showallitems
        public static readonly DependencyProperty ExpandButtonCommandProperty = DependencyProperty.Register("ExpandButtonCommand", typeof(ICommand), typeof(FilterListControl));

        #endregion  Dependency Property Declarations

        #region Commands

        public ICommand ExpandButtonCommand
        {
            get { return (ICommand)GetValue(ExpandButtonCommandProperty); }
            set { SetValue(ExpandButtonCommandProperty, value); }
        }

        #endregion Commands


        public ObservableCollection<FilterListItem> ListItems
        {
            get { return (ObservableCollection<FilterListItem>)GetValue(ListItemsProperty); }
            set { SetValue(ListItemsProperty, value); }
        }



        public string SelectedItems
        {
            get { return (string)GetValue(SelectedItemsProperty); }
            set { SetValue(SelectedItemsProperty, value); }
        }

        public Visibility ToggleButtonVisibility
        {
            get { return (Visibility)GetValue(ToggleButtonVisibilityProperty); }
            set { SetValue(ToggleButtonVisibilityProperty, value); }
        }

        public bool ShowItems
        {
            get { return (bool)GetValue(ShowItemsProperty); }
            set { SetValue(ShowItemsProperty, value); }
        }

        public string MoreText
        {
            get { return (string)GetValue(MoreTextProperty); }
            set { SetValue(MoreTextProperty, value); }
        }

        public string LessText
        {
            get { return (string)GetValue(LessTextProperty); }
            set { SetValue(LessTextProperty, value); }
        }

        public string Heading
        {
            get { return (string)GetValue(HeadingProperty); }
            set { SetValue(HeadingProperty, value); }
        }


    }

}
