using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Policy;
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

namespace WpfApp1
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class Window1 : Window
    {
        public Window1()
        {
            InitializeComponent();

            FlowDocumentScrollViewer fdsv = new FlowDocumentScrollViewer();

            FlowDocument doc = new FlowDocument();
            fdsv.Document = doc;
            fdsv.VerticalScrollBarVisibility = ScrollBarVisibility.Hidden;
            doc.PagePadding = new Thickness(0);
            Paragraph paragraph = new Paragraph();
            doc.Blocks.Add(paragraph);

            Run run = new Run("this is flow document text and ");
            paragraph.Inlines.Add(run);

            Run run2 = new Run("this is a hyperlink");
            Hyperlink hlink = new Hyperlink(run2);
            hlink.NavigateUri = new Uri("http://www.google.com");
            hlink.RequestNavigate += Hlink_RequestNavigate;
            paragraph.Inlines.Add(hlink);

            StackPanel sp = new StackPanel();
            TextBlock tb = new TextBlock();
            tb.Text = "this is textblock text";
            sp.Children.Add(tb);
            sp.Children.Add(fdsv);
            fdsv.Background = new SolidColorBrush(Colors.Fuchsia);

            this.Content = sp;
        }

        private static void Hlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            //Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri));
            Process.Start(new ProcessStartInfo { FileName = e.Uri.AbsoluteUri, UseShellExecute = true });
            e.Handled = true;
        }
    }
}
