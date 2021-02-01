using System;
using System.Collections;
using System.Collections.Generic;
using Bootstrappers;
using RemoteConfig;
using Scripts.Scenes.Client.UI;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.InputSystem.OnScreen;
using UnityEngine.UI;
using Utils;
using CharacterInfo = RemoteConfig.CharacterInfo;
using Object = UnityEngine.Object;

public class UIController : MonoBehaviour, IUIOwner
{
    [SerializeField] private MainUI main;
    [SerializeField] private GameUI game;
    [SerializeField] private LoadingUI loading;
    [SerializeField] private GameOverUI gameOver;

    public static Vector2 AttackDirection { get; private set; }

    public MainUI MainUI => main;
    public LoadingUI LoadingUI => loading;
    public GameUI GameUI => game;
    public GameOverUI GameOver => gameOver;


    private Vector3 _playerPosition;
    private GameObject _playerGo;
    
    // Start is called before the first frame update
    public void Init(IContainer container)
    {
        //_inputMaster = ClientBootstrapper.Container.Resolve<InputMaster>();
        Container = container;
            
        MainUI.SetOwner(this);
        GameUI.SetOwner(this);
        
        GameUI.OnMainButtonClickedAndDirectionSet += MainActionOnStarted;
        GameUI.OnFirstActionButtonClickedAndDirectionSet += FirstActionOnStarted;
    }
    
    private const string MainKeyButton = "space";
    private const string FirstKeyButton = "q";

    private readonly Dictionary<string, Coroutine> _cleanCoroutines = new Dictionary<string, Coroutine>();

    private void MainActionOnStarted(Vector2 direction)
    {
        QueueEvent(MainKeyButton);
        AttackDirection = direction;
    }
    private void FirstActionOnStarted(Vector2 direction)          
    {                                                            
        QueueEvent(FirstKeyButton);                               
        AttackDirection = direction;       
    } 
    
    //TODO:Have to find way out
    private void QueueEvent(string key)
    {
        var device = Keyboard.current;                                                               
        using (StateEvent.From(device, out var eventPtr))                                            
        {                                                                                            
            ((ButtonControl) device[key]).WriteValueIntoEvent(1.0f, eventPtr);             
            InputSystem.QueueEvent(eventPtr);                                                        
        }

        if (_cleanCoroutines.TryGetValue(key,out var coroutine))
        {
            StopCoroutine(coroutine);
        }
        
        _cleanCoroutines[key] = StartCoroutine(CleanEvent(key));
    }
    IEnumerator CleanEvent(string action)
    {
        yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfFrame();
        
        var device = Keyboard.current;
        using (StateEvent.From(device, out var eventPtr))
        {
            ((ButtonControl) device[action]).WriteValueIntoEvent(0.0f, eventPtr);
            InputSystem.QueueEvent(eventPtr);
        }
    }

    public void SetPlayerPosition(Vector3 position) => _playerPosition = position;
    public IContainer Container { get; private set; }
    public Vector3 GetPlayerPosition() => _playerPosition;
    public GameObject GetPlayerGo() => _playerGo;

    public void SetPlayerGo(GameObject go) => _playerGo = go;
}

public interface IUIOwner
{
    IContainer Container { get; }
    
    Vector3 GetPlayerPosition();

    void SetPlayerPosition(Vector3 position);
    
    void SetPlayerGo(GameObject go);
    GameObject GetPlayerGo();
}

public interface IHaveUIOwner
{
    IUIOwner Owner { get; }
    void SetOwner(IUIOwner owner);
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

   
public class SimpleUIController : IGameObjectActivator, IHaveUIOwner
{
    [SerializeField] protected GameObject body;
    private IGameObjectActivator _gameObjectActivator => new MainGOActivator(body);
    public virtual void Show() => _gameObjectActivator.Show();
    public virtual void Hide() => _gameObjectActivator.Hide();
    
    public IUIOwner Owner { get; private set; }
    public void SetOwner(IUIOwner owner) => Owner = owner;
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
    
    [SerializeField] private SkillsPanelData skillPanel2;
    
    [SerializeField] private OffScreenController offScreenController;

    private IContainer _container;

    public void Show( CharactersConfig characters, IList<string> primarySkills, IList<string> attackSkills )
    { 
        base.Show();
        
        offScreenController.gameObject.SetActive(true);
        
        startButton.onClick.RemoveAllListeners();
        startButton.onClick.AddListener( () => 
            OnGameStarted?.Invoke(skillsPanel.CurrentSkillId, skillPanel2.CurrentSkillId, characters[offScreenController.SelectedIndex]) );
        
        nextCharacterButton.onClick.RemoveAllListeners();
        nextCharacterButton.onClick.AddListener( () => offScreenController.Next() );
        
        prevCharacterButton.onClick.RemoveAllListeners();
        prevCharacterButton.onClick.AddListener( () => offScreenController.Prev() );

        foreach (var character in characters)
        {
            charactersPanel.Add(character.Id);
            offScreenController.AddCharacter(character);
        }
            
        foreach (var skillId in primarySkills)
        {
            //var sprite = contentFactory.GetSpriteById(skillId);
            skillsPanel.Add(skillId, Sprite.Create(new Texture2D(1, 1), new Rect(0, 0, 1, 1), Vector2.one * 0.5f));
        }

        skillsPanel.SetOn(0);
        
        foreach (var skillId in attackSkills)
        {
            //var sprite = contentFactory.GetSpriteById(skillId);
            skillPanel2.Add(skillId, Sprite.Create( new Texture2D(1,1), new Rect(0,0,1,1),Vector2.one*0.5f));
        }
        
        skillPanel2.SetOn(0);
    }

    public override void Hide()
    {
        base.Hide();
        
        offScreenController.gameObject.SetActive(false);
        
        charactersPanel.Clean();
        skillsPanel.Clean();
    }

    public event Action<string, string, CharacterInfo> OnGameStarted;
    
    public void SetOwner(IUIOwner owner)
    { 
       base.SetOwner(owner);

       offScreenController.SetOwner(owner);
    }
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

    public void Add(string skillId, Sprite sprite)
    {
        var newInstance = Object.Instantiate(skillPrefab, root.transform, false);
        
        newInstance.SetName(skillId);
        newInstance.SetGroup(toggleGroup);
        newInstance.SetIcon(sprite);
        newInstance.OnChangeState = value => CurrentSkillId = skillId;
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

    public void Add(string name)
    {
        var newInstance = Object.Instantiate(characterSelectData, root.transform, false);
        newInstance.SetName(name);
    }
}
    
[Serializable]
public class LoadingUI : SimpleUIController
{
    [SerializeField] private TMP_Text exitButton;
}
[Serializable]
public class GameOverUI : SimpleUIController
{
    [SerializeField] private Button exitButton;
    [SerializeField] private TextMeshProUGUI placeValue;
    [SerializeField] private TextMeshProUGUI skillRewardValue;
    [SerializeField] private TextMeshProUGUI skillCoinsValue;

    public void Setup( RewardData reward )
    {
        placeValue.text = "#" + reward.Place.ToString();
        skillRewardValue.text = reward.SkillReward.ToString();
        skillCoinsValue.text = reward.CoinsReward.ToString();
    }

    public void Show(Action onExit)
    {
        base.Show();
        
        exitButton.onClick.RemoveAllListeners();
        exitButton.onClick.AddListener( () =>
        {
            base.Hide();
            onExit?.Invoke();
        });
    }
}

[Serializable]
public class GameUI : SimpleUIController
{
    [SerializeField] private SkillButtonWithPopupStick action1Button;
    [SerializeField] private SkillButtonWithPopupStick action2Button;
    [SerializeField] private SkillButtonWithPopupStick action3Button;

    [SerializeField] private Slider healthBar;
       
    [SerializeField] private Button exitButton;
    [SerializeField] private SkillButtonWithPopupStick mainButton;
    
    [SerializeField] private MobileInputController rightStickInputController;
    [SerializeField] private GameObject attackDirectionArrow;
    [SerializeField] private GameObject attackZone;

    [SerializeField] private GemsPanel gemsPanel;
    
    [SerializeField] private Vector3 rot;

    [SerializeField] private GameObject[] bottomBlocks;

    public event Action<Vector2> OnMainButtonClickedAndDirectionSet;
    public event Action<Vector2> OnFirstActionButtonClickedAndDirectionSet;

    private Action<Vector2> mainActionAccept;
    private Action<Vector2> mainActionDragging;
    
    private Action<Vector2> attackActionAccept;
    private Action<Vector2> attackActionDragging;

    public void Show(SkillInfo mainSkill, SkillInfo attackSkill)
    {
        var isDirectionSkill = (mainSkill.AimType == AimType.Direction) 
                               || (mainSkill.AimType == AimType.Trajectory) 
                               || (mainSkill.AimType == AimType.Sector);
        
        var isAreaSkill = mainSkill.AimType == AimType.Area || mainSkill.AimType == AimType.Dot;
        
        if (isDirectionSkill)
        {
            mainActionAccept = MainButtonOnAcceptActionDirection;
            mainActionDragging = OnDraggingDirection;
        }
        else if (isAreaSkill)
        {
            mainActionAccept = MainButtonOnAcceptActionZone;
            mainActionDragging =  (d) => OnDraggingZone(mainSkill, d);
        }
        
        mainButton.OnAcceptAction += mainActionAccept;
        mainButton.OnDragging += mainActionDragging;
        
        isDirectionSkill = (attackSkill.AimType == AimType.Direction) 
                               || (attackSkill.AimType == AimType.Trajectory) 
                               || (attackSkill.AimType == AimType.Sector);
        
        isAreaSkill = attackSkill.AimType == AimType.Area || attackSkill.AimType == AimType.Dot;
        
        if (isDirectionSkill)
        {
            attackActionAccept = Action1ButtonOnAcceptActionDirection;
            attackActionDragging = OnDraggingDirection;
        }
        else if (isAreaSkill)
        {
            attackActionAccept = Action1ButtonOnAcceptActionZone;
            attackActionDragging = (d) => OnDraggingZone(attackSkill, d);
        }
        
        action1Button.OnAcceptAction += attackActionAccept;
        action1Button.OnDragging += attackActionDragging;
        
        //TODO:Need make some skill class for ui
        //mainButton.transform.Find("Icon").GetComponent<Image>().sprite = contentFactory.GetSpriteById(skillId);
        
        base.Show();
    }
    
    public void HideBottom()
    {
        foreach ( var b in bottomBlocks )
        {
            b.SetActive( false );
        }
    }

    private void OnDraggingDirection(Vector2 direction)
    {
        attackDirectionArrow.SetActive(true);
        attackDirectionArrow.transform.SetParent(Owner.GetPlayerGo().transform,true);
        
        var center = Owner.GetPlayerPosition();
        
        direction = direction.normalized;

        attackDirectionArrow.transform.up = direction;
        attackDirectionArrow.transform.rotation = Quaternion.Euler(new Vector3(90.0f,-90.0f, attackDirectionArrow.transform.rotation.eulerAngles.z));
                
        //attackDirectionArrow.transform.position = new Vector3(center.x, 1.0f, center.z);
        attackDirectionArrow.transform.localPosition =  new Vector3(0,1.0f, 0);
    }

    private void OnDraggingZone(SkillInfo skill, Vector2 direction)
    {
        attackZone.SetActive(true);
        
        var center = Owner.GetPlayerPosition();
        direction *= skill.Range;
        
        //Debug.Log($"OnDraggingZone:direction {direction}");

        attackZone.transform.position = center + new Vector3(direction.x, 1.0f, direction.y);
    }

    private void MainButtonOnAcceptActionDirection(Vector2 direction)
    {
        attackDirectionArrow.SetActive(false);
        OnMainButtonClickedAndDirectionSet?.Invoke(direction);
    }
    
    private void MainButtonOnAcceptActionZone(Vector2 direction)
    {
        attackZone.SetActive(false);
        OnMainButtonClickedAndDirectionSet?.Invoke(direction);
    }
    
    private void Action1ButtonOnAcceptActionDirection(Vector2 direction)               
    {                                                                        
        attackDirectionArrow.SetActive(false);                               
        OnFirstActionButtonClickedAndDirectionSet?.Invoke(direction);               
    }   
    
    private void Action1ButtonOnAcceptActionZone(Vector2 direction)               
    {                                                                        
        attackZone.SetActive(false);                               
        OnFirstActionButtonClickedAndDirectionSet?.Invoke(direction);               
    }   

    public override void Hide()
    {
        mainButton.OnAcceptAction -= mainActionAccept;
        mainButton.OnDragging -= mainActionDragging;

        action1Button.OnAcceptAction -= attackActionAccept;
        action1Button.OnDragging -= attackActionDragging; 
        
        base.Hide();
    }

    // private void ActiveRightJoystick()
    // {
    //     if (!_needSetDirection)
    //     {
    //         OnMainButtonClickedAndDirectionSet?.Invoke(Vector2.zero);
    //         return;
    //     }
    //         
    //     rightStickInputController.gameObject.SetActive(true);
    //     
    //     mainButton.OnPointerUp(new PointerEventData(EventSystem.current){ button = PointerEventData.InputButton.Left } );
    //     //mainButton.GetComponent<OnScreenButton>().enabled = false;
    //     
    //     // mainButton.OnPointerExit(new PointerEventData(EventSystem.current));
    //     //mainButton.OnDeselect(new BaseEventData(EventSystem.current));
    //     //mainButton.OnSubmit(new BaseEventData(EventSystem.current));
    //     //EventSystem.current.SetSelectedGameObject(rightStickInputController.gameObject);
    //
    //     // rightStickInputController.OnBeginDrag(new PointerEventData(EventSystem.current)
    //     // {
    //     //     button = PointerEventData.InputButton.Left,
    //     //     //position = Input.touches.Last().position,
    //     //     dragging = true
    //     // });
    //     attackDirectionArrow.SetActive(true);
    //     
    //     Observable.EveryLateUpdate()
    //         .TakeUntilDisable(rightStickInputController.gameObject)
    //         .Subscribe(x =>
    //         {
    //             //var center = Owner.GetPlayerPosition();
    //             var direction = rightStickInputController.Coordinate().normalized;
    //             
    //             // float angle = Vector3.Angle(new Vector3(0.0f, 1.0f, 0.0f), new Vector3(direction.x, direction.y, 0.0f));
    //             // if (x < 0.0f) {
    //             //     angle = -angle;
    //             //     angle = angle + 360;
    //             // }
    //             
    //             attackDirectionArrow.transform.up = direction;
    //             attackDirectionArrow.transform.rotation = Quaternion.Euler(new Vector3(90.0f,-90.0f, attackDirectionArrow.transform.rotation.eulerAngles.z));
    //             
    //             //attackDirectionArrow.transform.position = new Vector3(center.x,1.0f, center.z);
    //         });
    //     rightStickInputController.OnStopDrag += OnStopDrag;
    // }
    //
    // private void OnStopDrag(Vector2 obj)
    // {
    //     attackDirectionArrow.SetActive(false);
    //     //mainButton.GetComponent<OnScreenButton>().enabled
    //         
    //     rightStickInputController.gameObject.SetActive(false);
    //     rightStickInputController.OnStopDrag -= OnStopDrag;
    //     
    //     OnMainButtonClickedAndDirectionSet?.Invoke(obj);
    // }

    public void SetHealth(int playerDataHealth) => healthBar.value = playerDataHealth;

    public void SetGems(List<(PlayerDataExt.CrystalPlace, uint)> gems)
    {
        gemsPanel.UpdateData(gems);
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


[Serializable]
public class GemsPanel : SimpleUIPanel
{
    [SerializeField] protected GameObject[] redGems;
    [SerializeField] protected GameObject[] blueGems;
    [SerializeField] protected GameObject[] greenGems;

    private int _currentCount = 0;
    public void UpdateData(List<(PlayerDataExt.CrystalPlace, uint)> gems)
    {
        if (_currentCount == gems.Count) return;
            
        foreach (var gem in gems)
        {
            if (gem.Item1 == PlayerDataExt.CrystalPlace.Red)
            {
                for (int i = 0; i < gem.Item2; i++)
                {
                    redGems[i].transform.GetChild(0).gameObject.SetActive(true);
                    _currentCount++;
                }
            }
            else if (gem.Item1 == PlayerDataExt.CrystalPlace.Blue)
            {
                for (int i = 0; i < gem.Item2; i++)
                {
                    blueGems[i].transform.GetChild(0).gameObject.SetActive(true);
                    _currentCount++;
                }
            }
            else if (gem.Item1 == PlayerDataExt.CrystalPlace.Green)
            {
                for (int i = 0; i < gem.Item2; i++)
                {
                    greenGems[i].transform.GetChild(0).gameObject.SetActive(true);
                    _currentCount++;
                }
            }
        }
    }
}