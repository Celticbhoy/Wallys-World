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
                
                UpdateLayout();
            }
            else
            {
                addCus.Visibility = Visibility.Visible;
                
                UpdateLayout();
            }
        }

        private void Button_Click_OrderSearch(object sender, RoutedEventArgs e)
        {
            MySqlConnection cnn;
            cnn = new MySqlConnection(connectionStinrg);
            cnn.Open();
            string sql;

            sql = "select o.orderid as OrderID, o.customerid as CustomerID, p.FirstName as FirstName, p.lastName as LastName, o.orderDate AS Date, o.orderStatus As Status from orders o" +
                " inner join customer c on o.customerid = c.customerid" +
                " inner join person p" +
                " on c.personid = p.personid " +
                "where o.orderid = @OrderID; ";
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
                
                UpdateLayout();

            }
            else
            {
                SearchOrder.Visibility = Visibility.Visible;
             
                UpdateLayout();
            }
        }

        private void Button_Click_ShowInv(object sender, RoutedEventArgs e)
        {
            MySqlConnection cnn;
            cnn = new MySqlConnection(connectionStinrg);
            cnn.Open();
            string sql;

            sql = "SELECT * FROM Item;";
            var command = new MySqlCommand(sql, cnn);
            var ds = new DataSet();
            MySqlDataAdapter mya = new MySqlDataAdapter(command);
            mya.Fill(ds, "orderTable");
            dataGridOrder.DataContext = ds;
            cnn.Close();

        }

        private void Button_Click_OpenInv(object sender, RoutedEventArgs e)
        {
            if(ShowInv.Visibility == Visibility.Visible)
            {
                ShowInv.Visibility = Visibility.Collapsed;
                UpdateLayout();
            }
            else
            {
                ShowInv.Visibility = Visibility.Visible;
                
                UpdateLayout();
            }
        }

        private void Button_Click_CreateOrder(object sender, RoutedEventArgs e)
        {
            if(newOrder.Visibility== Visibility.Visible)
            {
                newOrder.Visibility = Visibility.Collapsed;
                UpdateLayout();
            }
            else
            {
                newOrder.Visibility = Visibility.Visible;
                UpdateLayout();
            }
        }

        private void Button_Click_OpenRefund(object sender, RoutedEventArgs e)
        {
            if(refundOrder.Visibility == Visibility.Visible)
            {
                refundOrder.Visibility = Visibility.Collapsed;
                UpdateLayout();
            }
            else
            {
                refundOrder.Visibility = Visibility.Visible;
                UpdateLayout();
            }
        }

        private void Button_Click_CompleteOrder(object sender, RoutedEventArgs e)
        {

        }

        private void Button_Click_AddCus(object sender, RoutedEventArgs e)
        {
            if (newCusFirst.Text == "" || newCusLast.Text == "" || newCustDOB.Text == "" || newCusTele.Text == "")
            {
                MessageBox.Show("Ensure all entries are filled", "Adding New Customer");
            }
            else
            { 
             MySqlConnection cnn;

            cnn = new MySqlConnection(connectionStinrg);
            cnn.Open();
            string sql;
            sql = "INSERT INTO Person (FirstName, LastName, DateofBirth, Telephone) VALUES (@FirstName, @LastName, @DOB, @Telephone);";

            var command = new MySqlCommand(sql, cnn);
            command.Parameters.AddWithValue("@FirstName", newCusFirst.Text);
            command.Parameters.AddWithValue("@LastName", newCusLast.Text);
            command.Parameters.AddWithValue("@DOB", newCustDOB.Text);
            command.Parameters.AddWithValue("@Telephone", newCusTele);
  
            cnn.Close();
        }
        }
    }
}
