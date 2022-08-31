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
            animator.SetFloat("Speed", 0);
            return;
        }
        else
        {
            reachedEndOfPath = false;
        }


        Vector2 direction = ((Vector2)path.vectorPath[currentWaypoint] - rb.position).normalized;
        float targetDistance = Vector2.Distance(rb.position, player.position);
        float newSpeed = runSpeed * Mathf.Clamp(targetDistance / 10, 0, 2);
        Vector2 force = (direction * newSpeed * Time.deltaTime);

        if(direction.x > 0.01f)
        {
            horizontalMove = 1 * runSpeed;
        }else if(direction.x < -0.01f)
        {
            horizontalMove = -1 * runSpeed;
        }

        animator.SetFloat("Speed", Mathf.Abs(horizontalMove));

        if (direction.y > 0.1f )
        {
            isJumping = true;
            animator.SetBool("IsJumping", true);
        }

        controller.Move(horizontalMove * Time.fixedDeltaTime, isCrouching, isJumping);
        isJumping = false;

        float distance = Vector2.Distance(rb.position, path.vectorPath[currentWaypoint]);

        if (distance < nextWaypointDistance)
        {
            currentWaypoint++;
        }
    }
}
