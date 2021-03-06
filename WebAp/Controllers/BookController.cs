﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebAp.Model;
using System.Web.Http.Cors;

namespace WebAp.Controllers
{
    
    [Route("api/[controller]")]
    [ApiController]
    public class BookController : ControllerBase
    {
        
        private static readonly string ConnectionString = Controllers.ConnectionString.GetConnectionString();

        // GET: api/Books
        [HttpGet]
        public IEnumerable<Book> GetAllBooks()
        {
            const string selectString = "select * from book order by id";
            using (SqlConnection databaseConnection = new SqlConnection(ConnectionString))
            {
                databaseConnection.Open();
                using (SqlCommand selectCommand = new SqlCommand(selectString, databaseConnection))
                {
                    using (SqlDataReader reader = selectCommand.ExecuteReader())
                    {
                        List<Book> bookList = new List<Book>();
                        while (reader.Read())
                        {
                            Book book = ReadBook(reader);
                            bookList.Add(book);
                        }
                        return bookList;
                    }
                }
            }
        }

        private static Book ReadBook(IDataRecord reader)
        {
            int id = reader.GetInt32(0);
            string title = reader.IsDBNull(1) ? null : reader.GetString(1);
            string author = reader.IsDBNull(2) ? null : reader.GetString(2);
            string publisher = reader.IsDBNull(3) ? null : reader.GetString(3);
            decimal price = reader.GetDecimal(4);
            Book book = new Book
            {
                Id = id,
                Title = title,
                Author = author,
                Publisher = publisher,
                Price = price
            };
            return book;
        }

        // GET: api/Books/5
        [Route("{id}")]
        public Book GetBookById(int id)
        {
            const string selectString = "select * from book where id=@id";
            using (SqlConnection databaseConnection = new SqlConnection(ConnectionString))
            {
                databaseConnection.Open();
                using (SqlCommand selectCommand = new SqlCommand(selectString, databaseConnection))
                {
                    selectCommand.Parameters.AddWithValue("@id", id);
                    using (SqlDataReader reader = selectCommand.ExecuteReader())
                    {
                        if (!reader.HasRows) { return null; }
                        reader.Read(); // advance cursor to first row
                        return ReadBook(reader);
                    }
                }
            }
        }

        // DELETE: api/ApiWithActions/5
        [HttpDelete("{id}")]
        public int DeleteBook(int id)
        {
            const string deleteStatement = "delete from book where id=@id";
            using (SqlConnection databaseConnection = new SqlConnection(ConnectionString))
            {
                databaseConnection.Open();
                using (SqlCommand insertCommand = new SqlCommand(deleteStatement, databaseConnection))
                {
                    insertCommand.Parameters.AddWithValue("@id", id);
                    int rowsAffected = insertCommand.ExecuteNonQuery();
                    return rowsAffected;
                }
            }
        }

        // POST: api/Books
        [HttpPost]
        public int AddBook([FromBody] Book value)
        {
            const string insertString = "insert into book (title, author, publisher, price) values (@title, @author, @publisher, @price)";
            using (SqlConnection databaseConnection = new SqlConnection(ConnectionString))
            {
                databaseConnection.Open();
                using (SqlCommand insertCommand = new SqlCommand(insertString, databaseConnection))
                {
                    insertCommand.Parameters.AddWithValue("@title", value.Title);
                    insertCommand.Parameters.AddWithValue("@author", value.Author);
                    insertCommand.Parameters.AddWithValue("@publisher", value.Publisher);
                    insertCommand.Parameters.AddWithValue("@price", value.Price);
                    int rowsAffected = insertCommand.ExecuteNonQuery();
                    return rowsAffected;
                }
            }
        }

        // PUT: api/Books/5
        [HttpPut("{id}")]
        public int UpdateBook(int id, [FromBody] Book value)
        {
            const string updateString =
                "update book set title=@title, author=@author, publisher=@publisher, price=@price where id=@id;";
            using (SqlConnection databaseConnection = new SqlConnection(ConnectionString))
            {
                databaseConnection.Open();
                using (SqlCommand updateCommand = new SqlCommand(updateString, databaseConnection))
                {
                    updateCommand.Parameters.AddWithValue("@title", value.Title);
                    updateCommand.Parameters.AddWithValue("@author", value.Author);
                    updateCommand.Parameters.AddWithValue("@publisher", value.Publisher);
                    updateCommand.Parameters.AddWithValue("@price", value.Price);
                    updateCommand.Parameters.AddWithValue("@id", id);
                    int rowsAffected = updateCommand.ExecuteNonQuery();
                    return rowsAffected;
                }
            }
        }
    }

    internal class ConnectionString
    {
        internal static string GetConnectionString()
        {
            return "Server = tcp:anax.database.windows.net,1433;" +
                 " Initial Catalog = CarRetailDB; Persist Security Info = False;" +
                 " User ID =anemarie; Password = anex1030_;" +
                 " MultipleActiveResultSets = False; Encrypt = True; TrustServerCertificate = False; " +
                 "Connection Timeout = 30;";
        }


    }

}