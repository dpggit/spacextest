using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpaceX
{
    public class Zoom : MonoBehaviour
    {
        private Touch touch0, touch1;
        private Vector2 touch0Pos, touch1Pos;
        private Camera mainCamera;
        private float lastFoV;
        private float startingDistance;

        void Start()
        {
            mainCamera = GetComponent<Camera>();
            lastFoV = mainCamera.fieldOfView;
        }

        void Update()
        {
            if (Input.touchCount >= 2)
            {
                CameraMovement();
            }
        }

        void CameraMovement()
        {
            touch0 = Input.GetTouch(0);
            touch1 = Input.GetTouch(1);

            switch (touch0.phase)
            {
                case TouchPhase.Began:
                    touch0Pos = touch0.position;
                    touch1Pos = touch1.position;
                    startingDistance = Vector2.Distance(touch0Pos, touch1Pos);
                    break;
                case TouchPhase.Moved:
                    float distance;
                    touch0Pos = touch0.position;
                    touch1Pos = touch1.position;
                    distance = Vector2.Distance(touch0Pos, touch1Pos);
                    mainCamera.fieldOfView = lastFoV + (distance - startingDistance) * 0.05f;
                    break;
                case TouchPhase.Ended:
                    lastFoV = mainCamera.fieldOfView;
                    break;
            }
        }
    }
}
