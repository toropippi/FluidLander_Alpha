using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetVolBGM : MonoBehaviour {

    // Use this for initialization
    void Start () {
        int bgmvol = GameObject.Find("referobj").GetComponent<Referobj>().BGMVOL;
        AudioSource audioSource = gameObject.GetComponent<AudioSource>();
        audioSource.volume = 0.01f* (float)bgmvol;
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
