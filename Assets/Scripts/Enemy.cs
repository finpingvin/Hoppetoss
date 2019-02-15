using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Controller2D))]
public class Enemy : MonoBehaviour
{
    Vector3 velocity;
    Controller2D controller;
    float moveSpeed = 6;
    float gravity = -20;

    // Use this for initialization
    void Start ()
    {
        controller = GetComponent<Controller2D>();
    }
	
	// Update is called once per frame
	void Update ()
    {
        if (controller.collisions.above || controller.collisions.below)
        {
            velocity.y = 0;
        }

        velocity.y += gravity * Time.deltaTime;
        transform.Translate(controller.Move(velocity * Time.deltaTime));
    }

    public void Shot()
    {
        velocity.x = moveSpeed;
    }
}
