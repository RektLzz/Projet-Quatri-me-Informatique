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


    //Clamped Fall Speed
    float MaxFallSpeed = 20; //The terminal velocity of the player (literally)

    //Controller Inputs (xBox)
    bool HoldingAButton;
    bool HoldingBButton;
    bool HoldingXButton;
    bool HoldingYButton;

    bool PressedAButton;
    bool PressedBButton;
    bool PressedXButton;
    bool PressedYButton;

    float xDirection; //Detects in which direction the player is going (in the x-axis between (-1;1))
    float yDirection; //Detects in which direction the player is going (in the y-axis between (-1;1))

    float DiscretDirectionX; //Detects in which direction the player is going discretly in the x-axis (-1, 0, 1)
    float DiscretDirectionY; //Detects in which direction the player is going discretly in the y-axis (-1, 0, 1)


    //Coyote Time
    float CoyoteTime = 0.15F;
    float CoyoteTimeCounter;
    float JumpBufferTime = 0.2F;
    float JumpBufferTimeCounter;

    //Friction
    float FrictionAmount = 4F; //The amount of friction the player is going to experience

    //Gravity
    float g = 25F; //The force of the gravitational attraction exerced on the player

    //Grounded
    bool isGrounded; //Detects wether the player is grounded (touching the ground) or not

    //Holding Space
    bool HoldingSpace; //Detects wether the player is holding space or not

    //Jumping
    float JumpForce = 13F; //The force of the jump of the player

    //Left and Right Mouvement
    float TopSpeed = 9F; //The maximum speed the player can achieve
    float Acceleration = 50F; //The acceleration applied when the player moves in either direction (left or right)
    float Deceleration = 5F; //The deceleration applied when the player stops moving in either direction (left or right)

    //Player Facing
    float PlayerFacingX;
    float PlayerFacingY;

    //Pressed Space
    bool PressedSpace;

    //Queuing Jumps
    float JumpQueueTime = 0.5F;
    float JumpQueueTimeCounter;

    //Variable Jump
    float NoSpaceBarGravityMultiplier = 5F; //Multiplies the gravity by a certain factor such that the player falls faster after letting go of space (bar)
    float FallingGravityMultiplier = 2F; //Multiplies the gravity by a certain factor when the player falls
    float GravityMultiplier; //Intermediate variable used to multiply the force of gravity by a certain factor



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

        PressedAButton = Input.GetButtonDown("Jump");
        PressedBButton = Input.GetButtonDown("B");
        PressedXButton = Input.GetButtonDown("Dash");
        PressedYButton = Input.GetButtonDown("Y");

        HoldingAButton = Input.GetButton("Jump");
        HoldingBButton = Input.GetButton("B");
        HoldingXButton = Input.GetButton("Dash");
        HoldingYButton = Input.GetButton("Y");

        //Joystick direction

        xDirection = Input.GetAxisRaw("Horizontal");
        yDirection = Input.GetAxisRaw("Vertical");

        DiscretDirectionX = Math.Sign(xDirection);
        DiscretDirectionY = Math.Sign(yDirection);

        //Other Inputs

        HoldingSpace = Input.GetButton("Jump");
        PressedSpace = Input.GetButtonDown("Jump");
        PlayerFacingX = Math.Sign(square.velocity.x);
        isGrounded = Physics2D.BoxCast(coll.bounds.center, coll.bounds.size, 0f, Vector2.down, .1f, jumpableGround);


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

        if (PressedSpace == true)
        {
            JumpBufferTimeCounter = JumpBufferTime;
        }
        else if (PressedSpace == false)
        {
            JumpBufferTimeCounter -= Time.deltaTime;
        }


        //Jump Queuing

        if (PressedSpace == true && isGrounded == false)
        {
            JumpQueueTimeCounter = JumpQueueTime;
        }
        else if (PressedSpace == false && isGrounded == false)
        {
            JumpQueueTimeCounter -= Time.deltaTime;
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

        if (HoldingSpace && square.velocity.y >= 0)
        {
            GravityMultiplier = 1;
        }
        else if (HoldingSpace == false && square.velocity.y >= 0)
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

        if (xDirection == 0)
        {
            square.AddForce(new Vector2(-PlayerFacingX * FrictionAmount, 0));
        }


        //Clamped Fall Speed

        if (square.velocity.y < -MaxFallSpeed)
        {
            square.velocity = new Vector2(square.velocity.x, -MaxFallSpeed);
        }

        
        //Dash


    }
}
