using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.InputSystem;
using System;

public class PlayerMouvement : MonoBehaviour
{

    //Initializing things for mouvement
    public static BoxCollider2D coll;
    public static Rigidbody2D square;
    [SerializeField] public LayerMask jumpableGround;


    //Box Collidiers
    bool isGrounded; //Detects wether the player is grounded (touching the ground) or not
    bool onCeilling;
    bool onLeftWall;
    bool onRightWall;

    //Clamped Fall Speed
    float MaxFallSpeed = 20F; //The terminal velocity of the player (literally)

    //Climbing
    bool onWall;
    bool isGripingOnWall;
    bool Climbing;
    bool WallJump;
    bool WallJumped;
    float ClimbingVelocity = 5F; //The speed of a player during its slow down while griping a wall (wall friction)
    float WallJumpTime = 0.1F;
    float WallJumpTimeCounter;
    float WallJumpDirectionX;
    float TopWallDirectionX; //Which direction the player is sent when on top of a wall
    float WallJumpForce = 14F;
    float ClimbingTime = 12F;
    float ClimbingTimeCounter;
    float VerticalWallJumpForce = 8F;

    //Controller Inputs (xBox)
    bool HoldingJumpButton;
    bool HoldingDashButton;
    bool HoldingClimbButton;

    bool PressedJumpButton;
    bool PressedDashButton;
    bool PressedClimbButton;

    Vector2 Direction;
    float xDirection; //Detects in which direction the player is going (in the x-axis between (-1;1))
    float yDirection; //Detects in which direction the player is going (in the y-axis between (-1;1))

    float DiscretDirectionX; //Detects in which direction the player is going discretly in the x-axis (-1, 0, 1)
    float DiscretDirectionY; //Detects in which direction the player is going discretly in the y-axis (-1, 0, 1)


    //Coyote Time
    float CoyoteTime = 0.15F;
    float CoyoteTimeCounter;
    float JumpBufferTime = 0.2F;
    float JumpBufferTimeCounter;

    //Dash
    Vector2 DashDirection; //Gets the direction of the dash
    float DashVelocity = 20F; // The speed of the dash
    float DashTime = 0.15F; //Duration of the dash
    float DashTimeCounter; //Timer used for the length (in seconds) of the dash
    float DashCooldown = 0.0833F; //(1/12th of a second or 5 frames at 60fps)
    float DashCooldownCounter; //Timer used for the Dash Cooldown
    bool CanDash; //Looks wether the player is allowed to dash
    bool isDashing; //Looks wether the player is still dashing
    bool wasDashing; //Looks wether the player was dashing one frame (or update) before
    bool NotDashingAnymore; //Looks wether the player was dashing and is still not grounded

    //Edge Detection (Vertical)
    float VerticalEdgeThreshold = 0.3F;
    bool WallCollisionUp;
    bool WallCollisionDown;
    Vector2 Amogus;

    //Edge Detection (Horizontal)
    

    //Friction
    float FrictionAmount = 4F; //The amount of friction the player is going to experience
    float StopFriction = 0.1F; //Stops friction at the designed speed (0.1 is good)

    //Gravity
    float g = 25F; //The force of the gravitational attraction exerced on the player

    //Holding Space
    bool HoldingJump; //Detects wether the player is holding space or not

    //Jumping
    float JumpForce = 14F; //The force of the jump of the player

    //Left and Right Mouvement
    float TopSpeed = 9F; //The maximum speed the player can achieve
    float Acceleration = 50F; //The acceleration applied when the player moves in either direction (left or right)
    float Deceleration = 5F; //The deceleration applied when the player stops moving in either direction (left or right)

    //Player Facing
    float PlayerFacingX; //Direction the player is looking in the x-direction
    float PlayerFacingY; //Direction the player is looking in the y-direction

    //Pressed Space
    bool PressedJump; //Detects wether the player has pressed jump

    //Variable Jump
    float NoSpaceBarGravityMultiplier = 5F; //Multiplies the gravity by a certain factor such that the player falls faster after letting go of space (bar)
    float FallingGravityMultiplier = 2F; //Multiplies the gravity by a certain factor when the player falls
    float JumpVelocityFallOff = 4F; //Starts downwards gravity multiplier a bit before it comes to it's apex making the jump sharper
    float DashingGravity = 5F;
    float WallJumpGravity = 2F;
    float GravityMultiplier; //Intermediate variable used to multiply the force of gravity by a certain factor

    /*
    //Temporary to get dash distance
    Vector2 StartPos;
    Vector2 EndPos;
    */


    //Unity's New Input System
    PlayerControls playerControls;

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

    // Start is called before the first frame update
    void Start()
    {
        square = GetComponent<Rigidbody2D>(); //Assigning a variable that we can call to the rigidbody we created
        coll = GetComponent<BoxCollider2D>();
        square.position = new Vector2(0, 0); //Assigning a initial position to the square (rigidbody)
        square.velocity = new Vector2(0, 0); //Assigning a initial velocity to the square (rigidbody)

    }


    //All user inputs go into Update()
    public void Update()
    {

        //User Inputs

        //Controller Inputs

        //A,B,X,Y Buttons

        PressedJumpButton = playerControls.Player.PressedJump.ReadValue<float>() > 0;
        PressedDashButton = playerControls.Player.PressedDash.ReadValue<float>() > 0;
        PressedClimbButton = playerControls.Player.PressedClimb.ReadValue<float>() > 0;


        HoldingJumpButton = playerControls.Player.Jump.ReadValue<float>() > 0;
        HoldingDashButton = playerControls.Player.Dash.ReadValue<float>() > 0;
        HoldingClimbButton = playerControls.Player.Climb.ReadValue<float>() > 0;

        //Joystick direction

        Direction = playerControls.Player.Joystick.ReadValue<Vector2>();
        xDirection = Direction.x;
        yDirection = Direction.y;

        if (Math.Abs(xDirection) < 0.5)
        {
            DiscretDirectionX = 0;
        }
        else
        {
            DiscretDirectionX = Math.Sign(xDirection);
        }


        if (Math.Abs(yDirection) < 0.5)
        {
            DiscretDirectionY = 0;
        }
        else
        {
            DiscretDirectionY = Math.Sign(yDirection);
        }

        //Other Inputs

        PlayerFacingX = Math.Sign(square.velocity.x);
        isGrounded = Physics2D.BoxCast(coll.bounds.center, coll.bounds.size, 0f, Vector2.down, .1f, jumpableGround);
        onCeilling = Physics2D.BoxCast(coll.bounds.center, coll.bounds.size, 0f, Vector2.up, .1f, jumpableGround);
        onLeftWall = Physics2D.BoxCast(coll.bounds.center, coll.bounds.size, 0f, Vector2.left, .1f, jumpableGround);
        onRightWall = Physics2D.BoxCast(coll.bounds.center, coll.bounds.size, 0f, Vector2.right, .1f, jumpableGround);
        onWall = (onLeftWall || onRightWall);
        isGripingOnWall = (((onLeftWall && DiscretDirectionX == -1) || (onRightWall && DiscretDirectionX == 1)) && square.velocity.y < 0);

        //Variables Update

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


        //Coyote Time

        if (isGrounded == true)
        {
            CoyoteTimeCounter = CoyoteTime;
        }
        else if (isGrounded == false)
        {
            CoyoteTimeCounter -= Time.deltaTime;
        }


        //Jump Buffer Time

        if (PressedJumpButton == true)
        {
            JumpBufferTimeCounter = JumpBufferTime;
        }
        else if (PressedJumpButton == false)
        {
            JumpBufferTimeCounter -= Time.deltaTime;
        }


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

        if (isGrounded == true)
        {
            DashCooldownCounter -= Time.deltaTime;
        }
        else
        {
            DashCooldownCounter = DashCooldown;
        }


        //Wall Jump Duration

        if (WallJump == true)
        {
            WallJumpTimeCounter -= Time.deltaTime;
        }
        else
        {
            WallJumpTimeCounter = -1;
        }

        //Climbing Duration

        if (Climbing == true)
        {
            ClimbingTimeCounter -= Time.deltaTime;
        }
        
        if (isGrounded == true)
        {
            ClimbingTimeCounter = ClimbingTime;
        }


    }


    //All physics goes into FixedUpdate()
    public void FixedUpdate()
    {




        //Gravity

        square.AddForce(new Vector2(0, -g * GravityMultiplier));


        //Jumping

        if (CoyoteTimeCounter > 0 && JumpBufferTimeCounter > 0)
        {
            square.velocity = new Vector2(square.velocity.x, 0);
            square.AddForce(new Vector2(0, JumpForce), ForceMode2D.Impulse);
            JumpBufferTimeCounter = 0;

        }


        //Variational Jump

        if (HoldingJumpButton && square.velocity.y > JumpVelocityFallOff)
        {
            GravityMultiplier = 1;
        }
        else if (HoldingJumpButton == false && square.velocity.y >= 0)
        {
            GravityMultiplier = NoSpaceBarGravityMultiplier;
        }
        else
        {
            GravityMultiplier = FallingGravityMultiplier;
        }

        if (WallJumped == true && square.velocity.y >= 0)
        {
            GravityMultiplier = WallJumpGravity;
        }

        if (NotDashingAnymore == true && square.velocity.y > 0)
        {
            GravityMultiplier = DashingGravity;
        }


        //Left and Right Mouvement

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

        if ((DiscretDirectionX != 0 || DiscretDirectionY != 0) && CanDash == true && HoldingDashButton == true)
        {
            //StartPos = new Vector2(square.position.x, square.position.y);

            DashDirection = new Vector2(DiscretDirectionX, DiscretDirectionY).normalized;
            isDashing = true;
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
        }

        if (wasDashing != isDashing)
        {
            NotDashingAnymore = true;
            //EndPos = new Vector2(square.position.x, square.position.y);
        }
        else if (square.velocity.y <= 0)
        {
            NotDashingAnymore = false;
        }

        if (isGrounded == true && DashCooldownCounter < 0 && isDashing == false && HoldingDashButton == false)
        {
            CanDash = true;
            DashTimeCounter = DashTime;
        }

        //Debug.Log((EndPos - StartPos).magnitude)

        //Climbing

        if (isGripingOnWall)
        {
            square.velocity = new Vector2(0, -ClimbingVelocity);
            square.AddForce(new Vector2(0, g * GravityMultiplier));
        }

        if (onWall && HoldingClimbButton)
        {
            Climbing = true;
        }
        else
        {
            Climbing = false;
        }


        if (Climbing == true && HoldingClimbButton && onWall && ClimbingTimeCounter > 0)
        {
            square.velocity = new Vector2(0, DiscretDirectionY * ClimbingVelocity);
            square.AddForce(new Vector2(0, g * GravityMultiplier));
        }
        else if (Climbing == true && HoldingClimbButton && onWall && ClimbingTimeCounter < 0)
        {
            square.velocity = new Vector2(0, -ClimbingVelocity);
            square.AddForce(new Vector2(0, g * GravityMultiplier));
        }
        else
        {
            Climbing = false;
        }


        if ((isGripingOnWall && PressedJumpButton) || (Climbing && DiscretDirectionX == WallJumpDirectionX && DiscretDirectionY == 0 && PressedJumpButton))
        {
            square.velocity = new Vector2(WallJumpDirectionX, 1) * WallJumpForce;
            WallJumpTimeCounter = WallJumpTime;
            WallJumped = true;
            WallJump = true;
        }


        if (WallJumpTimeCounter <= 0)
        {
            WallJump = false;
        }

        if (isGrounded == true && square.velocity.y >= 0)
        {
            WallJumped = false;
        }

    }
}
