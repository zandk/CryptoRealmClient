using System;
using System.IO;
using System.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;
using Loom.Unity3d;
using Loom.Nethereum.ABI.FunctionEncoding.Attributes;
using Org.BouncyCastle.Math;
using Newtonsoft.Json;
using System.Runtime.Serialization.Formatters.Binary;

[FunctionOutput]
public class TileData
{
    [Parameter("address", "owner", 1)]
    public string Owner {get; set;}

    [Parameter("int", "x", 2)]
    public Int16 X {get; set;}

    [Parameter("int", "y", 3)]
    public Int16 Y {get; set;}

    [Parameter("uint8", "resource", 4)]
    public uint Resource {get; set;}

    [Parameter("uint8", "resourceQuantity", 5)]
    public uint ResourceQuantity {get; set;}

    [Parameter("uint8", "improvement", 6)]
    public BigInteger Improvement {get; set;}

    public uint id;

}

public class OnUpdateTileOwnerEvent
{
    [Parameter("uint", "tileId", 1)]
    public BigInteger TileId {get; set;}
    [Parameter("address", "newOwner", 2)]
    public string NewOwner {get; set;}
}

public class OnUpdateTileImprovementEvent
{
    [Parameter("uint", "tileId", 1)]
    public BigInteger TileId {get; set;}
    [Parameter("uint8", "improvementId", 2)]
    public BigInteger ImprovementId {get; set;}
}

public class OnNewTileEvent
{
    [Parameter("uint", "tileId", 1)]
    public BigInteger TileId {get; set;}
}

[Serializable]
public class AccountData {
    public byte[] PrivateKey;
    public byte[] PublicKey;
    public AccountData(byte[] privateKey, byte[] publicKey) {
        PrivateKey = privateKey;
        PublicKey = publicKey;
    }
}

public class RealmBase : MonoBehaviour {

    // Select an ABI from our project resources
    // We got these from the migration script in Truffle
    public TextAsset realmBaseABI;
    public TextAsset realmBaseAddress;

    // Contract object
    public static EvmContract contract;

    // Specify which account to use
    public static string accountId = "";

    // Use this for initialization
	public async void Start () {
        // Generate new keys for this user
        // TODO - Either store these or let the user enter a private key
        // var privateKey = CryptoUtils.GeneratePrivateKey();
        // var publicKey = CryptoUtils.PublicKeyFromPrivateKey(privateKey);
        // var accountData = new AccountData(privateKey, publicKey);
        var accountData = LoadAccountData();
        if (accountData == null) {
            accountData = CreateAccountData();
        }
        
        
        print("Private key: " + accountData.PrivateKey);

        // Get the contract
        RealmBase.contract = await GetContract(accountData.PrivateKey, accountData.PublicKey);
	}

    // Save account data
    AccountData CreateAccountData() {
        var privateKey = CryptoUtils.GeneratePrivateKey();
        var publicKey = CryptoUtils.PublicKeyFromPrivateKey(privateKey);
        AccountData accountData = new AccountData(privateKey, publicKey);
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Open(Application.persistentDataPath + "/account" + RealmBase.accountId + ".dat", FileMode.OpenOrCreate);
        bf.Serialize(file, accountData);
        file.Close();
        return accountData;
    }

    // // Attempt to load account data
    AccountData LoadAccountData() {
        if (File.Exists(Application.persistentDataPath + "/account" + RealmBase.accountId + ".dat")) {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + "/account" + RealmBase.accountId + ".dat", FileMode.Open);
            AccountData accountData = (AccountData)bf.Deserialize(file);
            file.Close();
            return accountData;
        } else {
            return null;
        }
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

    // -----
    // METHOD CALLS
    // -----
    public static async Task<bool> ClaimTile(uint id) {
        if (RealmBase.contract == null)
        {
            throw new Exception("Not signed in!");
        }

        bool result = await RealmBase.contract.CallSimpleTypeOutputAsync<bool>("ClaimTile", new BigInteger(id.ToString()));
        return result;
    }

    public static async void ImproveTile(uint id, uint improvement) {
        if (RealmBase.contract == null)
        {
            throw new Exception("Not signed in!");
        }
        print("Improving tile!!: " + id + " with: " + improvement);


        bool result = await RealmBase.contract.CallSimpleTypeOutputAsync<bool>("ImproveTile", new object[2]{new BigInteger(id.ToString()), new BigInteger(improvement.ToString())});
    }

    // -----
    // STATIC CALLS
    // -----

    public static async Task<int> GetTileCount()
    {
        if (RealmBase.contract == null)
        {
            throw new Exception("Not signed in!");
        }

        int result = await RealmBase.contract.StaticCallSimpleTypeOutputAsync<int>("GetTileCount");
        Debug.Log("Smart contract returned: " + result);
        return result;
    }

    public static async Task<TileData> GetTile(uint id)
    {
        if (RealmBase.contract == null)
        {
            throw new Exception("Not signed in!");
        }

        TileData result = await RealmBase.contract.StaticCallDTOTypeOutputAsync<TileData>("GetTile", id);
        return result;
    }
}
