using Aspose.Cells;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace WpfApp1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Fluent.RibbonWindow
    {
        public MainWindow()
        {
             InitializeComponent();
        }

        public class PagingInfo
        {
            /// <summary>Rows per page</summary>
            public int RowsPerPage { get; set; }

            /// <summary>Current page</summary>
            public int CurrentPage { get; set; }

            /// <summary>Total pages</summary>
            public int TotalPages { get; set; }

            /// <summary>Total  items</summary>
            public int TotalItems { get; set; }

            /// <summary>Pages</summary>
            public List<string> Pages
            {
                get
                {
                    var result = new List<string>();

                    for (var i = 1; i <= TotalPages; i++)
                    {
                        result.Add($"Trang {i} / {TotalPages}");
                    }

                    return result;
                }
            }
        }

        PagingInfo _pagingInfo;
        int rowsPerPage = 4;

        /// <summary>
        /// BackstageTabItem_MouseLeftButtonDown
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BackstageTabItem_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Application.Current.Shutdown();
        }

        /// <summary>
        /// RibbonWindow_Loaded
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void RibbonWindow_Loaded(object sender, RoutedEventArgs e)
        {
            loadingProgressBar.IsIndeterminate = true;

            await Task.Run(() => {
                System.Threading.Thread.Sleep(2000);
            });

            var db = new MyStoreEntities4();

             var categories = db.Categories.ToArray();
            categoriesComboBox.ItemsSource = categories;
            categoriesComboBox.SelectedIndex = 0;

            statusTextBlock.Text = "All products is loaded";
            loadingProgressBar.IsIndeterminate = false;

            var allPurchaseStatus = db.PurchaseStatusEnums.ToList();
            purchaseStatesComboBox.ItemsSource = allPurchaseStatus;
            purchaseStatesComboBox.SelectedIndex = 0;
            
            loadAllPurchases();
        }

        /// <summary>
        /// loadAllPurchases
        /// </summary>
        void loadAllPurchases()
        {
            int status = (purchaseStatesComboBox.SelectedItem as PurchaseStatusEnum).Value;
            var db = new MyStoreEntities4();
            IQueryable<Purchase> dateFilter;

            if (fromDatePicker.SelectedDate != null && toDatePicker.SelectedDate != null)
            {
                var from = (DateTime)fromDatePicker.SelectedDate;
                var to = (DateTime)toDatePicker.SelectedDate;
                dateFilter = db.Purchases.Where(p => (from <= p.Created_At) && (p.Created_At <= to));
            }
            else
            {
                dateFilter = db.Purchases;
            }
            
            var query = dateFilter.GroupJoin(db.Customers,
                p => p.Customer_Tel,
                c => c.Tel,
                (x, y) => new { Purchases = x, Customers = y }
                )
               .SelectMany(
                    x => x.Customers.DefaultIfEmpty(),
                    (x, y) => new { Purchase = x.Purchases, Customer = y }
                )
                .Select(item => new
                {
                    item.Purchase.Status,
                    item.Purchase.Purchase_ID,
                    item.Purchase.Total,
                    item.Purchase.Created_At,
                    item.Customer.Customer_Name,
                    item.Customer.Tel
                }).OrderByDescending(p => p.Created_At).Join(db.PurchaseStatusEnums,item=> item.Status, s => s.Value, (item, s) => new {
                    item.Status,
                    item.Purchase_ID,
                    item.Total,
                    item.Created_At,
                    item.Customer_Name,
                    item.Tel,
                    s.Description
                });

            if (status != -1)
            {
                var subquery = query.Where(p => p.Status == status);
                purchaseDataGrid.ItemsSource = subquery.ToList();
            }
            else
            {
                purchaseDataGrid.ItemsSource = query.ToList();
            }
        }

        /// <summary>
        /// ComboBox_SelectionChanged
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            loadingProgressBar.IsIndeterminate = true;
            await Task.Run(() => {
                System.Threading.Thread.Sleep(1000);
            });

            CalculatePagingInfo();
            UpdateProductView();

            loadingProgressBar.IsIndeterminate = false;
        }

        /// <summary>
        /// importexcel_Click
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void importexcel_Click(object sender, RoutedEventArgs e)
        {
            var screen = new OpenFileDialog();

            if (screen.ShowDialog() == true)
            {
                var filename = screen.FileName;
                var fileinfo = new FileInfo(filename);
                var excelFile = new Workbook(filename);
                var tabs = excelFile.Worksheets;
                var db = new MyStoreEntities4();

                foreach (var tab in tabs)
                {
                    var category = new Category()
                    {
                        Name = tab.Name
                    };
                    db.Categories.Add(category);
                    //db.SaveChanges();

                    var row = 3;
                    var cell = tab.Cells[$"C3"];

                    while (cell.Value != null)
                    {
                        var product = new Product()
                        {
                            SKU = tab.Cells[$"C{row}"].StringValue,
                            Name = tab.Cells[$"D{row}"].StringValue,
                            Price = tab.Cells[$"E{row}"].IntValue,
                            Quantity = tab.Cells[$"F{row}"].IntValue,
                            Description = tab.Cells[$"G{row}"].StringValue,
                            Image = tab.Cells[$"H{row}"].StringValue
                        };

                        category.Products.Add(product);
                        //db.SaveChanges();

                        //var imageName = tab.Cells[$"H{row}"].StringValue;
                        //var imageFull = $"{fileinfo.Directory}\\images\\{imageName}";
                        //var image = new BitmapImage(new Uri(imageFull, UriKind.Absolute));
                        //var encoder = new JpegBitmapEncoder();
                        //encoder.Frames.Add(BitmapFrame.Create(image));

                        //using (var stream = new MemoryStream())
                        //{
                        //    encoder.Save(stream);
                        //    var photo = new Photo()
                        //    {
                        //        Product_id = product.Id,
                        //        Data = stream.ToArray()
                        //    };
                        //    db.Photos.Add(photo);

                        //    db.SaveChanges();
                        //}

                        row++;
                        cell = tab.Cells[$"C{row}"];
                    }
                    db.SaveChanges();
                    MessageBox.Show("Data imported");
                }
            }
        }

        /// <summary>
        /// previousButton_Click
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void previousButton_Click(object sender, RoutedEventArgs e)
        {
            var currentIndex = pagingComboBox.SelectedIndex;
            if (currentIndex > 0)
            {
                pagingComboBox.SelectedIndex--;
            }
        }

        /// <summary>
        /// nextButton_Click
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void nextButton_Click(object sender, RoutedEventArgs e)
        {
            var currentIndex = pagingComboBox.SelectedIndex;
            if (currentIndex >= 0)
            {
                pagingComboBox.SelectedIndex++;
            }
        }

        /// <summary>
        /// Calculate paging info
        /// </summary>
        void CalculatePagingInfo()
        {
            var db = new MyStoreEntities4();
            var selectedCategory = categoriesComboBox.SelectedItem
                as Category;
            var products =
                db.Categories.Find(selectedCategory.Id)
                    .Products;
            var query = from product in products
                        select new
                        {
                            product_name = product.Name,
                            Thumbnail = product.Photos
                                .First().Data,

                        };

            // Tinh toan thong tin phan trang
            var count = query.Count();
            _pagingInfo = new PagingInfo()
            {
                RowsPerPage = rowsPerPage,
                TotalItems = count,
                TotalPages = count / rowsPerPage +
                    (((count % rowsPerPage) == 0) ? 0 : 1),
                CurrentPage = 1
            };

            pagingComboBox.ItemsSource = _pagingInfo.Pages;
            pagingComboBox.SelectedIndex = 0;

            statusTextBlock.Text = $"Tổng sản phẩm tìm thấy: {count} ";
        }

        /// <summary>
        /// Update product view
        /// </summary>
        void UpdateProductView()
        {
            var db = new MyStoreEntities4();
            var selectedCategory = categoriesComboBox.SelectedItem
                as Category;
            var products =
                db.Categories.Find(selectedCategory.Id)
                    .Products;
            var keyword = keywordTextBox.Text;
            var query = from product in products
                        where product.Name.ToLower().Contains(keyword.ToLower())
                        select new
                        {
                            product_name = product.Name,
                            Thumbnail = product.Photos
                                .First().Data
                        };
            var skip = (_pagingInfo.CurrentPage - 1) * _pagingInfo.RowsPerPage;
            var take = _pagingInfo.RowsPerPage;

            productsListView.ItemsSource = query.Skip(skip).Take(take);
            productsListView.SelectedIndex = 0;
        }

        /// <summary>
        /// keywordTextBox_TextChanged
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void keywordTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            CalculatePagingInfo();
            UpdateProductView();
        }

        /// <summary>
        /// pagingComboBox_SelectionChanged
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void pagingComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int nextPage = pagingComboBox.SelectedIndex + 1;
            _pagingInfo.CurrentPage = nextPage;

            UpdateProductView();
        }

        /// <summary>
        /// AddCategory_Click
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AddCategory_Click(object sender, RoutedEventArgs e)
        {
            var category = categoriesComboBox.SelectedItem as Category;
            var screen = new EditCategoryWindow(category);

            if (screen.ShowDialog() == true)
            {
                var add = screen.Category;
                var db = new MyStoreEntities4();

                db.Categories.Add(add);
                db.SaveChanges();

                CalculatePagingInfo();
                UpdateProductView();

                var categories = db.Categories.ToArray();

                categoriesComboBox.ItemsSource = categories;
                categoriesComboBox.SelectedIndex = 0;
            }
        }

        /// <summary>
        /// DeleteCategory_Click
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DeleteCategory_Click(object sender, RoutedEventArgs e)
        {
            var category = categoriesComboBox.SelectedItem as Category;
            var db = new MyStoreEntities4();
            var a = db.Categories.Where(c => c.Id == category.Id).Single();

            db.Categories.Remove(a);
            db.SaveChanges();

            var categories = db.Categories.ToArray();
            categoriesComboBox.ItemsSource = categories;
            categoriesComboBox.SelectedIndex = 0;

        }

        /// <summary>
        /// EditCategory_Click
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EditCategory_Click(object sender, RoutedEventArgs e)
        {
            var category = categoriesComboBox.SelectedItem as Category;
            var screen = new EditCategoryWindow(category);

            if (screen.ShowDialog() == true)
            {
                var add = screen.Category;
                var db = new MyStoreEntities4();
                var a = db.Categories.Where(c => c.Id == category.Id).Single();
                a.Name = add.Name;
                db.SaveChanges();

                CalculatePagingInfo();
                UpdateProductView();
                var categories = db.Categories.ToArray();
                categoriesComboBox.ItemsSource = categories;
                categoriesComboBox.SelectedIndex = 0;
            }
        }

        /// <summary>
        /// AddProduct_Click
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AddProduct_Click(object sender, RoutedEventArgs e)
        {
            var screen1 = new OpenFileDialog();
            if(screen1.ShowDialog() == true)
            {
                var filename = screen1.FileName;
                var fileinfo = new FileInfo(filename);
                try
                {
                    var db = new MyStoreEntities4();
                    var product = new Product();
                    product.Image = fileinfo.Name;
                    var screen = new AddEditProductWindow(product, "insert");
                    if (screen.ShowDialog() == true)
                    {
                        var add = screen.Product;
                        db = new MyStoreEntities4();
                        var a = new Product();
                        a.CatId = add.CatId;
                        a.Name = add.Name;
                        a.SKU = add.SKU;
                        a.Price = add.Price;
                        a.Quantity = add.Quantity;
                        a.Description = add.Description;
                        a.Image = fileinfo.Name;
                        db.Products.Add(a);
                        //db.SaveChanges();
                        var addPhoto = new Photo();
                        var c = db.Products.Local.Single();
                        addPhoto.Product_id = c.Id;

                        var imageName = c.Image;
                        var imageFull = $"{fileinfo.Directory}\\{imageName}";
                        var image = new BitmapImage(new Uri(imageFull, UriKind.Absolute));
                        var encoder = new JpegBitmapEncoder();
                        encoder.Frames.Add(BitmapFrame.Create(image));

                        using (var stream = new MemoryStream())
                        {
                            encoder.Save(stream);
                            var photo = new Photo()
                            {
                                Product_id = c.Id,
                                Data = stream.ToArray()
                            };
                            db.Photos.Add(photo);

                            db.SaveChanges();
                        }

                        CalculatePagingInfo();
                        UpdateProductView();

                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                }
            }
        }

        int a;
        int b;

        /// <summary>
        /// Save changed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void savechanged(object sender, SelectionChangedEventArgs e)
        {
            a = productsListView.SelectedIndex;
            if (a != -1) {
                b = a;
            }
        }

        /// <summary>
        /// EditProduct_Click
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EditProduct_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var db = new MyStoreEntities4();
                var selectedCategory = categoriesComboBox.SelectedItem
                    as Category;
                var products =
                    db.Categories.Find(selectedCategory.Id)
                        .Products;
                var keyword = keywordTextBox.Text;
                var query1 = from producta in products
                             where producta.Name.ToLower().Contains(keyword.ToLower())
                             select producta;
                var skip = (_pagingInfo.CurrentPage - 1) * _pagingInfo.RowsPerPage;
                var take = _pagingInfo.RowsPerPage;
                productsListView.ItemsSource = query1.Skip(skip).Take(take);
                productsListView.SelectedIndex = b;
                var product = productsListView.SelectedItem as Product;
                var screen = new AddEditProductWindow(product, "update");

                if (screen.ShowDialog() == true)
                {
                    var add = screen.Product;
                    db = new MyStoreEntities4();
                    var a = db.Products.Where(c => c.Id == product.Id).Single();
                    a.Name = add.Name;
                    a.SKU = add.SKU;
                    a.Price = add.Price;
                    a.Quantity = add.Quantity;
                    a.Description = add.Description;
                    a.Image = add.Image;
                    db.SaveChanges();

                    CalculatePagingInfo();
                    UpdateProductView();

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        /// <summary>
        /// DeleteProduct_Click
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DeleteProduct_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var db = new MyStoreEntities4();
                var selectedCategory = categoriesComboBox.SelectedItem
                    as Category;
                var products =
                    db.Categories.Find(selectedCategory.Id)
                        .Products;
                var keyword = keywordTextBox.Text;
                var query1 = from producta in products
                             where producta.Name.ToLower().Contains(keyword.ToLower())
                             select producta;
                var skip = (_pagingInfo.CurrentPage - 1) * _pagingInfo.RowsPerPage;
                var take = _pagingInfo.RowsPerPage;
                productsListView.ItemsSource = query1.Skip(skip).Take(take);
                productsListView.SelectedIndex = b;
                var product = productsListView.SelectedItem as Product;
                var a = db.Products.Where(c => c.Id == product.Id).Single();
                var photo = db.Photos.Where(c => c.Product_id == product.Id).Single();

                db.Photos.Remove(photo);
                db.Products.Remove(a);
                db.SaveChanges();

                CalculatePagingInfo();
                UpdateProductView();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        /// <summary>
        /// addPurchaseButton_Click
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void addPurchaseButton_Click(object sender, RoutedEventArgs e)
        {
            var screen = new AddPurchaseWindow();

            if (screen.ShowDialog() == true)
            {
                loadAllPurchases();
            }
        }

        /// <summary>
        /// editPurchase_MenuItem_Click
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void editPurchase_MenuItem_Click(object sender, RoutedEventArgs e)
        {

            try
            {
                var db = new MyStoreEntities4();
                dynamic item = purchaseDataGrid.SelectedItem;
                var purchase = db.Purchases.Find(item.Purchase_ID);
                db.Purchases.Remove(purchase);
                db.SaveChanges();

                // Reload giao dien
                loadAllPurchases();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }

            var screen = new AddPurchaseWindow();

            if (screen.ShowDialog() == true)
            {
                loadAllPurchases();
            }
            MessageBox.Show("sửa dữ liệu thành công");
        }

        /// <summary>
        /// deletePurchase_MenuItem_Click
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void deletePurchase_MenuItem_Click(object sender, RoutedEventArgs e)
        {
            try {
                var db = new MyStoreEntities4();
                dynamic item = purchaseDataGrid.SelectedItem;
                var purchase = db.Purchases.Find(item.Purchase_ID);
                db.Purchases.Remove(purchase);
                db.SaveChanges();

                // Reload giao dien
                loadAllPurchases();
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }


            // Thong bao phan hoi
            MessageBox.Show("Đơn hàng đã được xóa thành công");
        }

        /// <summary>
        /// purchaseStatesComboBox_SelectionChanged
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void purchaseStatesComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            loadAllPurchases();
        }

        /// <summary>
        /// fromDatePicker_SelectedDateChanged
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void fromDatePicker_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        /// <summary>
        /// toDatePicker_SelectedDateChanged
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toDatePicker_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            if (fromDatePicker.SelectedDate != null)
            {
                var from = (DateTime)fromDatePicker.SelectedDate;
                var to = (DateTime)toDatePicker.SelectedDate;

                if (DateTime.Compare(from, to) < 0)
                {
                    loadAllPurchases();
                }
                else
                {
                    MessageBox.Show("Ngày kết thúc phải lớn hơn ngày bát đầu.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                MessageBox.Show("Vui lòng chọn ngày bắt đầu.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }

        /// <summary>
        /// TabControl_SelectionChanged
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        /// <summary>
        /// ConHang_Click
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ConHang_Click(object sender, RoutedEventArgs e)
        {
            var db = new MyStoreEntities4();
            //var query = from a in db.Products join b in db.PurchaseDetails on a.Id equals b.Product_ID where a.Quantity > b.Quantity select new {name = a.Name, SLCL = a.Quantity-b.Quantity };
            var query1 = from a in db.Products where a.Quantity > 0 select new { name = a.Name, SLCL = a.Quantity };
            purchaseDataGrid1.ItemsSource = query1.ToList();
        }

        /// <summary>
        /// SapHetHang_Click
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SapHetHang_Click(object sender, RoutedEventArgs e)
        {
            var db = new MyStoreEntities4();
            //var query = from a in db.Products join b in db.PurchaseDetails on a.Id equals b.Product_ID where a.Quantity - b.Quantity<5 && a.Quantity - b.Quantity > 0 select new { name = a.Name, SLCL = a.Quantity - b.Quantity };
            var query1 = from a in db.Products where a.Quantity < 5 select new { name = a.Name, SLCL = a.Quantity };

            purchaseDataGrid1.ItemsSource = query1.ToList();

        }

        /// <summary>
        /// DonHangGanNhat_Click
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DonHangGanNhat_Click(object sender, RoutedEventArgs e)
        {
            var screen = new ShowPurchaseWindow();
            screen.ShowDialog();
        }

        /// <summary>
        /// DoanhThu_Click
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DoanhThu_Click(object sender, RoutedEventArgs e)
        {
            var screen = new doanhthu();
            screen.ShowDialog();
        }

        /// <summary>
        /// BanChay_Click
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BanChay_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
