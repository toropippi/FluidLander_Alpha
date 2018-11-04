using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GageSpeedX : MonoBehaviour
{
    Material mat;
    Ufo ufocomp;
    Gagemaster ggmstrcomp;
    // Use this for initialization
    void Start () {
        mat = GetComponent<SpriteRenderer>().material;
        ufocomp = GameObject.Find("ufo").GetComponent<Ufo>();//ufo コンポーネント
        ggmstrcomp = GameObject.Find("gage").GetComponent<Gagemaster>();//ufo コンポーネント
    }
	
	// Update is called once per frame
	void Update ()
    {
        mat.SetVector("_Intensity", new Color(0.4f, Mathf.Clamp(0.6f + Mathf.Abs(ufocomp.ufo_spd.y), 0.6f, 1.0f), 1.0f, Mathf.Clamp(0.5f + 3.0f * Mathf.Abs(ufocomp.ufo_spd.y), 0.5f, 1.0f)* ggmstrcomp.alfa_gage));
        mat.SetFloat("_SPEED", ufocomp.ufo_spd.x);
        
    }
}
