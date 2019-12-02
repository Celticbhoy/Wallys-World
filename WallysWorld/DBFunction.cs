using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;

namespace WallysWorld
{
    class DBFunction
    {
        string connectionStinrg = "server=127.0.0.1; port=3306; username=root;password=Conestoga1;";

        public object SearchCustomer(string cusIn)
        {
            MySqlConnection cnn;
            cnn = new MySqlConnection(connectionStinrg);
            cnn.Open();
            MySqlCommand command = cnn.CreateCommand();
            
            
            command.CommandText = "SELECT * FROM Person Where Telephone like '%@tele' OR LastName = @last;";
            MySqlDataAdapter adp = new MySqlDataAdapter(command);
            DataSet ds = new DataSet();
            command.Parameters.AddWithValue("@tele", cusIn);
            command.Parameters.AddWithValue("@last", cusIn);
            
            command.ExecuteNonQuery();


            adp.Fill(ds, "cusTable");
            command.Dispose();
            cnn.Close();
            return ds;


        }
    }
}
