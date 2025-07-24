using UnityEngine;
using System.Collections;

public class PlayerCamera : MonoBehaviour
{
    public CameraMode cameraMode = CameraMode.FPSView;

    public float sensitivityX = 15F;
    public float sensitivityY = 15F;

    public float minimumX = -60F;
    public float maximumX = 60F;

    public float minimumY = -60F;
    public float maximumY = 60F;

    public float leanPosOffset = 0.4f;
    public float leanRotOffset = 3;

    private float offsetY = 0F;
    private float offsetX = 0F;
    private float rotationX = 0F;
    private float rotationY = 0F;
    private float x, y;

    public Transform cameraRoot;
    public Camera mainCamera;
    public Transform fallEffect;

    Quaternion originalRotation;
    Quaternion originalCameraRotation;

    private PlayerMovement movement;
    private PlayerInputs input;

    void Start()
    {
        movement = GetComponentInParent<PlayerMovement>();
        input = GetComponentInParent<PlayerInputs>();
       
        originalRotation = movement.transform.localRotation;
        originalCameraRotation = transform.localRotation;
    }

    void Update()
    {
        switch (cameraMode)
        {
            case CameraMode.FPSView:
                FPSView();
                break;
            case CameraMode.RagdollView:
                RagdollView();
                break;
            case CameraMode.SpectatorView:
                SpectatorView();
                break;
        }
    }

    private void FPSView()
    {
        if (Cursor.lockState == CursorLockMode.None) return;

        rotationX += ((input.mouseX * sensitivityX / 60 * mainCamera.fieldOfView + offsetX) + x);
        rotationX = ClampAngle(rotationX, minimumX, maximumX);

        Quaternion xQuaternion = Quaternion.AngleAxis(rotationX, Vector3.up);
        movement.transform.localRotation = originalRotation * xQuaternion;

        rotationY += ((input.mouseY * sensitivityY / 60 * mainCamera.fieldOfView + offsetY) + y);
        rotationY = ClampAngle(rotationY, minimumY, maximumY);

        Quaternion yQuaternion = Quaternion.AngleAxis(rotationY, Vector3.left);
        transform.localRotation = originalCameraRotation * yQuaternion;

        #region Leaning
        Vector3 leanPos = new Vector3(input.leanAxis * leanPosOffset, 0, 0);
        Quaternion leanRot = Quaternion.Euler(new Vector3(0, 0, input.leanAxis * -leanRotOffset));
        mainCamera.transform.localPosition = leanPos;
        mainCamera.transform.localRotation = leanRot;
        #endregion

        offsetY = 0F;
        offsetX = 0F;

        x = Mathf.Lerp(x, 0, Time.deltaTime * 2);
        y = Mathf.Lerp(y, 0, Time.deltaTime * 2);
    }

    private void RagdollView()
    {

    }

    private void SpectatorView()
    {

    }

    private void LateUpdate()
    {
        fallEffect.localRotation = Quaternion.Slerp(fallEffect.localRotation, Quaternion.identity, 10 * Time.deltaTime);
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

    public void Recoil(Vector2 recoilX, Vector2 recoilY)
    {
        x = Random.value > 0.5f ? recoilX.y : recoilX.x;
        y = Random.Range(recoilY.x, recoilY.y);
    }
}

public enum CameraMode
{
    FPSView, RagdollView, SpectatorView
}