using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using UTFBox_Server.Hubs;
using UTFBox_Server.Models;

namespace UTFBox_Server.Controllers
{
    [Controller]
    [Route("user")]
    public class UserController: Controller
    {
        private readonly IHubContext<ServerHub> _hubContext;
        private List<User> usersList; 
        public UserController(IHubContext<ServerHub> hubContext)
        {
            _hubContext = hubContext;
        }

        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> Login([FromBody]User user)
        {
            
            if(usersList.Where(u => u.UserName == user.UserName && u.Password == user.Password).Any()){

                var loggedUser = usersList.Where(u => u.UserName == user.UserName && u.Password == user.Password).FirstOrDefault();
                await _hubContext.Clients.All.SendAsync(loggedUser.Name + " entrou");
                return Ok(loggedUser.Name);
            }
            
            return Forbid();
        }

        [HttpPost]
        [Route("SignIn")]
        public async Task<IActionResult> Create([FromBody] User user)
        {
            if(usersList.Equals(null))
                usersList = new List<User>();
                
            if(!usersList.Contains(user)){
                usersList.Add(user);
                await _hubContext.Clients.All.SendAsync(user.Name + " criado com Sucesso");
                return Created("", user.Name);      
            }

            return Forbid();
        }
    }
}