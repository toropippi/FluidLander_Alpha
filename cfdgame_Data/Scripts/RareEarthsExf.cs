using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RareEarthsExf : MonoBehaviour {
    Material mat;
    int cnt;
    float alfa;
    // Use this for initialization
    void Start () {
        mat = GetComponent<SpriteRenderer>().material;
        cnt = 0;
    }
	
	// Update is called once per frame
	void Update () {
        if (cnt % 3 == 0)
        {
            alfa = Mathf.Clamp(0.03f * Mathf.Abs(cnt % 80 - 40), 0.2f, 1.0f);
            mat.SetVector("_Intensity", new Color(alfa, alfa, alfa, 1.0f));
        }
        cnt++;
    }
}
