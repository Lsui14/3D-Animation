using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fixer : MonoBehaviour
{
    public GameObject tela;
    List<Node> nodosFijos = new List<Node>();
    Vector3 FPosIni;
   
    void Start()
    {
        List<Node> nodos = tela.GetComponent<MassSpringCloth>().getNodes();
        FPosIni = transform.position;
        Bounds bounds = GetComponent<Collider>().bounds;

        foreach (Node node in nodos)
        {
            if (bounds.Contains(node.pos))
            {
                node.setFixed(true);
                nodosFijos.Add(node);
            }

        }

    }

    private void Update()
    {
        foreach (Node node in nodosFijos)
        {
            //Por cada nodo vamos a actualizar su posición sumandole la diferencia de posición global del Fixer
            node.pos = node.pos + (transform.position - FPosIni);

        }

        FPosIni = transform.position;
    }
}
