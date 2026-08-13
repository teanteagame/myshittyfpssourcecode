using System.Collections;
using UnityEngine;

namespace TNT
{
    public class PlayerCamera : MonoBehaviour
    {       
        public float sensitivityX = 15F;
        public float sensitivityY = 15F;

        public float minimumX = -60F;
        public float maximumX = 60F;

        public float minimumY = -60F;
        public float maximumY = 60F;   

        private float offsetY = 0F;
        private float offsetX = 0F;
        private float rotationX = 0F;
        private float rotationY = 0F;  

        public Transform cameraRoot;
        public Camera mainCamera;
        public Transform fallEffect;

        Quaternion originalRotation;
        Quaternion originalCameraRotation;

        private PlayerMovement movement;
        private PlayerInputs input;
        private CharacterController controller;

        void Start()
        {
            movement = GetComponentInParent<PlayerMovement>();
            input = GetComponentInParent<PlayerInputs>();
            controller = GetComponentInParent<CharacterController>();

            originalRotation = movement.transform.localRotation;
            originalCameraRotation = transform.localRotation;
        }

        void Update()
        {
            if (Cursor.lockState == CursorLockMode.None) return;

            rotationX += (input.mouseX * sensitivityX / 60 * mainCamera.fieldOfView + offsetX);
            rotationX = ClampAngle(rotationX, minimumX, maximumX);

            Quaternion xQuaternion = Quaternion.AngleAxis(rotationX, Vector3.up);
            movement.transform.localRotation = originalRotation * xQuaternion;

            rotationY += (input.mouseY * sensitivityY / 60 * mainCamera.fieldOfView + offsetY);
            rotationY = ClampAngle(rotationY, minimumY, maximumY);

            Quaternion yQuaternion = Quaternion.AngleAxis(rotationY, Vector3.left);
            transform.localRotation = originalCameraRotation * yQuaternion;          

            offsetY = 0F;
            offsetX = 0F;           
        }
       
        private void LateUpdate()
        {
            fallEffect.localRotation = Quaternion.Slerp(fallEffect.localRotation, Quaternion.identity, 10 * Time.deltaTime);
            cameraRoot.localPosition = new Vector3(0, controller.height - 0.2f, 0);
        }

        public static float ClampAngle(float angle, float min, float max)
        {
            if (angle < -360F)
                angle += 360F;
            if (angle > 360F)
                angle -= 360F;
            return Mathf.Clamp(angle, min, max);
        }

        public void SetRotation(float r)
        {
            rotationX = r;
        }

        public IEnumerator FallCamera(Vector3 d, float ta)
        {
            Quaternion s = fallEffect.localRotation;
            Quaternion e = fallEffect.localRotation * Quaternion.Euler(d);

            float r = 1.0f / ta;
            float t = 0.0f;
            while (t < 1.0f)
            {
                t += Time.deltaTime * r;
                fallEffect.localRotation = Quaternion.Slerp(s, e, t);
                yield return null;
            }
        }        
    }
}
