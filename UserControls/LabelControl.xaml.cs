using System.Windows;
using System.Windows.Controls;

namespace AssetManager.UserControls
{
    /// <summary>
    /// Interaction logic for LabelControl.xaml
    /// </summary>
    public partial class LabelControl : UserControl
    {
        public LabelControl()
        {
            InitializeComponent();
        }
      
        public static readonly DependencyProperty SelectedAreaProperty = DependencyProperty.Register("SelectedArea", typeof(string), typeof(LabelControl),
            new FrameworkPropertyMetadata(string.Empty, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, new PropertyChangedCallback(SelectedAreaCallback)));
        public static readonly DependencyProperty SelectedGroupProperty = DependencyProperty.Register("SelectedGroup", typeof(string), typeof(LabelControl),
            new FrameworkPropertyMetadata(string.Empty, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, new PropertyChangedCallback(SelectedGroupCallback)));
        public static readonly DependencyProperty SelectedIDProperty = DependencyProperty.Register("SelectedID", typeof(int), typeof(LabelControl),
            new FrameworkPropertyMetadata(0, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, new PropertyChangedCallback(SelectedIDCallback)));
        
        public static readonly DependencyProperty DelimiterProperty = DependencyProperty.Register("Delimiter", typeof(string), typeof(LabelControl),
            new FrameworkPropertyMetadata(string.Empty, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, new PropertyChangedCallback(DelimiterCallback)));
        public static readonly DependencyProperty MaximumIDValueProperty = DependencyProperty.Register("MaximumIDValue", typeof(int), typeof(LabelControl),
            new FrameworkPropertyMetadata(1, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, new PropertyChangedCallback(MaximumIDValueCallback)));
        
        public string SelectedArea
        {
            get { return (string)GetValue(SelectedAreaProperty); }
            set { SetValue(SelectedAreaProperty, value); }
        }

        public string SelectedGroup
        {
            get { return (string)GetValue(SelectedGroupProperty); }
            set { SetValue(SelectedGroupProperty, value); }
        }

        public int SelectedID
        {
            get { return (int)GetValue(SelectedIDProperty); }
            set { SetValue(SelectedIDProperty, value); }
        }

        public string Delimiter
        {
            get { return (string)GetValue(DelimiterProperty); }
            set { SetValue(DelimiterProperty, value); }
        }
                      
        public int MaximumIDValue
        {
            get { return (int)GetValue(MaximumIDValueProperty); }
            set { SetValue(MaximumIDValueProperty, value); }
        }
        
        private static void SelectedAreaCallback(DependencyObject source, DependencyPropertyChangedEventArgs args)
        {
            if (args.NewValue != null)
            {
                (source as LabelControl).area.Text = ((string)args.NewValue).ToUpper();
            }
        }

        private static void SelectedGroupCallback(DependencyObject source, DependencyPropertyChangedEventArgs args)
        {
            if (args.NewValue != null)
            {
                (source as LabelControl).group.Text = (string)args.NewValue;               
            }
        }

        private static void SelectedIDCallback(DependencyObject source, DependencyPropertyChangedEventArgs args)
        {
            if (args.NewValue != null)
            {
                (source as LabelControl).selectedid.Value = (int)args.NewValue;                
            }
        }
               
        private static void DelimiterCallback(DependencyObject source, DependencyPropertyChangedEventArgs args)
        {
            if (args.NewValue != null)
            {
                (source as LabelControl).Delimiter = (string)args.NewValue;               
            }
        }
       
        private static void MaximumIDValueCallback(DependencyObject source, DependencyPropertyChangedEventArgs args)
        {
            if (args.NewValue != null)
            {
                (source as LabelControl).MaximumIDValue = (int)args.NewValue;
            }
        }

    }
}
