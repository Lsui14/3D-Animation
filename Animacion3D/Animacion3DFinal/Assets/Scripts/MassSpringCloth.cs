using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MassSpringCloth : MonoBehaviour
{
    // Start is called before the first frame update

    public MassSpringCloth()
    {
        this.paused = false;
        this.TimeStep = 0.01f;
        this.Gravity = new Vector3(0.0f, -9.81f, 0.0f);
        this.IntegrationMethod = Integration.Symplectic;
        this.mass = 1.0f;
        this.stiffness = 15f;
        this.nodes = new List<Node> { };
        this.springs = new List<Spring> { };
        this.edges = new List<Edge> { };
        this.damp = 10f;
        this.wind = true;
        this.windDirection = new Vector3(1f, 0.0f, 0.0f);
        this.windDirection.Normalize();
        this.intensity = 15f;
    }


    public enum Integration
    {
        Explicit = 0,
        Symplectic = 1,
    };

    #region InEditorVariables

    public bool paused;
    public float TimeStep;
    public Vector3 Gravity;
    public Integration IntegrationMethod;
    public float mass;
    public float stiffness;
    public float damp;
    public bool wind;
    public Vector3 windDirection;
    public float intensity;

    private Mesh mesh;
    private Vector3[] vertices;
    private int[] triangles;

    private List<Node> nodes;
    private List<Spring> springs;
    private List<Edge> edges;

    public int substeps = 5;
    #endregion
    void Awake()
    {
        this.mesh = this.GetComponent<MeshFilter>().mesh;
        this.vertices = mesh.vertices;
        this.triangles = mesh.triangles;

        createNodes(this.vertices);
        createSprings(this.triangles);

    }

    public List<Node> getNodes()
    {
        return this.nodes;
    }
    public void createSprings(int[] t)
    {
        float totalTriangles = t.Length / 3; 
        for (int i = 0; i < totalTriangles; i++)
        {
            int j = i * 3;
            int vert1 = t[j];
            int vert2 = t[j + 1];
            int vert3 = t[j + 2];


            Edge newEdge = new Edge(vert1, vert2, vert3);
            Edge newEdge1 = new Edge(vert2, vert3, vert1);
            Edge newEdge2 = new Edge(vert3, vert1, vert2);

            edges.Add(newEdge);
            edges.Add(newEdge1);
            edges.Add(newEdge2);
        }

        edges.Sort(); 

        
        Spring newSpring1 = new Spring(nodes[edges[0].VertexA], nodes[edges[0].VertexB], this.stiffness, this.damp);
    
        springs.Add(newSpring1);

        for (int i = 1; i < edges.Count; i++)
        {
            if (edges[i].compare(edges[i - 1]))
            {
                Spring newSpring = new Spring(nodes[edges[i].VertexOther], nodes[edges[i - 1].VertexOther], this.stiffness, this.damp);
                
                newSpring.stiffness = stiffness * 3 / 4; 
                springs.Add(newSpring);
            }
            else
            {
                Spring newSpring = new Spring(nodes[edges[i].VertexA], nodes[edges[i].VertexB], this.stiffness, this.damp);
                newSpring.nodeA = nodes[edges[i].VertexA];
                newSpring.nodeB = nodes[edges[i].VertexB];
                springs.Add(newSpring);
            }
        }


    }
    public void createNodes(Vector3[] v)
    {
        for (int i = 0; i < v.Length; i++)
        {
            Vector3 pos = transform.TransformPoint(vertices[i]);

            Node newNode = new Node(pos, this.mass, this.Gravity);

            
            nodes.Add(newNode);
        }

    }

   
    public void Update()
    {
        

    }

    private void FixedUpdate()
    {
        if (this.paused)
            return;

        float SubTime = TimeStep / substeps;

        for (int i = 0; i < substeps; i++)
        {
            switch (this.IntegrationMethod)
            {
                case Integration.Explicit: this.stepExplicit(SubTime); break;
                case Integration.Symplectic: this.stepSymplectic(SubTime); break;
                default:
                    throw new System.Exception("[ERROR] Should never happen!");
            }
        }
    }

    private void stepSymplectic(float time)
    {
        this.mesh = this.GetComponent<MeshFilter>().mesh;
        this.vertices = new Vector3[mesh.vertexCount];

        foreach (Node node in nodes)
        {
            node.force = Vector3.zero;
            node.ComputeForces();
            if (wind)
                node.force += (this.windDirection * this.intensity);
        }

        foreach (Spring spring in springs)
        {
            spring.ComputeForces();

        }
       
        foreach (Node node in nodes)
        {
            if (!node.isFixed)
            {
                node.vel += time / this.mass * node.force;
                node.pos += time * node.vel;
            }
        }
      
        foreach (Spring spring in springs)
        {
            spring.UpdateLength();
        }
       
        for (int i = 0; i < vertices.Length; i++)
        {
            Vector3 pos = nodes[i].pos;
            vertices[i] = transform.InverseTransformPoint(pos);
        }

        mesh.vertices = vertices;
    }

    private void stepExplicit(float time)
    {
        this.mesh = this.GetComponent<MeshFilter>().mesh;
        this.vertices = new Vector3[mesh.vertexCount];

        foreach (Node node in nodes)
        {
            node.force = Vector3.zero;
            node.ComputeForces();
            if (wind)
                node.force += (this.windDirection * this.intensity);
        }

        foreach (Spring spring in springs)
        {
            spring.ComputeForces();

        }
      
        foreach (Node node in nodes)
        {
            if (!node.isFixed)
            {
                node.pos += time * node.vel;
                node.vel += time / this.mass * node.force;
            }
        }
        
        foreach (Spring spring in springs)
        {
            spring.UpdateLength();
        }
        

        for (int i = 0; i < vertices.Length; i++)
        {
            Vector3 pos = nodes[i].pos;
            vertices[i] = transform.InverseTransformPoint(pos);
        }

        mesh.vertices = vertices;
    }
}
