using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;


namespace TheWitnesses
{
    public class Clicker : NetworkBehaviour
    {

        //[SyncVar]
        bool blue = false;



        public Color GetNextColor()
        {
            if (blue)
            {
                blue = false;
                return Color.red;
            }
            else
            {
                blue = true;
                return Color.blue;
            }
        }


       public void ChangeColor(Color col)
        {
            GetComponent<MeshRenderer>().material.color = col;
        }


        [ClientRpc]
        public void RpcPaint()
        {
            ChangeColor(GetNextColor());
            //obj.GetComponent<MeshRenderer>().material.color = col;      // this is the line that actually makes the change in color happen
        }
    }

    

}

