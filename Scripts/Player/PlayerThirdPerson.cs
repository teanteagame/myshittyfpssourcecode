using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerThirdPerson : MonoBehaviour
{
    public bool renderThisModel;

    [Header("IKAim")]
    public Transform targetTransform;
    public Transform aimTransform;
    public HumanBone[] humanBones;
    public int iterations = 10;
    [Range(0, 1)] public float weight = 1;
    public float angleLimit = 90;
    public float distanceLimit = 1;

    [Header("Player")]
    public List<ThirdPersonWeapon> thirdPersonWeapons = new List<ThirdPersonWeapon>();
    public GameObject[] helmets;
    public GameObject[] vests;
    public Renderer[] playerGraphics;

    private Animator animator;
    private Transform[] boneTransforms;

    private bool canAim;
    private bool useIK;
    private int currentIndex;
    private Transform shootOrigin;

    private void Start()
    {
        animator = GetComponent<Animator>();
        boneTransforms = new Transform[humanBones.Length];
        for (int i = 0; i < boneTransforms.Length; i++)
        {
            boneTransforms[i] = animator.GetBoneTransform(humanBones[i].bone);
        }

        if (renderThisModel)
        {
            RenderPlayer();
        }
        else
        {
            UnrenderPlayer();
        }
    }

    private void LateUpdate()
    {
        if (canAim)
        {
            Vector3 targetPosition = GetTargetPosition();
            for (int i = 0; i < iterations; i++)
            {
                for (int b = 0; b < boneTransforms.Length; b++)
                {
                    AimIK(boneTransforms[b], targetPosition, weight * humanBones[b].weight);
                }
            }
        }
    }

    private void AimIK(Transform bone, Vector3 targetPosition, float weight)
    {
        Vector3 aimDirection = shootOrigin.forward;
        Vector3 targetDirection = targetPosition - shootOrigin.position;
        Quaternion aimTowards = Quaternion.FromToRotation(aimDirection, targetDirection);
        Quaternion blendedRotation = Quaternion.Slerp(Quaternion.identity, aimTowards, weight);
        bone.rotation = blendedRotation * bone.rotation;
    }

    Vector3 GetTargetPosition()
    {
        Vector3 targetDirection = targetTransform.position - shootOrigin.position;
        Vector3 aimDirection = shootOrigin.forward;
        float blendOut = 0.0f;
        float targetAngle = Vector3.Angle(targetDirection, aimDirection);
        if (targetAngle > angleLimit)
        {
            blendOut += (targetAngle - angleLimit) / 50.0f;
        }
        float targetDistance = targetDirection.magnitude;
        if (targetDistance < distanceLimit)
        {
            blendOut += distanceLimit - targetDistance;

        }
        Vector3 direction = Vector3.Slerp(targetDirection, aimDirection, blendOut);
        return shootOrigin.position + direction;
    }

    public void SetMovementAnimatorValues(float vertical, float horizontal, bool isGround, bool isCrouch, bool isProne, bool isClimb, bool isSwim, bool isDead)
    {
        if (animator == null) return;
        animator.SetFloat("Horizontal", horizontal);
        animator.SetFloat("Vertical", vertical);
        animator.SetBool("Ground", isGround);
        animator.SetBool("Crouch", isCrouch);
        animator.SetBool("Prone", isProne);
        animator.SetBool("Climb", isClimb);
        animator.SetBool("Swim", isSwim);
        animator.SetBool("Dead", isDead);
    }

    public void PlayAnimationClip(string animName)
    {
        if (animator == null) return;
        animator.CrossFadeInFixedTime(animName, 0.1f);
    }

    public void PlayFireAnimation(string fireAnimName, bool useMuzzleFlash)
    {
        if (animator == null) return;
        animator.CrossFadeInFixedTime(fireAnimName, 0.1f);

        if (useMuzzleFlash)
        {
            thirdPersonWeapons[currentIndex].muzzleFlash.Stop();
            thirdPersonWeapons[currentIndex].muzzleFlash.Play();
        }
    }

    public void SetArmedAnimatorValue(bool armed, int weaponIndex, bool usingIK)
    {
        if (animator == null) return;
        if (armed)
        {
            canAim = true;
            animator.SetBool("Armed", true);
            //enable weapon
            if (!thirdPersonWeapons[currentIndex].weapon.activeInHierarchy)
            {
                for (int i = 0; i < thirdPersonWeapons.Count; i++)
                {
                    thirdPersonWeapons[i].weapon.SetActive(false);
                }

                thirdPersonWeapons[currentIndex].weapon.SetActive(true);
            }
            shootOrigin = thirdPersonWeapons[currentIndex].weapon.transform;
        }
        else
        {
            canAim = false;
            animator.SetBool("Armed", false);
            //disable all weapon
            for (int i = 0; i < thirdPersonWeapons.Count; i++)
            {
                if (thirdPersonWeapons[i].weapon.activeInHierarchy)
                {
                    thirdPersonWeapons[i].weapon.SetActive(false);
                }
            }
            shootOrigin = aimTransform;
        }
        useIK = usingIK;
        currentIndex = weaponIndex;
    }

    public void SetArmorTPSValue(bool hasHelmet, int helmetIndex, bool hasVest, int vestIndex)
    {
        if (hasHelmet)
        {
            if (!helmets[helmetIndex].activeInHierarchy)
            {
                for (int i = 0; i < helmets.Length; i++)
                {
                    helmets[i].SetActive(false);
                }
                helmets[helmetIndex].SetActive(true);
            }
        }
        else
        {
            for (int i = 0; i < helmets.Length; i++)
            {
                helmets[i].SetActive(false);
            }
        }
        if (hasVest)
        {
            if (!vests[vestIndex].activeInHierarchy)
            {
                for (int i = 0; i < vests.Length; i++)
                {
                    vests[i].SetActive(false);
                }
                vests[vestIndex].SetActive(true);
            }
        }
        else
        {
            for (int i = 0; i < vests.Length; i++)
            {
                vests[i].SetActive(false);
            }
        }
    }

    private void OnAnimatorIK()
    {
        if (animator == null) return;
        if (useIK)
        {
            animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, 1);
            animator.SetIKRotationWeight(AvatarIKGoal.LeftHand, 1);
            animator.SetIKPosition(AvatarIKGoal.LeftHand, thirdPersonWeapons[currentIndex].handIKPos.position);
            animator.SetIKRotation(AvatarIKGoal.LeftHand, thirdPersonWeapons[currentIndex].handIKPos.rotation);
        }
        else
        {
            animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, 0);
            animator.SetIKRotationWeight(AvatarIKGoal.LeftHand, 0);
        }
    }

    public void RenderPlayer()
    {
        if (playerGraphics != null)
        {
            for (int i = 0; i < playerGraphics.Length; i++)
            {
                playerGraphics[i].shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;
            }
        }

        if (thirdPersonWeapons != null)
        {
            for (int i = 0; i < thirdPersonWeapons.Count; i++)
            {
                thirdPersonWeapons[i].weaponRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;
                for (int c = 0; c < thirdPersonWeapons[i].weaponRenderer.transform.childCount; c++)
                {
                    if (thirdPersonWeapons[i].weaponRenderer.transform.GetChild(c).GetComponent<Renderer>())
                    {
                        thirdPersonWeapons[i].weaponRenderer.transform.GetChild(c).GetComponent<Renderer>().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;
                    }
                }
            }
        }
    }

    public void UnrenderPlayer()
    {
        if (playerGraphics != null)
        {
            for (int i = 0; i < playerGraphics.Length; i++)
            {
                playerGraphics[i].shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.ShadowsOnly;
            }
        }

        if (thirdPersonWeapons != null)
        {
            for (int i = 0; i < thirdPersonWeapons.Count; i++)
            {
                thirdPersonWeapons[i].weaponRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.ShadowsOnly;
                for (int c = 0; c < thirdPersonWeapons[i].weaponRenderer.transform.childCount; c++)
                {
                    if (thirdPersonWeapons[i].weaponRenderer.transform.GetChild(c).GetComponent<Renderer>())
                    {
                        thirdPersonWeapons[i].weaponRenderer.transform.GetChild(c).GetComponent<Renderer>().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.ShadowsOnly;
                    }
                }
            }
        }
    }
}

[System.Serializable]
public class ThirdPersonWeapon
{
    public GameObject weapon;
    public MeshRenderer weaponRenderer;
    public ParticleSystem muzzleFlash;
    public Transform handIKPos;
}

[System.Serializable]
public class HumanBone
{
    public HumanBodyBones bone;
    [Range(0, 1)] public float weight;
}
