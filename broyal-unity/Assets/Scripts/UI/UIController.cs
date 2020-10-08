﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Data;
using TMPro;
using UniRx.Async.Triggers;
using Unity.NetCode;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UI.ProceduralImage;
using Object = UnityEngine.Object;

public class UIController : MonoBehaviour
{
    [SerializeField] private MainUI main;
    [SerializeField] private GameUI game;
    [SerializeField] private LoadingUI loading;
    [SerializeField] private LobbyUI lobby;
    
    
    
    public MainUI MainUI => main;
    public LoadingUI LoadingUI => loading;
    public GameUI GameUI => game;
    public LobbyUI Lobby => lobby;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

public interface IGameObjectActivator
{
    void Show();
    void Hide();
}
    
public class MainGOActivator : IGameObjectActivator
{
    private GameObject _go;
    public MainGOActivator(GameObject go) => _go = go;
        
    public void Show() =>  _go.SetActive(true);
    public void Hide() => _go.SetActive(false);
}

   
public class SimpleUIController : IGameObjectActivator
{
    [SerializeField] protected GameObject body;
    private IGameObjectActivator _gameObjectActivator => new MainGOActivator(body);
    public virtual void Show() => _gameObjectActivator.Show();
    public virtual void Hide() => _gameObjectActivator.Hide();
}
    
[Serializable]
public class MainUI : SimpleUIController
{
    [SerializeField] private Button startButton;
    [SerializeField] private Button exitButton;

    [SerializeField] private Button nextCharacterButton;
    [SerializeField] private Button prevCharacterButton;
    
    [SerializeField] private CharactersPanel charactersPanel;
    
    [SerializeField] private SkillsPanelData skillsPanel;
    
    [SerializeField] private OffScreenController offScreenController;

   
    
    public void Show(IList<string> characterIds, IList<string> skillIds)
    { 
        base.Show();
        
        startButton.onClick.RemoveAllListeners();
        startButton.onClick.AddListener( () => OnGameStarted?.Invoke(skillsPanel.CurrentSkillId, characterIds[offScreenController.SelectedIndex]) );
        
        nextCharacterButton.onClick.RemoveAllListeners();
        nextCharacterButton.onClick.AddListener( () => offScreenController.Next() );
        
        prevCharacterButton.onClick.RemoveAllListeners();
        prevCharacterButton.onClick.AddListener( () => offScreenController.Prev() );

        foreach (var characterId in characterIds)
        {
            charactersPanel.Add(characterId);
        }
            
        foreach (var skillId in skillIds)
        {
            skillsPanel.Add(skillId);
        }

        skillsPanel.SetOn(0);
    }

    public override void Hide()
    {
        base.Hide();
        
        charactersPanel.Clean();
        skillsPanel.Clean();
    }

    public event Action<string, string> OnGameStarted;
}

[Serializable]
public class SkillsPanelData
{
    [SerializeField] private GameObject root;
        
    [SerializeField] private ToggleGroup toggleGroup;
    [SerializeField] private SkillData skillPrefab;

    public string CurrentSkillId { get; private set; }
    //[SerializeField] private Button exitButton;
    public void Clean()
    {
        foreach (Transform obj in root.transform)
        {
           Object.Destroy(obj.gameObject);
        }
    }

    public void Add(string skillId)
    {
        var newInstance = Object.Instantiate(skillPrefab, root.transform, false);
        newInstance.SetName(skillId);
        newInstance.SetGroup(toggleGroup);
        newInstance.OnChangeState = s => CurrentSkillId = skillId;
    }
    
    public void SetOn(int index)
    {
        root.transform.GetChild(index).GetComponent<Toggle>().isOn = true;
    }
}

[Serializable]
public class CharactersPanel
{
    [SerializeField] private GameObject root;
    [SerializeField] private CharacterData characterSelectData;
    //[SerializeField] private Button exitButton;
    public void Clean()
    {
        foreach (Transform obj in root.transform)
        {
            Object.Destroy(obj.gameObject);
        }
    }

    public void Add(string characterId)
    {
        var newInstance = Object.Instantiate(characterSelectData, root.transform, false);
        newInstance.SetName(characterId);
    }
}
    
[Serializable]
public class LoadingUI : SimpleUIController
{
    [SerializeField] private TMP_Text exitButton;
}
    
[Serializable]
public class GameUI : SimpleUIController
{
    [SerializeField] private Button action1Button;
    [SerializeField] private Button action2Button;
    [SerializeField] private Button action3Button;

    [SerializeField] private Slider healthBar;
       
    [SerializeField] private Button exitButton;
    [SerializeField] private Button mainButton;

    public void SetHealth(int playerDataHealth)
    {
        healthBar.value = playerDataHealth;
    }
}

[Serializable]
public class LobbyUI : SimpleUIController
{
    public enum ConnectionStatus
    {
        Disconnected,
        Connecting,
        Connected,
        WaitForGameStart
    }
    
    [SerializeField] private Button startGameButton;
    [SerializeField] private Button newGameButton;
    [SerializeField] private Button exitButton;

    [SerializeField] private Image statusIcon;
    [SerializeField] private TMP_Text statusText;
        
    [SerializeField] private RoomPanel roomPanel;
    [SerializeField] private UsersPanel usersPanel;
    
    [SerializeField] private TMP_Text Title;
    [SerializeField] private TMP_Text Timer;
    
    private Action<string> onSelectRoom;
    private ConnectionStatus status;
    public void Show(IEnumerable<string> rooms, ConnectionStatus status, Action<string> onSelectRoom,  Action onCreateRoom, Action onStartGame)
    { 
        base.Show();

        Timer.text = "";
        roomPanel.onSelectRoom = onSelectRoom;
        
        startGameButton.onClick.AddListener( () => onStartGame?.Invoke() );
        newGameButton.onClick.AddListener( () => onCreateRoom?.Invoke() );

        UpdateConnectionStatus(status);
        SetInLobby();
    }

    public void SetInRoom(string roomName, bool isOwner)
    {
        Title.text = roomName;
            
        startGameButton.interactable = isOwner && status != ConnectionStatus.WaitForGameStart;
        newGameButton.interactable = false;
        
        roomPanel.Hide();
        usersPanel.Show();
    }
    
    public void SetTimer(int time)
    {
        Timer.text = time > 0 ? time.ToString() : "";
            
        startGameButton.interactable = false;
    }
    
    public void SetInLobby()
    {
        startGameButton.interactable = false;
        newGameButton.interactable = true;
        
        roomPanel.Show();
        usersPanel.Hide();
    }

    public override void Hide()
    {
        base.Hide();
        
        roomPanel.Clean();
        usersPanel.Clean();
    }

    //public event Action<string> OnGameStarted;
    public void UpdateRooms(GameData[] games)
    {
        roomPanel.Clean();
        
        foreach (var game in games)
        {
            roomPanel.Add(game.id, $"{game.name} ({game.users.Length})");
        }
    }
    
    public void UpdateUsers(IEnumerable<string> users)
    {
        usersPanel.Clean();
        
        foreach (var user in users)
        {
            usersPanel.Add(user);
        }
    }

    public void UpdateConnectionStatus(ConnectionStatus connecting)
    {
        status = connecting;
        statusText.text = connecting.ToString();
        
        if(connecting == ConnectionStatus.Disconnected || 
           connecting == ConnectionStatus.Connecting )
            statusIcon.color = Color.red;
        else statusIcon.color = Color.green;
    }
}

[Serializable]
public class SimpleUIPanel : IGameObjectActivator
{
    [SerializeField] protected GameObject root;
    [SerializeField] protected GameObject elementPrefab;
    private IGameObjectActivator _gameObjectActivator => new MainGOActivator(root);
    public virtual void Show() => _gameObjectActivator.Show();
    public virtual void Hide() => _gameObjectActivator.Hide();
    
    public void Clean()
    {
        foreach (Transform obj in root.transform)
        {
            if(obj.gameObject.activeSelf)
                Object.Destroy(obj.gameObject);
        }
    }
}

[Serializable]
public class RoomPanel : SimpleUIPanel
{
  
    [SerializeField] private ToggleGroup toggleGroup;
    public Action<string> onSelectRoom;
    public string CurrentSkillId { get; private set; }
    public void Add(string id, string roomName)
    {
        var newInstance = Object.Instantiate(elementPrefab, root.transform, false);
        newInstance.GetComponentInChildren<TMP_Text>().text = roomName;
        newInstance.GetComponentInChildren<Button>().onClick.AddListener(() => onSelectRoom?.Invoke(id));
        newInstance.SetActive(true);
    }
}

[Serializable]
public class UsersPanel : SimpleUIPanel
{
    public void Add(string userName)
    {
        var newInstance = Object.Instantiate(elementPrefab, root.transform, false);
        newInstance.GetComponentInChildren<TMP_Text>().text = userName;
        newInstance.SetActive(true);
        //newInstance.SetName(characterId);
    }
}