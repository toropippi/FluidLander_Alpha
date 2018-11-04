using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class loopmanager : MonoBehaviour {
    public GameObject loopbackfab;
	// Use this for initialization
	void Start () {
		for(int i = 0; i < 5; i++)
        {
            for (int j= 0; j < 5; j++)
            {
                Instantiate(loopbackfab, new Vector3(3.9f * i - 8.6667f / 2, 3.9f * j - 8.6667f / 2, 1.0f), Quaternion.identity);
            }
        }
	}
	
}
