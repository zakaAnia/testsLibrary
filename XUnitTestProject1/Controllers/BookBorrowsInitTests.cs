using Library.Entities;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace XUnitTestProject1.Controllers
{
    public class BookBorrowsInitTests
    {
        private readonly TestServer _server;
        private readonly HttpClient _client;

        public BookBorrowsInitTests()
            {

            _server = ServerFactory.GetServerInstance();
            _client = _server.CreateClient();


            using (var scope = _server.Host.Services.CreateScope())
            {
                var _db = scope.ServiceProvider.GetRequiredService<LibraryContext>();

                _db.BookBorrow.Add(new BookBorrow
                {
                    IdBookBorrow = 132,
                    IdBook = 21,
                    IdUser = 1,
                    BorrowDate = new DateTime(2020, 03, 19),
                    ReturnDate = new DateTime(2020, 04, 02),
                    Comments = "borrowed"
    });


                _db.SaveChanges();

            }
        }


        [Fact]
        public async Task PostBookBorrow_200()
        {
            var newBookBorrowed = new BookBorrow
            {
                IdBookBorrow = 125,
                IdBook = 122,
                IdUser = 1,
                BorrowDate = new DateTime(2020, 03, 19),
                ReturnDate = new DateTime(2020, 04, 02),
                Comments = "borrowed"
            };

            

            var httpResponse = await _client.PostAsync($"{_client.BaseAddress.AbsoluteUri}api/book-borrows",
                                 new StringContent(JsonConvert.SerializeObject(newBookBorrowed),
                                 Encoding.UTF8,"application/json"
                ));

            httpResponse.EnsureSuccessStatusCode();
            var content = await httpResponse.Content.ReadAsStringAsync();
            var borrows = JsonConvert.DeserializeObject<BookBorrow>(content);

            Assert.True(borrows.IdBook == 122);
        }

        [Fact]
        public async Task PutBookBorrow_200()
        {
            string newComment = "returned";
            int idBookToCheck = 132;

            var changedBookBorrowed = new BookBorrow
            {
                IdBookBorrow = idBookToCheck,
                IdBook = 21,
                IdUser = 1,
                BorrowDate = new DateTime(2020, 03, 19),
                ReturnDate = new DateTime(2020, 04, 02),
                Comments = newComment
            };
            

            var httpResponse = await _client.PutAsync($"{_client.BaseAddress.AbsoluteUri}api/book-borrows/{idBookToCheck}",
                                 new StringContent(JsonConvert.SerializeObject(changedBookBorrowed),
                                 Encoding.UTF8, "application/json"
                ));

            httpResponse.EnsureSuccessStatusCode();

            using (var scope = _server.Host.Services.CreateScope())
            {
                var _db = scope.ServiceProvider.GetRequiredService<LibraryContext>();
                Assert.True(_db.BookBorrow.Any(e => e.IdBookBorrow == 132 && e.Comments == newComment));
            }

        }

    }
}
