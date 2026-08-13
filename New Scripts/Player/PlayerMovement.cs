using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TNT
{
    public class PlayerMovement : MonoBehaviour
    {
        public LayerMask ObstacleLayer;  

        [Header("Speeds")]
        [SerializeField] float crouchSpeed = 2;
        [SerializeField] float walkSpeed = 4;
        [SerializeField] float runSpeed = 8;
        [SerializeField] float jumpSpeed = 6;
        [SerializeField] float climbSpeed = 3;
        [SerializeField] float swimSpeed = 3;
        [SerializeField] float slideSpeed = 10;

        [Header("Param")]
        [SerializeField] float slideLimit = 60;
        [SerializeField] float lerpSpeed = 4;
        [SerializeField] float antiBumpFactor = 0.75f;
        [SerializeField] float antiBunnyHopFactor = 0.2f;
        [SerializeField] float antiSpamCrouchFactor = 0.2f;
        [SerializeField] float gravityMul = 2;
        [SerializeField] float fallDamMul = 2;
        [SerializeField] float fallDamThreshold = 8;
        [SerializeField] float waterOffset = 0.5f;

        [Header("Controller Heights")]
        [SerializeField] float standHeight = 1.8f;
        [SerializeField] float crouchHeight = 1.3f;
        [SerializeField] float swimHeight = 1;       

        [Header("States")]
        public bool isGrounded = true;
        public bool isSliding;
        public bool isRunning;
        public bool isCrouching;
        public bool isSwiming;
        public bool isClimbing;

        private bool canStand;
        private float jumpTimer;
        private float crouchTimer;
        private float fallDistance;
        private float highestPoint;
        private float currentWaterLevel;
        private float swimAccel;    
        public string currentSurface;

        private Vector3 moveDir;
        private Vector3 currentPosition;
        private Vector3 lastPosition;
        private Vector3 climbDir;
        private Vector3 surfaceNormal;

        private CharacterController controller;
        private PlayerInputs inputs;
        private PlayerCamera playerCam;
        private PlayerEffect effect;
        private CollisionFlags collisionFlags;

        private void Awake()
        {
            controller = GetComponent<CharacterController>();
            inputs = GetComponent<PlayerInputs>();
            playerCam = GetComponentInChildren<PlayerCamera>();
            effect = GetComponent<PlayerEffect>();
            Cursor.lockState = CursorLockMode.Locked;
        }

        private void Update()
        {           
            float inputX = inputs.horizontal;
            float inputY = inputs.vertical;

            CheckSuroundings();

            if (isClimbing)
            {
                LadderMovement(inputX, inputY);
            }
            else if (isSwiming)
            {
                SwimMovement(inputX, inputY);
            }
            else if (isSliding)
            {
                SlideMovement(inputX, inputY);
            }
            else if (isGrounded)
            {
                GroundedMovement(inputX, inputY);
            }
            else
            {
                AerialMovement(inputX, inputY);
            }

            collisionFlags = controller.Move(moveDir * Time.deltaTime);
        }

        private void GroundedMovement(float inputX, float inputY)
        {
            float inputModifyFactor = (inputX != 0.0f && inputY != 0.0f) ? .7071f : 1.0f;
            float speed = isCrouching ? crouchSpeed : (isRunning ? runSpeed : walkSpeed);
            if (Cursor.lockState == CursorLockMode.Locked)
                moveDir = new Vector3(inputX * inputModifyFactor, -antiBumpFactor, inputY * inputModifyFactor);
            else
                moveDir = new Vector3(0, -antiBumpFactor, 0);

            moveDir = transform.TransformDirection(moveDir);
            moveDir *= speed;

            if (!inputs.jump.hold)
            {
                jumpTimer += Time.deltaTime;
            }
            else if (jumpTimer >= antiBunnyHopFactor)
            {
                jumpTimer = 0;
                moveDir.y = jumpSpeed;
                isGrounded = false;
                Debug.Log("Jump");
            }

            if (inputs.crouch.hold)
            {
                if (crouchTimer + antiSpamCrouchFactor < Time.time)
                    isCrouching = true;
            }
            else
            {
                if (canStand) 
                {
                    isCrouching = false;
                    crouchTimer = Time.time;
                }
            }

            isRunning = inputs.run.hold && inputY > 0.1f;
        }

        private void SlideMovement(float inputX, float inputY)
        {
            Vector3 slideDirection = Vector3.ProjectOnPlane(Vector3.down, surfaceNormal);
            moveDir = slideDirection * slideSpeed;
        }

        private void AerialMovement(float inputX, float inputY)
        {
            moveDir += gravityMul * Time.deltaTime * Physics.gravity;

            currentPosition = transform.position;
            if (currentPosition.y > lastPosition.y)
            {
                highestPoint = transform.position.y;
                lastPosition.y = highestPoint;
            }
        }

        private void SwimMovement(float inputX, float inputY)
        {
            float surfaceDistance = (currentWaterLevel - waterOffset) - transform.position.y;

            if (swimAccel > 0.0f)
                swimAccel -= Time.deltaTime * 4.0f;

            if (inputs.run.hold && inputY > 0.1f)
            {
                Vector3 swimDir = playerCam.transform.TransformDirection(Vector3.forward);

                if (swimDir.y > 0 && swimDir.y > surfaceDistance) swimDir.y = surfaceDistance;

                if (swimAccel <= 1.0f)
                {
                    swimAccel = jumpSpeed;
                    effect.PlaySwimBurstSound();
                }

                if (swimAccel > 1.0f)
                    moveDir = swimDir * swimAccel;
            }
            else
            {
                Vector3 desiredMove = transform.forward * inputY + transform.right * inputX;

                float floatSpeed = swimSpeed * 0.33f;
                float tolerance = 0.05f;

                if (Mathf.Abs(surfaceDistance) > tolerance)
                {
                    moveDir.y = Mathf.Sign(surfaceDistance) * floatSpeed;
                }
                else
                {
                    moveDir.y = 0f;
                }

                moveDir.x = desiredMove.x * swimSpeed;
                moveDir.z = desiredMove.z * swimSpeed;
            }

            highestPoint = transform.position.y;
            fallDistance = 0;
        }

        private void LadderMovement(float inputX, float inputY)
        {
            Vector3 verticalMove;
            verticalMove = climbDir.normalized;
            verticalMove *= inputY * climbSpeed;

            float moveY = 0;

            if (controller.isGrounded)
            {
                moveY = inputY;
            }
            else
            {
                moveY = Mathf.Clamp01(moveY);
            }

            Vector3 desireMove = new Vector3(0, 0, moveY);
            desireMove = transform.TransformDirection(desireMove);
            moveDir = verticalMove + desireMove;

            highestPoint = transform.position.y;
            fallDistance = 0.0f;
        }

        private void CheckSuroundings()
        {
            #region Standing/Crouching
            float radius = controller.radius;
            Vector3 origin = transform.position;

            canStand = !Physics.SphereCast(new Ray(origin + Vector3.up * 0.1f, Vector3.up), radius, out _, standHeight - 0.1f, ObstacleLayer);

            float desireHeight = isSwiming ? swimHeight : (isCrouching ? crouchHeight : standHeight);
            controller.height = Mathf.Lerp(controller.height, desireHeight, lerpSpeed * Time.deltaTime);
            controller.center = new Vector3(0, controller.height / 2f, 0);
            #endregion

            #region TriggerCheck
            Vector3 p1 = transform.position + Vector3.up * radius;
            Vector3 p2 = transform.position + Vector3.up * (controller.height - radius);
            Collider[] cols = Physics.OverlapCapsule(p1, p2, radius);

            bool detectedLadder = false;
            Collider ladderObj = null;
            bool detectedWater = false;
            Collider waterObj = null;

            for (int i = 0; i < cols.Length; i++)
            {
                if (cols[i].CompareTag("Ladder")) { detectedLadder = true; ladderObj = cols[i]; }
                if (cols[i].CompareTag("Water")) { detectedWater = true; waterObj = cols[i]; }
            }

            float camHeight = playerCam.transform.position.y;

            if (detectedWater && waterObj != null)
            {
                currentWaterLevel = waterObj.transform.position.y;
                isSwiming = (transform.position.y + waterOffset) <= currentWaterLevel;
            }
            else
            {
                isSwiming = false;
                currentWaterLevel = float.NegativeInfinity;
            }

            bool headSubmerged = camHeight < currentWaterLevel;

            if (detectedLadder && ladderObj != null && !headSubmerged)
            {
                if (!isClimbing)
                {
                    BoxCollider col = ladderObj.GetComponent<BoxCollider>();
                    climbDir = (ladderObj.transform.position + new Vector3(0, col.size.y / 2, 0)) -
                         (ladderObj.transform.position - new Vector3(0, col.size.y / 2, 0));
                    isClimbing = true;
                }
            }
            else if (isClimbing)
            {
                isClimbing = false;
                if (inputs.vertical > 0.1f && !isSwiming && !detectedLadder)
                {
                    moveDir = transform.forward * 5.0f;
                }
            }
            #endregion

            #region CheckGround            
            if (isClimbing)
            {
                isGrounded = false;
                currentSurface = "Ladder";               
            }
            else
            {
                if (isSwiming)
                {
                    isGrounded = false;
                    currentSurface = "Deep Water";                    
                }
                else
                {
                    if (detectedWater)
                    {
                        currentSurface = "Water";
                    }
                    else
                    {
                        if (Physics.Raycast(transform.position + Vector3.up * 0.1f, Vector3.down, out RaycastHit hit, 0.5f, ObstacleLayer))
                        {
                            currentSurface = hit.collider.tag;
                        }
                    }

                    if (controller.isGrounded && !isGrounded)
                    {
                        fallDistance = highestPoint - currentPosition.y;

                        if (fallDistance > fallDamThreshold)
                        {
                            Debug.Log("Take" + (fallDistance >= fallDamThreshold * 1.5f ? fallDistance * fallDamMul : fallDistance));
                            effect.PlayLandingSound();
                        }
                        else if (fallDistance < fallDamThreshold && fallDistance > controller.stepOffset)
                        {
                            Debug.Log("Land");
                            effect.PlayLandingSound();
                        }
                        moveDir.y = 0;
                        lastPosition = transform.position;
                        isGrounded = true;
                    }

                    if (!controller.isGrounded && isGrounded)
                    {
                        highestPoint = transform.position.y;
                        isGrounded = false;
                    }                                     
                }
            }

            if (Physics.Raycast(transform.position, -Vector3.up, out RaycastHit slopeHit, 0.5f))
            {
                float hitangle = Vector3.Angle(slopeHit.normal, Vector3.up);
                if (hitangle > slideLimit)
                {
                    isSliding = true;
                }
                else
                {
                    isSliding = false;
                }

                surfaceNormal = slopeHit.normal;
            }            
            #endregion
        }       

        private void OnControllerColliderHit(ControllerColliderHit hit)
        {            
            Rigidbody body = hit.collider.attachedRigidbody;
            if (body != null && !body.isKinematic && body.mass < 10)
            {
                Vector3 pushDir = new Vector3(hit.moveDirection.x, 0, hit.moveDirection.z);
                body.linearVelocity += pushDir * 5;
            }
        }           
    }
}