using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Text.RegularExpressions;
using System.Collections.Generic;

namespace AssetManager.UserControls
{
    /// <summary>
    /// Interaction logic for MaskedTextBox.xaml
    /// </summary>
    public partial class MaskedTextBox : UserControl
    {
        List<char> reservedchars = new List<char>() { 'L', '0' };

        public MaskedTextBox()
        {
            InitializeComponent();
        }

        public readonly static DependencyProperty ClearProperty = DependencyProperty.Register("Clear", typeof(bool), typeof(MaskedTextBox), new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, ClearChanged));

        public bool Clear
        {
            get { return (bool)GetValue(ClearProperty); }
            set { SetValue(ClearProperty, value); }
        }

        private static void ClearChanged(DependencyObject target, DependencyPropertyChangedEventArgs e)
        {
            ((MaskedTextBox)target).Clear = (bool)e.NewValue;

            if ((bool)e.NewValue == true)
            {
                ((MaskedTextBox)target).DrawMask();
            }
        }

        public readonly static DependencyProperty TextProperty = DependencyProperty.Register("Text", typeof(string), typeof(MaskedTextBox), new FrameworkPropertyMetadata(string.Empty, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, TextChanged));

        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        private static void TextChanged(DependencyObject target, DependencyPropertyChangedEventArgs e)
        {
            //((MaskedTextBox)target).Text = (string)e.NewValue;
            ((MaskedTextBox)target).tb.Text = (string)e.NewValue;
        }       
       
        public static readonly DependencyProperty PlaceholderProperty = DependencyProperty.Register("Placeholder", typeof(char), typeof(MaskedTextBox),
            new UIPropertyMetadata('_', OnPlaceholderChange));

        public char Placeholder
        {
            get { return (char)GetValue(PlaceholderProperty); }
            set { SetValue(PlaceholderProperty, value); }
        }

        private static void OnPlaceholderChange(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((MaskedTextBox)d).Placeholder = (char)e.NewValue;
            ((MaskedTextBox)d).DrawMask();
        }

        public static readonly DependencyProperty MaskProperty = DependencyProperty.Register("Mask", typeof(string), typeof(MaskedTextBox),
            new FrameworkPropertyMetadata(string.Empty, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnMaskChange));

        private static void OnMaskChange(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((MaskedTextBox)d).Mask = (string)e.NewValue;
            ((MaskedTextBox)d).DrawMask();
        }

        public string Mask
        {
            get { return (string)GetValue(MaskProperty); }
            set { SetValue(MaskProperty, value); }
        }

        private void DrawMask()
        {
            string str = Mask.Replace('L', Placeholder);
            tb.Text = str.Replace('0', Placeholder);                       
        }

        private string GenerateRegEx()
        {
            if (!string.IsNullOrEmpty(Mask))
            {
                string regex = string.Empty;
                char[] ch = Mask.ToCharArray();
                for (int i = 0; i < ch.Length; i++)
                {
                    if (ch[i] == 'L')
                        regex = regex + @"[A-Z|{}" + Regex.Escape(Placeholder.ToString()) + @"]";
                    else
                    if (ch[i] == '0')
                        regex = regex + @"[0-9|{}" + Regex.Escape(Placeholder.ToString()) + @"]";
                    else
                        regex = regex + Regex.Escape(ch[i].ToString());
                }
                return "^" + regex + "$";
            }
            else
                return string.Empty;
        }

        private void Tb_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (!string.IsNullOrEmpty(Mask))
            {
                char[] ch = tb.Text.ToCharArray();
                ch[tb.CaretIndex] = char.Parse(e.Text);
                string txt = new string(ch);
                e.Handled = !Regex.IsMatch(txt, GenerateRegEx());
            }
            base.OnPreviewTextInput(e);
        }

        private void Tb_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (!string.IsNullOrEmpty(Mask))
            {
                OnForceOverride(sender, e);
            }
        }

        private int NextChar(ref TextBox tb)
        {
            int caretindx = tb.CaretIndex;
            while(caretindx < Mask.Length)
            {
                if (!reservedchars.Contains(Mask[caretindx]))                
                    caretindx++;                                  
                else                
                    break;                                    
            }                                 
            return caretindx;
        }

        private int PrevChar(ref TextBox tb)
        {
            int caretindx = tb.CaretIndex;
            while (caretindx >= 0)
            {               
                if(caretindx == 0)                
                    break;
                
                if (reservedchars.Contains(Mask[caretindx-1]))
                   break; 
                caretindx--;
            }            
            return caretindx;
        }

        private void OnForceOverride(object sender, KeyEventArgs e)
        {
            TextBox t = (TextBox)sender;
            switch (e.Key)
            {
                case Key.Delete:
                    if (t.CaretIndex < tb.Text.Length)
                    {
                        char[] ch = t.Text.ToCharArray();
                        int idx = t.CaretIndex;
                        int newidx = t.CaretIndex + 1;
                        if (reservedchars.Contains(Mask[idx]))
                            ch[idx] = Placeholder; 

                        t.Text = new string(ch);
                        t.CaretIndex = newidx;
                    }
                    e.Handled = true;
                    break;

                case Key.Back:
                    if (t.CaretIndex > 0)
                    {
                        char[] ch = t.Text.ToCharArray();
                        int newidx = t.CaretIndex - 1;
                        if (reservedchars.Contains(Mask[newidx]))                        
                            ch[newidx] = Placeholder;
                                                
                        t.Text = new string(ch);                       
                        t.CaretIndex = newidx;
                    }
                    e.Handled = true;
                    break;

                case Key.Left:
                    break;
                case Key.Up:
                    break;
                case Key.Right:
                    break;
                case Key.Down:
                    break;
                case Key.Enter:
                    break;

                default:
                    if (!string.IsNullOrEmpty(t.Text))
                    {
                        if (t.CaretIndex < tb.Text.Length)
                        {
                            t.CaretIndex = NextChar(ref t);
                            t.Select(t.CaretIndex, 1);
                        }
                        else
                            e.Handled = true;
                    }
                    break;
            }
        }

        //private void OLDOnForceOverride(object sender, KeyEventArgs e)
        //{
        //    TextBox t = (TextBox)sender;
        //    switch (e.Key)
        //    {
        //        case Key.Delete:
        //            if (t.CaretIndex < tb.Text.Length)
        //            {
        //                char[] ch = t.Text.ToCharArray();
        //                int idx = t.CaretIndex;
        //                int newidx = t.CaretIndex + 1;

        //                if (ch[idx] != Delimiter)
        //                    ch[idx] = Placeholder; // index starts at 0!

        //                if (newidx < t.Text.Length)
        //                {
        //                    if (t.Text[newidx] == Delimiter) 
        //                        newidx++;
        //                }
        //                else
        //                    newidx--;

        //                t.Text = new string(ch);
        //                t.CaretIndex = newidx;                       
        //            }
        //            e.Handled = true;

        //            break;

        //        case Key.Back:                                        
        //            if (t.CaretIndex > 0)
        //            {
        //                char[] ch = t.Text.ToCharArray();
        //                int idx = t.CaretIndex;
        //                int newidx = t.CaretIndex - 1;

        //                if (ch[newidx] == Delimiter)
        //                    ch[newidx] = Delimiter;
        //                else
        //                    ch[newidx] = Placeholder; // index starts at 0!

        //                t.Text = new string(ch);

        //                t.CaretIndex = newidx;
        //            }                    
        //            e.Handled = true;

        //            break;
                                                       
        //            case Key.Left:
        //                break;
        //            case Key.Up:
        //                break;
        //            case Key.Right:
        //                break;
        //            case Key.Down:
        //                break;
        //            case Key.Enter:
        //                break;

        //            default:                        
        //                if (!string.IsNullOrEmpty(t.Text))
        //                {
        //                    if (t.CaretIndex < tb.Text.Length)
        //                    {
        //                        if (t.Text[t.CaretIndex] == Delimiter)
        //                            t.Select(t.CaretIndex + 1, 1);
        //                        else
        //                            t.Select(t.CaretIndex, 1);
        //                    }                     
        //                else
        //                    e.Handled = true;
        //            }                   
        //            break;
        //    }
        //}

        private void Tb_TextChanged(object sender, TextChangedEventArgs e)
        {           
            Text = tb.Text;
        }

        private void Tb_PreviewKeyUp(object sender, KeyEventArgs e)
        {
            TextBox t = (TextBox)sender;
            switch (e.Key)
            {
                case Key.Delete:
                    t.CaretIndex = NextChar(ref t);                  
                    break;

                case Key.Back:                    
                        t.CaretIndex = PrevChar(ref t);                                   
                    break;

                case Key.Left:
                    t.CaretIndex = PrevChar(ref t);
                    break;
                case Key.Up:
                    break;
                case Key.Right:
                    t.CaretIndex = NextChar(ref t);
                    break;
                case Key.Down:
                    break;
                case Key.Enter:
                    break;

                default:
                    if (!string.IsNullOrEmpty(t.Text))
                    {
                        if (t.CaretIndex < tb.Text.Length)                        
                            t.CaretIndex = NextChar(ref t);                        
                        else
                            e.Handled = true;
                    }
                    break;
            }
        }
    }
}
