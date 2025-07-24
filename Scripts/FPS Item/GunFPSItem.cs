using System.Collections;
using UnityEngine;

public class GunFPSItem : FPSItem
{
    [Header("Setting")]
    public FireMode fireMode;
    public int bulletPerBurst = 3;
    public int pelletCount = 1;
    public float fireRate = 0.1f;

    [Header("Ammo")]
    public int bulletInClip = 30;
    public int clipSize = 30;
    public int bulletLeft = 120;///soon replace by ammo item from inventory
    public BulletObject bulletObject;
    public GameObject shellObject;
    public Transform shootPoint;

    public Vector3 normalPos, aimPos;

    public bool isReady, isAiming, isReloading, isFiring;

    private void Update()
    {
        transform.localPosition = Vector3.Slerp(transform.localPosition, isAiming ? aimPos : normalPos, Time.deltaTime * 4);
    }

    public override void OnFireHold()
    {
        if (fireMode == FireMode.Auto)
        {
            CheckFire();
        }        
    }

    public override void OnFirePress()
    {     
        if(fireMode == FireMode.Semi)
        {
            CheckFire();
        }
        else if (fireMode == FireMode.Burst)
        {

        }
    }

    public override void OnFireRelease()
    {
        if (bulletInClip <= 0)
            CheckReload();
    }

    public override void OnReloadPress()
    {
       
    }

    public override void OnAimHold()
    {
        
    }

    public override void OnAimRelease()
    {
        
    }

    void CheckFire()
    {
        if (!isReady || isReloading || isFiring) return;

        if (bulletInClip > 0)
        {
            Fire();
        }
        else
        {
            DryFire();
        }
    }

    void CheckReload()
    {
        if (isReloading || bulletLeft <= 0 || bulletInClip == clipSize) return;
    }

    void Fire()
    {
        isFiring = true;
        bulletInClip--;

        if (bulletObject != null)
        {
            GameObject bullet = Instantiate(bulletObject, shootPoint.position, transform.rotation).gameObject;
           
        }

        if (shellObject != null)
        {
            Instantiate(shellObject, transform.position, transform.rotation);
        }

        GetComponentInParent<PlayerCamera>().Recoil(new Vector2(0.01f, -0.01f), new Vector2(0.02f, 0.1f));
        GetComponent<Animator>().CrossFadeInFixedTime("Fire", 0.1f);
        StartCoroutine(ResetFiring());
    }

    void DryFire()
    {
        isFiring = true;

        StartCoroutine(ResetFiring());
    }

    public void Reload()
    {

    }

    IEnumerator ResetFiring()
    {
        yield return new WaitForSeconds(fireRate);
        isFiring = false;
    }
}

public enum FireMode
{
    Semi,Auto,Burst
}