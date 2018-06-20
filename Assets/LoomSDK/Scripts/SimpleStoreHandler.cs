using System;
using System.Threading.Tasks;
using UnityEngine;
using Loom.Unity3d;
using Loom.Nethereum.ABI.FunctionEncoding.Attributes;

public class SimpleStoreHandler : MonoBehaviour {

    // Select an ABI from our project resources
    // We got these from the migration script in Truffle
    public TextAsset simpleStoreABI;
    public TextAsset simpleStoreAddress;

    // Use this for initialization
	public async void Start () {
        // Generate new keys for this user
        // TODO - Either store these or let the user enter a private key
        var privateKey = CryptoUtils.GeneratePrivateKey();
        var publicKey = CryptoUtils.PublicKeyFromPrivateKey(privateKey);

        // Get the contract
        var contract = await GetContract(privateKey, publicKey);
        // Make a call
        await StaticCallContract(contract);
	}

    // Get's the contract as an object 
    async Task<EvmContract> GetContract(byte[] privateKey, byte[] publicKey)
    {   
        // Get the writer and reader for the Loom node
        var writer = RPCClientFactory.Configure()
            .WithLogger(Debug.unityLogger)
            .WithWebSocket("ws://127.0.0.1:46657/websocket")
            .Create();
        var reader = RPCClientFactory.Configure()
            .WithLogger(Debug.unityLogger)
            .WithWebSocket("ws://127.0.0.1:9999/queryws")
            .Create();

        // Create a client object from them
        var client = new DAppChainClient(writer, reader)
            { Logger = Debug.unityLogger };

        // required middleware
        client.TxMiddleware = new TxMiddleware(new ITxMiddlewareHandler[]
        {
            new NonceTxMiddleware
            {
                PublicKey = publicKey,
                Client = client
            },
            new SignedTxMiddleware(privateKey)
        });

        // ABI of the Solidity contract
        string abi = simpleStoreABI.ToString();
        // Address of the Solidity contract
        var contractAddr = Address.FromHexString(simpleStoreAddress.ToString());
        // Address of the user
        var callerAddr = Address.FromPublicKey(publicKey);
        // Return the Contract object
        return new EvmContract(client, contractAddr, callerAddr, abi);
    }

    public async Task StaticCallContract(EvmContract contract)
    {
        if (contract == null)
        {
            throw new Exception("Not signed in!");
        }

        Debug.Log("Calling smart contract...");

        int result = await contract.StaticCallSimpleTypeOutputAsync<int>("get");
        if (result != null)
        {
            Debug.Log("Smart contract returned: " + result);
        } else
        {
            Debug.LogError("Smart contract didn't return anything!");
        }
    }
}
