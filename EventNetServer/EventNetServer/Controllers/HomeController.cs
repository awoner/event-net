using EventNetServer.Database;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;

namespace EventNetServer.Controllers
{
    public class HomeController : Controller
    {
        public string Index()
        {
            return "Server working...";
        }    

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        [HttpGet]
        public string GetEvents()
        {
            using (var context = new EventNetContext())
            {
                var events = context.Events.ToList();

                foreach (MyEvent myEvent in events)
                {
                    context.Entry(myEvent).Reference(e => e.Beacon).Load();
                }
                
                return JsonConvert.SerializeObject(context.Events.ToList());
            }
        }

        [HttpPost]
        public string SignUp(string str)
        {
            if (str != null)
            {
                User newUser = JsonConvert.DeserializeObject<User>(str);
                if (newUser != null)
                {
                    if (!Regex.Match(newUser.Email, @"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$").Success)
                    {
                        return "ERROR! Invalid e-mail address.";
                    }

                    if (newUser.Name == null)
                    {
                        return "ERROR! Please enter your name!";
                    }

                    using (var context = new EventNetContext())
                    {
                        List<User> users = context.Users.ToList();

                        foreach (var user in users)
                        {
                            if (user.Email == newUser.Email)
                            {
                                return "ERROR! This e-mail already existst.";
                            }
                        }

                        context.Users.Add(newUser);
                        context.SaveChanges();

                        return JsonConvert.SerializeObject(newUser);
                    }
                }
                else
                {
                    return "ERROR! Please send the user data.";
                }
            }
            else
            {
                return "ERROR! Argument is null";
            }           
        }

        [HttpPost]
        public string SignIn(string str)
        {
            if (str != null)
            {
                User user = JsonConvert.DeserializeObject<User>(str);
                if (user != null)
                {
                    using (var context = new EventNetContext())
                    {
                        List<User> users = context.Users.ToList();

                        foreach (var existingUser in users)
                        {
                            if (existingUser.Email == user.Email && existingUser.Password == existingUser.Password)
                            {
                                context.Entry(existingUser).Reference(u => u.Event).Load();
                                return JsonConvert.SerializeObject(existingUser);
                            }
                        }
                        return "ERROR! Invalid e-mail or password!";
                    }
                }
                else
                {
                    return "ERROR! Please send the user data.";
                }
            }
            else
            {
                return "ERROR! Argument is null";
            }
        }

        [HttpPost]
        public void AddToEvent(string str)
        {
            User user = JsonConvert.DeserializeObject<User>(str);
            using (var context = new EventNetContext())
            {
                foreach (var existEvent in context.Events)
                {
                    if (existEvent.Beacon.UUID == user.Event.Beacon.UUID)
                    {
                        existEvent.Users.Add(user);
                        context.SaveChanges();
                    }
                }
            }
        }

        [HttpPost]
        public void RemoveFromEvent(string str)
        {
            User user = JsonConvert.DeserializeObject<User>(str);
            using (var context = new EventNetContext())
            {
                foreach (var existEvent in context.Events)
                {
                    if (existEvent.Beacon.UUID == user.Event.Beacon.UUID)
                    {
                        existEvent.Users.Remove(user);
                        context.SaveChanges();
                    }
                }
            }
        }
    }
}