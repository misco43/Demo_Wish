using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class TimeLineMgr : SingletonMono<TimeLineMgr>
{
    [SerializeField] private PlayableDirector _director;

    public void OnPlay()
    {
        _director.Play();
    }

    public void OnStop()
    {
        _director.Stop();
    }

    public void OnPause()
    {
        _director.Pause();
    }
}
