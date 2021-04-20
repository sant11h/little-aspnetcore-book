using AspNetCoreTodo.Data;
using AspNetCoreTodo.Models;
using AspNetCoreTodo.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace AspNetCoreTodo.UnitTests
{
    public class TodoItemServiceShould
    {
        [Fact]
        public async Task AddNewItemAsIncompleteWithDueDate()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "Test_AddNewItem")
                .Options;

            using (var context = new ApplicationDbContext(options))
            {
                var service = new TodoItemService(context);

                var fakeUser = new IdentityUser
                {
                    Id = "fake-322",
                    UserName = "fake@test.com",
                    EmailConfirmed = true
                };

                await service.AddItemAsync(new TodoItem { Title = "Testing" }, fakeUser);
            }

            using (var context = new ApplicationDbContext(options))
            {
                var itemsInDatabase = await context.Items.CountAsync();
                Assert.Equal(1, itemsInDatabase);

                var item = await context.Items.FirstAsync();
                Assert.Equal("Testing", item.Title);
                Assert.False(item.IsDone);

                var difference = DateTimeOffset.Now.AddDays(3) - item.DueAt;
                Assert.True(difference < TimeSpan.FromSeconds(1));
            }
        }

        [Fact]
        public async Task MarkDoneAsync_ShouldReturnFalsePassingInexistentID()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "Test_AddNewItem")
                .Options;

            using (var context = new ApplicationDbContext(options))
            {
                var service = new TodoItemService(context);

                var fakeUser = new IdentityUser
                {
                    Id = "fake-322",
                    UserName = "fake@test.com",
                };

                await service.AddItemAsync(new TodoItem { Title = "Testing" }, fakeUser);

                var result = await service.MarkDoneAsync(new Guid(), fakeUser);

                Assert.False(result);
            }
        }

        [Fact]
        public async Task MarkDoneAsync_ShouldReturnTrueMakingCompleteAValidItem()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
               .UseInMemoryDatabase(databaseName: "Test_AddNewItem")
               .Options;

            using (var context = new ApplicationDbContext(options))
            {
                var service = new TodoItemService(context);

                var fakeUser = new IdentityUser
                {
                    Id = "fake-322",
                    UserName = "fake@test.com",
                };

                await service.AddItemAsync(new TodoItem {Title = "Testing" }, fakeUser);
                var item = await context.Items.FirstAsync();

                var result = await service.MarkDoneAsync(item.Id, fakeUser);

                Assert.True(result);
            }
        }
    }
}
