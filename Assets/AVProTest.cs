using UnityEngine;
using System.Collections;
using RenderHeads.Media.AVProVideo;
using System;

public class AVProTest : MonoBehaviour
{
    AVProControl AVPro;

    Transform UIRoot;
    UISlider movieSlider;
    /// <summary>
    /// 显示实际进度
    /// </summary>
    UILabel movieSliderTime;

    // Use this for initialization
    void Start()
    {
        AVPro = new AVProControl(GetComponent<DisplayIMGUI>()._mediaPlayer);
        AVPro.AddEvent();
        UIRoot = GameObject.Find("UI Root").transform;
        UIEventListener.Get(UIRoot.Find("btnPlay").gameObject).onClick += (GameObject go) => { AVPro.Play(); };
        UIEventListener.Get(UIRoot.Find("btnStop").gameObject).onClick += (GameObject go) => { AVPro.Stop(); };
        UIEventListener.Get(UIRoot.Find("btnRePlay").gameObject).onClick += (GameObject go) => { AVPro.RePlay(); };

        //拖动进度条设置视屏长度
        movieSlider = UIRoot.Find("SliderMovie").GetComponent<UISlider>();
        UIEventListener.Get(movieSlider.gameObject).onPress += (GameObject go, bool state) =>
        {
            if (state)
            {
                AVPro.Stop();
            }
            else
            {
                AVPro.SetProgress(movieSlider.value);
                AVPro.Play();
            }
        };
        movieSliderTime = movieSlider.transform.Find("time").GetComponent<UILabel>();
        //设置音量
        UISlider sourceSlider = UIRoot.Find("SliderSource").GetComponent<UISlider>();
        UIEventListener.Get(sourceSlider.gameObject).onDrag += (go, delta) =>
        {           
            AVPro.SetVolume(sourceSlider.value);
        };

        //设置静音
        UIToggle  muteToggle = UIRoot.Find("MuteToggle").GetComponent<UIToggle>();
        UIEventListener.Get(muteToggle.gameObject).onClick += (GameObject go) =>
        {
            AVPro.SetMute(muteToggle.value);
        };
        AVPro.videoFinishCallBack = () => { Debug.Log("播放完毕"); };
       
        StartCoroutine(AVPro.LoadVideoWithFading("http://cn-sdqd-cu-v-06.acgvideo.com/upgcxcode/80/91/96079180/96079180-1-6.mp4?expires=1560238200&platform=html5&ssig=Ys-lUuAouPWffq0qXcSCFg&oi=2008310406&trid=92cf2fc43bdd4f7294c8ed02855b3138&nfb=maPYqpoel5MI3qOUX6YpRA==&nfc=1&mid=0"));
        //StartCoroutine(AVPro.LoadVideoWithFading("chuanjianyinwei.mp4"));
    }
    
    // Update is called once per frame
    void Update()
    {
        if (AVPro.isPlaying())
        {
            movieSlider.value = AVPro.GetProgress();
            movieSliderTime.text = AVPro.GetNowProgressTime() + ":" + AVPro.GetTotalProgressTime();
        }
    }
}