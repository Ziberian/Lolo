using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerController : MonoBehaviour
{
    Rigidbody2D rigidbody2d;
    float horizontal;
    float horizontalRaw;
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
    public bool facingRight;
    //true = looking to the right

    //hitting
    public Transform hitBox;
    public float hitRadius;
    public LayerMask moveableObject;
    public float hitPower = 5.0f;
    public float hitTimer = 0.3f;   

    //bomb
    public GameObject bombPrefab;
    Vector3 throwDirection;
    public float throwSpeed = 5.0f;

    void Start()
    {
        rigidbody2d = GetComponent<Rigidbody2D>();

        //animation
        animator = GetComponent<Animator>();
        facingRight = true;
    }

    void Update()
    {
        Jump();
        BetterJump();
        Move();
        CheckIfGrounded();
        HitAnimation();

        //animation
        animator.SetFloat("runDirectionFloat", rigidbody2d.velocity.magnitude);

        if(Input.GetKeyDown(KeyCode.G))
        {
            ThrowBomb();
        }
    }

    void FixedUpdate()
    {
        horizontal = Input.GetAxis("Horizontal");
        Flip(horizontal);
    }

    void Move()
    {
        horizontalRaw = Input.GetAxisRaw("Horizontal");
        float moveBy = horizontalRaw * speed; 
        rigidbody2d.velocity = new Vector2(moveBy, rigidbody2d.velocity.y);
    }

    void Jump() 
    { 
        if (Input.GetKeyDown(KeyCode.Space) && (isGrounded || Time.time - lastTimeGrounded <= rememberGroundFor || additionalJumps > 0)) 
        { 
            rigidbody2d.velocity = new Vector2(rigidbody2d.velocity.x, jumpForce);
            additionalJumps--;

            //animation
            animator.SetTrigger("Jumping");
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

    void HitAnimation()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            animator.SetTrigger("Attacking");
            Invoke("Hit", 0.3f);
        }
    }
    
    void Hit()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(hitBox.position, hitRadius);

        foreach (Collider2D hit in colliders)
        {
            Rigidbody2D rb = hit.GetComponent<Rigidbody2D>();

            if (rb != null && rb.tag != "Player")
            {
                Vector2 direction = (rb.transform.position - transform.position).normalized;
                direction.y += 1.0f;
                Debug.Log(direction);   
                rb.AddForce(direction * hitPower, ForceMode2D.Impulse);
            }
        }
    }

    void CheckIfGrounded()
    {  
        Collider2D collider = Physics2D.OverlapCircle(isGroundedChecker.position, checkGroundRadius, groundLayer);

        if(collider != null)
        {
            isGrounded = true;
            animator.SetBool("isFalling", false);
            additionalJumps = defaultAdditionalJumps;
        }
        else
        {
            if (isGrounded)
            {
                lastTimeGrounded = Time.time;
            }
            isGrounded = false;
            animator.SetBool("isFalling", true);
        }
    }

    void Flip(float lookDirection)
    {
        if (horizontal > 0 && !facingRight || horizontal < 0 && facingRight)
        {
            facingRight = !facingRight;

            Vector3 theScale = transform.localScale;
            theScale.x *= -1;
            transform.localScale = theScale;
        }
    }

    void ThrowBomb()
    {
        Vector3 worldMousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 throwDirection = (Vector2)((worldMousePos - transform.position));
        throwDirection.Normalize ();

        GameObject bombObject = Instantiate(bombPrefab, rigidbody2d.position, 
        Quaternion.identity);

        Physics2D.IgnoreCollision(bombObject.GetComponent<Collider2D>(), 
        GetComponent<Collider2D>());
        
        Bomb bomb =  bombObject.GetComponent<Bomb>();
        bomb.Launch(throwDirection, 500);
    }
}
