using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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

namespace MAction.WpfCustomControlLibrary
{
    /// <summary>
    /// Follow steps 1a or 1b and then 2 to use this custom control in a XAML file.
    ///
    /// Step 1a) Using this custom control in a XAML file that exists in the current project.
    /// Add this XmlNamespace attribute to the root element of the markup file where it is 
    /// to be used:
    ///
    ///     xmlns:MyNamespace="clr-namespace:WpfApp1"
    ///
    ///
    /// Step 1b) Using this custom control in a XAML file that exists in a different project.
    /// Add this XmlNamespace attribute to the root element of the markup file where it is 
    /// to be used:
    ///
    ///     xmlns:MyNamespace="clr-namespace:WpfApp1;assembly=WpfApp1"
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
    ///     <MyNamespace:MyCustomControl/>
    ///
    /// </summary>
    public class MyRichTextBox : //RichTextBox
                                 FlowDocumentScrollViewer
    {
        static MyRichTextBox()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(MyRichTextBox), new FrameworkPropertyMetadata(typeof(MyRichTextBox)));
        }



        public string ContentText
        {
            get { return (string)GetValue(ContentTextProperty); }
            set { SetValue(ContentTextProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ContentText.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ContentTextProperty =
            DependencyProperty.Register("ContentText", typeof(string), typeof(MyRichTextBox), new PropertyMetadata(propertyChangedCallback: ContentTextChangeCallBack));

        public static void ContentTextChangeCallBack(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var obj = d as FlowDocumentScrollViewer;
            obj.Document = obj.Document ?? new FlowDocument();
            obj.Document.Blocks.Clear();
            var text = e.NewValue.ToString();
            Paragraph p = new Paragraph();
            p.FontFamily = obj.FontFamily;
            p.FontSize = obj.FontSize;
            p.FontStyle = obj.FontStyle;

            foreach (var t in text.Split('\n'))
            {
                Regex regex = new Regex(@"(http|ftp|https):\/\/([\w_-]+(?:(?:\.[\w_-]+)+))([\w.,@?^=%&:\/~+#-]*[\w@?^=%&\/~+#-])", RegexOptions.IgnoreCase);
                var match = regex.Matches(t);
                if (!match.OfType<Match>().Any(x => x.Success))
                {
                    //obj.Document.Blocks.Add(new Paragraph(new Run(t)));
                    p.Inlines.Add(t);
                }
                else
                {
                    int index = 0;
                    for (int i = 0; i < match.Count; i++)
                    {
                        p.Inlines.Add(t.Substring(index, match[i].Index - index));
                        index = match[i].Index + match[i].Length;
                        Run run2 = new Run(match[i].Groups[2].Value +
                            (match[i].Groups[3].Value[0] == '/' || match[i].Groups[3].Value[0] == '?' || match[i].Groups[3].Value[0] == ':' ? "" : match[i].Groups[3].Value));
                        //Paragraph paragraph = new Paragraph();
                        Hyperlink hlink = new Hyperlink(run2);
                        hlink.NavigateUri = new Uri(match[i].Value);
                        hlink.FontFamily = obj.FontFamily;
                        hlink.FontSize = obj.FontSize;
                        hlink.FontWeight = obj.FontWeight;
                        hlink.FontStyle = obj.FontStyle;
                        hlink.ToolTip = match[i].Value;
                        hlink.RequestNavigate += Hlink_RequestNavigate;
                        p.Inlines.Add(hlink);
                        //paragraph.Inlines.Add(hlink);

                    }
                }
                p.Inlines.Add(new LineBreak());
            }
            obj.Document.Blocks.Add(p);
            //obj.Document.Blocks.Add(paragraph);
        }

        private static void Hlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            Process.Start(new ProcessStartInfo { FileName = e.Uri.AbsoluteUri, UseShellExecute = true });
            e.Handled = true;
        }
    }
}
