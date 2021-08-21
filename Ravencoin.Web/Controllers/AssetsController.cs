using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Ravencoin.ApplicationCore.Models;
using Ravencoin.ApplicationCore.BusinessLogic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using System;

namespace Ravencoin.Web.Controllers
{
    [ApiController]
    [Route("/api/Assets/[action]")]
    public class AssetsController : Controller
    {
        private readonly ILogger logger;

        //Inject Server Configuration
        private readonly IOptions<ServerConnection> serverConnection;
        public AssetsController(IOptions<ServerConnection> serverConnection, ILogger<AssetsController> logger)
        {
            this.serverConnection = serverConnection;
            this.logger = logger;
        }

        public async Task<ServerResponse> GetAssetData(string asset){
            logger.LogInformation($"Getting Asset data for {asset}");
            try
            {
                ServerResponse response = await Assets.GetAssetData(asset, serverConnection.Value);
                return response;
            } catch (Exception ex)
            {
                logger.LogError($"Exception: {ex.Message}");
                ServerResponse errResponse = new ServerResponse()
                {
                    statusCode = System.Net.HttpStatusCode.InternalServerError,
                    errorEx = ex.Message
                };
                return errResponse;
            }
        }

        public async Task<ServerResponse> TransferAsset(string asset, int quantity, string toAddress){
            logger.LogInformation($"Transferring Asset {asset}, Quantity: {quantity}, To: {toAddress}");
            try
            {
                ServerResponse response = await Assets.TransferAsset(asset, quantity, toAddress, serverConnection.Value);
                return response;
            } catch (Exception ex)
            {
                logger.LogError($"Exception: {ex.Message}");
                ServerResponse errResponse = new ServerResponse()
                {
                    statusCode = System.Net.HttpStatusCode.InternalServerError,
                    errorEx = ex.Message
                };
                return errResponse;
            }
        }
    }
}
