using Newtonsoft.Json.Linq;
using Ravencoin.ApplicationCore.Models;
using System;
using System.Threading.Tasks;

namespace Ravencoin.ApplicationCore.BusinessLogic
{
     public class Transactions
    {
        public static async Task<ServerResponse> GetRawTransaction(string txid, ServerConnection connection)
        {
            //Set up parameters to get the hex string of the transaction
            JObject commandParams = new JObject(
                new JProperty("txid", txid)
            );
            //Set up the Ravcencoin Object
            ServerCommand request = new ServerCommand(){
                commandId = "0",
                commandMethod = "getrawtransaction",
                commandParams = commandParams,
                commandJsonRpc = "2.0"
            };

            //Get the hex string of the transaction back from getrawtransaction, and then parse it to get just the raw hex string from result
            ServerResponse response = await RpcConnections.RavenCore.Connect(request, connection);

            //Parse the result for the hexstring
            JObject result = JObject.Parse(response.responseContent);
            JToken hexstring = result["result"];

            response.responseContent = hexstring.ToString();
            return response;
        }

        public static async Task<ServerResponse> DecodeRawTransaction(string hexstring, ServerConnection connection)
        {
            //Set up parameters to get the hex string of the transaction
            JObject commandParams = new JObject(
                new JProperty("hexstring", hexstring)
            );
            //Set up the Ravcencoin Object
            ServerCommand request = new ServerCommand(){
                commandId = "0",
                commandMethod = "decoderawtransaction",
                commandParams = commandParams,
                commandJsonRpc = "2.0"
            };

            //Get the hex string of the transaction back from getrawtransaction, and then parse it to get just the raw hex string from result
            ServerResponse response = await RpcConnections.RavenCore.Connect(request, connection);
            return response;
        }

        public static async Task<ServerResponse> GetPublicTransaction(string txid, ServerConnection connection)
        {
            //Get the RawTransaction and return the hexcode
            ServerResponse hexcode = await GetRawTransaction(txid, connection);

            //Get Full Transaction from Hexcode
            ServerResponse response = await DecodeRawTransaction(hexcode.responseContent, connection);

            return response;
        }

        /// <summary>
        /// Gets the transaction details for an in-wallet transaction ID. This will only return data on your own wallets transactions, not external ones. Use GetPublicTransaction for out of wallet transactions
        /// </summary>
        /// <param name="hexstring"></param>
        /// <param name="connection"></param>
        /// <returns>
        /// {
        ///  "amount" : x.xxx,        (numeric) The transaction amount in RVN
        ///  "fee": x.xxx,            (numeric) The amount of the fee in RVN.This is negative and only available for the 
        ///                              'send' category of transactions.
        ///  "confirmations" : n,     (numeric) The number of confirmations
        ///  "blockhash" : "hash",  (string) The block hash
        ///  "blockindex" : xx,       (numeric) The index of the transaction in the block that includes it
        ///  "blocktime" : ttt,       (numeric) The time in seconds since epoch(1 Jan 1970 GMT)
        ///  "txid" : "transactionid",   (string) The transaction id.
        ///  "time" : ttt, (numeric) The transaction time in seconds since epoch (1 Jan 1970 GMT)
        ///  "timereceived" : ttt,    (numeric) The time received in seconds since epoch(1 Jan 1970 GMT)
        ///  "bip125-replaceable": "yes|no|unknown",  (string) Whether this transaction could be replaced due to BIP125(replace-by-fee);
        ///        may be unknown for unconfirmed transactions not in the mempool
        ///  "details" : [
        ///    {
        ///      "account" : "accountname",      (string) DEPRECATED.The account name involved in the transaction, can be "" for the default account.
        ///      "address" : "address",          (string) The raven address involved in the transaction
        ///      "category" : "send|receive",    (string) The category, either 'send' or 'receive'
        ///      "amount" : x.xxx,                 (numeric) The amount in RVN
        ///      "label" : "label",              (string) A comment for the address/transaction, if any
        ///      "vout" : n,                       (numeric) the vout value
        ///      "fee": x.xxx,                     (numeric) The amount of the fee in RVN.This is negative and only available for the 
        ///                                           'send' category of transactions.
        ///      "abandoned": xxx(bool) 'true' if the transaction has been abandoned(inputs are respendable). Only available for the 
        ///                                           'send' category of transactions.
        ///    }
        ///    ,...
        ///  ],
        ///  "asset_details" : [
        ///    {
        ///      "asset_type" : "new_asset|transfer_asset|reissue_asset", (string) The type of asset transaction
        ///      "asset_name" : "asset_name",          (string) The name of the asset
        ///      "amount" : x.xxx,                 (numeric) The amount in RVN
        ///      "address" : "address",          (string) The raven address involved in the transaction
        ///      "vout" : n,                       (numeric) the vout value
        ///      "category" : "send|receive",    (string) The category, either 'send' or 'receive'
        ///    }
        ///    ,...
        ///  ],
        ///  "hex" : "data"(string) Raw data for transaction
        ///}
        /// </returns>
        public static async Task<ServerResponse> GetTransaction(string txid, ServerConnection connection)
        {
            //Set up parameters to get the hex string of the transaction
            JObject commandParams = new JObject(
                new JProperty("txid", txid)
            );
            //Set up the Ravcencoin Object
            ServerCommand request = new ServerCommand()
            {
                commandId = "0",
                commandMethod = "gettransaction",
                commandParams = commandParams,
                commandJsonRpc = "2.0"
            };

            //Get the hex string of the transaction back from getrawtransaction, and then parse it to get just the raw hex string from result
            ServerResponse response = await RpcConnections.RavenCore.Connect(request, connection);
            return response;
        }

        /// <summary>
        /// Returns details about an unspent transaction output.
        /// </summary>
        /// <param name="txid"></param>
        /// <param name="connection"></param>
        /// <returns>
        /// {
        ///  "bestblock" : "hash",    (string) the block hash
        ///  "confirmations" : n,       (numeric) The number of confirmations
        ///  "value" : x.xxx,           (numeric) The transaction value in RVN
        ///  "scriptPubKey" : {         (json object)
        ///     "asm" : "code",       (string) 
        ///     "hex" : "hex",        (string) 
        ///     "reqSigs" : n,          (numeric) Number of required signatures
        ///     "type" : "pubkeyhash", (string) The type, eg pubkeyhash
        ///     "addresses" : [          (array of string) array of raven addresses
        ///        "address"     (string) raven address
        ///        ,...
        ///     ]
        ///  },
        ///  "coinbase" : true|false   (boolean) Coinbase or not
        ///}
        /// </returns>
        public static async Task<ServerResponse> GetTxOut(string txid, ServerConnection connection)
        {
            //Set up parameters to get the hex string of the transaction
            JObject commandParams = new JObject(
                new JProperty("txid", txid),
                new JProperty("n", 0)
            );
            //Set up the Ravcencoin Object
            ServerCommand request = new ServerCommand()
            {
                commandId = "0",
                commandMethod = "gettxout",
                commandParams = commandParams,
                commandJsonRpc = "2.0"
            };

            //Get the hex string of the transaction back from getrawtransaction, and then parse it to get just the raw hex string from result
            ServerResponse response = await RpcConnections.RavenCore.Connect(request, connection);

            return response;
        }

        /// <summary>
        /// Looks up the Transaction info from TxOut and responsds with the number of confirmations.
        /// </summary>
        /// <param name="txid"></param>
        /// <param name="connection"></param>
        /// <returns>string representation of number of confirmations</returns>
        public static async Task<ServerResponse> GetTransactionConfirmations(string txid, ServerConnection connection)
        {
            //Get the RawTransaction and return the hexcode
            ServerResponse response = await GetTxOut(txid, connection);

            //Parse the result for the confirmations
            JObject result = JObject.Parse(response.responseContent);

            //put the confirmations back into the ServerResponse object
            response.responseContent = result["result"]["confirmations"].ToString();

            return response;
        }

        public static async Task<ServerResponse> GetSenderAddress(string txid, ServerConnection connection)
        {
            //Get the transaction info for the incoming txid
            ServerResponse firstTxRequest = await Transactions.GetPublicTransaction(txid, connection);
            JObject  firstTxResponse = JObject.Parse(firstTxRequest.responseContent);

            //The assumption is, if we look up the vin txid from the incoming transaction, and look at the first vout - this SHOULD be the owners wallet. 
            //USE AT YOUR OWN RISK, THIS IS NOT GUARANTEED
            string secondTxId = firstTxResponse["result"]["vin"][0]["txid"].ToString();
            //Grab the vout from the vin of the first transaction. We'll use this to match the previous transactions index of the vout.
            int voutToMatch = Int32.Parse(firstTxResponse["result"]["vin"][0]["vout"].ToString());

            ServerResponse secondTxRequest = await Transactions.GetPublicTransaction(secondTxId, connection);
            JObject secondTxResponse = JObject.Parse(secondTxRequest.responseContent);
            //Parse out for the first address in vout

            secondTxRequest.responseContent = secondTxResponse["result"]["vout"][voutToMatch]["scriptPubKey"]["addresses"][0].ToString();


            return secondTxRequest;
        }
    }
}
