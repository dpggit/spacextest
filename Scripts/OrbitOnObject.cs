using UnityEngine;
using UnityEngine;
using System.Collections;

namespace SpaceX
{
    public class OrbitOnObject : MonoBehaviour
    {
        public Transform target;
        public float distance = 20.0f;
        public float xSpeed = 120.0f;
        public float ySpeed = 120.0f;

        public float yMinLimit = -20f;
        public float yMaxLimit = 80f;

        public float distanceMin = .5f;
        public float distanceMax = 15f;

        public float smoothTime = 2f;

        float rotationYAxis = 0.0f;
        float rotationXAxis = 0.0f;

        float velocityX = 0.0f;
        float velocityY = 0.0f;

        Camera mainCamera;

        void Start()
        {

            Vector3 angles = transform.eulerAngles;
            rotationYAxis = angles.y;
            rotationXAxis = angles.x;
            mainCamera = Camera.main;
        }

        void LateUpdate()
        {
            if (target)
            {
                if (Input.touchCount > 0)
                {
                    Touch touch = Input.GetTouch(0);
                    CameraMovement(mainCamera.ScreenToWorldPoint(touch.position), touch.phase);
                }

                rotationYAxis += velocityX;
                rotationXAxis -= velocityY;

                rotationXAxis = ClampAngle(rotationXAxis, yMinLimit, yMaxLimit);

                Quaternion fromRotation = Quaternion.Euler(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y, 0);
                Quaternion toRotation = Quaternion.Euler(rotationXAxis, rotationYAxis, 0);
                Quaternion rotation = toRotation;

                Vector3 negDistance = new Vector3(0.0f, 0.0f, -distance);
                Vector3 position = rotation * negDistance + target.position;

                transform.rotation = rotation;
                transform.position = position;

                velocityX = Mathf.Lerp(velocityX, 0, Time.deltaTime * smoothTime);
                velocityY = Mathf.Lerp(velocityY, 0, Time.deltaTime * smoothTime);
            }

        }

        void CameraMovement(Vector3 pos, TouchPhase phase)
        {
            switch (phase)
            {
                case TouchPhase.Began:
                    break;
                case TouchPhase.Moved:
                    velocityX += xSpeed * Input.GetAxis("Mouse X") * 0.02f;
                    velocityY += ySpeed * Input.GetAxis("Mouse Y") * 0.02f;
                    break;
                case TouchPhase.Ended:
                    break;
            }
        }

        public static float ClampAngle(float angle, float min, float max)
        {
            if (angle < -360F)
                angle += 360F;
            if (angle > 360F)
                angle -= 360F;
            return Mathf.Clamp(angle, min, max);
        }
    }
}