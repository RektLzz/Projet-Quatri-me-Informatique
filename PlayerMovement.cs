using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using System;

public class PlayerMouvement : MonoBehaviour
{
    double xposition; //Initializing the "x" position variable of the square (that we can change)
    double yposition; //Initializing the "y" position variable of the square (that we can change)
    double xvelocity; //Initializing the velocity in "x" variable of the square (that we can change)
    double yvelocity; //Initializing the velocity in "y" variable of the square (that we can change)

    [SerializeField] private double g = 25; //Gravitational strength of the directed gravity field
    [SerializeField] private double GravityMultiplier = 4; //Multiplies downward force when player stops pressing spacebar
    [SerializeField] private double JumpForce = 13;


    //Initializing things for mouvement
    public static BoxCollider2D coll;
    public static Rigidbody2D square;
    [SerializeField] public LayerMask jumpableGround;

    //Initializing Horizontal Mouvement Variables
    double Acceleration = 13;
    double Deceleration = 16;
    double TargetSpeed;
    double TopSpeed = 9;
    double SpeedDif;
    double AccelRate;
    double Mouvement;
    double Direction;
    bool IsGrounded;

    //Fall gravity
    float FallGravityMultiplier = 2F;

    //Clamp (cap) fall speed
    float MaxFallSpeed = -15;

    //Friction
    float amount;
    float FrictionAmount;

    //Coyote Time
    float CoyoteTime = 0.15F; //How much time the player has to have pressed space bar since last grounded to jump
    float CoyoteTimeCounter;
    float JumpBufferTime = 0.2F;
    float JumpBufferTimeCounter;

    //Edge Detection
    float PreviousVelocity;
    float NudgedPosition;
    float Cooldown = 0.2F;
    float CooldownCounter;
    float BoostCoefficent = 4F;
    float WidthTolerance = 0.3F;
    bool EdgeDetection;


    //Defining all user input variables
    bool PressSpace;
    bool HoldingSpace;
    float ArrowKeyDirection;

    // Start is called before the first frame update
    void Start()
    {
        square = GetComponent<Rigidbody2D>(); //Assigning a variable that we can call to the rigidbody we created
        coll = GetComponent<BoxCollider2D>();
        square.position = new Vector2(0, 0); //Assigning a initial position to the square (rigidbody)
        square.velocity = new Vector2(0, 0); //Assigning a initial velocity to the square (rigidbody)
    }

    // Update is called once per frame

    public void Update()
    {

        //Checks If Player is Grounded (on the ground)

        bool isGrounded()
        {
            return Physics2D.BoxCast(coll.bounds.center, coll.bounds.size, 0f, Vector2.down, .1f, jumpableGround);
        }

        bool isCeilling()
        {
            return Physics2D.BoxCast(coll.bounds.center, coll.bounds.size, 0f, Vector2.up, .1f, jumpableGround);
        }


        //Coyote Time

        //Checks when was the last time the player was grounded

        if (isGrounded() == true)
        {
            CoyoteTimeCounter = CoyoteTime;
        }
        else if (isGrounded() == false)
        {
            CoyoteTimeCounter -= Time.deltaTime; //Counts down time
        }


        //Checks when was the last time the player pressed space bar

        if (Input.GetButtonDown("Jump"))
        {
            JumpBufferTimeCounter = JumpBufferTime;
        }
        else
        {
            JumpBufferTimeCounter -= Time.deltaTime;
        }

        //Edge detection

        CooldownCounter -= Time.deltaTime;


        //Horizontal Edge Detection

        if (isCeilling())
        {
            square.position = new Vector2(square.position.x + WidthTolerance, square.position.y + 0.1F);
            square.velocity = new Vector2(square.velocity.x, PreviousVelocity);

            if (isCeilling())
            {
                square.position = new Vector2(square.position.x - 2*WidthTolerance, square.position.y);
                square.velocity = new Vector2(square.velocity.x, PreviousVelocity);
                if (isCeilling())
                {
                    square.position = new Vector2(square.position.x + WidthTolerance, square.position.y - 0.1F);
                    square.velocity = new Vector2(square.velocity.x, 0);
                }
                else
                {

                }
            }
            else
            {

            }
        }
    }

     


    public void FixedUpdate()
    {
        //Variable Jump

        if (Input.GetKey("space") == false && square.velocity.y > 0)
        {
            square.AddForce(new Vector2(0, -Convert.ToSingle(g * GravityMultiplier)));

            CoyoteTimeCounter = 0f; //Once we stop pressing spacebar, the coyote time goes back to 0

        }
        else
        {
            square.AddForce(new Vector2(0, -Convert.ToSingle(g * FallGravityMultiplier)));
        }

        //Clamping (capping) fall speed
        if (square.velocity.y <= MaxFallSpeed)
        {
            square.velocity = new Vector2(square.velocity.x, MaxFallSpeed);
        }


        //Horizontal mouvement

        if (Input.GetAxisRaw("Horizontal") != 0)
        {


            Direction = Input.GetAxisRaw("Horizontal");

            TargetSpeed = Direction * TopSpeed;

            SpeedDif = TargetSpeed - Convert.ToDouble(square.velocity.x);

            AccelRate = (Math.Abs(TargetSpeed) > 0f) ? Acceleration : Deceleration;

            Mouvement = Math.Pow(Math.Abs(SpeedDif) * AccelRate, 0.96) * Math.Sign(SpeedDif);

            square.AddForce(new Vector2(Convert.ToSingle(Mouvement), 0));


        }

        //Friction

        if (Input.GetAxisRaw("Horizontal") == 0)
        {

            FrictionAmount = 0.1F;

            amount = Math.Min(Math.Abs(square.velocity.x), Math.Abs(FrictionAmount));

            amount *= Math.Sign(square.velocity.x);

            square.AddForce(new Vector2(-amount, 0), ForceMode2D.Impulse);

        }

        //Jump


        if (JumpBufferTimeCounter > 0 && CoyoteTimeCounter > 0)
        {
            square.AddForce(new Vector2(0, Convert.ToSingle(JumpForce)), ForceMode2D.Impulse);

            JumpBufferTimeCounter = 0;

        }


        //Checks when was the last time the player has pressed spacebar


        //Edge Detection

        //Vertical Edge Detection

        if (square.velocity.y < 0 && PreviousVelocity > 0 && square.velocity.x == 0 && CooldownCounter < 0 && Input.GetAxisRaw("Horizontal") != 0)
        {
            Debug.Log(square.position.y);
            square.AddForce(new Vector2(square.velocity.x, BoostCoefficent), ForceMode2D.Impulse);
            CooldownCounter = Cooldown;

        }

        PreviousVelocity = square.velocity.y;

    }
}