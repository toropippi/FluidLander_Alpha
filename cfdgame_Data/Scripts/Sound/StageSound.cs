using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageSound : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioClip[] audioClip= new AudioClip[9];
    Stagemanager stgmngrcomp;
    int now_frame_audioClip_no;
    int pre_frame_audioClip_no;
    int[] stagesAudio;
    void Start()
    {
        audioSource = gameObject.GetComponent<AudioSource>();
        audioSource.clip = audioClip[0];
        stgmngrcomp = GameObject.Find("StageManager").GetComponent<Stagemanager>();//コンポーネント
        now_frame_audioClip_no = -1;
        pre_frame_audioClip_no = -1;
        stagesAudio = new int[19] { 0, 1, 1, 1,1, 1, 2, 2, 2, 3,3,3,3,4,4,5,5,5,5 };
    }
    

    // Update is called once per frame
    void Update()
    {
        pre_frame_audioClip_no = now_frame_audioClip_no;
        now_frame_audioClip_no = stagesAudio[stgmngrcomp.nowstage];

        //ステージの値が変わったフレームにBGMを切り替える
        if (now_frame_audioClip_no!= pre_frame_audioClip_no)
        {
            if (pre_frame_audioClip_no != -1) { audioSource.Stop(); }
            audioSource.clip = audioClip[now_frame_audioClip_no];
            audioSource.Play();
        }
        
    }
}