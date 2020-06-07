using System.Collections.Generic;
using System.Data.SQLite;
using Newtonsoft.Json;
using System.Data.SqlClient;

namespace socket_client
{
    class PersonsData
    {
        public List<List<string>> id_order = new List<List<string>>();
        public List<List<string>> name_customer = new List<List<string>>();
        public List<List<string>> categor_customer = new List<List<string>>();
        public List<List<string>> name_product = new List<List<string>>();
        public List<List<string>> categor_product = new List<List<string>>();
        
    }

    class Get
    {
        public static PersonsData GetPersonsData()
        {
            string sConnLiteStr = new SqlConnectionStringBuilder
            {
                DataSource = @"LAPTOP-1GAL0TI1\SQLEXPRESS",
                InitialCatalog = "StartDB",
                IntegratedSecurity = true,
            }.ConnectionString;

            PersonsData data = new PersonsData();
            data.categor_customer = ReadDataLite(sConnLiteStr, @"SELECT DISTINCT categor_customer FROM OrderN", "categor_customer", "", "");
            data.categor_product = ReadDataLite(sConnLiteStr, @"SELECT DISTINCT categor_product FROM OrderN", "categor_product", "", "");
            data.name_product= ReadDataLite(sConnLiteStr, @"SELECT DISTINCT name_product, price, categor_product FROM OrderN", "name_product", "price", "categor_product");
            data.name_customer = ReadDataLite(sConnLiteStr, @"SELECT DISTINCT name_customer, categor_customer FROM OrderN", "name_customer", "categor_customer", "");
            data.id_order = ReadDataLite(sConnLiteStr, @"SELECT DISTINCT id_order, name_product, name_customer FROM OrderN", "id_order", "name_product", "name_customer");
            
        
            return data;
        }
        public static List<List<string>> ReadDataLite(string sConnLiteStr, string request, string param1, string param2, string param3)
        {
            List<List<string>> values = new List<List<string>>();

            using (var sConnLite = new SqlConnection(sConnLiteStr))
            {
                sConnLite.Open();

                var sCommandLite = new SqlCommand
                {
                    Connection = sConnLite,
                    CommandText = request,
                };

                using (var reader = sCommandLite.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        List<string> list = new List<string>();
                        list.Add((string)reader[param1]);
                        if (param2 != "") list.Add((string)reader[param2]);
                        if (param3 != "") list.Add((string)reader[param3]);
                        values.Add(list);
                    }
                }
            }
            return values;
        }
    }
}
