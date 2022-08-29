using System;
using System.Windows;

namespace WpfApp1
{
    /// <summary>
    /// Interaction logic for AddEditProductWindow.xaml
    /// </summary>
    public partial class AddEditProductWindow : Window
    {
        public Product Product; 
        Product _producta;
        Product _product;
        Product _product2;
        Product _product3;
        Product _product4;
        Product _product5;
        Product _productanh;

        /// <summary>
        /// Add edit product window
        /// </summary>
        /// <param name="product">Product</param>
        /// /// <param name="status">Status</param>
        public AddEditProductWindow(Product product, string status)
        {
            InitializeComponent();

            if(status == "update")
            {
                idTextbox.IsEnabled = false;
                imageTextbox.IsEnabled = false;
            }
            else if(status == "insert")
            {
                idTextbox.IsEnabled = false;
                imageTextbox.IsEnabled = false;
            }
            try
            {
                _producta = new Product()
                {
                   CatId = product.CatId
                };
                _product = new Product()
                {
                    SKU = product.SKU
                };
                _product2 = new Product()
                {
                    Name = product.Name
                };
                _product3 = new Product()
                { 
                    Price = product.Price
                };
                _product4 = new Product()
                { 
                    Quantity= product.Quantity
                };
                _product5 = new Product()
                {
                    Description = product.Description
                };
                _productanh = new Product()
                {
                    Image = product.Image
                };
                
                idTextbox.DataContext = _producta;
                skuTextbox.DataContext = _product;
                nameTextbox.DataContext = _product2;
                priceTextbox.DataContext = _product3;
                quantityTextbox.DataContext = _product4;
                DescriptionTextbox.DataContext = _product5;
                imageTextbox.DataContext = _productanh;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        /// <summary>
        /// Button_Click
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Product = new Product()
            {
                CatId = 1,
                SKU = skuTextbox.Text,
                Name = nameTextbox.Text,
                Price = (string.IsNullOrEmpty(priceTextbox.Text) == false) ? Convert.ToInt32(priceTextbox.Text) : 0,
                Quantity = (string.IsNullOrEmpty(quantityTextbox.Text) == false) ? Convert.ToInt32(quantityTextbox.Text) : 0,
                Description = DescriptionTextbox.Text,
            };

            DialogResult = true;
        }
    }
}
