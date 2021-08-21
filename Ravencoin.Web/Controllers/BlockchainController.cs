using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Threading.Tasks;
using Ravencoin.ApplicationCore.BusinessLogic;
using Ravencoin.ApplicationCore.Models;
using Newtonsoft.Json.Linq;
using Microsoft.Extensions.Options;

namespace Ravencoin.Web.Controllers
{
    [ApiController]
    [Route("/api/Blockchain/[action]")]
    public class BlockchainController : Controller
    { 
        //Inject Server Configuration
        private readonly IOptions<ServerConnection> serverConnection;
        public BlockchainController(IOptions<ServerConnection> serverConnection)
        {
            this.serverConnection = serverConnection;
        }

        public async Task<ServerResponse> GetBlockchainInfo(){
            ServerResponse response = await Blockchain.GetBlockchainInfo(serverConnection.Value);
            return response;
        }

        public async Task<ServerResponse> GetBlockCount(){
            ServerResponse response = await Blockchain.GetBlockCount(serverConnection.Value);
            return response;
        }
    }
}
