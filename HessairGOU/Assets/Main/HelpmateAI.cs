using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class HelpmateAI : MonoBehaviour
{
    public CharacterController2D controller;

    public float horizontalMove = 0f;
    public float verticalMove = 0f;

    public bool isJumping = false;
    public bool isCrouching = false;

    public Animator animator;

    public Transform player;
    public float runSpeed = 200f;
    public float nextWaypointDistance = 3f;

    public Transform helpmateGFX;

    Path path;
    Vector2 direction;
    int currentWaypoint = 0;
    bool reachedEndOfPath = false;

    Seeker seeker;
    Rigidbody2D rb;

    void Start()
    {
        seeker = GetComponent<Seeker>();
        rb = GetComponent<Rigidbody2D>();

        InvokeRepeating("UpdatePath", 0f, 0.1f);
    }

    void UpdatePath()
    {
        if (seeker.IsDone())
        {
            seeker.StartPath(rb.position, new Vector3(player.position.x, player.position.y - 1, player.position.z), OnPathComplete);
        }
    }

    void OnPathComplete(Path p)
    {
        if (!p.error)
        {
            path = p;
            currentWaypoint = 0;
        }
    }

    public void OnLanding()
    {
        animator.SetBool("IsJumping", false);
    }

    void FixedUpdate()
    {
        if (path == null)
        {
            return;
        }

        if (currentWaypoint >= path.vectorPath.Count || Vector2.Distance(rb.position, new Vector2(player.position.x, player.position.y - 1 )) <= 2f)
        {
            reachedEndOfPath = true;
            horizontalMove = 0;
            controller.Move(horizontalMove * Time.fixedDeltaTime, isCrouching, isJumping);
            animator.SetFloat("Speed", Mathf.Abs(horizontalMove));
            return;
        }
        else
        {
            reachedEndOfPath = false;
        }

        direction = ((Vector2)path.vectorPath[currentWaypoint] - rb.position).normalized;
        float playerDistance = Vector2.Distance(rb.position, player.position);

        if(playerDistance > 10)
        {
            rb.position = player.position;
        }

        if(direction.x > 0.01f)
        {
            horizontalMove = 1 * runSpeed;
        }else if(direction.x < -0.01f)
        {
            horizontalMove = -1 * runSpeed;
        }

        animator.SetFloat("Speed", Mathf.Abs(horizontalMove));

        controller.Move(horizontalMove * Time.fixedDeltaTime, isCrouching, isJumping);
        isJumping = false;

        if (direction.y > 0.1f && !animator.GetBool("IsJumping"))
        {
            isJumping = true;
            animator.SetBool("IsJumping", true);
        }

        float distance = Vector2.Distance(rb.position, path.vectorPath[currentWaypoint]);

        if (distance < nextWaypointDistance)
        {
            currentWaypoint++;
        }
    }
}
