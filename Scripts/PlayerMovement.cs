using UnityEngine;
using System;
using XInputDotNetPure;
using Cinemachine;

public class PlayerMovement : MonoBehaviour
{
    HitBox hitBox;

    //controller
    PlayerIndex playerindex;
    GamePadState state;
    GamePadState prevState;

    //Initializing things for mouvement
    private BoxCollider2D coll;
    private SpriteRenderer sprite;
    private Rigidbody2D square;
    private Animator anim;

    //Box Collidiers
    public bool isTouchingGround; //Detects wether the player is grounded (touching the ground) or not
    private bool onCeilling;
    private bool onLeftWall;
    private bool onRightWall;

    //Controller Inputs (xBox)
    private bool HoldingJumpButton;
    private bool HoldingDashButton;
    private bool HoldingClimbButton;

    private bool PressedJumpButton;
    private bool PressedDashButton;
    private bool PressedClimbButton;

    private Vector2 Direction;
    private float dirX; //Detects in which direction the player is going (in the x-axis between (-1;1))
    private float dirY; //Detects in which direction the player is going (in the y-axis between (-1;1))

    public float DiscretDirectionX; //Detects in which direction the player is going discretly in the x-axis (-1, 0, 1)
    public float DiscretDirectionY; //Detects in which direction the player is going discretly in the y-axis (-1, 0, 1)

    //Climbing
    [Header("Climbing")]
    [SerializeField] public float ClimbingVelocity = 5F; //The speed of a player during its slow down while griping a wall (wall friction)
    [SerializeField] public float WallJumpTime = 0.1F;
    [SerializeField] public float WallJumpForce = 14F;
    [SerializeField] public float ClimbingTime = 5F;
    [SerializeField] public float ClimbingAcceleration = 50F;
    [SerializeField] public float ClimbingDeceleration = 5F;
    public bool onWall;
    public bool isGripingOnWall;
    public bool Climbing;
    private bool WallJump;
    private bool WallJumped;
    private float WallJumpTimeCounter;
    private float WallJumpDirectionX;
    private float TopWallDirectionX; //Which direction the player is sent when on top of a wall
    private float ClimbingTimeCounter;
    private float VerticalWallJumpForce = 8F;

    //Coyote Time
    [Header("Coyote Time")]
    [SerializeField] public float CoyoteTime = 0.15F;
    [SerializeField] public float JumpBufferTime = 0.2F;
    private float CoyoteTimeCounter;
    private float JumpBufferTimeCounter;

    //Dash
    [Header("Dash")]
    [SerializeField] public float DashVelocity = 20F; // The speed of the dash
    [SerializeField] public float DashTime = 0.15F; //Duration of the dash
    [SerializeField] public float DashCooldown = 0.0833F; //(1/12th of a second or 5 frames at 60fps)
    [SerializeField] public float DashNumber = 1F; // The speed of the dash
    private Vector2 DashDirection; //Gets the direction of the dash
    private float DashTimeCounter; //Timer used for the length (in seconds) of the dash
    private float DashCooldownCounter; //Timer used for the Dash Cooldown
    public bool CanDash; //Looks wether the player is allowed to dash
    public bool isDashing; //Looks wether the player is still dashing
    private bool wasDashing; //Looks wether the player was dashing one frame (or update) before
    private bool NotDashingAnymore; //Looks wether the player was dashing and is still not grounded

    //Edge Detection (Vertical)
    private float VerticalEdgeThreshold = 0.3F;
    private bool WallCollisionUp;
    private bool WallCollisionDown;

    //Edge Detection (Horizontal)


    //Friction
    [Header("Friction")]
    [SerializeField] public float FrictionAmount = 4F; //The amount of friction the player is going to experience
    [SerializeField] public float StopFriction = 0.1F; //Stops friction at the designed speed (0.1 is good)


    //Holding Space
    private bool HoldingJump; //Detects wether the player is holding space or not

    //Jumping
    [Header("Jump")]
    [SerializeField] public float JumpForce = 14F; //The force of the jump of the player
    [SerializeField] public float g = 25F; //The force of the gravitational attraction exerced on the player
    [SerializeField] public float MaxFallSpeed = 20F; //The terminal velocity of the player (literally)

    //Variable Jump
    [SerializeField] public float NoSpaceBarGravityMultiplier = 5F; //Multiplies the gravity by a certain factor such that the player falls faster after letting go of space (bar)
    [SerializeField] public float FallingGravityMultiplier = 2F; //Multiplies the gravity by a certain factor when the player falls
    [SerializeField] public float JumpVelocityFallOff = 4F; //Starts downwards gravity multiplier a bit before it comes to it's apex making the jump sharper
    [SerializeField] public float DashingGravity = 5F;
    [SerializeField] public float WallJumpGravity = 2F;
    public float GravityMultiplier; //Intermediate variable used to multiply the force of gravity by a certain factor
    private bool gravity;

    //Left and Right Mouvement
    [Header("Movement")]
    [SerializeField] public float TopSpeed = 9F; //The maximum speed the player can achieve
    [SerializeField] public float Acceleration = 50F; //The acceleration applied when the player moves in either direction (left or right)
    [SerializeField] public float Deceleration = 5F; //The deceleration applied when the player stops moving in either direction (left or right)

    //Player Facing
    private float PlayerFacingX; //Direction the player is looking in the x-direction
    private float PlayerFacingY; //Direction the player is looking in the y-direction
    private bool facingRight;

    //Pressed Space
    private bool PressedJump; //Detects wether the player has pressed jump

    //Unity's New Input System
    PlayerControls playerControls;

    //Camera Shake
    [Header("Reference")]
    [SerializeField] CinemachineImpulseSource _source;
    public Ghost ghost;
    [SerializeField] public LayerMask jumpableGround;
    //Particle System
    public ParticleSystem jumpDust;
    private bool spawnDust;
    private GameMaster gm;


    // NEW INPUT SYSTEM SETUP
    #region
    private void Awake()
    {
        playerControls = new PlayerControls();
    }

    private void OnEnable()
    {
        playerControls.Enable();
    }


    private void OnDisable()
    {
        playerControls.Disable();
    }
    #endregion

   
    void Start()
    {
        square = GetComponent<Rigidbody2D>(); //Assigning a variable that we can call to the rigidbody we created

        coll = GetComponent<BoxCollider2D>();

        anim = GetComponent<Animator>();    

        square.velocity = new Vector2(0, 0); //Assigning a initial velocity to the square (rigidbody)
        
        gm = GameObject.FindGameObjectWithTag("GM").GetComponent<GameMaster>();

        transform.position = gm.lastCheckPointPos;

        gravity = true;
    }


    //All user inputs go into Update()
    public void Update()
    {

        isGrounded();

        //User Inputs
        #region
        PressedJumpButton = playerControls.Player.PressedJump.ReadValue<float>() > 0;
        PressedDashButton = playerControls.Player.PressedDash.ReadValue<float>() > 0;
        PressedClimbButton = playerControls.Player.PressedClimb.ReadValue<float>() > 0;

        HoldingJumpButton = playerControls.Player.Jump.ReadValue<float>() > 0;
        HoldingDashButton = playerControls.Player.Dash.ReadValue<float>() > 0;
        HoldingClimbButton = playerControls.Player.Climb.ReadValue<float>() > 0;
        #endregion
        //Joystick direction
        #region
        Direction = playerControls.Player.Joystick.ReadValue<Vector2>();
        dirX = Direction.x;
        dirY = Direction.y;

        if (Math.Abs(dirX) < 0.5)
        {
            DiscretDirectionX = 0;
        }
        else
        {
            DiscretDirectionX = Math.Sign(dirX);
        }


        if (Math.Abs(dirY) < 0.5)
        {
            DiscretDirectionY = 0;
        }
        else
        {
            DiscretDirectionY = Math.Sign(dirY);
        }

        PlayerFacingX = Math.Sign(square.velocity.x);
        #endregion

        //COLLISION CHECK
        #region
        onCeilling = Physics2D.BoxCast(coll.bounds.center, coll.bounds.size, 0f, Vector2.up, .1f, jumpableGround);

        onLeftWall = Physics2D.BoxCast(coll.bounds.center, coll.bounds.size, 0f, Vector2.left, .1f, jumpableGround);

        onRightWall = Physics2D.BoxCast(coll.bounds.center, coll.bounds.size, 0f, Vector2.right, .1f, jumpableGround);

        onWall = (onLeftWall || onRightWall);

        isGripingOnWall = (((onLeftWall && DiscretDirectionX == -1) || (onRightWall && DiscretDirectionX == 1)) && square.velocity.y < 0);
        #endregion

        //FLIPPING CHARACTER
        #region

        if (!onWall)
        {
            if (dirX < 0 && !facingRight)
            {
                Flip();
            }
            if (dirX > 0 && facingRight)
            {
                Flip();
            }
        }

        #endregion

        //Variables Update
        #region
        if (onLeftWall)
        {
            WallJumpDirectionX = 1;
            TopWallDirectionX = -1;
        }

        if (onRightWall)
        {
            WallJumpDirectionX = -1;
            TopWallDirectionX = 1;
        }
        #endregion

        //Coyote Time
        #region
        if (isTouchingGround == true)
        {
            CoyoteTimeCounter = CoyoteTime;
        }
        else if (isTouchingGround == false)
        {
            CoyoteTimeCounter -= Time.deltaTime;
        }
        #endregion

        //Jump Buffer Time
        #region
        if (PressedJumpButton == true)
        {
            JumpBufferTimeCounter = JumpBufferTime;
        }
        else if (PressedJumpButton == false)
        {
            JumpBufferTimeCounter -= Time.deltaTime;
        }
        #endregion

        //Dash Time
        #region
        if (isDashing == false)
        {
            DashTimeCounter = DashTime;
        }
        else
        {
            DashTimeCounter -= Time.deltaTime;
        }

        if(!isDashing)
        dashCooldown();

        if (isDashing)
        {
            DashCooldownCounter = DashCooldown;
        }
        #endregion

        //Wall Jump Duration
        #region
        if (WallJump == true)
        {
            WallJumpTimeCounter -= Time.deltaTime;
        }
        else
        {
            WallJumpTimeCounter = -1;
        }
        #endregion

        //Climbing Duration
        #region
        if (Climbing || isGripingOnWall)
        {
            ClimbingTimeCounter -= Time.deltaTime;
        }

        if (isTouchingGround == true)
        {
            ClimbingTimeCounter = ClimbingTime;
        }
        #endregion

        //Setting up animations booleans
        #region
        anim.SetBool("isRunning", DiscretDirectionX != 0);
        anim.SetBool("grounded", isTouchingGround);
        anim.SetBool("isFalling", square.velocity.y < 0 && !isTouchingGround);
        anim.SetBool("isRising", square.velocity.y > 0 && !isTouchingGround);
        anim.SetBool("smallJump", GravityMultiplier != 1 && CanDash && !HoldingJumpButton && !isTouchingGround && square.velocity.y != -16.136f);
        #endregion

        //PARTICLES
        #region
        if (isTouchingGround)
        {
            if (spawnDust)
            {
                Vector2 previousPosition = new(square.position.x, square.position.y - 0.64f);
                jumpDust.Play();
                spawnDust = false;
                jumpDust.transform.parent = null;
                jumpDust.transform.position = previousPosition;
            }
        }
        else
        {
            spawnDust = true;
        }
        #endregion


    }

   
    //All physics goes into FixedUpdate()
    public void FixedUpdate()
    {

        //ENABLING GRAVITY
        if(gravity)
        square.AddForce(new Vector2(0, -g * GravityMultiplier));


        //Jumping

        if (isTouchingGround && CoyoteTimeCounter > 0 && JumpBufferTimeCounter > 0)
        {
            Jump(0, 1, JumpForce);

        }

        //Variational Jump

        if (HoldingJumpButton && square.velocity.y > JumpVelocityFallOff)
        {
            GravityMultiplier = 1;
        }
        else if (HoldingJumpButton == false && square.velocity.y > 0)
        {
            GravityMultiplier = NoSpaceBarGravityMultiplier;
        }
        else
        {
            GravityMultiplier = FallingGravityMultiplier;
        }

        if (WallJumped == true && square.velocity.y > 0)
        {
            GravityMultiplier = WallJumpGravity;
        }

        if (NotDashingAnymore == true && square.velocity.y > 0)
        {
            GravityMultiplier = DashingGravity;
        }


        //Left and Right Movement

        if (DiscretDirectionX != 0 && Math.Abs(square.velocity.x) < TopSpeed && (isGripingOnWall == false && Climbing == false) && WallJumpTimeCounter < 0)
        {
            square.AddForce(new Vector2(DiscretDirectionX * Acceleration, 0));
        }
        else
        {
            square.AddForce(new Vector2(-Deceleration, 0) * square.velocity.x);
        }


        //Friction

        if (DiscretDirectionX == 0 && Math.Abs(square.velocity.x) > StopFriction)
        {
            square.AddForce(new Vector2(-PlayerFacingX * FrictionAmount, 0));
        }

        if (DiscretDirectionX == 0 && Math.Abs(square.velocity.x) <= StopFriction)
        {
            square.velocity = new Vector2(0, square.velocity.y);
        }


        //Clamped Fall Speed

        if (square.velocity.y < -MaxFallSpeed)
        {
            square.velocity = new Vector2(square.velocity.x, -MaxFallSpeed);
        }


        //Dash

        if ((DiscretDirectionX != 0 || DiscretDirectionY != 0) && CanDash == true && HoldingDashButton == true && !isDashing && DashNumber > 0)
        {
         
            DashDirection = new Vector2(DiscretDirectionX, DiscretDirectionY).normalized;

            isDashing = true;

            Shake();

            GamePad.SetVibration(playerindex, .3f, .3f);

            ghost.makeGhost = true;

            CanDash = false ;

            DashNumber--;

        }

        wasDashing = isDashing;

        if (isDashing == true && DashTimeCounter > 0)
        {
            square.velocity = new Vector2(DashDirection.x * DashVelocity, DashDirection.y * DashVelocity);

        }
        else
        {
            isDashing = false;
           
            ghost.makeGhost = false;

            GamePad.SetVibration(playerindex, 0, 0);
        }

        if (wasDashing != isDashing)
        {
            NotDashingAnymore = true; 

        }
        else if (square.velocity.y <= 0)
        {
            NotDashingAnymore = false;
        }

        if (isTouchingGround == true && DashCooldownCounter < 0 && isDashing == false && HoldingDashButton == false)
        {
            CanDash = true;

            DashTimeCounter = DashTime;

            DashNumber = 1;
        }



        //CLIMBING

        if (isGripingOnWall && ClimbingTimeCounter > 0)
        {
            gravity = false;

            square.AddForce(new Vector2(0, 1),ForceMode2D.Impulse);

        }

        if (onWall && HoldingClimbButton)
        {
            Climbing = true;
        }

        else
        {
            Climbing = false;

            gravity = true;

        }

        if (Climbing == true && ClimbingTimeCounter > 0)
        {

            gravity = false;

            if (Math.Abs(square.velocity.y) < ClimbingVelocity && DiscretDirectionY != 0)
            {

                square.AddForce(new Vector2(0, DiscretDirectionY * ClimbingAcceleration));
            }
            else
            {
                square.AddForce(new Vector2(0, -ClimbingDeceleration) * square.velocity.y);
            }

        }

        if (Climbing == true && ClimbingTimeCounter < 0)
        {

            gravity = true;

            GamePad.SetVibration(playerindex, .3f, .3f);
        }

        if (PressedJumpButton && ClimbingTimeCounter > 0)
        {
            if ((isGripingOnWall) || (Climbing && DiscretDirectionX == WallJumpDirectionX && DiscretDirectionY == 0) || Climbing && DiscretDirectionY == 1)
            {
                square.velocity = new Vector2(WallJumpDirectionX, 1) * WallJumpForce;

                WallJumpTimeCounter = WallJumpTime;

                WallJumped = true;

                WallJump = true;

                gravity = true;

                ClimbingTimeCounter -= 1f;
            }
        }


        if (WallJumpTimeCounter <= 0)
        {
            WallJump = false;
        }

        if (isTouchingGround == true && square.velocity.y >= 0)
        {
            WallJumped = false;
        }

    }

    //METHODS
    private void dashCooldown()
    {
        //Dash Cooldown

        DashCooldownCounter -= Time.deltaTime;

    }

    public void Jump(float xDir, float yDir, float force)
    {
        square.velocity = new Vector2(square.velocity.x, 0);

        square.AddForce(new Vector2(xDir * force, yDir * force), ForceMode2D.Impulse);

        JumpBufferTimeCounter = 0;
    }

    public void Shake() // CAMERA SHAKE
    {
        _source.GenerateImpulse();
    }

    private void Flip() // FLIPS PLAYER'S SPRITE
    {
        Vector3 currentScale = gameObject.transform.localScale;

        currentScale.x *= -1;

        gameObject.transform.localScale = currentScale;

        facingRight = !facingRight;
    }

    public void isGrounded() // GROUND CHECK
    {
        RaycastHit2D Ground = Physics2D.BoxCast(coll.bounds.center, coll.bounds.size, 0f, Vector2.down, .2f, jumpableGround);

        if (Ground)
        {
            isTouchingGround = true;
        }
        else
        {
            isTouchingGround = false;
        }

    }

}