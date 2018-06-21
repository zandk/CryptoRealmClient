using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;
using Loom.Unity3d;
using Loom.Nethereum.ABI.FunctionEncoding.Attributes;
using Org.BouncyCastle.Math;

[FunctionOutput]
public class Tile
{
    [Parameter("address", "owner", 1)]
    public string Owner {get; set;}

    [Parameter("int", "x", 2)]
    public BigInteger Description {get; set;}

    [Parameter("int", "y", 3)]
    public BigInteger Sender {get; set;}
}
public class RealmBase : MonoBehaviour {

    // Select an ABI from our project resources
    // We got these from the migration script in Truffle
    public TextAsset realmBaseABI;
    public TextAsset realmBaseAddress;

    // Contract object
    public static EvmContract contract;

    // Use this for initialization
	public async void Start () {
        // Generate new keys for this user
        // TODO - Either store these or let the user enter a private key
        var privateKey = CryptoUtils.GeneratePrivateKey();
        var publicKey = CryptoUtils.PublicKeyFromPrivateKey(privateKey);

        // Get the contract
        RealmBase.contract = await GetContract(privateKey, publicKey);
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
        string abi = realmBaseABI.ToString();
        // Address of the Solidity contract
        var contractAddr = Address.FromHexString(realmBaseAddress.ToString());
        // Address of the user
        var callerAddr = Address.FromPublicKey(publicKey);
        // Return the Contract object
        return new EvmContract(client, contractAddr, callerAddr, abi);
    }

    public static async Task<int> GetTileCount()
    {
        if (RealmBase.contract == null)
        {
            throw new Exception("Not signed in!");
        }

        Debug.Log("Calling smart contract...");

        int result = await RealmBase.contract.StaticCallSimpleTypeOutputAsync<int>("GetTileCount");
        Debug.Log("Smart contract returned: " + result);
        return result;
    }

    public static async void GetTile(int id)
    {
        if (RealmBase.contract == null)
        {
            throw new Exception("Not signed in!");
        }

        Debug.Log("Calling smart contract...");

        Tile result = await RealmBase.contract.StaticCallDTOTypeOutputAsync<Tile>("GetTile", id);
        Debug.Log("Smart contract returned: " + result);
    }
}
