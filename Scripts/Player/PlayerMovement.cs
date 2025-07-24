using TreeEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float crouchSpeed = 2;
    [SerializeField] private float walkSpeed = 4;
    [SerializeField] private float runSpeed = 8;
    [SerializeField] private float jumpSpeed = 6;
    [SerializeField] private float swimSpeed = 3;
    [SerializeField] private float climpSpeed = 3;

    [Header("Stamina")]
    [SerializeField] private float staminaRunCost = 2;
    [SerializeField] private float staminaJumpCost = 20;
    [SerializeField] private float staminaSwimCost = 10;

    [Header("Gravity")]
    [SerializeField] private float gravity = 10;
    [SerializeField] private float gravityMul = 2;
    [SerializeField] private float fallDamMul = 2;
    [SerializeField] private float fallDamThreshold = 8;

    [Header("PlayerAdjust")]
    [SerializeField] private float lerpSpeed = 4;
    [SerializeField] private float normalHeight = 2, crouchHeight = 1.5f, swimHeight = 1;
    [SerializeField] private float normalCamOffset = 1.8f, crouchCamOffset = 1.3f, swimCamOffset = 0.3f;
    [SerializeField] private float waterOffset = 1f;
  
    [Header("States")]
    public bool isGrounded;
    public bool isRunning;
    public bool isCrouching;
    public bool isSwiming;    
    public bool isClimbing;
    public bool isSliding;
    public bool isInWater;

    private float speed;
    private float swimAccel;
    private float highestPoint;
    private float fallDistance;
    [HideInInspector] public float velMagnitude;
    private float currentWaterLevel;
    public string currentSurface;

    private Vector3 moveDir;
    private Vector3 climbDirection;
    private Vector3 currentPosition;
    private Vector3 lastPosition;

    private CharacterController controller;
    private PlayerEffect effects;
    private PlayerCamera playerCamera;
    private PlayerInputs input;
    private PlayerStats stats;
    private CollisionFlags collisionFlags;

    public Vehicle currentVehicle;
    public Seat currentSeat;
    public GameObject currentWater;
    public GameObject currentLadder;

    private void Start()
    {
        controller = GetComponent<CharacterController>();
        input = GetComponent<PlayerInputs>();
        effects = GetComponent<PlayerEffect>();
        stats = GetComponent<PlayerStats>();
        playerCamera = GetComponentInChildren<PlayerCamera>();
    }

    private void Update()
    {
        GetInput();
        GroundCheck();
        PlayerStateHandler();
    }

    private void FixedUpdate()
    {
        if (isClimbing)
        {
            ClimbMovement();
        }
        else
        {
            if (isSwiming)
            {
                SwimMovement();
            }
            else
            {
                if (isGrounded)
                {
                    GroundedMovement();
                }
                else
                {
                    AerialMovement();
                }
            }
        }
        effects.PlayFootsteps(speed, currentSurface);
        collisionFlags = controller.Move(moveDir * Time.fixedDeltaTime);
    }


    void GroundedMovement()
    {
        Vector3 desiredMove = transform.forward * input.vertical + transform.right * input.horizontal;
        RaycastHit hitInfo;
        Physics.SphereCast(transform.position, controller.radius, Vector3.down, out hitInfo,
                               controller.height / 2f, Physics.AllLayers, QueryTriggerInteraction.Ignore);
        desiredMove = Vector3.ProjectOnPlane(desiredMove, hitInfo.normal).normalized;

        moveDir.x = desiredMove.x * speed;
        moveDir.y = -gravity;
        moveDir.z = desiredMove.z * speed;

        if (input.jump.pressed && stats.stamina > 0) 
        {
            input.jump.pressed = false;
            moveDir.y = jumpSpeed;
            isGrounded = false;
            effects.PlayJumpAudio();
            stats.DrainStamina(staminaJumpCost);
        }
        
        if(stats.stamina <= 0)
        {
            input.jump.pressed = false;
            input.run.hold = false;
        }

        if (input.crouch.hold)
        {
            isCrouching = true;
        }
        else
        {
            if (CheckDistance() >= normalHeight)
            {
                isCrouching = false;
            }
        }

        if (input.run.hold && stats.stamina > 0)
        {
            isRunning = true;
            stats.DrainStamina(Time.fixedDeltaTime * staminaRunCost);
        }
        else
        {
            isRunning = false;
        }
    }

    void SwimMovement()
    {
        float surfaceDistance = (currentWaterLevel - 0.5f) - transform.position.y;

        if (swimAccel > 0.0f)
            swimAccel -= Time.fixedDeltaTime * 4.0f;

        if (input.run.hold && stats.stamina >= staminaSwimCost)
        {
            Vector3 swimDir = playerCamera.cameraRoot.transform.TransformDirection(Vector3.forward);

            if (swimAccel <= 1.0f)
            {
                stats.DrainStamina(staminaSwimCost);
                effects.PlayLandAudio("Water");
                StartCoroutine(playerCamera.FallCamera(new Vector3(7, Random.Range(-5.0f, 5.0f), 0), 0.15f));
                swimAccel = jumpSpeed;
            }

            if (swimAccel > 1.0f)
                moveDir = swimDir * swimAccel;
        }
        else
        {
            Vector3 desiredMove = transform.forward * input.vertical + transform.right * input.horizontal;

            if (stats.stamina > 0)
            {
                moveDir.y = surfaceDistance;                
            }
            else
            {
                moveDir.y = -gravity * Time.fixedDeltaTime;
            }

            moveDir.x = desiredMove.x * speed;
            moveDir.z = desiredMove.z * speed;
        }

        float camHeight = playerCamera.cameraRoot.transform.position.y;

        if(camHeight < currentWaterLevel)
        {
            stats.HoldBreath(true);
        }
        else
        {
            stats.HoldBreath(false);
        }

        highestPoint = transform.position.y;
        fallDistance = 0;
        input.jump.pressed = false;
    }
   
    void ClimbMovement()
    {    
        Vector3 verticalMove;
        verticalMove = climbDirection.normalized;
        verticalMove *= input.vertical * speed;
        verticalMove *= (playerCamera.cameraRoot.transform.forward.y > -0.4f) ? 1 : -1;   

        Vector3 desireMove = new Vector3(input.horizontal, 0, input.vertical);
        desireMove = transform.TransformDirection(desireMove);
        moveDir = verticalMove + desireMove;

        highestPoint = transform.position.y;
        fallDistance = 0.0f;
        input.jump.pressed = false;
        input.run.pressed = false;
    }

    void AerialMovement()
    {
        moveDir += gravityMul * Time.fixedDeltaTime * Physics.gravity;

        currentPosition = transform.position;
        if (currentPosition.y > lastPosition.y)
        {            
            highestPoint = transform.position.y;
            lastPosition.y = highestPoint;
        }
    }

    void GroundCheck()
    {
        if (isClimbing) 
        {
            isGrounded = false; 
        }
        else
        {
            if (isSwiming)
            {
                isGrounded = false;
            }
            else
            {
                if (controller.isGrounded && !isGrounded)
                {
                    fallDistance = highestPoint - currentPosition.y;

                    if(fallDistance > fallDamThreshold)
                    {
                        Debug.Log("Take" + (fallDistance >= fallDamThreshold * 1.5f ? fallDistance * fallDamMul : fallDistance));
                        stats.ApplyDamage(fallDistance >= fallDamThreshold * 1.5f ? fallDistance * fallDamMul : fallDistance, true);
                        effects.PlayLandAudio(currentSurface);
                        StartCoroutine(playerCamera.FallCamera(new Vector3(7, Random.Range(-1.0f, 1.0f), 0), 0.15f));
                    }
                    else if(fallDistance < fallDamThreshold && fallDistance > controller.stepOffset)
                    {                       
                        effects.PlayLandAudio(currentSurface);
                        StartCoroutine(playerCamera.FallCamera(new Vector3(7, Random.Range(-1.0f, 1.0f), 0), 0.15f));
                    }
                    moveDir.y = 0;
                    lastPosition = transform.position;
                    isGrounded = true;
                }     
                
                if(!controller.isGrounded && isGrounded)
                {
                    highestPoint = transform.position.y;
                    isGrounded = false;                               
                }
            }
        }
    }

    void PlayerStateHandler()
    {
        float desiredHeight = isSwiming ? swimHeight : (isCrouching ? crouchHeight : normalHeight);
        float center = desiredHeight / 2;
        float desiredCamOffset = isSwiming ? swimCamOffset : (isCrouching ? crouchCamOffset : normalCamOffset);

        controller.height = Mathf.Lerp(controller.height, desiredHeight, Time.deltaTime * lerpSpeed);
        controller.center = Vector3.Lerp(controller.center, new Vector3(0, center, 0), Time.deltaTime * lerpSpeed);
        playerCamera.cameraRoot.transform.localPosition = Vector3.Lerp(playerCamera.cameraRoot.transform.localPosition, new Vector3(0, desiredCamOffset, 0), Time.deltaTime * lerpSpeed);
    }

    void GetInput()
    {
        speed = isClimbing ? climpSpeed : (isSwiming ? swimSpeed : (isCrouching ? crouchSpeed : (isRunning ? runSpeed : walkSpeed))) * (isInWater ? 0.75f : 1);
        velMagnitude = controller.velocity.magnitude;
    }

    public float CheckDistance()
    {
        float distanceToObstacle;
        Vector3 pos = transform.position + controller.center - new Vector3(0, controller.height / 2, 0);
        RaycastHit hit;
        if (Physics.SphereCast(pos, controller.radius, transform.up, out hit, 10))
        {
            distanceToObstacle = hit.distance;
            Debug.DrawLine(pos, hit.point, Color.red, 2.0f);
        }
        else
        {
            distanceToObstacle = 3;
        }
        return distanceToObstacle;
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        Rigidbody body = hit.collider.attachedRigidbody;
        if (body != null && !body.isKinematic && body.mass < 10)
        {
            Vector3 pushDir = new Vector3(hit.moveDirection.x, 0, hit.moveDirection.z);
            body.velocity += pushDir * 5;
        }

        if (isInWater)
        {
            currentSurface = "Water";
        }
        else
        {
            if (controller.isGrounded && hit.normal.y > 0.3f)
            {
                currentSurface = hit.collider.tag;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Ladder"))
        {
            BoxCollider col = other.GetComponent<BoxCollider>();
            climbDirection = (other.transform.position + new Vector3(0, col.size.y / 2, 0)) - (other.transform.position - new Vector3(0, col.size.y / 2, 0));
            isClimbing = true;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Water"))
        {
            float playerWaterCheckHeight = transform.position.y + waterOffset;
            currentWaterLevel = other.transform.position.y;
            if(playerWaterCheckHeight <= currentWaterLevel)
            {                
                isSwiming = true;
            }
            isInWater = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Ladder"))
        {
            isClimbing = false;
            Vector3 dir = gameObject.transform.forward;
            if (input.vertical > 0.1f)
            {
                moveDir = dir.normalized * 5.0f;
            }
        }

        if (other.CompareTag("Water"))
        {
            if (isSwiming)
            {
                isSwiming = false;
                stats.HoldBreath(false);
            }
            isInWater = false;
        }
    }
}
