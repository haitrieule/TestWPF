using LiveCharts;
using LiveCharts.Wpf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace WpfApp1
{
    /// <summary>
    /// Interaction logic for doanhthu.xaml
    /// </summary>
    public partial class doanhthu : Window
    {
        /// <summary>
        /// Doanh thu
        /// </summary>
        public doanhthu()
        {
            InitializeComponent();
        }

        /// <summary>
        /// load_Click
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void load_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                MyStoreEntities4 db = new MyStoreEntities4();
                List<int?> values = new List<int?>();
                var query = from a in db.Purchases select new { a.Total };
                var b = query.SingleOrDefault().Total;
                values.Add(b);
                var series = new SeriesCollection()
            {
                new LineSeries()
                {
                    Values = new ChartValues<int?> (values)


                }
            };
                reportChart.Series = series;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }
    }
}
