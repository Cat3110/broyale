using System;
using UnityEngine;
using UnityEngine.UI;

public class SkillData : MonoBehaviour
{
    [SerializeField] private Toggle toggle;
    [SerializeField] private TMPro.TMP_Text label;
    [SerializeField] private Image icon;

    public Action<bool> OnChangeState;

    private void OnEnable()
    {
        toggle.onValueChanged.AddListener( state => OnChangeState?.Invoke(state) );
    }

    private void OnDisable()
    {
        toggle.onValueChanged.RemoveAllListeners();
    }

    public void SetName(string name) => label.text = name;
    public void SetGroup(ToggleGroup group) => toggle.group = group;
    public void SetIcon(Sprite sprite) => icon.sprite = sprite;
}
