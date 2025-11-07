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
        pos = new Vector3(-507.269989f, -247.800003f, 2.55910659f);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
