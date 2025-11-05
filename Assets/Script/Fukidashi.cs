using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fukidashi : MonoBehaviour
{
    private Vector3 pos;
    // Start is called before the first frame update
    void Start()
    {
        pos = this.gameObject.GetComponent<Transform>().position;
        pos = new Vector3(-497, -283, 0);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
