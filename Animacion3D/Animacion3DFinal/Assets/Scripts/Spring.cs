using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spring
{

    public Node nodeA, nodeB;

    public float Length0;
    public float Length;

    public float stiffness;
    public float damp;
    public Spring(Node a, Node b, float s, float d)
    {
        this.nodeA = a;
        this.nodeB = b;
        this.stiffness = s;
        this.damp = d;

        UpdateLength();
        Length0 = Length;
    }

 

    public void UpdateLength()
    {
        Length = (nodeA.pos - nodeB.pos).magnitude;
    }

    public void ComputeForces()
    {
        Vector3 u = nodeA.pos - nodeB.pos;
        u.Normalize();
        Vector3 force = -stiffness * (Length - Length0) * u;
        //Amortiguamiento
        force += -damp * Vector3.Project((nodeA.vel - nodeB.vel), u);
        
        nodeA.force += force;
        nodeB.force -= force;
    }
}