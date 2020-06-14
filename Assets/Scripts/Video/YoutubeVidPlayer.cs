using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
//using YoutubeExtractor;

public class YoutubeVidPlayer : MonoBehaviour
{
    public string url;
    public int quality;
/*
    public async void Run()
    {
        IEnumerable<VideoInfo> videoInfos = await DownloadUrlResolver.GetDownloadUrlsAsync(url);
        VideoInfo video = videoInfos.First(info => info.VideoType == VideoType.Mp4 && info.Resolution == quality);

        if(video.RequiresDecryption)
        {
            DownloadUrlResolver.DecryptDownloadUrl(video);
        }

        //GetComponent<MediaPlayerCtrl>().m_strFileName = video.DownloadUrl;
        GetComponent<UnityEngine.Video.VideoPlayer>().url = video.DownloadUrl;
    }*/
}
