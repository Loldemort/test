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
using System.Net.Http;
using Newtonsoft.Json;


namespace web.Controllers
{

    [Authorize]
    public class UserController : Controller
    {
        private readonly PlanerContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        static readonly HttpClient client = new HttpClient();
        private string query;

        public UserController(PlanerContext context, UserManager<ApplicationUser> userManager,SignInManager<ApplicationUser> signInManager)
        {
            _context = context;
            _userManager = userManager;
            _signInManager = signInManager;
        }

        

        // GET: User
        public async Task<IActionResult> Index()
        {
            
            //get logged in user
            var user = await _userManager.GetUserAsync(User);
            var user1 = await _userManager.GetLoginsAsync(user);
           
            //userFacebookId in Token
            var facebookId = user1[0].ProviderKey;
            var accessToken = "EAACPZCiLcE78BAMFHdaHpZCsmu6gfyeVMZA1RK2Fzwi6wg59W5nemdo6RJGDvrlXM5VqzZBkOd3PzMjhz4o2JZCLorXDTuhShTIVbEAjnQpPFvcXhhZAy2OZCZBuoPpodJIFZAwc7v9JigYyTY0qPwP4f7cWtdCODPRndB0MiijTWqkhtRpvVv79ZCVVGt6SeuuGjVgrwjv8hE7EhVZCTwZBoEZAd9YZC4lrFmi36215GxYW3gQAZDZD";

            var graphQuery = "https://graph.facebook.com/v9.0/"+facebookId+"?fields=id%"+"2Cname"+"%2Clocation"+"%2Cpicture"+"%2Cfriends"+"&access_token="+accessToken;

            HttpResponseMessage response = await client.GetAsync(graphQuery);
            response.EnsureSuccessStatusCode();
            string responseBody = await response.Content.ReadAsStringAsync();
            

            dynamic facebookData = JsonConvert.DeserializeObject(responseBody);

            var facebookUserName = facebookData.name;
            var facebeookLocation = facebookData.location.name;
            var facebookPicture = facebookData.picture.data.url.ToString();
            //getFriendsId
            var friendsCount = facebookData.friends.data.Count;
            var listOfFriends = new List<string>();
            for (int i = 0; i < friendsCount; i++)
            {
                listOfFriends.Add(facebookData.friends.data[i].id.ToString());
            }
    
            //create user if he doesn't exist
            if(!UsersExists(Convert.ToInt64(facebookId))){

                //ime, priimek, lokacija
                string firstName = facebookUserName.ToString().Split(" ")[0];
                string lastName = facebookUserName.ToString().Split(" ")[1];
        
                var format = "HH:mm";
                //var stringTime = DateTime.Now.ToString(format);
                var stringTime = DateTime.Now.ToString(format);

                query = string.Format("insert into users values ({0},'{1}','{2}','{3}','{4}','{5}')",facebookId,lastName,firstName,stringTime,facebeookLocation,facebookPicture);
                await _context.Database.ExecuteSqlCommandAsync(query);
            }

            query = string.Format("Select * from users where userId IN ({0})",String.Join(",",listOfFriends));
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
        public async Task<IActionResult> Edit(long id, [Bind("userId,firstName,lastName,lunchTime,Location,PictureUrl")] Users users)
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

            //ob deletu odjavi uporabnika
            //await _signInManager.SignOutAsync();

            var user = await _userManager.GetUserAsync(User);
            await _signInManager.SignOutAsync(); 
            await _userManager.DeleteAsync(user);

            return RedirectToAction(nameof(Index));
        }

        private bool UsersExists(long id)
        {
            return _context.Users.Any(e => e.userId == id);
        }
    }
}
