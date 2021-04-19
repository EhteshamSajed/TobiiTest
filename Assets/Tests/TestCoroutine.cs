using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestCoroutine : MonoBehaviour
{
    long ticksCoroutine, ticksUpdate;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(MyCoroutine());
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log( "Update realTime: "+Time.realtimeSinceStartup );
        // Debug.Log("Update delta: " + Time.deltaTime);
        Debug.Log ("Update elapsed ticks: " + (System.DateTime.Now.Ticks - ticksUpdate));
        ticksUpdate = System.DateTime.Now.Ticks;
    }

    void FixedUpdate()
    {
        //Debug.Log( "FixedUpdate realTime: "+Time.realtimeSinceStartup);
        // Debug.Log("FixedUpdate delta: " + Time.deltaTime);
    }
    IEnumerator MyCoroutine()
    {
        while (true)
        {
            ticksCoroutine = System.DateTime.Now.Ticks;
            // yield return new WaitForSeconds(1f);
            yield return null;
            Debug.Log("MyCoroutine elapsed ticks: " + (System.DateTime.Now.Ticks - ticksCoroutine));
        }
    }
}
