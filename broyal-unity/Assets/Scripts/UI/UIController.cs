using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

public class UIController : MonoBehaviour
{
    [SerializeField] private MainUI main;
    [SerializeField] private GameUI game;
    [SerializeField] private LoadingUI loading;
    
    public MainUI MainUI => main;
    public LoadingUI LoadingUI => loading;
    public GameUI GameUI => game;
    
    
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

    [SerializeField] private CharactersPanel charactersPanel;
    
    [SerializeField] private SkillsPanelData skillsPanel;
    
    public void Show(IEnumerable<string> characterIds, IEnumerable<string> skillIds)
    { 
        base.Show();
        
        startButton.onClick.AddListener( () => OnGameStarted?.Invoke(skillsPanel.CurrentSkillId) );
        
        
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

    public event Action<string> OnGameStarted;
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