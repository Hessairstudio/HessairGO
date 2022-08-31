using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class FollowerAI : MonoBehaviour
{
    public Transform player;
    public float speed = 200f;
    public float nextWaypointDistance = 3f;

    public Transform followerGFX;

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
            seeker.StartPath(rb.position, new Vector3(player.position.x, player.position.y + 2, player.position.z) , OnPathComplete);
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

    void FixedUpdate()
    {
        if(path == null)
        {
            return;
        }

        if(currentWaypoint >= path.vectorPath.Count || Vector2.Distance(rb.position, new Vector2(player.position.x, player.position.y + 2)) <= 1f)
        {
            reachedEndOfPath = true;
            return;
        }
        else
        {
            reachedEndOfPath = false;
        }

        
        Vector2 direction = ((Vector2)path.vectorPath[currentWaypoint] - rb.position).normalized;
        float targetDistance = Vector2.Distance(rb.position, player.position );
        float newSpeed = speed * Mathf.Clamp(targetDistance/10, 0,2);
        Vector2 force = (direction * newSpeed * Time.deltaTime);

        rb.AddForce(force);

        float distance = Vector2.Distance(rb.position, path.vectorPath[currentWaypoint]);

        if (distance < nextWaypointDistance)
        {
            currentWaypoint++;
        }

        if (force.x >= 0.01f)
        {
            followerGFX.localScale = new Vector3(1f, 1f, 1f);
        }
        else if (force.x <= -0.01f)
        {
            followerGFX.localScale = new Vector3(-1f, 1f, 1f);
        }

    }
}
