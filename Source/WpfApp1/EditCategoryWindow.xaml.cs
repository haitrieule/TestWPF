using System.Windows;

namespace WpfApp1
{
    /// <summary>
    /// Interaction logic for EditCategoryWindow.xaml
    /// </summary>
    public partial class EditCategoryWindow : Window
    {
        public Category Category;
        Category _category;

        /// <summary>
        /// Edit category window
        /// </summary>
        /// <param name="category"></param>
        public EditCategoryWindow(Category category)
        {
            InitializeComponent();
            _category = new Category()
            {
                Name = category.Name
            };
            nameTextBox.DataContext = _category;
        }

        /// <summary>
        /// Submit_Click
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Submit_Click(object sender, RoutedEventArgs e)
        {
            Category = new Category()
            {
                Name = _category.Name
            };

            DialogResult = true;
        }
    }
}
