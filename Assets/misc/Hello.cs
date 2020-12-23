using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hello : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Debug.Log( "Hello Fuge at Start()!" );
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = Vector3.MoveTowards( transform.position,new Vector3( 0,0,0 ),Time.deltaTime );
    }
}
