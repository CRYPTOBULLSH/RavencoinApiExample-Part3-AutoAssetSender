using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Ravencoin.ApplicationCore.Models;
using Ravencoin.ApplicationCore.BusinessLogic;
using System.Threading.Tasks;
using System;

namespace Ravencoin.Web.Controllers
{
    [ApiController]
    [Route("api/Transactions/[action]")]
    public class TransactionController : ControllerBase
    {
        private readonly ILogger logger;

        //Inject Server Configuration
        private readonly IOptions<ServerConnection> serverConnection;
        public TransactionController(IOptions<ServerConnection> serverConnection, ILogger<TransactionController> logger)
        {
            this.serverConnection = serverConnection;
            this.logger = logger;
        }

        [HttpGet]
        public async Task<ServerResponse> GetPublicTransaction(string txid)
        {
            logger.LogInformation($"Getting Transaction data for {txid}");
            try
            {
                ServerResponse response = await Transactions.GetPublicTransaction(txid, serverConnection.Value);
                return response;
            }
            catch (Exception ex)
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

        [HttpGet]
        public async Task<ServerResponse> GetTransaction(string txid)
        {
            logger.LogInformation($"Getting Transaction data for {txid}");
            try
            {
                ServerResponse response = await Transactions.GetTransaction(txid, serverConnection.Value);
                return response;
            }
            catch (Exception ex)
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

        [HttpGet]
        public async Task<ServerResponse> GetTxOut(string txid)
        {
            logger.LogInformation($"Getting Transaction txout data for {txid}");
            try
            {
                ServerResponse response = await Transactions.GetTxOut(txid, serverConnection.Value);
                return response;
            }
            catch (Exception ex)
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

        [HttpGet]
        public async Task<ServerResponse> GetTransactionConfirmations(string txid)
        {
            logger.LogInformation($"Getting Transaction confirmations data for {txid}");
            try
            {
                ServerResponse response = await Transactions.GetTransactionConfirmations(txid, serverConnection.Value);
                return response;
            }
            catch (Exception ex)
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

        [HttpGet]
        public async Task<ServerResponse> GetSenderAddress(string txid)
        {
            logger.LogInformation($"Getting Sender address data for {txid}");
            try
            {
                ServerResponse response = await Transactions.GetSenderAddress(txid, serverConnection.Value);
                return response;
            }
            catch (Exception ex)
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
