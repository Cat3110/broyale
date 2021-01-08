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
    
    public static Vector2 AttackDirection;
    
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
        GameUI.OnButtonClickedAndDirectionSet += MainActionOnStarted;
        //GameUI.NeedSetDirection = true;
    }

    private Coroutine _cleanCoroutine = null;

    private void MainActionOnStarted(Vector2 direction)
    {
        var device = Keyboard.current;
        using (StateEvent.From(device, out var eventPtr))
        {
            ((ButtonControl) device["space"]).WriteValueIntoEvent(1.0f, eventPtr);
            InputSystem.QueueEvent(eventPtr);
        }

        AttackDirection = direction.normalized;
        
        /*var leftStick = Gamepad.current;
        using (StateEvent.From(leftStick, out var eventPtr))
        {
            ((ButtonControl) leftStick["rightStick"]).WriteValueIntoEvent(direction, eventPtr);
            InputSystem.QueueEvent(eventPtr);
        }*/

        if (_cleanCoroutine != null)
        {
            StopCoroutine(_cleanCoroutine);
        }
        
        _cleanCoroutine = StartCoroutine(CleanEvent());

        /*using (StateEvent.From(device, out var eventPtr))
        {
            ((ButtonControl) device["space"]).WriteValueIntoEvent(0.0f, eventPtr);
            InputSystem.QueueEvent(eventPtr);
        }*/

        //InputSystem.QueueStateEvent(Keyboard.current, new KeyboardState());
        //GameUI.CallAction();
        //Keyboard.current.spaceKey.ProcessValue(1.0f);
        //Keyboard.current.spaceKey.w
        //_inputMaster.Player.MainAction.f
    }

    //TODO:Have to find way out
    IEnumerator CleanEvent()
    {
        yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfFrame();
        
        var device = Keyboard.current;
        using (StateEvent.From(device, out var eventPtr))
        {
            ((ButtonControl) device["space"]).WriteValueIntoEvent(0.0f, eventPtr);
            InputSystem.QueueEvent(eventPtr);
        }
    }
    

    // Update is called once per frame
    // void Update()
    // {
    // }

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
    
    [SerializeField] private OffScreenController offScreenController;

    private IContainer _container;

    public void Show( CharactersConfig characters, IList<string> skillIds)
    { 
        base.Show();
        
        offScreenController.gameObject.SetActive(true);
        
        startButton.onClick.RemoveAllListeners();
        startButton.onClick.AddListener( () => OnGameStarted?.Invoke(skillsPanel.CurrentSkillId, characters[offScreenController.SelectedIndex]) );
        
        nextCharacterButton.onClick.RemoveAllListeners();
        nextCharacterButton.onClick.AddListener( () => offScreenController.Next() );
        
        prevCharacterButton.onClick.RemoveAllListeners();
        prevCharacterButton.onClick.AddListener( () => offScreenController.Prev() );

        foreach (var character in characters)
        {
            charactersPanel.Add(character.Id);
            offScreenController.AddCharacter(character);
        }
            
        foreach (var skillId in skillIds)
        {
            //var sprite = contentFactory.GetSpriteById(skillId);
            skillsPanel.Add(skillId, Sprite.Create( new Texture2D(1,1), new Rect(0,0,1,1),Vector2.one*0.5f));
        }

        skillsPanel.SetOn(0);
    }

    public override void Hide()
    {
        base.Hide();
        
        offScreenController.gameObject.SetActive(false);
        
        charactersPanel.Clean();
        skillsPanel.Clean();
    }

    public event Action<string, CharacterInfo> OnGameStarted;
    
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
    [SerializeField] private Button action1Button;
    [SerializeField] private Button action2Button;
    [SerializeField] private Button action3Button;

    [SerializeField] private Slider healthBar;
       
    [SerializeField] private Button exitButton;
    [SerializeField] private SkillButtonWithPopupStick mainButton;
    
    [SerializeField] private MobileInputController rightStickInputController;
    [SerializeField] private GameObject attackDirectionArrow;

    [SerializeField] private GemsPanel gemsPanel;

    private Session _session;
    public event Action<Vector2> OnButtonClickedAndDirectionSet;
    public bool NeedSetDirection { get; set; }
    
    public void Show(string skillId)
    {
        mainButton.OnAcceptAction += MainButtonOnOnAcceptAction;
        mainButton.OnDraggin += MainButtonOnOnDraggin;
            
        _session = ClientBootstrapper.Container.Resolve<Session>();
        
        //TODO: need make some flag in table
        NeedSetDirection = _session.SkillId > 1;
        
        //TODO:Need make some skill class for ui
        //mainButton.transform.Find("Icon").GetComponent<Image>().sprite = contentFactory.GetSpriteById(skillId);
        
        base.Show();
    }

    private void MainButtonOnOnDraggin(Vector2 direction)
    {
        attackDirectionArrow.SetActive(true);
        attackDirectionArrow.transform.SetParent(Owner.GetPlayerGo().transform,true);
        
        var center = Owner.GetPlayerPosition();
        
        direction = direction.normalized;
                
        // float angle = Vector3.Angle(new Vector3(0.0f, 1.0f, 0.0f), new Vector3(direction.x, direction.y, 0.0f));
        // if (x < 0.0f) {
        //     angle = -angle;
        //     angle = angle + 360;
        // }
                
        attackDirectionArrow.transform.up = direction;
        attackDirectionArrow.transform.rotation = Quaternion.Euler(new Vector3(90.0f,-90.0f, attackDirectionArrow.transform.rotation.eulerAngles.z));
                
        //attackDirectionArrow.transform.position = new Vector3(center.x,1.0f, center.z);
        attackDirectionArrow.transform.localPosition =  new Vector3(0,1.0f, 0);
    }

    private void MainButtonOnOnAcceptAction(Vector2 obj)
    {
        attackDirectionArrow.SetActive(false);
        OnButtonClickedAndDirectionSet?.Invoke(obj);
    }

    public override void Hide()
    {
        mainButton.OnAcceptAction -= MainButtonOnOnAcceptAction;
        mainButton.OnDraggin -= MainButtonOnOnDraggin;
        
        base.Hide();
    }

    private void ActiveRightJoystick()
    {
        if (!NeedSetDirection)
        {
            OnButtonClickedAndDirectionSet?.Invoke(Vector2.zero);
            return;
        }
            
        rightStickInputController.gameObject.SetActive(true);
        
        mainButton.OnPointerUp(new PointerEventData(EventSystem.current){ button = PointerEventData.InputButton.Left } );
        //mainButton.GetComponent<OnScreenButton>().enabled = false;
        
        // mainButton.OnPointerExit(new PointerEventData(EventSystem.current));
        //mainButton.OnDeselect(new BaseEventData(EventSystem.current));
        //mainButton.OnSubmit(new BaseEventData(EventSystem.current));
        //EventSystem.current.SetSelectedGameObject(rightStickInputController.gameObject);

        // rightStickInputController.OnBeginDrag(new PointerEventData(EventSystem.current)
        // {
        //     button = PointerEventData.InputButton.Left,
        //     //position = Input.touches.Last().position,
        //     dragging = true
        // });
        attackDirectionArrow.SetActive(true);
        
        Observable.EveryLateUpdate()
            .TakeUntilDisable(rightStickInputController.gameObject)
            .Subscribe(x =>
            {
                //var center = Owner.GetPlayerPosition();
                var direction = rightStickInputController.Coordinate().normalized;
                
                // float angle = Vector3.Angle(new Vector3(0.0f, 1.0f, 0.0f), new Vector3(direction.x, direction.y, 0.0f));
                // if (x < 0.0f) {
                //     angle = -angle;
                //     angle = angle + 360;
                // }
                
                attackDirectionArrow.transform.up = direction;
                attackDirectionArrow.transform.rotation = Quaternion.Euler(new Vector3(90.0f,-90.0f, attackDirectionArrow.transform.rotation.eulerAngles.z));
                
                //attackDirectionArrow.transform.position = new Vector3(center.x,1.0f, center.z);
            });
        rightStickInputController.OnStopDrag += OnStopDrag;
    }

    private void OnStopDrag(Vector2 obj)
    {
        attackDirectionArrow.SetActive(false);
        //mainButton.GetComponent<OnScreenButton>().enabled
            
        rightStickInputController.gameObject.SetActive(false);
        rightStickInputController.OnStopDrag -= OnStopDrag;
        
        OnButtonClickedAndDirectionSet?.Invoke(obj);
    }

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