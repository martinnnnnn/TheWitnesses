using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;


namespace TheWitnesses
{
    public class PlayerController : NetworkBehaviour
    {


        public LayerMask layerMask;
        public GameObject CubePrefab;

        [SyncVar]
        private GameObject objectID;
        private NetworkIdentity objNetId;


        void Update()
        {
            if (!isLocalPlayer)
            {
                return;
            }


            if (Input.GetMouseButtonDown(1))
            {
                CmdSpawnCube();
            }


            if (Input.GetMouseButtonDown(0))
            {
                Fire();
            }
        }


        void Fire()
        {
            RaycastHit info;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out info, 100, layerMask))
            {
                Clicker clicker = info.collider.GetComponent<Clicker>();
                if (clicker)
                {
                    CmdPaint(clicker.gameObject);
                    //Grid.Cmd_SetCoord(coord);
                }
            }

        }

        [Command]
        void CmdSpawnCube()
        {
            GameObject cube = Instantiate(CubePrefab, Vector3.zero, new Quaternion());
            NetworkServer.Spawn(cube);
        }


        [Command]
        void CmdPaint(GameObject obj)
        {
            objNetId = obj.GetComponent<NetworkIdentity>();        // get the object's network ID
            objNetId.AssignClientAuthority(connectionToClient);    // assign authority to the player who is changing the color
            obj.GetComponent<Clicker>().RpcPaint();                                    // usse a Client RPC function to "paint" the object on all clients
            objNetId.RemoveClientAuthority(connectionToClient);    // remove the authority from the player who changed the color
        }


    }

}


