using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
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
using MySql.Data.MySqlClient;

namespace WallysWorld
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        
        string connectionStinrg = "server=127.0.0.1; port=3306; username=root; database=dtwallysworld;";
        public MainWindow()
        {
            InitializeComponent();
        }



        private void Button_Click(object sender, RoutedEventArgs e)
        {
            MySqlConnection cnn;
            cnn = new MySqlConnection(connectionStinrg);
            cnn.Open();
            MySqlCommand command = cnn.CreateCommand();



            command.CommandText = "SELECT PersonID as Customer_Number, FirstName, LastName, Telephone FROM person where lastname='" + custInfo.Text + "' OR Telephone like '%" + custInfo.Text + "%' ";
            //command.Parameters.AddWithValue("@tele", custInfo.Text);
            //command.Parameters.AddWithValue("@last", custInfo.Text);

            var myCommand = new MySqlCommand(command.CommandText, cnn);
            var ds = new DataSet();
            MySqlDataAdapter mya = new MySqlDataAdapter(myCommand);
            mya.Fill(ds,"cusTable");
            dataGridCustomer.DataContext = ds;

            cnn.Close();

        }

        private void Button_Click_OpenAdd(object sender, RoutedEventArgs e)
        {
            if (addCus.Visibility == Visibility.Visible)
            {
                addCus.Visibility = Visibility.Collapsed;
                SearchOrder.Visibility = Visibility.Visible;
                UpdateLayout();
            }
            else
            {
                addCus.Visibility = Visibility.Visible;
                SearchOrder.Visibility = Visibility.Collapsed;
                UpdateLayout();
            }
        }

        private void Button_Click_OrderSearch(object sender, RoutedEventArgs e)
        {
            MySqlConnection cnn;
            cnn = new MySqlConnection(connectionStinrg);
            cnn.Open();
            string sql;

            sql = "SELECT * from Orders WHERE OrderID=@OrderID";
            var command = new MySqlCommand(sql, cnn);
            command.Parameters.AddWithValue("@OrderID", orderIDSearch.Text);
            var ds = new DataSet();
            MySqlDataAdapter mya = new MySqlDataAdapter(command);
            mya.Fill(ds, "orderTable");
            dataGridOrder.DataContext = ds;
            cnn.Close();
            

        }

        private void Button_Click_OpenSearch(object sender, RoutedEventArgs e)
        {
            if(SearchOrder.Visibility == Visibility.Visible)
            {
                SearchOrder.Visibility = Visibility.Collapsed;
                addCus.Visibility = Visibility.Visible;
                UpdateLayout();

            }
            else
            {
                SearchOrder.Visibility = Visibility.Visible;
                addCus.Visibility = Visibility.Collapsed;
                UpdateLayout();
            }
        }
    }
}
