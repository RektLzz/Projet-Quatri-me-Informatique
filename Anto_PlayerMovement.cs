using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using XInputDotNetPure;
using Cinemachine;

public class PlayerMovement : MonoBehaviour
{
    #region
    HitBox hitBox;

    //controller
    PlayerIndex playerindex;
    GamePadState state;
    GamePadState prevState;
    //Vertical movement
    [Header("Vertical movement")]
    [Range(20, 40)] private float g = 28.4f; //Gravitational strength of the directed gravity field
    private float GravityMultiplier = 4; //Multiplies downward force when player stops pressing spacebar
    [Range(13, 20)] private float JumpForce = 14.5f;
    public bool DirectionGravity = true; //Do we apply Directional gravity ? false = no, true = yes

    //Initializing Horizontal Mouvement Variables
    [Header("Horizontal movement")]
    [Range(10, 20)] private float Acceleration = 50f;
    [Range(15, 25)] private float Deceleration = 22f;
    float TargetSpeed;
    [Range(6, 20)] private float TopSpeed = 9.4f;
    float SpeedDif;
    float AccelRate;
    float Mouvement;
    float dirX;
    float dirY;
    bool dashInput;

    //Fall gravity
    const float FallGravityMultiplier = 2F;

    //Clamp (cap) fall speed
    const float MaxFallSpeed = 15f;

    //Friction
    float amount;
    float FrictionAmount = 10f;
    float StopFriction = 0.1f;

    //Coyote Time
    float CoyoteTime = 0.15F; //How much time the player has to have pressed space bar since last grounded to jump
    float CoyoteTimeCounter;
    float JumpBufferTime = 0.2F;
    float JumpBufferTimeCounter;

    //Edge Detection
    float PreviousVelocity;
    float NudgedPosition;
    float Cooldown = 0.3F;
    float CooldownCounter;
    float BoostCoefficient = 6F;
    [Range(-0.5f, 1f)] public float edgeGap;

    //Particle System
    public ParticleSystem jumpDust;
    private bool spawnDust;

    //Player Facing
    float PlayerFacingX;
    float PlayerFacingY;

    //Variable Jump
    float NoSpaceBarGravityMultiplier = 5F; //Multiplies the gravity by a certain factor such that the player falls faster after letting go of space (bar)
    float FallingGravityMultiplier = 2F; //Multiplies the gravity by a certain factor when the player falls

    //Calling objects
    BoxCollider2D coll;
    SpriteRenderer sprite;
    public Rigidbody2D square;
    [SerializeField] public LayerMask jumpableGround;
    private Animator anim;
    public BoxCollider2D ceillingBounds;
    private bool ceillingCollision;
    public bool isTouchingGround;
    private bool facingRight;

    [Header("Dashing")]
    //Dash
    Vector2 DashDirection; //Gets the direction of the dash
    float DashVelocity = 25F; // The speed of the dash
    float DashTime = 0.3F; //Duration of the dash
    float DashTimeCounter; //Timer used for the length (in seconds) of the dash
    float DashCooldown = 0; //(1/12th of a second or 5 frames at 60fps)
    float DashCooldownCounter; //Timer used for the Dash Cooldown
    public bool CanDash; //Looks wether the player is allowed to dash
    public bool isDashing; //Looks wether the player is still dashing
    bool wasDashing; //Looks wether the player was dashing one frame (or update) before
    bool NotDashingAnymore; //Looks wether the player was dashing and is still not grounded
    #endregion

    //Controller Inputs (xBox)
    bool HoldingAButton;
    bool HoldingBButton;
    bool HoldingXButton;
    bool HoldingYButton;

    bool PressedAButton;
    bool PressedBButton;
    bool PressedXButton;
    bool PressedYButton;

    float xDirection; //Detects in which direction the player is going (in the x-axis)
    float yDirection;

    float DiscretDirectionX;
    public float DiscretDirectionY;

    Vector2 leftStick;

    bool PressedSpace;
    bool HoldingSpace;

    public Ghost ghost;

    [SerializeField] CinemachineImpulseSource _source;

    public void Shake()
    {
        _source.GenerateImpulse();
    }

    // Start is called before the first frame update
    public void Start()

    {
        #region
     
        square = GetComponent<Rigidbody2D>();
        sprite = GetComponent<SpriteRenderer>();
        coll = GetComponent<BoxCollider2D>();
        anim = GetComponent<Animator>();
        ceillingBounds = GetComponent<BoxCollider2D>();

        #endregion
    }

   

    // Update is called once per frame
    public void Update()

    {
        //User Inputs

        //Controller Inputs

        //A,B,X,Y Buttons

        PressedAButton = Input.GetButtonDown("Jump");
        //PressedBButton = Input.GetButtonDown("B");
        PressedXButton = Input.GetButtonDown("Dash");
        //PressedYButton = Input.GetButtonDown("Y");

        HoldingAButton = Input.GetButton("Jump");
        //HoldingBButton = Input.GetButton("B");
        HoldingXButton = Input.GetButton("Dash");
        //HoldingYButton = Input.GetButton("Y");

        //Joystick direction

        xDirection = Input.GetAxisRaw("Horizontal");
        yDirection = Input.GetAxisRaw("Vertical");

        DiscretDirectionX = Math.Sign(xDirection);
        DiscretDirectionY = Math.Sign(yDirection);

        dirX = Input.GetAxisRaw("Horizontal");
        dirY = Input.GetAxisRaw("Vertical");
        dashInput = Input.GetButtonDown("Dash");

        HoldingSpace = Input.GetButton("Jump");
        PressedSpace = Input.GetButtonDown("Jump");
        PlayerFacingX = Math.Sign(square.velocity.x);


        //FLIPPING CHARACTER
        #region
        if (dirX < 0 && !facingRight)
        {
            Flip();
        }
        if (dirX > 0 && facingRight)
        {
            Flip();
        }
        #endregion

        #region 
        /*if (dashInput && _canDash)
            {
                _isDashing = true;
                _canDash = false;
                _dashingDir = new Vector2(dirX, dirY);
                if(_dashingDir == Vector2.zero)
                {
                    _isDashing = false;
                }
                StartCoroutine(StopDashing());
            }
            if(_isDashing)
            {
                CameraEffects.ShakeOnce();
                GamePad.SetVibration(playerindex, .3f, .3f);
                ghost.makeGhost = true;
                square.velocity = _dashingDir.normalized * _dashingVelocity;
                return;
            }
            if (isTouchingGround)
                _canDash = true;
            */
        #endregion

        //COYOTE TIME
        #region
        if (isTouchingGround)
        {
            CoyoteTimeCounter = CoyoteTime;
        }
        else if (!isTouchingGround)
        {
            CoyoteTimeCounter -= Time.deltaTime;
        }
        #endregion

        //JUMP BUFFER TIME
        #region
        if (PressedSpace == true)
        {
            JumpBufferTimeCounter = JumpBufferTime;
        }
        else if (PressedSpace == false)
        {
            JumpBufferTimeCounter -= Time.deltaTime;
        }
        #endregion

        //Edge Detection

        CooldownCounter -= Time.deltaTime;


        //Calling Methods
        upCollision();
        IsGrounded();

        //Setting up animations
        anim.SetBool("isRunning", dirX != 0 );
        anim.SetBool("grounded", isTouchingGround && !HoldingSpace);
        anim.SetBool("isFalling", square.velocity.y < 0 && !isTouchingGround);
        anim.SetBool("isRising", square.velocity.y > 0 && !isTouchingGround);
        anim.SetBool("smallJump", GravityMultiplier != 1 && CanDash && !HoldingSpace && !isTouchingGround && square.velocity.y != -16.136f);
      

        //Impact particles when landing
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

        //Dash Time

        if (isDashing == false)
        {
            DashTimeCounter = DashTime;
        }
        else
        {
            DashTimeCounter -= Time.deltaTime;
        }


        //Dash Cooldown

        if (isTouchingGround == true)
        {
            DashCooldownCounter -= Time.deltaTime;
        }
        else
        {
            DashCooldownCounter = DashCooldown;
        }


        //Vertical Edge Detection (When dashing horizontally for example)


    }

    //shifts the player on the side when hitting the corner of a platform
    void upCollision()
    {
        RaycastHit2D ceillingBounds = Physics2D.BoxCast(coll.bounds.center, coll.bounds.size, 0f, Vector2.up, .1f, jumpableGround);

        float xCoordinate = ceillingBounds.point.x;
        float xPlayerCoordinate = square.position.x;
        if (ceillingBounds)
            if (xPlayerCoordinate - xCoordinate > 0.1f && dirX == 0)
            {
                square.position = new Vector2(square.position.x + 0.2f, square.position.y + 0.3f);

            }
    }


    //Flips the sprite of the character
    public void Flip()
    {
        Vector3 currentScale = gameObject.transform.localScale;
        currentScale.x *= -1;
        gameObject.transform.localScale = currentScale;

        facingRight = !facingRight;
    }

    //Check if the player is on the ground
    public void IsGrounded()
    {
        RaycastHit2D Ground = Physics2D.BoxCast(coll.bounds.center, coll.bounds.size, 0f, Vector2.down, .1f, jumpableGround);

        if (Ground)
        {
            isTouchingGround = true;
        }
        else
        {
            isTouchingGround = false;
        }

    }

    public void FixedUpdate()
    {

        IsGrounded();


        //Gravity

        square.AddForce(new Vector2(0, -g * GravityMultiplier));


        //Jumping

        if (CoyoteTimeCounter > 0 && JumpBufferTimeCounter > 0)
        {
            square.velocity = new Vector2(square.velocity.x, 0);
            square.AddForce(new Vector2(0, JumpForce), ForceMode2D.Impulse);
            JumpBufferTimeCounter = 0;
        }

        if (square.velocity.y > 0)
        {
            CoyoteTimeCounter = 0;
        }

        //Variational Jump

        if (HoldingSpace && square.velocity.y > 0)
        {
            GravityMultiplier = 1;
        }
        else if (HoldingSpace == false && square.velocity.y > 0)
        {
            GravityMultiplier = NoSpaceBarGravityMultiplier;
        }
        else
        {
            GravityMultiplier = FallingGravityMultiplier;
        }


        //Left and Right Mouvement

        if (xDirection != 0 && Math.Abs(square.velocity.x) < TopSpeed)
        {
            square.AddForce(new Vector2(xDirection * Acceleration, 0));
        }
        else
        {
            square.AddForce(new Vector2(-Deceleration, 0) * square.velocity.x);
        }


        //Friction

        if (xDirection == 0 && Math.Abs(square.velocity.x) > StopFriction)
        {
            square.AddForce(new Vector2(-PlayerFacingX * FrictionAmount, 0));
        }
        if (xDirection == 0 && Math.Abs(square.velocity.x) <= StopFriction)
        {
            square.velocity = new Vector2(0, square.velocity.y);
        }


        //Clamped Fall Speed

        if (square.velocity.y < -MaxFallSpeed)
        {
            square.velocity = new Vector2(square.velocity.x, -MaxFallSpeed);
        }

        //DASH
        #region
        if ((DiscretDirectionX != 0 || DiscretDirectionY != 0) && CanDash == true && HoldingXButton == true)
        {

            DashDirection = new Vector2(DiscretDirectionX, DiscretDirectionY).normalized;
            isDashing = true;
            Shake();
            GamePad.SetVibration(playerindex, .3f, .3f);
            ghost.makeGhost = true;
            CanDash = false;
        }

        wasDashing = isDashing;

        if (isDashing == true && DashTimeCounter > 0)
        {
            square.velocity = new Vector2(DashDirection.x * DashVelocity, DashDirection.y * DashVelocity);
            square.AddForce(new Vector2(0, g * GravityMultiplier));
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

        if (isTouchingGround == true && DashCooldownCounter < 0 && isDashing == false && !HoldingXButton)
        {
            CanDash = true;
            DashTimeCounter = DashTime;
        }
        #endregion
    }

   /* public bool canShoot()
    {
        return !onWall();
    }*/
}
