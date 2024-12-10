using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient;

namespace ConsoleApp
{       
    public interface IProductDal
    {
        List<Product> GetAllProducts();
        Product GetProductById(int id);
        List<Product> Find(string productName);
        int Count();
        int Create(Product p);
        int Update(Product p);
        int Delete(int productId);
    }

    public class MySQLProductDal : IProductDal
    {
        private MySqlConnection GetMySqlConnection()
        {
            string connectionString = @"server=;port=;database=;user=;password=;"; //weite your sql con string
            return new MySqlConnection(connectionString);         
        }
        public int Create(Product p)
        {
            int result = 0;

            using(var connection = GetMySqlConnection())
            {
                try
                {
                    connection.Open();

                    string sql = "insert into products (product_name, list_price, discontinued) VALUES (@productname, @unitprice, @discontinued)";
                    MySqlCommand command = new MySqlCommand(sql,connection);

                    command.Parameters.AddWithValue("@productname", p.Name);
                    command.Parameters.AddWithValue("@unitprice", p.Price);
                    command.Parameters.AddWithValue("@discontinued", 1);
                    
                    result = command.ExecuteNonQuery();

                    Console.WriteLine($"{result} adet kayıt eklendi");

                }
                catch(Exception e)
                {
                    Console.WriteLine(e.Message);
                }
                finally
                {
                    connection.Close();
                }
            }
            
            return result;
        }

        public int Delete(int productId)
        {
            int result = 0;

            using(var connection = GetMySqlConnection())
            {
                try
                {
                    connection.Open();

                    string sql = "delete from products where ProductId=@productid";
                    MySqlCommand command = new MySqlCommand(sql,connection);

                    command.Parameters.AddWithValue("@productid", productId);
                    
                    result = command.ExecuteNonQuery();

                }
                catch(Exception e)
                {
                    Console.WriteLine(e.Message);
                }
                finally
                {
                    connection.Close();
                }
            }
            
            return result;
        }

        public List<Product> GetAllProducts()
        {
            List<Product> products = null;

            using(var connection = GetMySqlConnection())
            {
                try
                {
                    connection.Open();
                    
                    string sql = "select * from products";

                    MySqlCommand command = new MySqlCommand(sql,connection);

                    MySqlDataReader reader = command.ExecuteReader();
                    products = new List<Product>();

                    while(reader.Read())
                    {
                        products.Add(
                            new Product
                            {
                                ProductId=int.Parse(reader["id"].ToString()),
                                Name = reader["product_name"].ToString(),
                                Price = double.Parse(reader["list_price"]?.ToString())
                            }
                        );
                    }
                    reader.Close();
                }
                catch(Exception e)
                {
                    Console.WriteLine(e.Message);
                }
                finally
                {
                    connection.Close();
                }
            }

            return products;  
        }

        public Product GetProductById(int id)
        {
            Product product = null;

            using(var connection = GetMySqlConnection())
            {
                try
                {
                    connection.Open();
                    
                    string sql = "select * from products where id=@productid";

                    MySqlCommand command = new MySqlCommand(sql,connection);
                    command.Parameters.Add("@productid", MySqlDbType.Int32).Value = id;

                    MySqlDataReader reader = command.ExecuteReader();

                    reader.Read();

                    if (reader.HasRows)
                    {                    
                        product = new Product()
                        {                       
                            ProductId=int.Parse(reader["id"].ToString()),
                            Name = reader["product_name"].ToString(),
                            Price = double.Parse(reader["list_price"]?.ToString())
                        };
                    }
                 
                    reader.Close();
                }
                catch(Exception e)
                {
                    Console.WriteLine(e.Message);
                }
                finally
                {
                    connection.Close();
                }
            }

            return product;  
        }

        public int Update(Product p)
        {
            int result = 0;

            using(var connection = GetMySqlConnection())
            {
                try
                {
                    connection.Open();

                    string sql = "update products set product_name=@productname, list_price=@unitprice where id=@productid";
                    MySqlCommand command = new MySqlCommand(sql,connection);

                    command.Parameters.AddWithValue("@productname", p.Name);
                    command.Parameters.AddWithValue("@unitprice", p.Price);
                    command.Parameters.AddWithValue("@productid", p.ProductId);
                    
                    result = command.ExecuteNonQuery();

                }
                catch(Exception e)
                {
                    Console.WriteLine(e.Message);
                }
                finally
                {
                    connection.Close();
                }
            }
            
            return result;
        }

        public List<Product> Find(string productName)
        {
            List<Product> products = null;

            using(var connection = GetMySqlConnection())
            {
                try
                {
                    connection.Open();
                    
                    string sql = "select * from products where product_name LIKE @productName";

                    MySqlCommand command = new MySqlCommand(sql,connection);
                    command.Parameters.Add("@productName", MySqlDbType.String).Value = "%"+ productName+"%";

                    MySqlDataReader reader = command.ExecuteReader();

                    products = new List<Product>();

                    while(reader.Read())
                    {
                        products.Add(
                            new Product
                            {
                                ProductId=int.Parse(reader["id"].ToString()),
                                Name = reader["product_name"].ToString(),
                                Price = double.Parse(reader["list_price"]?.ToString())
                            }
                        );
                    }                 
                 
                    reader.Close();
                }
                catch(Exception e)
                {
                    Console.WriteLine(e.Message);
                }
                finally
                {
                    connection.Close();
                }
            }

            return products;  
        }

        public int Count()
        {
            int count = 0; 

            using(var connection = GetMySqlConnection())
            {
                try
                {
                    connection.Open();
                    
                    string sql = "select count(*) from products";

                    MySqlCommand command = new MySqlCommand(sql,connection);
                    object result = command.ExecuteScalar();
                    if (result!=null)
                    {
                        count = Convert.ToInt32(result); 
                    }
                 
                }
                catch(Exception e)
                {
                    Console.WriteLine(e.Message);
                }
                finally
                {
                    connection.Close();
                }
            }

            return count;  
        }
    }
    
    public class ProductManager : IProductDal
    {
        IProductDal _productDal;
        public ProductManager(IProductDal productDal)
        {
            _productDal = productDal;
        }

        public int Count()
        {
            return _productDal.Count();
        }

        public int Create(Product p)
        {
           return _productDal.Create(p);
        }

        public int Delete(int productId)
        {
            return _productDal.Delete(productId);
        }

        public List<Product> Find(string productName)
        {
           return _productDal.Find(productName);
        }

        public List<Product> GetAllProducts()
        {
            return _productDal.GetAllProducts();
        }

        public Product GetProductById(int id)
        {
            return _productDal.GetProductById(id);
        }

        public int Update(Product p)
        {
            return _productDal.Update(p);
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            var productDal = new ProductManager(new MySQLProductDal());

            //    int result = productDal.Delete(78);

            // Update
            var p = productDal.GetProductById(77);

            p.Name = "Samsung S8";
            p.Price = 5000;

            int count1 = productDal.Update(p);
            
            Console.WriteLine($"Güncellenen kayıt sayısı: {count1}");

            // Search
            string searchQuery = "efe";
            List<Product> result = productDal.Find(searchQuery);

            if (result != null && result.Count > 0){
                foreach (var product in result){
                    Console.WriteLine($"Id: {product.ProductId}, Name: {product.Name}, Price: {product.Price:C}");
                }
            }
            else{
                Console.WriteLine("No products found for your query.");
            }

            // Count
            int count = productDal.Count();
            Console.WriteLine($"{count} adet ürün bulundu.");
        }   
    }
}
