using UnityEngine;
using Mirror;

public class Authority : NetworkBehaviour
{
    NetworkManagerGame network;
    private void Start()
    {
        network = GameObject.Find("Network Manager").GetComponent<NetworkManagerGame>();
        PrintConnectedClients();
    }


    [Command]
    public void CmdAssignAuthority(NetworkIdentity clientId, NetworkIdentity BearNetID)
    {        
        if (!isServer)
        {
            Debug.Log("Not Server");
            return;
        }
        Debug.Log("Before authority" +
            "\nclientID on Server: " + clientId.connectionToClient +
            "\nBearID on Server: " + BearNetID.connectionToClient);

        BearNetID.AssignClientAuthority(clientId.connectionToClient);

        Debug.Log("After authority" +
            "\nclientID on Server: " + clientId.connectionToClient +
            "\nBearID on Server: " + BearNetID.connectionToClient);
    }

    [Command]
    public void CmdRemoveAuthority(NetworkIdentity BearNetID)
    {
        Debug.Log("Before removing authority" +
            "Is Bear Owned: " + BearNetID.isOwned);
        
        BearNetID.RemoveClientAuthority();
        
        Debug.Log("After removing authority" +
            "Is Bear Owned: " + BearNetID.isOwned);
    }

    [Command]
    public void PrintConnectedClients()
    {
        Debug.Log("Clients Connected: " + NetworkServer.connections.Count);
    }
}
