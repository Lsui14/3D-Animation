using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node
{

    public Vector3 pos;
    public Vector3 vel;
    public Vector3 force;
    public Vector3 grav;
    public float mass;

    public bool isFixed;

    public Node(Vector3 p, float m, Vector3 g)
    {
        this.pos = p;
        this.vel = Vector3.zero;
        this.force = Vector3.zero;
        this.mass = m;
        this.grav = g;

        this.isFixed = false;
    }
  

    public void ComputeForces()
    {
        force += this.mass * this.grav;
    }

    public void setFixed(bool b)
    {
        this.isFixed = b;
    }
}
