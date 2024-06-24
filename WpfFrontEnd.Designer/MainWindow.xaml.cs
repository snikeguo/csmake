using System.Runtime.InteropServices;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WpfFrontEnd.Designer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            Menu menu = new Menu();
            var menuType = typeof(Menu);
            var properties=menuType.GetProperties();
            foreach (var property in properties)
            {
                var v=property.GetValue(menu);
                SelectedList<object> selectedList=v as SelectedList<object>;    
            }
            Test(GCHandle.);
        }
        int a = 5;
        private void Test(object t)
        {
            if(t==(object) a)
            {
                MessageBox.Show("111");
            }
        }
        private void TextBox_LostFocus(object sender, RoutedEventArgs e)
        {

        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

        }
    }
    public class A
    {
        public int a = 5;
        public void Test(object t)
        {
            if (t == (object)a)
            {
                MessageBox.Show("111");
            }
        }
    }
    public class SelectedList<T>
    {
        public T Selected { get; set; }
        public List<T> Items { get; set; }
    }
    public class Menu
    {
        public SelectedList<int> A { get; set; }=new SelectedList<int>() {  Items=new List<int>() { 1,2,3,4,5}, Selected=2 };
    }
}