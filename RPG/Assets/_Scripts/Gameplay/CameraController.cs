using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LitJson;
using UnityEngine.SceneManagement;

namespace ayy
{
    public class CameraController : MonoBehaviour
    {
        Camera camera = null;
        GameObject followTarget = null;


        public float cameraHeight = 5;

        private void Awake()
        {
                        
        }

        void Start()
        {
            camera = GetComponent<Camera>();
            transform.position = new Vector3(0, cameraHeight, 0);
            transform.forward = new Vector3(0,-cameraHeight,0);
        }

        void Update()
        {
            
        }

        private void LateUpdate()
        {
            ChaseFollowTarget();
        }


        public void SetFollowTarget(GameObject go)
        {
            followTarget = go;
        }

        public GameObject GetTarget()
        {
            return followTarget;
        }


        private void ChaseFollowTarget()
        {
            if (followTarget == null)
            {
                return;
            }
            transform.position = new Vector3(followTarget.transform.position.x,cameraHeight,followTarget.transform.position.z);
        }

    }
}
