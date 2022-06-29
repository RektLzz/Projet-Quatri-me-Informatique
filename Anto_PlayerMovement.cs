using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;

public class PlayerMovement : MonoBehaviour
{
    #region
    //Vertical movement
    [Range(20, 30)] private float g = 24.1f; //Gravitational strength of the directed gravity field
    private float GravityMultiplier = 4; //Multiplies downward force when player stops pressing spacebar
    [Range(13, 20)] private float JumpForce = 16f;
    public bool DirectionGravity = true; //Do we apply Directional gravity ? false = no, true = yes

    //Initializing Horizontal Mouvement Variables
    [Range(10,20)] private float Acceleration = 13;
    [Range(15, 25)] private float Deceleration = 16.66f;
    float TargetSpeed;
    [Range(6, 20)] private float TopSpeed = 9.4f;
    float SpeedDif;
    float AccelRate;
    float Mouvement;
    float dirX;

    //Fall gravity
    const float FallGravityMultiplier = 2F;

    //Clamp (cap) fall speed
    const float MaxFallSpeed = -15;

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
    float Cooldown = 0.3F;
    float CooldownCounter;
    float BoostCoefficient = 6F;
    [Range(-0.5f, 1f)] public float edgeGap;

    //Particle System
    public ParticleSystem jumpDust;
    private bool spawnDust;


    //Calling objects
    BoxCollider2D coll;
    SpriteRenderer sprite;
    Rigidbody2D square;
    [SerializeField] public LayerMask jumpableGround;
    private Animator anim;
    public BoxCollider2D ceillingBounds;
    private bool ceillingCollision;
    public static bool isTouchingGround;
    #endregion



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


        // [-1;1]
        dirX = Input.GetAxisRaw("Horizontal");

        //makes the place face the correct direction (wouldn't work with console)
        if (dirX != 0)
        {
            sprite.transform.localScale = new Vector3(dirX, 1, 2);


        }

        //coyote time
        if (isTouchingGround)
        {
            CoyoteTimeCounter = CoyoteTime;
        }
        else if (!isTouchingGround)
        {
            CoyoteTimeCounter -= Time.deltaTime; //Counts down time
        }

        if (Input.GetButtonDown("Jump"))
        {
            JumpBufferTimeCounter = JumpBufferTime;
        }
        else
        {
            JumpBufferTimeCounter -= Time.deltaTime;
        }

        //Edge Detection

        CooldownCounter -= Time.deltaTime;

        if (isDead())
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex * 0);
        }

        anim.SetBool("isRunning", dirX != 0);
        upCollision();
        anim.SetBool("grounded", isTouchingGround);
        anim.SetBool("isFalling", square.velocity.y < 0 && !isTouchingGround);
        anim.SetBool("isRising", square.velocity.y > 0 && !isTouchingGround);
        //sideCollision();

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
  

        bool isDead()
        {
            return square.position.y <= -10;
        }

        //check if the player is Grounded

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

        /*  void sideCollision()
         {
             RaycastHit2D wallBounds = Physics2D.BoxCast(coll.bounds.center, coll.bounds.size, 0f, Vector2.right, .1f, jumpableGround);

             float yCoordinate = wallBounds.point.y;
             float yPlayerCoordinate = square.position.y;

             if (wallBounds)
             {

                 Debug.Log(yPlayerCoordinate + " " + yCoordinate);
                 if (Math.Abs(yCoordinate) - Math.Abs(yPlayerCoordinate) > edgeGap)
                 {
                     square.position = new Vector2(square.position.x + 0.3f, square.position.y + 0.2f);
                     Debug.Log("ok");
                 }
             }*/


    }

    public void isGrounded()
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

    public void FixedUpdate() {

        isGrounded();

        /*if (!isTouchingGround)
            dustTrail.Play();
        */
        //Full and Cut jump
        if (Input.GetKey("space") == false && square.velocity.y > 0)
        {
            square.AddForce(new Vector2(0, -g * GravityMultiplier));

            //Once we stop pressing spacebar, the coyote time goes back to 0
            CoyoteTimeCounter = 0f;
        }
        else
        {
            // we use the smaller coefficient
            square.AddForce(new Vector2(0, -g * FallGravityMultiplier));

        }

        //Clamping (capping) fall speed
        if (square.velocity.y <= MaxFallSpeed)
        {
            square.velocity = new Vector2(square.velocity.x, MaxFallSpeed);
        }


        //Horizontal mouvement

        if (Input.GetAxisRaw("Horizontal") != 0)
        {

            TargetSpeed = dirX * TopSpeed;

            SpeedDif = TargetSpeed - square.velocity.x;

            AccelRate = (Math.Abs(TargetSpeed) > 0f) ? Acceleration : Deceleration;

            Mouvement = (float)(Math.Pow(Math.Abs(SpeedDif) * AccelRate, 0.96) * Math.Sign(SpeedDif));

            square.AddForce(new Vector2(Mouvement, 0));

        }

        //Friction

        if (Input.GetAxisRaw("Horizontal") == 0)
        {

            FrictionAmount = 0.4F;

            amount = Math.Min(Math.Abs(square.velocity.x), Math.Abs(FrictionAmount));

            amount *= Math.Sign(square.velocity.x);

            square.AddForce(new Vector2(-amount, 0), ForceMode2D.Impulse);

        }

        // Jump

        if (JumpBufferTimeCounter > 0 && CoyoteTimeCounter > 0)
        {
            square.AddForce(Vector2.up * JumpForce, ForceMode2D.Impulse);

            JumpBufferTimeCounter = 0;

        }

    }

}
