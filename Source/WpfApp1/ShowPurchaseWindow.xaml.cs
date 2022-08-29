using System.Linq;
using System.Windows;

namespace WpfApp1
{
    /// <summary>
    /// Interaction logic for ShowPurchaseWindow.xaml
    /// </summary>
    public partial class ShowPurchaseWindow : Window
    {
        /// <summary>
        /// ShowPurchaseWindow
        /// </summary>
        public ShowPurchaseWindow()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            MyStoreEntities4 db = new MyStoreEntities4();
            var query = (from a in db.Purchases select new { tel = a.Customer_Tel, Created_At = a.Created_At, Total = a.Total, Description = a.Status}).OrderByDescending(a=>a.Created_At).Take(3);
            purchaseDataGrid2.ItemsSource = query.ToList();
        }
    }
}
