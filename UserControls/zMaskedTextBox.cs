
using System;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace AssetManager.UserControls
{
    public class zMaskedTextBox : TextBox
    {

        public zMaskedTextBox() : base()
        {
            //disable copy and paste
            DataObject.AddPastingHandler(this, this.OnCancelCommand);
            DataObject.AddCopyingHandler(this, this.OnCancelCommand);
            this.ContextMenu = null;

            AddHandler(PreviewMouseLeftButtonDownEvent, new MouseButtonEventHandler(SelectivelyIgnoreMouseButton), true);
           
        }
        private void OnCancelCommand(object sender, DataObjectEventArgs e)
        {
            e.CancelCommand();
        }

        private void SelectivelyIgnoreMouseButton(object sender, MouseButtonEventArgs e)
        {
            if (!this.IsKeyboardFocusWithin)
            {
                // If the text box is not yet focussed, give it the focus and
                // stop further processing of this click event.
                this.Focus();
                e.Handled = true;
            }
        }

        //public new static readonly DependencyProperty  TextProperty = DependencyProperty.Register("Text", typeof(string), typeof(MaskedTextBox),
        //     new UIPropertyMetadata(string.Empty, OnTextChange));

        //private static void OnTextChange(DependencyObject d, DependencyPropertyChangedEventArgs e)
        //{
        //    ((MaskedTextBox)d).Text = (string)e.NewValue;
           
        //}

        //public string Text
        //{
        //    get { return (string)GetValue(TextProperty); }
        //    set { SetValue(TextProperty, value); }
        //}



        public static readonly DependencyProperty MaskProperty = DependencyProperty.Register("Mask", typeof(string), typeof(zMaskedTextBox),
             new UIPropertyMetadata(string.Empty, OnMaskChange));

        private static void OnMaskChange(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((zMaskedTextBox)d).Mask = (string)e.NewValue;
            ((zMaskedTextBox)d).DrawMask();

        }

        public string Mask
        {
            get { return (string)GetValue(MaskProperty); }
            set { SetValue(MaskProperty, value); }
        }
       
        private void DrawMask()
        {
            string str = Mask.Replace("L", " ");
            str = str.Replace("0", "bbb ");
            Text = "str";
        }
       
        private bool IsValidText()
        {

            return false;
        }

        public static readonly DependencyProperty DelimiterProperty = DependencyProperty.Register("Delimiter", typeof(string), typeof(zMaskedTextBox),
            new UIPropertyMetadata(string.Empty, OnDelimiterChange));

        public string Delimiter
        {
            get { return (string)GetValue(DelimiterProperty); }
            set { SetValue(DelimiterProperty, value); }
        }

        private static void OnDelimiterChange(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((zMaskedTextBox)d).Delimiter = (string)e.NewValue;
        }


        /// <summary>
        /// We check whether we are ok
        /// </summary>
        /// <param name="e"></param>
        protected override void OnPreviewTextInput(System.Windows.Input.TextCompositionEventArgs e)
        {
          
            //e.Handled = !TextBoxTextAllowed(e.Text);

            base.OnPreviewTextInput(e);

        }
        private bool TextBoxTextAllowed(string Text2)
        {
            return true; // System.Array.TrueForAll<System.Char>(Text2.ToCharArray(), delegate (System.Char c) {
            //    return System.Char.IsDigit(c) || Char.IsControl(c);
           // });
        }
        /// <summary>
        /// When text is received by the TextBox we check whether to accept it or not
        /// </summary>
        /// <param name="e"></param>
        //protected override void OnTextInput(System.Windows.Input.TextCompositionEventArgs e)
        //{
        //    string PreviousText = this.Text;
        //    if (NewTextIsOk)
        //    {
        //        base.OnTextInput(e);
        //        if (_mprovider.VerifyString(this.Text) == false) this.Text = PreviousText;
        //        while (!_mprovider.IsEditPosition(this.CaretIndex) && _mprovider.Length > this.CaretIndex) this.CaretIndex++;

        //    }
        //    else
        //        e.Handled = false; //true;
        //}



    }
}
