using System;
using System.Collections;
using System.Collections.Generic;
using AnimeRx;
using UniRx;
using UnityEngine;
using Motion = AnimeRx.Motion;

public class OffScreenController : MonoBehaviour
{
    [SerializeField] private Camera offScreenCamera;
    [SerializeField] private GameObject[] characters;
    
    [SerializeField] private float showTime = 0.5f;

    private int currentCharacterIndex;

    public int SelectedIndex => currentCharacterIndex;

    private int NormalizeIndex(int index)
    {
        if( index > characters.Length -1) return 0;
        return index < 0 ? characters.Length-1 : index;
    }
    

    private IDisposable currentAnimation = null;
    
    private void StartAnimation(int newIndex)
    {
        if (currentAnimation != null)
        {
            currentAnimation.Dispose();
        }
        
        var curPos = offScreenCamera.transform.position;
        var nextPos = characters[newIndex].transform.position;
        
        var animator = Easing.InOutCubic(showTime);

        currentAnimation = Anime.Play(curPos, nextPos, animator)
            .Subscribe( 
                p => offScreenCamera.transform.position = new Vector3(p.x, curPos.y, curPos.z),
                onCompleted: () => currentCharacterIndex = newIndex)
            .AddTo(this);
    }

    public void Next() => StartAnimation( NormalizeIndex(currentCharacterIndex + 1) );
    public void Prev() => StartAnimation( NormalizeIndex(currentCharacterIndex - 1) );
}