using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof (Controller2D))]
public class Player : MonoBehaviour {
    public LayerMask grappableMask;
    public float jumpHeight = 3.5f;
    public float moveSpeed = 6;
    public float timeToJumpApex = .4f;
    public float accelerationTimeAirborne = .2f;
    public float accelerationTimeGrounded = .1f;
    public float grappleMaxLength = 5;
    public float grappleSpeed = 30;

    bool grappling = false;
    Vector2 grapplePoint;
    float grappleLength;
    float gravity;
    float jumpVelocity;
    float velocityXSmoothing;

    bool wallSliding = false;

    Vector3 velocity;
    Controller2D controller;

	// Use this for initialization
	void Start ()
    {
        controller = GetComponent<Controller2D>();

        gravity = -(2 * jumpHeight) / Mathf.Pow(timeToJumpApex, 2);
        jumpVelocity = Mathf.Abs(gravity) * timeToJumpApex;
	}
	
	// Update is called once per frame
	void Update ()
    {
        if (controller.collisions.above || controller.collisions.below)
        {
            velocity.y = 0;
        }

        wallSliding = velocity.y != 0 && !controller.collisions.below && (controller.collisions.left || controller.collisions.right);

        Vector2 input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));

        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (wallSliding)
            {
                velocity.x = controller.collisions.left ? jumpVelocity : jumpVelocity * -1;
                velocity.y = jumpVelocity;
            }
            else if (controller.collisions.below)
            {
                velocity.y = jumpVelocity;
            }
        }

        if (Input.GetKeyDown(KeyCode.X))
        {
            Grapple();
            //Debug.DrawRay(transform.position, new Vector2(grappleMaxLength, grappleMaxLength), Color.red);
        }

        if (grappling && Input.GetKeyUp(KeyCode.X))
        {
            ReleaseGrapple();
        }

        float targetVelocityX = input.x * moveSpeed;
        float accelerationTime = controller.collisions.below ? accelerationTimeGrounded : accelerationTimeAirborne;

        velocity.x = Mathf.SmoothDamp(velocity.x, targetVelocityX, ref velocityXSmoothing, accelerationTime);

        if (wallSliding && velocity.y < 0)
        {
            velocity.y = gravity * 4 * Time.deltaTime;
        }
        else
        {
            velocity.y += gravity * Time.deltaTime;
        }


        Vector3 moveDelta = controller.Move(velocity * Time.deltaTime);

        if (grappling)
        {
            Vector3 newPosition = transform.position + moveDelta;
            if (((Vector2)newPosition - grapplePoint).magnitude > grappleLength)
            {
                // we're past the end of our rope, pull the hero back in.
                newPosition = grapplePoint + ((Vector2)newPosition - grapplePoint).normalized * grappleLength;
                velocity = (newPosition - transform.position) / Time.deltaTime;
                transform.position = newPosition;
            }
        }
        else
        {
            transform.Translate(moveDelta);
        }
       

        if (Input.GetKeyDown(KeyCode.Z))
        {
            Shoot();
        }
    }

    void Shoot()
    {
        LayerMask enemies = LayerMask.GetMask("Enemy");
        // Assign as property instead? 
        Transform weapon = transform.GetChild(0);
        RaycastHit2D hit = Physics2D.Raycast(weapon.position, Vector2.right, 0.5f, enemies);
        if (hit)
        {
            Enemy enemy = hit.transform.gameObject.GetComponent<Enemy>();
            if (enemy)
            {
                enemy.Shot();
            }
        }

        //Debug.DrawRay(weapon.position, Vector2.right * 0.5f, Color.red);
    }

    void Grapple()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, new Vector2(1, 1).normalized, grappleMaxLength, grappableMask);
        if (hit)
        {
            grappling = true;
            grapplePoint = hit.point;
            grappleLength = Vector3.Distance(hit.point, transform.position);
        }
    }

    void ReleaseGrapple()
    {
        grappling = false;
    }
}
