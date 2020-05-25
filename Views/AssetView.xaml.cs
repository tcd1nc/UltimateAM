using AssetManager.ViewModels;
using System.Windows;
using System.Windows.Input;


namespace AssetManager.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class AssetView : Window
    {
          
        public AssetView(int customerid, int parentid)
        {  
            InitializeComponent();
            this.DataContext = new AssetViewModel(customerid, parentid);
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
          
        }


        public AssetView(AssetTreeExViewModel tva, TVAssetViewModel obj)
        {
            InitializeComponent();
            this.Loaded += (o, e) =>
            {
                this.DataContext = new AssetViewModel(tva, obj);
                Cascadewindow();
            };
           
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (Keyboard.Modifiers == ModifierKeys.Alt && e.SystemKey == Key.F4)            
                e.Handled = true;            
        }

        private void Cascadewindow()
        {
            double _screenwidth = SystemParameters.PrimaryScreenWidth;
            double _screenheight = SystemParameters.PrimaryScreenHeight;
            double _windowheight = this.Height;
            double _windowwidth = this.Width;
            int _childwindowsctr = (this as AssetView).Owner.OwnedWindows.Count;
            double _titleheight = SystemParameters.WindowCaptionHeight;
            double _borderwidth = 8;
            if (_childwindowsctr == 1)
            {
                this.Left = (_screenwidth / 2) - (_windowwidth / 2);
                this.Top = (_screenheight / 3) - (_windowheight / 3) + (_titleheight + 8) ;
            }
            else
                if(_childwindowsctr > 1)
                {
                int _windowdepth = ((int)_childwindowsctr / 10);

                this.Left = (this as AssetView).Owner.OwnedWindows[_childwindowsctr - 2].Left + _borderwidth + (_windowdepth * 20);

                    if(_childwindowsctr % 10 == 0)
                        this.Top = (_screenheight / 3) - (_windowheight / 3) + (_titleheight + 8);
                    else
                        this.Top = (this as AssetView).Owner.OwnedWindows[_childwindowsctr - 2].Top + _titleheight + 8 ;
                }
        }
                
    }

}
