using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using web.Data;
using web.Models;
using Microsoft.AspNetCore.Identity;

namespace web.Controllers
{

    [Authorize]
    public class UserController : Controller
    {
        private readonly PlanerContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private string query;

        public UserController(PlanerContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: User
        public async Task<IActionResult> Index()
        {
            //return View(await _context.Users.ToListAsync());
            //get logged in user
            var user = await _userManager.GetUserAsync(User);
            var user1 = await _userManager.GetLoginsAsync(user);
            //userFacebookId
            var facebookId = user1[0].ProviderKey;

            //create user if he doesn't exist
            if(!UsersExists(Convert.ToInt64(facebookId))){
        
                string firstName = user.UserName.Split(".")[0];
                string  lastName = user.UserName.Split(".")[1].Split("@")[0];
        
                var format = "yyyy-MM-dd HH:mm:ss:fff";
                var stringDate = DateTime.Now.ToString(format);

                query = string.Format("insert into users values ({0},'{1}','{2}','{3}')",facebookId,firstName,lastName,stringDate);
                await _context.Database.ExecuteSqlCommandAsync(query);
            }



            //List of friends from facebook query temp
            var Friend1 = new { Id = 2, Name = "Foo" };
            var Friend2 = new { Id = 3, Name = "Fai" };
            var listOfFriends = new[] { Friend1, Friend2 }.ToList(); //get list from facebook

            //Extract friendIDs
            var friendIdList = new List<long>();
            foreach (var item in listOfFriends)
            {
                friendIdList.Add(item.Id);
            }

            //Get data from Database
            query = string.Format("Select * from users where userId IN ({0})",String.Join(",",friendIdList));
            return View(await _context.Users.FromSqlRaw(query).ToListAsync());

        }


        // GET: User/Details/5
        public async Task<IActionResult> Details(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var users = await _context.Users
                .FirstOrDefaultAsync(m => m.userId == id);
            if (users == null)
            {
                return NotFound();
            }

            return View(users);
        }

        // GET: User/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: User/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("userId,firstName,lastName,lunchTime")] Users users)
        {
            if (ModelState.IsValid)
            {
                _context.Add(users);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(users);
        }

        // GET: User/Edit/5
        public async Task<IActionResult> Edit(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var users = await _context.Users.FindAsync(id);
            if (users == null)
            {
                return NotFound();
            }
            return View(users);
        }

        // POST: User/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id, [Bind("userId,firstName,lastName,lunchTime")] Users users)
        {
            if (id != users.userId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(users);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if(!UsersExists(users.userId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(users);
        }

        // GET: User/Delete/5
        public async Task<IActionResult> Delete(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var users = await _context.Users
                .FirstOrDefaultAsync(m => m.userId == id);
            if (users == null)
            {
                return NotFound();
            }

            return View(users);
        }

        // POST: User/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(long id)
        {
            var users = await _context.Users.FindAsync(id);
            _context.Users.Remove(users);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool UsersExists(long id)
        {
            return _context.Users.Any(e => e.userId == id);
        }
    }
}
