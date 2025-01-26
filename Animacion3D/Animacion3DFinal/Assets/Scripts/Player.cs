using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public float velocidadMov;
    public float velocidadSen;
    float x, y;
    public GameObject espada;

    // Start is called before the first frame update
    void Start()
    {
        espada.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        x = Input.GetAxis("Horizontal");
        y = Input.GetAxis("Vertical");


        transform.Rotate(0, x * velocidadSen * Time.deltaTime, 0);
        transform.Translate(0, 0, (y * velocidadMov * Time.deltaTime));
    }
}
