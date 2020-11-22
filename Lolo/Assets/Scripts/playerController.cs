using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerController : MonoBehaviour
{
    Rigidbody2D rigidbody2d;
    float horizontal;
    public float speed = 4.0f;

    //jumping
    public float jumpForce = 5.0f;
    public float fallMultiplier = 2.5f;
    public float lowJumpMultiplier = 2.0f;
    public float rememberGroundFor = 0.1f;
    float lastTimeGrounded;
    bool isGrounded = false; 
    public Transform isGroundedChecker; 
    public float checkGroundRadius; 
    public LayerMask groundLayer;
    public int defaultAdditionalJumps = 1;
    int additionalJumps;

    //animation
    Animator animator;
    public float lookDirection;
    public float runDirection;
    public float animationSpeed;
    //true = looking to the right

    void Start()
    {
        rigidbody2d = GetComponent<Rigidbody2D>();

        //animation
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        Jump();
        BetterJump();
        Move();
        CheckIfGrounded();

        lookDirection = Input.GetAxisRaw("Horizontal");

        //animation
        animationSpeed = rigidbody2d.velocity.x;
        animationSpeed = Mathf.Abs(animationSpeed);

        Debug.Log(animationSpeed);

        if (animationSpeed <= 0)
        {
            if (lookDirection == -1.0f)
            {
                animator.SetFloat("lookDirectionFloat", -1.0f);
            }
            else if (lookDirection == 1.0f)
            {
                animator.SetFloat("lookDirectionFloat", 1.0f);
            }  

            animator.SetFloat("runDirectionFloat", 0);   
        }
        else
        {
             if (lookDirection == -1.0f)
            {   
                animator.SetFloat("runDirectionFloat", -1.0f);
            }
            else if (lookDirection == 1.0f)
            {   
                animator.SetFloat("runDirectionFloat", 1.0f);
            }     

            animator.SetFloat("runDirectionFloat", animationSpeed);
        }

        

        if (lookDirection == -1.0f)
        {
            animator.SetFloat("lookDirectionFloat", -1.0f);
        }
        else if (lookDirection == 1.0f)
        {
            animator.SetFloat("lookDirectionFloat", 1.0f);
        }
    }

    void FixedUpdate()
    {
        
    }

    void Move()
    {
        float horizontal = Input.GetAxisRaw("Horizontal"); 
        float moveBy = horizontal * speed; 
        rigidbody2d.velocity = new Vector2(moveBy, rigidbody2d.velocity.y);

    }

    void Jump() 
    { 
        if (Input.GetKeyDown(KeyCode.Space) && (isGrounded || Time.time - lastTimeGrounded <= rememberGroundFor || additionalJumps > 0)) 
        { 
            rigidbody2d.velocity = new Vector2(rigidbody2d.velocity.x, jumpForce); 
            additionalJumps--;

            //animation

        } 
    }

    void BetterJump()
    {
        if (rigidbody2d.velocity.y < 0)
        {
            //rigidbody2d.velocity += Vector2.up * Physics2D.gravity * (fallMultiplier - 1) * Time.deltaTime;
        }
        else if (rigidbody2d.velocity.y > 0 || !Input.GetKey(KeyCode.Space))
        {
            rigidbody2d.velocity += Vector2.up * Physics2D.gravity * (lowJumpMultiplier - 1) * Time.deltaTime;
        }
    }

    void CheckIfGrounded()
    {
        Collider2D collider = Physics2D.OverlapCircle(isGroundedChecker.position, checkGroundRadius, groundLayer);

        if(collider != null)
        {
            isGrounded = true;
            additionalJumps = defaultAdditionalJumps;
        }
        else
        {
            if (isGrounded)
            {
                lastTimeGrounded = Time.time;
            }
            isGrounded = false;
        }
    }
}
