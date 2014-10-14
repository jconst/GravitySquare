﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;

public class Player : MonoBehaviour
{
    const float speed = 12f;
    const float GRAVITY_SCALE = 80f;

    private HashSet<Collision2D> collisions =
        new HashSet<Collision2D> ();
    public Vector2 gravity {
        get {
            return Physics2D.gravity.normalized;
        }
        set {
            Physics2D.gravity = value * GRAVITY_SCALE;
        }
    }

    void Start() {
        Physics2D.gravity = -Vector2.up * GRAVITY_SCALE;
    }
    
    void Update() {
        if (TouchingFloor()) {
            UpdateGravity();
            Move();
        }
    }

    void UpdateGravity() {
        KeyConfig.GravityForKey.Where(kvp => Input.GetKeyDown(kvp.Key))
                               .Select(kvp => kvp.Value)
                               .ToList()
                               .ForEach(vec => gravity = vec);
    }

    void Move() {
        Vector3 heading = KeyConfig.HeadingForKey.Where(kvp => Input.GetKey(kvp.Key))
                                                 .Select(kvp => kvp.Value)
                                                 .Where(v => v.Abs() != gravity.Abs())
                                                 .Aggregate(default(Vector2), (v1, v2) => v1 + v2);
        Vector3 newPos = transform.position + heading * (Time.deltaTime * speed);
        transform.position = newPos;
    }

    void OnCollisionEnter2D(Collision2D coll) {
        if (coll.rigidbody != null) {
            collisions.Add(coll);
        }
    }

    void OnCollisionExit2D(Collision2D coll) {
        if (coll.rigidbody != null) {
            collisions.RemoveWhere(c => c.gameObject == coll.gameObject);
        }
        Debug.Log(collisions.Count);
    }

    bool TouchingFloor() {
        return collisions.SelectMany(coll => coll.contacts)
                         .Select(contact => contact.normal)
                         .Any(normal => Vector2.Dot(normal, gravity) < -0.9);
    }

    public void Die() {
        PlayerStart ps = (PlayerStart)GameObject.FindObjectOfType(typeof(PlayerStart));
        ps.GeneratePlayer();
        Destroy(gameObject);
    }
}
