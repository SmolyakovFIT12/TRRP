using Newtonsoft.Json;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace socket_server
{
    class PersonsData
    {
        public List<List<string>> id_order = new List<List<string>>();
        public List<List<string>> name_customer = new List<List<string>>();
        public List<List<string>> categor_customer = new List<List<string>>();
        public List<List<string>> name_product = new List<List<string>>();
        public List<List<string>> categor_product = new List<List<string>>();
    }

    class Put
    {
        static void ClearTable(string sConnStr)
        {
            using (var sConn = new SqlConnection(sConnStr))
            {
                sConn.Open();

                var sCommand = new SqlCommand
                {
                    Connection = sConn,
                    CommandText = @"ALTER TABLE Customer
DROP CONSTRAINT FK_Customer_CategoryCustomer;
ALTER TABLE CategoryCustomer
DROP CONSTRAINT PK_CategoryCustomer;
ALTER TABLE InfProducts
DROP CONSTRAINT FK_InfProducts_CategoryProduct;
ALTER TABLE CategoryProduct
DROP CONSTRAINT PK_CategoryProduct;
ALTER TABLE NameOrder
DROP CONSTRAINT FK_NameOrder_Customer;
ALTER TABLE NameOrder
DROP CONSTRAINT FK_NameOrder_InfProducts;
ALTER TABLE Customer
DROP CONSTRAINT PK_Customer;
ALTER TABLE InfProducts
DROP CONSTRAINT PK_InfProducts;
TRUNCATE TABLE dbo.NameOrder
TRUNCATE TABLE dbo.InfProducts
TRUNCATE TABLE dbo.Customer
TRUNCATE TABLE dbo.CategoryProduct
TRUNCATE TABLE dbo.CategoryCustomer;
ALTER TABLE Customer
ADD CONSTRAINT PK_Customer PRIMARY KEY CLUSTERED (id);
ALTER TABLE CategoryCustomer
ADD CONSTRAINT PK_CategoryCustomer PRIMARY KEY CLUSTERED (id);
ALTER TABLE InfProducts
ADD CONSTRAINT PK_InfProducts PRIMARY KEY CLUSTERED (id);
ALTER TABLE CategoryProduct
ADD CONSTRAINT PK_CategoryProduct PRIMARY KEY CLUSTERED (id);
ALTER TABLE Customer
ADD CONSTRAINT FK_Customer_CategoryCustomer FOREIGN KEY (categoryid)
REFERENCES CategoryCustomer (id);
ALTER TABLE InfProducts
ADD CONSTRAINT FK_InfProducts_CategoryProduct FOREIGN KEY (id_categ)
REFERENCES CategoryProduct (id);
ALTER TABLE NameOrder
ADD CONSTRAINT FK_NameOrder_Customer FOREIGN KEY (customerid)
REFERENCES Customer (id);
ALTER TABLE NameOrder
ADD CONSTRAINT FK_NameOrder_InfProducts FOREIGN KEY (id_product)
REFERENCES InfProducts (id)",

                    //CommandText = @"TRUNCATE TABLE NameOrder; TRUNCATE TABLE InfProducts; TRUNCATE TABLE Customer; TRUNCATE TABLE CategoryProduct; TRUNCATE TABLE CategoryCustomer",
                };
                sCommand.ExecuteNonQuery();
            }
        }

        public static void JsonCon(string message)
        {
            PersonsData data = new PersonsData();
             data = JsonConvert.DeserializeObject<PersonsData>(message);
            PutPersonsData(data);           
        }

        public static void PutPersonsData(PersonsData data)
        {
            string sConnStr = new SqlConnectionStringBuilder
            {
                DataSource = @"LAPTOP-1GAL0TI1\SQLEXPRESS",
                InitialCatalog = "FinishDB",
                IntegratedSecurity = true,
            }.ConnectionString;

            ClearTable(sConnStr);
            WriteDataMS(sConnStr, @"INSERT INTO CategoryProduct (name_categ) VALUES(@name_categ)", "@name_categ", "", "", "", "", data.categor_product);
            WriteDataMS(sConnStr, @"INSERT INTO CategoryCustomer (name_category) VALUES(@name_category)", "@name_category", "", "", "", "", data.categor_customer);
            WriteDataMS2(sConnStr, @"INSERT INTO InfProducts (name_product, price, id_categ) VALUES(@name_product, @price, @id_categ)", "@name_product", 
                "@price", "@id_categ", "", @" SELECT id FROM CategoryProduct WHERE name_categ = '", data.name_product);
            WriteDataMS(sConnStr, @"INSERT INTO Customer (name_customer, categoryid) 
                                        VALUES(@name_customer,  @id_category)",
                               "@name_customer", "@id_category", "", @" SELECT id FROM CategoryCustomer WHERE name_category = '", "", data.name_customer);
            WriteDataMS(sConnStr, @"INSERT INTO NameOrder (id_order, id_product, customerid) 
                                        VALUES(@id_order,  @id_product, @id_customer)",
                                           "@id_order", "@id_product", "@id_customer", @"select id from InfProducts where name_product = '", @"select id from Customer where name_customer = '", data.id_order);

        }


        public static void WriteDataMS(string sConnStr, string request, string param1, string param2, string param3, string request2, string request3, List<List<string>> data)
        {
            using (var sConn = new SqlConnection(sConnStr))
            {
                sConn.Open();
                for (int i = 0; i < data.Count; i++)
                {
                    var sCommand = new SqlCommand
                    {
                        Connection = sConn,
                        CommandText = request,
                    };
                    sCommand.Parameters.AddWithValue(param1, data[i][0]);

                    if (param2 != "")
                    {
                        var sCommand1 = new SqlCommand
                        {
                            Connection = sConn,
                            CommandText = request2 + data[i][1] + "'",
                        };
                        
                        using (var reader2 = sCommand1.ExecuteReader())
                        {
                            reader2.Read();
                            sCommand.Parameters.AddWithValue(param2, (int)reader2["id"]);
                        }
                    }
                    if (param3 != "")
                    {
                        var sCommand2 = new SqlCommand
                        {
                            Connection = sConn,
                            CommandText = request3 + data[i][2] + "'",
                        };
                       
                        using (var reader3 = sCommand2.ExecuteReader())
                        {
                            reader3.Read();
                            sCommand.Parameters.AddWithValue(param3, (int)reader3["id"]);
                        }
                    }
                   
                    sCommand.ExecuteNonQuery();
                }
            }
        }
        public static void WriteDataMS2(string sConnStr, string request, string param1, string param2, string param3, string request2, string request3, List<List<string>> data)
        {
            using (var sConn = new SqlConnection(sConnStr))
            {
                sConn.Open();
                for (int i = 0; i < data.Count; i++)
                {
                    var sCommand = new SqlCommand
                    {
                        Connection = sConn,
                        CommandText = request,
                    };
                    sCommand.Parameters.AddWithValue(param1, data[i][0]);
                    sCommand.Parameters.AddWithValue(param2, data[i][1]);

                    if (param3 != "")
                    {
                        var sCommand2 = new SqlCommand
                        {
                            Connection = sConn,
                            CommandText = request3 + data[i][2] + "'",
                        };

                        using (var reader3 = sCommand2.ExecuteReader())
                        {
                            reader3.Read();
                            sCommand.Parameters.AddWithValue(param3, (int)reader3["id"]);
                        }
                    }

                    sCommand.ExecuteNonQuery();
                }
            }
        }
    }
}
