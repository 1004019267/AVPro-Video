using UnityEngine;
using System.Collections;
using RenderHeads.Media.AVProVideo;
using System;
using System.IO;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Text;
using System.Security.Cryptography;

public class AVProControl
{
    MediaPlayer mediaPlayer;
    public Action videoFinishCallBack;
    /// <summary>
    /// 当前视屏总长度毫秒
    /// </summary>
    float duration;

    public AVProControl(MediaPlayer mediaPlayer)
    {
        this.mediaPlayer = mediaPlayer;
        //使用DASH协议
        mediaPlayer.ForceFileFormat = FileFormat.DASH;
        //使用硬解
        mediaPlayer.PlatformOptionsWindows.useHardwareDecoding = true;

    }

    public void Play()
    {
        mediaPlayer.Play();
    }

    public void Stop()
    {
        if (isPlaying())
        {
            mediaPlayer.Stop();
        }
    }

    public void RePlay()
    {
        mediaPlayer.Rewind(false);
        mediaPlayer.Play();
    }

    /// <summary>
    /// 设置速率
    /// </summary>
    public void SetSpeedRate(float val)
    {
        mediaPlayer.Control.SetPlaybackRate(val);
    }

    /// <summary>
    /// 获取总时间00:00格式
    /// </summary>
    /// <returns></returns>
    public string GetTotalProgressTime()
    {
        return Helper.GetTimeString(duration * 0.001f);
    }

    /// <summary>
    /// 获取当前时间00:00格式
    /// </summary>
    /// <returns></returns>
    public string GetNowProgressTime()
    {
        //当前在几毫秒
        float time = mediaPlayer.Control.GetCurrentTimeMs();
        //最少取0.1    
        TimeSpan ts = TimeSpan.FromSeconds(time * 0.001f);
        //插件自带 把时间转化为00：00
        return Helper.GetTimeString(time * 0.001f);
    }

    /// <summary>
    /// 获取进度条0-1
    /// </summary>
    /// <returns></returns>
    public float GetProgress()
    {
        //当前在几毫秒
        float time = mediaPlayer.Control.GetCurrentTimeMs();
        return Mathf.Clamp(time / duration, 0.0f, 1.0f);
    }
    /// <summary>
    /// 设置进度条
    /// </summary>
    /// <param name="value">0-1</param>
    public void SetProgress(float value)
    {
        mediaPlayer.Control.SeekFast(value * mediaPlayer.Info.GetDurationMs());
    }

    public float GetBufferingProgress()
    {
        return mediaPlayer.Control.GetBufferingProgress();
    }

    /// <summary>
    /// 设置音量大小
    /// </summary>
    /// <param name="value">0-1</param>
    public void SetVolume(float value)
    {
        if (isPlaying())
        {
            mediaPlayer.Control.SetVolume(value);
        }
    }

    /// <summary>
    /// 静音按钮设置
    /// </summary>
    /// <param name="go"></param>
    public void SetMute(bool isMute)
    {
        if (isPlaying())
        {
            mediaPlayer.Control.MuteAudio(isMute);
        }
    }

    /// <summary>
    /// 加载加密视频
    /// </summary>
    /// <param name="path"></param>
    public IEnumerator LoadEncryptVideoWithFading(string path)
    {
        string newPath = Application.dataPath + "/StreamingAssets/videoNew";
        path = newPath + "/" + path;

        byte[] enBytes = DecryptVideo(newPath, FileTools.ReadFile(path));

        // Wait 3 frames for display object to update
        yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfFrame();
        // Load the video
        if (Application.isPlaying)
        {
            if (!mediaPlayer.OpenVideoFromBuffer(enBytes))
            {
                Debug.LogError("Failed to open video!");
            }
            else
            {
                // Wait for the first frame to come through (could also use events for this)
                while (Application.isPlaying && (VideoIsReady(mediaPlayer) || AudioIsReady(mediaPlayer)))
                {
                    yield return null;
                }
                // Wait 3 frames for display object to update
                yield return new WaitForEndOfFrame();
                yield return new WaitForEndOfFrame();
                yield return new WaitForEndOfFrame();
            }

        }
        duration = mediaPlayer.Info.GetDurationMs();

    }

    /// <summary>
    /// 加载普通视频
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    public IEnumerator LoadVideoWithFading(string path)
    {
        //网上或者本地加载
        if (path.Contains("http://"))
        {
            mediaPlayer.m_VideoLocation = MediaPlayer.FileLocation.AbsolutePathOrURL;
        }
        else
        {
            mediaPlayer.m_VideoLocation = MediaPlayer.FileLocation.RelativeToStreamingAssetsFolder;
        }
        // Wait 3 frames for display object to update
        yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfFrame();
        // Load the video
        if (Application.isPlaying)
        {
            if (!mediaPlayer.OpenVideoFromFile(mediaPlayer.m_VideoLocation, path))
            {
                Debug.LogError("Failed to open video!");
            }
            else
            {
                // Wait for the first frame to come through (could also use events for this)
                while (Application.isPlaying && (VideoIsReady(mediaPlayer) || AudioIsReady(mediaPlayer)))
                {
                    yield return null;
                }
                // Wait 3 frames for display object to update
                yield return new WaitForEndOfFrame();
                yield return new WaitForEndOfFrame();
                yield return new WaitForEndOfFrame();
            }

        }
        duration = mediaPlayer.Info.GetDurationMs();

    }
    /// <summary>
    /// 视频是否准备好了
    /// </summary>
    /// <param name="mp"></param>
    /// <returns></returns>
    private bool VideoIsReady(MediaPlayer mp)
    {
        return (mp != null && mp.TextureProducer != null && mp.TextureProducer.GetTextureFrameCount() <= 0);
    }
    /// <summary>
    ///  声音是否准备好了
    /// </summary>
    /// <param name="mp"></param>
    /// <returns></returns>
    private bool AudioIsReady(MediaPlayer mp)
    {
        return (mp != null && mp.Control != null && mp.Control.CanPlay() && mp.Info.HasAudio() && !mp.Info.HasVideo());
    }
    /// <summary>
    /// 是否在播放
    /// </summary>
    /// <returns></returns>
    public bool isPlaying()
    {
        return mediaPlayer.Control.IsPlaying();
    }

    public void AddEvent()
    {
        mediaPlayer.Events.AddListener(EventCallBack);
    }

    public void RemoveEvent()
    {
        mediaPlayer.Events.RemoveListener(EventCallBack);
    }

    private void EventCallBack(MediaPlayer meidaPlayer, MediaPlayerEvent.EventType type, ErrorCode error)
    {
        switch (type)
        {
            case MediaPlayerEvent.EventType.FinishedPlaying:
                if (videoFinishCallBack != null)
                {
                    videoFinishCallBack();
                }
                break;
        }
    }


    /// <summary>
    /// 解密
    /// </summary>
    /// <param name="KeyPath"></param>
    /// <param name="data"></param>
    /// <returns></returns>
    public byte[] DecryptVideo(string KeyPath, byte[] data)
    {
        Rijndael rij = new Rijndael();

        RijndaelKey rijKey = rij.GetKeyAndIV(KeyPath);
        return rij.Decrypt(data, rijKey.key, rijKey.IV);
    }
}