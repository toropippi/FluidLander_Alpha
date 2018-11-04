using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class looptitle : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        Vector3 objpos;// = transform.position;
        objpos = transform.position;
        objpos.x += 0.023f;
        objpos.y += 0.023f;
        if (objpos.y > 8.6667f)
        {
            objpos.y -= 3.9f * 5f;
        }
        if (objpos.x > 8.6667f)
        {
            objpos.x  -=3.9f * 5f;
        }
        transform.position = objpos;

        //transform.localScale = new Vector3(scalex[select_x_id] + 0.5f * Mathf.Sin(0.1f * cnt), scaley[select_x_id], 1.0f);
        //GetComponent<SpriteRenderer>().material.SetVector("_Intensity", new Color(0.4f, 0.5f, 0.9f, 0.5f + 0.2f * Mathf.Cos(0.167234f * cnt)));
    }
}
