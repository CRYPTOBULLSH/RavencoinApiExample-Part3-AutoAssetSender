using Ravencoin.ApplicationCore.Models;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using System;
using System.Net;

namespace Ravencoin.ApplicationCore.BusinessLogic
{
    public class Assets
    {
        /// <summary>
        /// Gets data about a particular asset. 
        /// </summary>
        /// <param name="assetName"> asset_name (string, required)</param>
        /// <param name="connection"> ServerConnection (required) </param>
        /// <returns>
        /// Result:
        ///{
        ///  name: (string),
        ///  amount: (number),
        ///  units: (number),
        ///  reissuable: (number),
        ///  has_ipfs: (number),
        ///  ipfs_hash: (hash), (only if has_ipfs = 1 and that data is a ipfs hash)
        ///  txid_hash: (hash), (only if has_ipfs = 1 and that data is a txid hash)
        ///  verifier_string: (string)
        /// }
        /// </returns>
        /// 
        public static async Task<ServerResponse> GetAssetData(string assetName, ServerConnection connection)
        {

            //Wrap properties in a JObject
            JObject commandParams = new JObject();
            commandParams.Add("asset_name", assetName);


            //Set up the ServerCommand
            ServerCommand request = new ServerCommand()
            {
                commandId = "0",
                commandMethod = "getassetdata",
                commandParams = commandParams,
                commandJsonRpc = "2.0"
            };

            //Send the ServerCommand to RavenCore. See comments for Response Value
            ServerResponse response = await RpcConnections.RavenCore.Connect(request, connection);

            return response;
        }

        /// <summary>
        /// Transfers an Asset from your wallet to another.
        /// </summary>
        /// <param name="assetName">asset_name (string, required)</param>
        /// <param name="quantity">quantity (int, required)</param>
        /// <param name="toAddress">toAddress (string, required)</param>
        /// <param name="connection">ServerConnection (required)</param>
        /// <returns>
        /// Result:
        /// txid[txid]
        /// </returns>
        public static async Task<ServerResponse> TransferAsset(string assetName, int quantity, string toAddress, ServerConnection connection)
        {
            //Wrap properties in a JObject
            JObject commandParams = new JObject{
                { "asset_name", assetName },
                {"qty", quantity },
                {"to_address", toAddress }
            };

            //Set up the ServerCommand
            ServerCommand request = new ServerCommand()
            {
                commandId = "0",
                commandMethod = "transfer",
                commandParams = commandParams,
                commandJsonRpc = "2.0"
            };

            //Send the ServerCommand to RavenCore. See comments for Response Value
            ServerResponse response = await RpcConnections.RavenCore.Connect(request, connection);

            return response;
        }

        /// <summary>
        /// Looks up a specific asset from your wallet. This is an in-wallet only transaction.
        /// </summary>
        /// <param name="assetName"></param>
        /// <param name="connection"></param>
        /// <returns>Count of assets in your wallet.</returns>
        public static async Task<ServerResponse> ListMyAssets(string assetName, ServerConnection connection)
        {
            //Wrap properties in a JObject
            JObject commandParams = new JObject{
                { "asset", assetName }
            };

            //Set up the ServerCommand
            ServerCommand request = new ServerCommand()
            {
                commandId = "0",
                commandMethod = "listmyassets",
                commandParams = commandParams,
                commandJsonRpc = "2.0"
            };

            //Send the ServerCommand to RavenCore. See comments for Response Value
            ServerResponse response = await RpcConnections.RavenCore.Connect(request, connection);

            return response;
        }
        public static async Task<ServerResponse> ExchangeRvnForAsset(string txid, string rvnReceiveAddress, string asset, int? multiplier, int minConfirmations, ServerConnection serverConnection)
        {
            int quantity = new int();

            //Get our in-wallet transaction data. Unlike GetPublicTransaction, this will give us details like category, amount, confirmations etc.
            ServerResponse response = await Transactions.GetTransaction(txid, serverConnection);
            JObject result = JObject.Parse(response.responseContent);

            if (result != null)
            {
                //Get the data we need to ensure this transaction is something we want to send an asset to 
                //Check to see is this is a payment in RVN or a fee payment or asset transfer transaction
                JToken detailsArray = result["result"]["details"];
                if (detailsArray.HasValues) {
                    string category = result["result"]["details"][0]["category"].ToString();
                    string receiveAddress = result["result"]["details"][0]["address"].ToString();
                    double amountReceived = Double.Parse(result["result"]["amount"].ToString());
                    int confirmations = Int32.Parse(result["result"]["confirmations"].ToString());

                    //Round down the amount received. Rounding down discourages sending fractional RVN :)
                    int finalAmountReceived = Convert.ToInt32(Math.Floor(amountReceived));

                    //Check if we have a multiplier, and if its higher than one. If the multiplier is set to 100 for example, every 1 RVN Received will get 100 assets in response.
                    if (multiplier > 1 || multiplier != null) { quantity = (int)(finalAmountReceived * multiplier); }

                    //Check to see if we have enough of this asset to send.
                    ServerResponse assetBalanceRequest = await Assets.ListMyAssets(asset, serverConnection);
                    JObject assetBalanceResponse = JObject.Parse(assetBalanceRequest.responseContent);
                    int assetBalance = Int32.Parse(assetBalanceResponse["result"][asset].ToString());

                    if (assetBalance >= quantity)
                    {
                        // Requirements:
                        // Category must be "receive" aka an icoming transaction
                        // Our expecteded receive address must match the one we're watching for
                        // The total amount of RVN is 1 or more.
                        // Confirmations must be equal or greater to the minimum confirmations we want. 
                        if (category == "receive" && receiveAddress == rvnReceiveAddress && finalAmountReceived >= 1 && confirmations >= minConfirmations)
                        {
                            //Do a GetPublicTransaction on the txid to grab the vout address
                            ServerResponse getSenderAddress = await Transactions.GetSenderAddress(txid, serverConnection);

                            if (getSenderAddress != null)
                            {
                                //Validate it's a valid Ravencoin Address  
                                ServerResponse checkIfValid = await Utilities.ValidateAddress(getSenderAddress.responseContent, serverConnection);
                                JObject checkIfValidResponse = JObject.Parse(checkIfValid.responseContent);
                                bool isValid = bool.Parse(checkIfValidResponse["result"]["isvalid"].ToString());

                                if (isValid == true)
                                {
                                    ServerResponse sendAsset = await Assets.TransferAsset(asset, quantity, getSenderAddress.responseContent, serverConnection);
                                    sendAsset.errorEx = confirmations.ToString();
                                    return sendAsset;
                                }
                                else
                                {
                                    return new ServerResponse { statusCode = HttpStatusCode.InternalServerError, errorEx = "Invalid sender address" };
                                }
                            }
                            else
                            {
                                return new ServerResponse { statusCode = HttpStatusCode.InternalServerError, errorEx = "Could not find Sender Address" };
                            }
                        } else
                        {
                            return new ServerResponse { statusCode = HttpStatusCode.InternalServerError, errorEx = $"The transaction didn't meet the minimum requirements. Incoming Address: {rvnReceiveAddress}, Amount: {finalAmountReceived}, Category: {category}, Confirmationss: { confirmations }" };
                        }
                    }
                    else
                    {
                        return new ServerResponse { statusCode = HttpStatusCode.InternalServerError, errorEx = $"Not enough Asset: {asset} left in wallet." };
                    }
                }
                else
                {
                    return new ServerResponse { statusCode = HttpStatusCode.InternalServerError, errorEx = $"Transaction {txid} has no details section. Likely a fee transaction or an asset transfer transaction." };
                }
            } else
            {
                return new ServerResponse { statusCode = HttpStatusCode.InternalServerError, errorEx = $" Could not look up Transaction {txid}" };
            }
        }
    }
}
