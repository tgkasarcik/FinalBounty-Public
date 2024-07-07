using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotate : MonoBehaviour
{
    [SerializeField] float speed;
    // Update is called once per frame
    void Update()
    {
        transform.Rotate(speed * Time.deltaTime *new Vector3(0,1,0), Space.World);
    }
}
