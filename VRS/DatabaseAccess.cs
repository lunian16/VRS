using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VRS
{
    public class DatabaseAccess
    {
        public MySqlConnectionStringBuilder BuilderString { get; set; }

        //May need to change.
        public DatabaseAccess()
        {
            var builder = new MySqlConnectionStringBuilder
            {
                Server = "localhost",
                UserID = "root",
                Password = "password",
                Database = "vrs",
            };

            BuilderString = builder;
        }        

        //************************************************************************//
        //manage cateogory database
            public void InsertRecordIfNotExists(int category_id, string category_name)
            {
                using (var connection = new MySqlConnection(BuilderString.ConnectionString))
                {
                    connection.Open();


                    // Proceed with insertion if the record does not exist
                    string query = "INSERT INTO category (category_id, category_name) value (@category_id, @category_name)"; using (var command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@category_id", category_id);
                        command.Parameters.AddWithValue("@category_name", category_name);
                        int result = command.ExecuteNonQuery();
                        if (result < 0)
                        {
                            Console.WriteLine("Error inserting data into the database.");
                        }
                    }

                    connection.Close();
                }
            }


            public void DeleteRecord(int category_id)
            {
                using (var connection = new MySqlConnection(BuilderString.ConnectionString))
                {
                    connection.Open();


                    string query = "Delete from category WHERE category_id=@category_id";
                    using (var command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@category_id", category_id);
                        command.ExecuteNonQuery();

                    }

                    connection.Close();
                }
            }

            public string LoadCategoryToListView()
            {
                StringBuilder allCategoryDetails = new StringBuilder();

                using (var connection = new MySqlConnection(BuilderString.ConnectionString))
                {
                    connection.Open();
                    string sql = "SELECT category_id, category_name FROM category";
                    MySqlCommand command = new MySqlCommand(sql, connection);
                    MySqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {

                        int CategoryId = reader.GetInt32(0);
                        string CategoryName = reader.GetString(1);

                        // Append each category's details to the StringBuilder
                        allCategoryDetails.AppendLine($"Category ID: {CategoryId}, Category Name: {CategoryName}");

                    }
                    return allCategoryDetails.ToString();
                }

            }

        //*****************************************//

        //manage customer

            public void InsertRecordIfNotExists(int customerid, string lastname, string firstname, string contactphone, string email)
            {
                using (var connection = new MySqlConnection(BuilderString.ConnectionString))
                {
                    connection.Open();


                    // Proceed with insertion if the record does not exist
                    string query = "INSERT INTO customer (customer_id, last_name, first_name, contact_phone, email) value (@customer_id, @last_name, @first_name, @contact_phone, @email)"; using (var command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@customer_id", customerid);
                        command.Parameters.AddWithValue("@last_name", lastname);
                        command.Parameters.AddWithValue("@first_name", firstname);
                        command.Parameters.AddWithValue("@contact_phone", contactphone);
                        command.Parameters.AddWithValue("@email", email);
                        int result = command.ExecuteNonQuery();
                        if (result < 0)
                        {
                            Console.WriteLine("Error inserting data into the database.");
                        }
                    }

                    connection.Close();
                }
            }


            public void DeleteCustomerRecord(int customerid)
            {
                using (var connection = new MySqlConnection(BuilderString.ConnectionString))
                {
                    connection.Open();


                    string query = "Delete from customer WHERE customer_id=@customer_id";
                    using (var command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@customer_id", customerid);
                        command.ExecuteNonQuery();

                    }

                    connection.Close();
                }
            }


            public string LoadCustomersToListView()
            {
                StringBuilder allCustomerDetails = new StringBuilder();

                using (var connection = new MySqlConnection(BuilderString.ConnectionString))
                {
                    connection.Open();
                    string sql = "SELECT customer_id, last_name, first_name, contact_phone, email FROM customer";
                    MySqlCommand command = new MySqlCommand(sql, connection);
                    MySqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {

                        int CustomerId = reader.GetInt32(0);
                        string LastName = reader.GetString(1);
                        string FirstName = reader.GetString(2);
                        string PhoneNumber = reader.GetString(3);
                        string Email = reader.GetString(4);

                        // Append each customer's details to the StringBuilder
                        allCustomerDetails.AppendLine($"Customer ID: {CustomerId}, Name: {LastName} {FirstName}, Phone Number: {PhoneNumber}, Email: {Email}");

                    }
                    return allCustomerDetails.ToString();
                }

            }




        //*****************************************//
        //manage equipment

            //insert into equipment database
            public void InsertRecordIfNotExists(int equipment_id, string equipment_name, string equipment_description, double daily_rate, int category_id)
            {
                using (var connection = new MySqlConnection(BuilderString.ConnectionString))
                {

                    connection.Open();


                    // Proceed with insertion if the record does not exist
                    string query = "INSERT INTO equipment (equipment_id, equipment_name, equipment_description, daily_rate, category_id) value (@equipment_id, @equipment_name, @equipment_description, @daily_rate, @category_id)";
                    using (var command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@equipment_id", equipment_id);
                        command.Parameters.AddWithValue("@equipment_name", equipment_name);
                        command.Parameters.AddWithValue("@equipment_description", equipment_description);
                        command.Parameters.AddWithValue("@daily_rate", daily_rate);
                        command.Parameters.AddWithValue("@category_id", category_id);
                        int result = command.ExecuteNonQuery();
                        if (result < 0)
                        {
                            Console.WriteLine("Error inserting data into the database.");
                        }
                    }

                }
            }


            public delegate void AlertCallback(string title, string message, string cancel);


            public AlertCallback OnAlert { get; set; }

            //delete equipment from database
            public void DeleteEquipmentRecord(int equipmentId)
            {
                try
                {
                    using (var connection1 = new MySqlConnection(BuilderString.ConnectionString))
                    {
                        connection1.Open();

                        string query = "DELETE FROM equipment WHERE equipment_id = @equipment_id";

                        using (var command = new MySqlCommand(query, connection1))
                        {
                            command.Parameters.AddWithValue("@equipment_id", equipmentId);
                            var result = command.ExecuteNonQuery();


                            if (result > 0)
                            {
                                OnAlert?.Invoke("Success", "Record deleted successfully.", "OK");
                            }
                            else
                            {
                                OnAlert?.Invoke("Notice", "No record was deleted.", "OK");
                            }
                        }
                    }
                }

                catch (Exception ex)
                {
                    OnAlert?.Invoke("Error", "An error occurred: " + ex.Message, "OK");
                }
            }

            //load equipment from database and list them
            public string LoadEquipmentsToListView()
            {
                StringBuilder allEquipmentDetails = new StringBuilder();

                using (var connection = new MySqlConnection(BuilderString.ConnectionString))
                {
                    connection.Open();
                    string sql = "SELECT equipment_id, equipment_name, equipment_description, daily_rate, category_id FROM equipment";
                    MySqlCommand command = new MySqlCommand(sql, connection);
                    MySqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {

                        int equipmentid = reader.GetInt32(0);
                        string equipmentName = reader.GetString(1);
                        string equipmentDescription = reader.GetString(2);
                        double dailyRate = reader.GetDouble(3);
                        int categoryId = reader.GetInt32(4);

                        // Append each equipment's details to the StringBuilder
                        allEquipmentDetails.AppendLine($"Equipment ID: {equipmentid}, Equipment Name: {equipmentName} Equipment Description: {equipmentDescription}, Daily Rate: {dailyRate}, Category ID: {categoryId}");

                    }
                    return allEquipmentDetails.ToString();
                }

            }

            //show all category in the picker

            public List<CategoryList> FetchAllCategory()
            {
                List<CategoryList> categories = new List<CategoryList>();
                using (var connection = new MySqlConnection(BuilderString.ConnectionString))
                {
                    connection.Open();
                    string sql = "SELECT category_id, category_name FROM category";
                    MySqlCommand command = new MySqlCommand(sql, connection);
                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            categories.Add(new CategoryList()
                            {
                                Categoryid = reader.GetInt32(0),
                                Categoryname = reader.GetString(1),
                            });
                        }
                    }
                    connection.Close();
                }
                return categories;
            }

        





        //******************************************//
        //manage rental
        public void InsertRecordIfNotExists(int rental_id, DateTime applydate, DateTime rental_date, DateTime return_date, double cost, int equipment_id, int customer_id)
            {
                using (var connection = new MySqlConnection(BuilderString.ConnectionString))
                {
                    connection.Open();


                    // Proceed with insertion if the record does not exist
                    string query = "INSERT INTO rental (rental_id, applydate, rental_date, return_date, cost,equipment_id,customer_id) value (@rental_id, @applydate, @rental_date, @return_date, @cost, @equipment_id, @customer_id)"; using (var command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@rental_id", rental_id);
                        command.Parameters.AddWithValue("@applydate", applydate);
                        command.Parameters.AddWithValue("@rental_date", rental_date);
                        command.Parameters.AddWithValue("@return_date", return_date);
                        command.Parameters.AddWithValue("@cost", cost);
                        command.Parameters.AddWithValue("@equipment_id", equipment_id);
                        command.Parameters.AddWithValue("@customer_id", customer_id);
                        int result = command.ExecuteNonQuery();
                        if (result < 0)
                        {
                            Console.WriteLine("Error inserting data into the database.");
                        }
                    }

                    connection.Close();
                }
            }


            public void DeleteRentalRecord(int rental_id)
            {
                using (var connection = new MySqlConnection(BuilderString.ConnectionString))
                {
                    connection.Open();


                    string query = "Delete from rental WHERE rental_id=@rental_id";
                    using (var command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@rental_id", rental_id);
                        command.ExecuteNonQuery();
                    }

                    connection.Close();
                }
            }


            public string LoadRentalsToListView()
            {
                StringBuilder allRentalDetails = new StringBuilder();

                using (var connection = new MySqlConnection(BuilderString.ConnectionString))
                {
                    connection.Open();
                    string sql = "SELECT rental_id, applydate, rental_date, return_date, cost,equipment_id,customer_id FROM rental";
                    MySqlCommand command = new MySqlCommand(sql, connection);
                    MySqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {

                        int rental_id = reader.GetInt32(0);
                        DateTime applydate = reader.GetDateTime(1);
                        DateTime rental_date = reader.GetDateTime(2);
                        DateTime return_date = reader.GetDateTime(3);
                        double cost = reader.GetDouble(4);
                        int equipment_id = reader.GetInt32(5);
                        int customer_id = reader.GetInt32(6);

                        // Append each customer's details to the StringBuilder
                        allRentalDetails.AppendLine($"Rental ID: {rental_id}, Applydate: {applydate}, Rental from {rental_date} to {return_date}, Total cost: {cost}, Equipment Id:{equipment_id}, Customer Id:{customer_id}");

                    }
                    return allRentalDetails.ToString();
                }

            }

            //list all customers from database
            public List<Customer> FetchAllCustomer()
            {
                List<Customer> customer = new List<Customer>();
                using (var connection = new MySqlConnection(BuilderString.ConnectionString))
                {
                    connection.Open();
                    string sql = "SELECT CONCAT(last_name, ' ', first_name) AS customer_name, customer_id, contact_phone FROM customer";
                    MySqlCommand command = new MySqlCommand(sql, connection);
                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            customer.Add(new Customer()
                            {
                                Name = reader.GetString(0),
                                Customer_Num = reader.GetInt32(1),
                                PhoneNumber = reader.GetString(2),
                            });
                        }

                    }

                    connection.Close();
                }
                return customer;
            }

            //list all equipments from database
            public List<Equipment> FetchAllEquipment()
            {
                List<Equipment> equipment = new List<Equipment>();
                using (var connection = new MySqlConnection(BuilderString.ConnectionString))
                {
                    connection.Open();
                    string sql = "SELECT equipment_id, equipment_name, daily_rate FROM equipment " +
                        "WHERE equipment_id NOT IN (SELECT equipment_id FROM rental)";
                    MySqlCommand command = new MySqlCommand(sql, connection);
                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            equipment.Add(new Equipment()
                            {
                                Equipmentid = reader.GetInt32(0),
                                Equipmentname = reader.GetString(1),
                                Equipmentprice = reader.GetDouble(2),
                            });
                        }
                    }
                    connection.Close();

                }
                return equipment;
            }






        }
}
