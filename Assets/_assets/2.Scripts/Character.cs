using UnityEngine;
using UnityEngine.Networking;
using System.Collections;


namespace PaintBall
{
    public class Character : NetworkBehaviour
    {

        public bool smooth;
        public bool lockCursor;
        public float upDownRange = 60;

        public GameObject bulletPrefab;

        private Camera _camera;

        private bool _unLockCursor;
        private CharacterController _character;
        private float rotUpDown = 0;

        public const int maxHealth = 100;

        [SyncVar]
        public int health = maxHealth;

        public override void OnStartLocalPlayer()
        {
            GetComponent<MeshRenderer>().material.color = Color.red;
        }


        void Start()
        {
            if (!isLocalPlayer)
            {
                GetComponentInChildren<Camera>().enabled = false;
                return;
            }

            _camera = GetComponentInChildren<Camera>();
            _camera.enabled = true;

            _character = GetComponent<CharacterController>();

            _unLockCursor = true;
        }



        void Update()
        {
            if (!isLocalPlayer)
                return;

            // Mouse Rotation
            float rotLeftRight = Input.GetAxis("Mouse X");
            transform.Rotate(0, rotLeftRight, 0);

            rotUpDown -= Input.GetAxis("Mouse Y");
            rotUpDown = Mathf.Clamp(rotUpDown, -upDownRange, upDownRange);
            _camera.transform.localRotation = Quaternion.Euler(rotUpDown, 0, 0);



            // Movement
            float movSide = Input.GetAxis("Horizontal") * 5f;
            float movForward = Input.GetAxis("Vertical") * 5f;
            Vector3 movement = new Vector3(movSide, 0, movForward);
            movement = transform.rotation * movement;
            _character.SimpleMove(movement);


            CursorLockUpdate();

            if (Input.GetMouseButtonDown(0))
            {
                CmdFire();
            }

        }

        [Command]
        void CmdFire()
        {
            // This [Command] code is run on the server!

            // create the bullet object locally
            var bullet = (GameObject)Instantiate(
                 bulletPrefab,
                 transform.position + transform.forward,
                 Quaternion.identity);

            bullet.GetComponent<Rigidbody>().velocity = transform.forward * 20;

            // spawn the bullet on the clients
            NetworkServer.Spawn(bullet);

            // when the bullet is destroyed on the server it will automaticaly be destroyed on clients
            Destroy(bullet, 2.0f);
        }



        public void TakeDamage(int amount)
        {
            if (!isServer)
                return;

            health -= amount;
            if (health <= 0)
            {
                health = maxHealth;

                // called on the server, will be invoked on the clients
                RpcRespawn();
                Debug.Log("Dead!");
            }
        }

        [ClientRpc]
        void RpcRespawn()
        {
            if (isLocalPlayer)
            {
                // move back to zero location
                transform.position = Vector3.zero;
            }
        }

        private void CursorLockUpdate()
        {
            if (Input.GetKeyUp(KeyCode.Escape))
            {
                _unLockCursor = false;
            }
            else if (Input.GetMouseButtonUp(0))
            {
                _unLockCursor = true;
            }

            if (_unLockCursor)
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
            else if (!_unLockCursor)
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
        }


    }
}


