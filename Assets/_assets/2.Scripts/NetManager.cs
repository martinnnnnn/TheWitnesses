using UnityEngine;
using UnityEngine.Networking;
using System.Collections;


namespace TheWitnesses
{

    public class NetManager : NetworkManager
    {

        //Transform GridPosition;
        //public GameObject gridPrefab;
        //GameObject grid;

        //void Start()
        //{
        //    GridPosition = transform.Find("GridPosition");

        //}

        //bool gridInitialized = false;

        //public override void OnServerConnect(NetworkConnection conn)
        //{
        //    if (!gridInitialized)
        //    {
        //        gridInitialized = true;
        //        grid = Instantiate(gridPrefab, GridPosition.position, GridPosition.rotation);
        //        NetworkServer.Spawn(grid);
        //    }
        //    base.OnServerConnect(conn);
        //}

    }
}


