using System;
using System.Collections;
using System.Collections.Generic;
using AnimeRx;
using Bootstrappers;
using UniRx;
using UnityEngine;
using Utils;
using CharacterInfo = RemoteConfig.CharacterInfo;


public class OffScreenController : MonoBehaviour, IHaveUIOwner
{
    [SerializeField] private Camera offScreenCamera;
    [SerializeField] private float showTime = 0.5f;
    [SerializeField] private Vector3 offset = new Vector3(-3,0,0);
    
    private int _currentCharacterIndex;
    private readonly List<GameObject> _characters = new List<GameObject>();

    public int SelectedIndex => _currentCharacterIndex;

    private int NormalizeIndex(int index)
    {
        if( index > _characters.Count - 1) return 0;
        return index < 0 ? _characters.Count - 1 : index;
    }

    private IDisposable _currentAnimation = null;
    private IAssetManager _assetManager;

    private void StartAnimation(int newIndex)
    {
        _currentAnimation?.Dispose();

        var curPos = offScreenCamera.transform.position;
        var nextPos = _characters[newIndex].transform.position;
        
        var animator = Easing.InOutCubic(showTime);

        _currentAnimation = Anime.Play(curPos, nextPos, animator)
            .Subscribe( 
                p => offScreenCamera.transform.position = new Vector3(p.x, curPos.y, curPos.z),
                onCompleted: () => _currentCharacterIndex = newIndex)
            .AddTo(this);
    }

    public void Next() => StartAnimation( NormalizeIndex(_currentCharacterIndex + 1) );
    public void Prev() => StartAnimation( NormalizeIndex(_currentCharacterIndex - 1) );

    
    
    public void AddCharacter(CharacterInfo character)
    {
        var placeholder = new GameObject("CharacterPlaceHolder" + _characters.Count );
        placeholder.transform.SetParent(transform,false);
        _characters.Add(placeholder);

        _assetManager.LoadAssetByNameAsync(character.Id, 
        go =>
        {
            var index = _characters.IndexOf(placeholder);
            Destroy(placeholder);
            
            go.transform.SetParent(transform,false);
            go.transform.localPosition = offset * index;
            _characters[index] = go;

            var data = go.GetComponent<CharactersBindData>();
            data.SetSkinType(character.SkinType);
            data.SetHealthBarType(CharactersBindData.HealthBarType.None);
        });
    }

    public IUIOwner Owner { get; private set; }
    public void SetOwner(IUIOwner owner)
    {
        Owner = owner;
        _assetManager = Owner.Container.Resolve<IAssetManager>();
    }
}