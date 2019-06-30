using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using UTFBox_Server.Hubs;
using UTFBox_Server.Models;
using UTFBox_Server.Repositories;

namespace UTFBox_Server.Controllers
{
    [Controller]
    [EnableCors("MyPolicy")]
    [Route("user")]
    public class UserController: Controller
    {
        private readonly IHubContext<ServerHub> _hubContext;
        private readonly FileRepository _repo;
        public UserController(IHubContext<ServerHub> hubContext, FileRepository repo)
        {
            _hubContext = hubContext;
            _repo = repo;
        }

        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> Login([FromBody]User user)
        {
            var usersList = _repo.GetAllUsers();
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
            var usersList = _repo.GetAllUsers();

            if(usersList.Equals(null))
                usersList = new List<User>();

            if(!usersList.Contains(user)){
                _repo.AddUser(user);
                await _hubContext.Clients.All.SendAsync(user.Name + " criado com Sucesso");
                return Created("", user.Name);      
            }

            return Forbid();
        }

        [HttpGet]
        [Route("GetAll")]
        public async Task<List<User>> GetAll()
        {
          await Task.Yield();
          return _repo.GetAllUsers();
        }
    }
}