using UnityEngine;

namespace TNT
{
    public class PlayerEffect : MonoBehaviour
    {
        [Header("Footstep")]
        public float crouchLen = 0.5f;
        public float walkLen = 1;
        public float runLen = 2;
        public float climbLen = 0.75f;
        public float swimLen = 0.75f;

        private float currentLen;
        private float stepCycle;

        private CharacterController controller;
        private PlayerMovement movement;
        private PlayerInputs inputs;

        private void Start()
        {
            movement = GetComponent<PlayerMovement>();
            controller = GetComponent<CharacterController>();
            inputs = GetComponent<PlayerInputs>();
        }

        private void Update()
        {
            UpdateFootstep();
        }

        private void UpdateFootstep()
        {
            if (movement.isClimbing)
            {
                currentLen = climbLen;
            }
            else if (movement.isSwiming)
            {
                currentLen = swimLen;
            }
            else
            {                              
                currentLen = movement.isCrouching ? crouchLen : (movement.isRunning ? runLen : walkLen);
            }

            bool canPlay = (movement.isGrounded || movement.isClimbing) || (movement.isSwiming && !inputs.run.hold);

            if (controller.velocity.magnitude >= 0.1f && !movement.isSliding && canPlay)
            {
                stepCycle -= Time.deltaTime;

                if (stepCycle <= 0)
                {
                    stepCycle = currentLen;
                    PlayFootStepSound();
                }
            }
        }

        public void PlayFootStepSound()
        {
            /*
            int index = surfaceProfile.GetIndex(currentSurface);
            List<AudioClip> footsteps = surfaceProfile.GetFootStepSound(index);

            if (footsteps == null) return;
            int n = Random.Range(1, footsteps.Count);
            footstepSource.pitch = Random.Range(0.9f, 1.1f);
            footstepSource.PlayOneShot(footsteps[n]);

            PlayAudioCMD(index);
            */
        }

        public void PlaySwimBurstSound()
        {

        }

        public void PlayLandingSound()
        {

        }
    }
}
