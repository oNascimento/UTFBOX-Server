using System;
using System.Collections.Generic;
using System.Text;
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
    [Route("Transfer")]
    public class FileTransferController : Controller
    {
        private readonly IHubContext<ServerHub> _hubContext;
        private readonly FileRepository _repo;
        public FileTransferController(IHubContext<ServerHub> hubContext, FileRepository repo)
        {
            _hubContext = hubContext;
            _repo = repo;
        }

        [HttpGet]
        [Route("GetAll")]
        public async Task<List<Revision>> GetAll(User user)
        {
            var response = new List<Revision>();

            response = _repo.GetAllRevisionsByUser(user);

            await Task.Yield();
            return response;
        }

        [HttpPost]
        [Route("Send")]
        public async Task<IActionResult> SendFile([FromBody] Revision revision){
            
            string path = Revision.GetFileServerPath(revision);
            
            await _hubContext.Clients.All.SendAsync(revision.userName + 
                            " iniciou uma transferencia de arquivo: " + revision.fileName);
            
            try{
                var encodedBytes = Convert.FromBase64String(
                    Convert.ToBase64String(System.Text.Encoding.Unicode.GetBytes(revision.fileData)));
                await System.IO.File.WriteAllBytesAsync(path, encodedBytes);
            }catch(Exception e){
                return BadRequest("Não foi possivel criar seu arquivo, seu animal");
            }

            revision.LastModificationDate = DateTime.Now;
            await _repo.AddToRepository(revision);
            await _hubContext.Clients.All.SendAsync("Transferencia finalizada: " + revision.fileName);

            return Ok();
        }

        [HttpDelete]
        [Route("Delete")]
        public async Task<IActionResult> DeleteFile([FromBody] Revision revision)
        {
            string path = Revision.GetFileServerPath(revision);

            System.IO.File.Delete(path);
            await _hubContext.Clients.All.SendAsync(revision.userName + " apagou um arquivo: " + revision.fileName);
            await _repo.RemoveOfRepository(revision);

            await Task.Yield();
            return Ok();
        }

        [HttpGet]
        [Route("Download")]
        public async Task<IActionResult> DownloadFile([FromBody] Revision revision)
        {
            string path = Revision.GetFileServerPath(revision);

            var response = await System.IO.File.ReadAllBytesAsync(path);
            await _hubContext.Clients.All.SendAsync(revision.userName + " realizou o download um arquivo: " + revision.fileName);
            
            await Task.Yield();
            return new FileContentResult(response ,"application/octet-stream");
        }

        [HttpPost]
        [Route("Share")]
        public async Task<IActionResult> ShareFile([FromBody] Revision revision, User receiver)
        {
            _repo.ShareRevision(receiver, revision);
            
            await Task.Yield();
            return Created("", null);
        }
    }
}