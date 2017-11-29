using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NinjaMovement : MonoBehaviour {
	private Rigidbody2D playerBody;

    private Animator playerAnimator;

    [SerializeField]
    private float movementSpeed;

    private bool facingRight;

    private bool attack;

    [SerializeField]
    private Transform[] groundPoints;

    [SerializeField]
    private float groundRadius;

    [SerializeField]
    private LayerMask whatIsGround;

    private bool isGrounded;

    private bool jump;

    [SerializeField]
    private float jumpForce;

    private Vector3 startPosition;

    void Start ()
    {
        facingRight = true;
        playerBody = GetComponent<Rigidbody2D>();
        playerAnimator = GetComponent<Animator>();
        startPosition = transform.position;
	}
	
    private void HandleMovement(float horizontal)
    {
        if(playerBody.velocity.y < 0)
        {
            playerAnimator.SetBool("land", true);
        }

        if(!this.playerAnimator.GetCurrentAnimatorStateInfo(0).IsTag("attack"))
        {
            playerBody.velocity = new Vector2(horizontal * movementSpeed, playerBody.velocity.y);
        }

        if(isGrounded && jump)
        {
            isGrounded = false;
            playerBody.AddForce(new Vector2(0, jumpForce * 1000));
            playerAnimator.SetTrigger("jump");
        }
        
        playerAnimator.SetFloat("speed", Mathf.Abs(horizontal));
    }

    private void HandleAttacks()
    {
        if(attack && !this.playerAnimator.GetCurrentAnimatorStateInfo(0).IsTag("attack"))
        {
            playerAnimator.SetTrigger("attack");
            playerBody.velocity = Vector2.zero;
        }
    }

    private void HandleInput()
    {
        if (this.playerAnimator.GetCurrentAnimatorStateInfo(0).IsTag("dead"))
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            attack = true;
        }

        if(Input.GetKeyDown(KeyCode.Space))
        {
            jump = true;
        }
    }

    private void Flip(float horizontal)
    {
        if((horizontal > 0 && !facingRight || horizontal < 0 && facingRight))
        {
            facingRight = !facingRight;

            Vector3 scale = transform.localScale;
            scale.x *= -1;

            transform.localScale = scale;
        }
    }

	// Update is called once per frame
	void FixedUpdate () {
        if (playerAnimator.GetCurrentAnimatorStateInfo(0).IsTag("dead"))
        {
            return;
        }
        playerAnimator.SetBool("restart", false);
        float horizontal = Input.GetAxis("Horizontal");

        isGrounded = IsGrounded();

        HandleMovement(horizontal);

        Flip(horizontal);

        HandleAttacks();

        HandleLayers();

        ResetValues();
	}

    private void Update()
    {
        HandleInput();
    }

    private void ResetValues()
    {
        attack = false;
        jump = false;
    }

    private bool IsGrounded()
    {
        if(playerBody.velocity.y <= 0)
        {
            foreach (Transform point in groundPoints)
            {
                Collider2D[] colliders = Physics2D.OverlapCircleAll(point.position, groundRadius, whatIsGround);

                for (int i = 0; i < colliders.Length; i++)
                {
                    if(colliders[i].gameObject != gameObject)
                    {
                        playerAnimator.ResetTrigger("jump");
                        playerAnimator.SetBool("land", false);
                        return true;
                    }
                }
            }
        }

        return false;
    }

    private void HandleLayers()
    {
        if(!isGrounded)
        {
            playerAnimator.SetLayerWeight(1, 1);
        } else
        {
            playerAnimator.SetLayerWeight(1, 0);
        }
    }

    public void ResetToStartPosition()
    {
        transform.position = startPosition;
        ResetValues();
        playerAnimator.ResetTrigger("dead");
        playerAnimator.SetBool("restart", true);
    }
}
