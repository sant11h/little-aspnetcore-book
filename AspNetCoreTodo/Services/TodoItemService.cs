using AspNetCoreTodo.Data;
using AspNetCoreTodo.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AspNetCoreTodo.Services
{
    public class TodoItemService : ITodoItemService
    {
        private readonly ApplicationDbContext context;

        public TodoItemService(ApplicationDbContext context)
        {
            this.context = context;
        }

        public async Task<bool> AddItemAsync(TodoItem item, IdentityUser user)
        {
            item.Id = Guid.NewGuid();
            item.IsDone = false;
            item.DueAt = DateTimeOffset.Now.AddDays(3);
            item.UserId = user.Id;

            await context.Items.AddAsync(item);
            
            var saveResult = await context.SaveChangesAsync();
            return saveResult == 1;
        }

        public async Task<TodoItem[]> GetIncompleteItemsAsync(IdentityUser user)
        {
            return await context.Items
                .Where(x => x.IsDone == false && x.UserId == user.Id)
                .ToArrayAsync();
        }

        public async Task<bool> MarkDoneAsync(Guid id, IdentityUser user)
        {
            var item = await context.Items
                .Where(x => x.Id == id && x.UserId == user.Id)
                .SingleOrDefaultAsync();

            if (item == null) return false;

            item.IsDone = true;

            var saveResult = await context.SaveChangesAsync();
            return saveResult == 1;
        }
    }
}
