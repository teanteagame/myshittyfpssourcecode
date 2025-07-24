using System.Collections.Generic;
using UnityEngine;

public class PlayerEffect : MonoBehaviour
{
    public List<AudioClip> soundBank;
    public AudioSource playerFootstep;
    public AudioSource playerVoice;

    public float stepInterval = 5;

    private float audioStepLengthCrouch = 0.75f;
    private float audioStepLengthWalk = 0.45f;
    private float audioStepLengthRun = 0.25f;
    private float audioStepLenghtClimb = 0.5f;
    private float audioVolumeCrouch = 0.1f;
    private float audioVolumeWalk = 0.2f;
    private float audioVolumeRun = 0.3f;
    private float stepCycle, nextStep;

    private PlayerMovement movement;

    private void Start()
    {
        movement = GetComponent<PlayerMovement>();
        stepCycle = 0;
        nextStep = stepCycle / 2f;
    }

    public void PlayFootsteps(float speed ,string surface)
    {
        CharacterController controller = GetComponent<CharacterController>();

        float length = movement.isClimbing ? audioStepLenghtClimb : (movement.isCrouching ? audioStepLengthCrouch : (movement.isRunning ? audioStepLengthRun : audioStepLengthWalk));
        float volume = movement.isCrouching ? audioVolumeCrouch : (movement.isRunning ? audioStepLengthRun : audioVolumeWalk);

        if(controller.velocity.sqrMagnitude > 0 && movement.velMagnitude != 0)
        {
            stepCycle += (controller.velocity.magnitude + (speed * length)) * Time.fixedDeltaTime;
        }

        if (!(stepCycle > nextStep))
        {
            return;
        }

        nextStep = stepCycle + stepInterval;

        int random = Random.Range(1, 5);

        if (movement.isClimbing)
        {
            playerFootstep.PlayOneShot(GetSound("Ladder (" + random + ")"), audioVolumeWalk);
        }
        else
        {
            if (!movement.isGrounded) return;
            switch (surface)
            {
                case "Grass":
                    playerFootstep.PlayOneShot(GetSound("Grass (" + random + ")"), volume);
                    break;
                case "Dirt":
                    playerFootstep.PlayOneShot(GetSound("Dirt (" + random + ")"), volume);
                    break;
                case "Metal":
                    playerFootstep.PlayOneShot(GetSound("Metal (" + random + ")"), volume);
                    break;
                case "Stone":
                    playerFootstep.PlayOneShot(GetSound("Stone (" + random + ")"), volume);
                    break;
                case "Wood":
                    playerFootstep.PlayOneShot(GetSound("Wood (" + random + ")"), volume);
                    break;
                case "Water":
                    playerFootstep.PlayOneShot(GetSound("Water (" + random + ")"), volume);
                    break;
                default:
                    playerFootstep.PlayOneShot(GetSound("Default (" + random + ")"), volume);
                    break;
            }
        }
    }

    public void PlayJumpAudio()
    {
        if (movement.isClimbing) return;

        playerFootstep.PlayOneShot(GetSound("Jump"));
    }

    public void PlayLandAudio(string surface)
    {
        int random = Random.Range(1, 5);
        switch (surface)
        {
            case "Grass":
                playerFootstep.PlayOneShot(GetSound("Grass (" + random + ")"));
                break;
            case "Dirt":
                playerFootstep.PlayOneShot(GetSound("Dirt (" + random + ")"));
                break;
            case "Metal":
                playerFootstep.PlayOneShot(GetSound("Metal (" + random + ")"));
                break;
            case "Stone":
                playerFootstep.PlayOneShot(GetSound("Stone (" + random + ")"));
                break;
            case "Wood":
                playerFootstep.PlayOneShot(GetSound("Wood (" + random + ")"));
                break;
            case "Water":
                playerFootstep.PlayOneShot(GetSound("Water (" + random + ")"));
                break;
            default:
                playerFootstep.PlayOneShot(GetSound("Default (" + random + ")"));
                break;
        }

        nextStep = stepCycle + .5f;
    }

    public void PlayerMouthAudio(string name)
    {
        playerVoice.PlayOneShot(GetSound(name));
    }

    public AudioClip GetSound(string name)
    {
        for (int i = 0; i < soundBank.Count; i++)
        {
            if (soundBank[i].name == name)
            {
                return soundBank[i];
            }
        }

        return null;
    }
}
