using System.Windows.Controls;
using System.Windows.Input;


namespace AssetManager.UserControls
{
    /// <summary>
    /// Follow steps 1a or 1b and then 2 to use this custom control in a XAML file.
    ///
    /// Step 1a) Using this custom control in a XAML file that exists in the current project.
    /// Add this XmlNamespace attribute to the root element of the markup file where it is 
    /// to be used:
    ///
    ///     xmlns:MyNamespace="clr-namespace:BladeWear"
    ///
    ///
    /// Step 1b) Using this custom control in a XAML file that exists in a different project.
    /// Add this XmlNamespace attribute to the root element of the markup file where it is 
    /// to be used:
    ///
    ///     xmlns:MyNamespace="clr-namespace:BladeWear;assembly=BladeWear"
    ///
    /// You will also need to add a project reference from the project where the XAML file lives
    /// to this project and Rebuild to avoid compilation errors:
    ///
    ///     Right click on the target project in the Solution Explorer and
    ///     "Add Reference"->"Projects"->[Browse to and select this project]
    ///
    ///
    /// Step 2)
    /// Go ahead and use your control in the XAML file.
    ///
    ///     <MyNamespace:ExGrid/>
    ///
    /// </summary>
    public class ExGrid : DataGrid
    {
    //    static ExGrid()
   //     {
   //         DefaultStyleKeyProperty.OverrideMetadata(typeof(ExGrid), new FrameworkPropertyMetadata(typeof(ExGrid)));
   //     }

        public ExGrid() : base()
        { }

        protected override void OnPreviewKeyDown(KeyEventArgs e)
        {
            base.OnPreviewKeyDown(e);

            if (e.Key == Key.Down)
            {
                CommitEdit();
                if(SelectedIndex < Items.Count-1)
                SelectedIndex++;                
            }
            else
                if (e.Key == Key.Up)
            {
                CommitEdit();
                if(SelectedIndex > 0)
                SelectedIndex--;
            }
            
            
        }
        



    }
}
