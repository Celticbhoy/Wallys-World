using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
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
        int i = 0;   
        string connectionStinrg = "server=127.0.0.1; port=3306; username=root; password=Conestoga1;database=dtwallysworld;";
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



            command.CommandText = "SELECT CustomerID as Customer_Number, FirstName, LastName, Telephone FROM Customer where lastname='" + custInfo.Text + "' OR Telephone like '%" + custInfo.Text + "%' ";


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
                
                ShowInv.Visibility = Visibility.Collapsed;
                SearchOrder.Visibility = Visibility.Collapsed;
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

            sql = "select o.orderid as OrderID, o.customerid as CustomerID, c.FirstName as FirstName, c.lastName as LastName, o.orderDate AS Date, o.orderStatus As Status from orders o" +
                " inner join customer c on o.customerid = c.customerid" +
                " where o.orderid = @OrderID; ";
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
                
                addCus.Visibility = Visibility.Collapsed;
                ShowInv.Visibility = Visibility.Collapsed;
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

            sql = "SELECT * FROM item;";
            var command = new MySqlCommand(sql, cnn);
            var ds = new DataSet();
            MySqlDataAdapter mya = new MySqlDataAdapter(command);
            mya.Fill(ds, "invTable");
            dataGridOrder1.DataContext = ds;
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
            int i = 0;

            dataGridCurrentOrder.DataContext = null;
            MySqlConnection cnn;
            cnn = new MySqlConnection(connectionStinrg);
            cnn.Open();

            var currOrder = cnn.CreateCommand();
            currOrder.CommandText = "SELECT MAX(OrderID) from orders;";
            var OrderID = Convert.ToInt32(currOrder.ExecuteScalar());
            currOrder.Dispose();

            cnn = new MySqlConnection(connectionStinrg);
            cnn.Open();
            MySqlCommand command = cnn.CreateCommand();
            command.CommandText = "SELECT sum(sPrice) from OrderLine where orderid = @orderid;";
            command.Parameters.AddWithValue("@orderid", OrderID);
            var subtotal = Convert.ToDouble(command.ExecuteScalar());
            var saletotal = subtotal * 1.13;

            MySqlCommand commandCusName = cnn.CreateCommand();
            commandCusName.CommandText = "SELECT FirstName, LastName from Customer where CustomerID = @CustomerID;";
            commandCusName.Parameters.AddWithValue("@CustomerID", orderCusID.Text);
            MySqlDataReader dr = commandCusName.ExecuteReader();
            var cusName = "";
            while (dr.Read())
            {
                int j = 0;
                while (j < dr.FieldCount)
                {
                    cusName = cusName + dr.GetValue(j) + " ";
                    j++;

                }

            }
            commandCusName.Dispose();

            MySqlCommand orderDetails = cnn.CreateCommand();
            orderDetails.CommandText = "SELECT  i.name, ol.orderquantity, ol.sprice from orderline ol "+
                                        "inner join item i on ol.itemid = i.itemid " +
                                        "where ol.orderid = @OrderID";
            orderDetails.Parameters.AddWithValue("@OrderID", OrderID);
            dr = orderDetails.ExecuteReader();
            List<string> orderInfo = new List<string>();
            int k = 0;
            while (dr.Read())
            {
                int j = 0;
                while(j < dr.FieldCount)
                {
                    
                    orderInfo.Add(Convert.ToString(dr.GetValue(j)));
                    j++;

                }
                k++;

            }

            string path = "Wally'sWorldSalesRecord" + OrderID + ".txt";
            if (!File.Exists(path))
            {
                using (StreamWriter sw = File.CreateText(path))
                {
                    sw.WriteLine("Thank you for shopping at Wally's World \nOn " + DateTime.Now.ToString("yyyy-MM-dd") +" , "+ cusName + "\n" +
                        "Order ID: " + OrderID + " \n");
                    for(int j = 0; j < orderInfo.Count;  j = j+3){
                        sw.WriteLine("" + orderInfo[j] + " " + orderInfo[j + 1] + " x " + orderInfo[j + 2] + " = " + Convert.ToDouble(orderInfo[j + 1]) * Convert.ToDouble(orderInfo[j + 2]) + " \n");
                    }
                    sw.WriteLine("Subtotal = $" + subtotal + "\n");
                    sw.WriteLine("HST(13%) = $" + subtotal * .13 + "\n");
                    sw.WriteLine("SaleTotal = $" + saletotal + "\n");
                    sw.WriteLine("Paid -- Thank You");


                }

            }

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
            MySqlCommand command = cnn.CreateCommand();
            command.CommandText = "INSERT INTO Customer (FirstName, LastName, DateofBirth, Telephone) VALUES (@FirstName, @LastName, @DOB, @Telephone);";

            
            command.Parameters.AddWithValue("@FirstName", newCusFirst.Text);
            command.Parameters.AddWithValue("@LastName", newCusLast.Text);
            command.Parameters.AddWithValue("@DOB", newCustDOB.Text);
            command.Parameters.AddWithValue("@Telephone", newCusTele.Text);
                command.ExecuteNonQuery();
  
            cnn.Close();
        }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            
            MySqlConnection cnn;
            cnn = new MySqlConnection(connectionStinrg);
            cnn.Open();
            MySqlCommand command = cnn.CreateCommand();
            command.CommandText = "SELECT stock from item where itemid = @ItemID;";
            command.Parameters.AddWithValue("@ItemID",itemid.Text);
            var val =  Convert.ToDouble(command.ExecuteScalar());

            command.CommandText = "Select wprice from item where itemid = @itemID";
            var price = Convert.ToDouble(command.ExecuteScalar());

            var conval = Convert.ToDouble(val);
            var oq = Convert.ToDouble(orderQuantity.Text);
            
            if (val - oq < 0)
            {
                MessageBox.Show("Invalid Quantity - Check Inventory", "Order Error");
                return;
            }
            else
            {
                var stockcmd = cnn.CreateCommand();
                var stock = val - oq;
                stockcmd.CommandText = "Update Item SET stock = @stock WHERE ItemID = @ItemID";
                stockcmd.Parameters.AddWithValue("@ItemID", itemid.Text);
                stockcmd.Parameters.AddWithValue("@stock", stock);
                stockcmd.ExecuteNonQuery();

                stockcmd.Dispose();
                
            }

            if(i == 0)
            {
                var createOrderCmd = cnn.CreateCommand();
                createOrderCmd.CommandText = "INSERT INTO Orders (CustomerID, BranchID, OrderDate, OrderStatus) VALUES (@CustomerID, @BranchID, @OrderDate, @OrderStatus);";
                createOrderCmd.Parameters.AddWithValue("@CustomerID",orderCusID.Text);
                createOrderCmd.Parameters.AddWithValue("@BranchID", orderCusID.Text);
                createOrderCmd.Parameters.AddWithValue("@OrderDate", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                createOrderCmd.Parameters.AddWithValue("@OrderStatus", "PAID");
                createOrderCmd.ExecuteNonQuery();
                createOrderCmd.Dispose();

                var currOrder = cnn.CreateCommand();
                currOrder.CommandText = "SELECT MAX(OrderID) from orders;";
                var OrderID = Convert.ToInt32(currOrder.ExecuteScalar());
                currOrder.Dispose();

                var updateOrder = cnn.CreateCommand();
                updateOrder.CommandText ="INSERT INTO Orderline(OrderID, ItemID, OrderQuantity, sPrice) VALUES (@OrderID, @ItemID, @OrderQuantity, @sPrice);";
                updateOrder.Parameters.AddWithValue("@OrderID", OrderID);
                updateOrder.Parameters.AddWithValue("@ItemID", itemid.Text);
                updateOrder.Parameters.AddWithValue("@OrderQuantity", orderQuantity.Text);
                updateOrder.Parameters.AddWithValue("@sPrice", (price * 1.4));
                updateOrder.ExecuteNonQuery();
                updateOrder.Dispose();

                var orderState = cnn.CreateCommand();
                orderState.CommandText = "Select * from orderline where orderid = @orderid";
                orderState.Parameters.AddWithValue("@orderid", OrderID);
                var ds = new DataSet();
                MySqlDataAdapter mya = new MySqlDataAdapter(orderState);
                mya.Fill(ds, "currentOrder");
                dataGridCurrentOrder.DataContext = ds;

                command.Dispose();
                cnn.Close();
                i++;
            }
            else
            {
                var currOrder = cnn.CreateCommand();
                currOrder.CommandText = "SELECT MAX(OrderID) from Orders;";
                var OrderID = Convert.ToInt32(currOrder.ExecuteScalar());
                currOrder.Dispose();


                var updateOrder = cnn.CreateCommand();
                updateOrder.CommandText = "INSERT INTO Orderline(OrderID, ItemID, OrderQuantity, sPrice) VALUES (@OrderID, @ItemID, @OrderQuantity, @sPrice);";
                updateOrder.Parameters.AddWithValue(@"OrderID", OrderID);
                updateOrder.Parameters.AddWithValue("@ItemID", itemid.Text);
                updateOrder.Parameters.AddWithValue("@OrderQuantity", orderQuantity.Text);
                updateOrder.Parameters.AddWithValue("@sPrice", price * 1.4);
                updateOrder.ExecuteNonQuery();

                var orderState = cnn.CreateCommand();
                orderState.CommandText = "Select * from orderline where orderid = @orderid";
                orderState.Parameters.AddWithValue("@orderid", OrderID);
                var ds = new DataSet();
                MySqlDataAdapter mya = new MySqlDataAdapter(orderState);
                mya.Fill(ds, "currentOrder");
                dataGridCurrentOrder.DataContext = ds;

                orderState.Dispose();
                updateOrder.Dispose();
                command.Dispose();
                cnn.Close();

            }

        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {

            MySqlConnection cnn;
            cnn = new MySqlConnection(connectionStinrg);
            cnn.Open();
            MySqlCommand command = cnn.CreateCommand();

            command.CommandText = "UPDATE Orders SET OrderStatus = @OrderStatus where OrderID = @OrderID";
            command.Parameters.AddWithValue("@OrderStatus", "RFND");
            command.Parameters.AddWithValue("@OrderID", refundOrderID.Text);
            command.ExecuteNonQuery();
            

        }
    }
}
