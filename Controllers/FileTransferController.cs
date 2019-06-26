using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using UTFBox_Server.Hubs;
using UTFBox_Server.Models;

namespace UTFBox_Server.Controllers
{
    [Controller]
    [Route("Transfer")]
    public class FileTransferController : Controller
    {
        const string _repository = "./Repository";
        private readonly IHubContext<ServerHub> _hubContext;
        public FileTransferController(IHubContext<ServerHub> hubContext)
        {
            _hubContext = hubContext;
        }

        [HttpPost]
        [Route("Send")]
        public async Task<IActionResult> SendFile([FromBody] Revision revision){
            
            string path;
            
            // Verifica se o arquivo é público 
            if(revision.isPublic)
                path = Path.Combine(_repository, revision.fileName);
            else
                path = Path.Combine(_repository, revision.userName, revision.fileName);
            
            // Verifica se o diretório Existe
            if(!Directory.Exists(path))
                Directory.CreateDirectory(path);
            
            await _hubContext.Clients.All.SendAsync(revision.userName + 
                            " iniciou uma transferencia de arquivo: " + revision.fileName);
            
            //await System.IO.File.WriteAllBytesAsync(path, file);

            await _hubContext.Clients.All.SendAsync("Transferencia finalizada: " + revision.fileName);

            return Ok();
        }

        [HttpDelete]
        [Route("Delete")]
        public async Task<IActionResult> DeleteFile([FromBody] Revision revision)
        {
            string path;

            if(revision.isPublic)
                path = Path.Combine(_repository, revision.fileName);
            else
                path = Path.Combine(_repository, revision.userName, revision.fileName);

            Directory.Delete(path);
            await _hubContext.Clients.All.SendAsync(revision.userName + " apagou um arquivo: " + revision.fileName);

            await Task.Yield();
            return Ok();
        }

        [HttpGet]
        [Route("Download")]
        public async Task<IActionResult> DownloadFile([FromBody] Revision revision)
        {
            string path;

            if(revision.isPublic)
                path = Path.Combine(_repository, revision.fileName);
            else
                path = Path.Combine(_repository, revision.userName, revision.fileName);

            var response = await System.IO.File.ReadAllBytesAsync(path);
            await _hubContext.Clients.All.SendAsync(revision.userName + " realizou o download um arquivo: " + revision.fileName);
            
            await Task.Yield();
            return new FileContentResult(response ,"application/octet-stream");
        }
    }
}