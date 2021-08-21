using log4net;
using log4net.Config;
using Ravencoin.ApplicationCore.BusinessLogic;
using Ravencoin.ApplicationCore.Models;
using System;
using System.Threading.Tasks;
using System.Configuration;
namespace Ravencoin.AssetSender
{
    class Program
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(Program));
        static async Task Main(string[] args)
        {
            XmlConfigurator.Configure();

            ServerConnection serverConnection = new ServerConnection{
                host = ConfigurationManager.AppSettings["host"],
                port = Int32.Parse(ConfigurationManager.AppSettings["port"]),
                username = ConfigurationManager.AppSettings["username"],
                password = ConfigurationManager.AppSettings["password"]
        };

            //Pull values from the config file
            string rvnListenAddress = ConfigurationManager.AppSettings["rvnListenAddress"];
            string assetToSend = ConfigurationManager.AppSettings["assetToSend"];
            int multiplier = Int32.Parse(ConfigurationManager.AppSettings["multiplier"]);
            try {
                //Get the txid from the args that the ravencoin node will supply when a wallet transaction comes in.
                if (args.Length > 0){
                    string txid = args[0].ToString();
                    log.Info($"Incoming transaction detected from {txid}");
                    log.Info($"Beginning asset send for {txid}");

                    //Send the details to the ExchangeRvnForAsset function in ApplicationCore.
                    ServerResponse response = await Assets.ExchangeRvnForAsset(txid, rvnListenAddress, assetToSend, multiplier, 1, serverConnection);

                    //Check if everything worked, and log the transaction ID of the Asset send. Otherwise, log the error.
                    if (response.statusCode == System.Net.HttpStatusCode.OK){
                        log.Info($"Asset delivered successfully. Resulting transaction id = {response.responseContent}");
                        log.Info($"Confirmations: {response.errorEx}");
                    }
                    else{
                        log.Error($"Error: {response.errorEx}");
                    }
                }
                else{
                    log.Error("No transaction ID passed in arguments");
                }
            }
            catch (Exception ex){
                log.Error(ex.Message);
            }
        }
    }
}
