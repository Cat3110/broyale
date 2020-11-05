using System;
using UnityEngine;

public class CharactersBindData : MonoBehaviour
{
   public enum HealthBarType
   {
      None,
      Friendly,
      Enemy
   }
   
   [Serializable]
   public class BindData
   {
      [SerializeField] private Transform root;
   
      [SerializeField] private GameObject[] heads;
      [SerializeField] private GameObject[] bodies;
      [SerializeField] private GameObject[] pants;
   
      [SerializeField] private Transform rightHandPivot;
      [SerializeField] private Animator animator;

      public Animator Animator => animator;
      public GameObject Weapon { get; private set; }
      
      public void AttachWeapon(GameObject weapon)
      {
         Weapon = weapon;
         weapon.transform.SetParent(rightHandPivot, false);
      }
      
      public void SetSkinType(uint skinType)
      {
         for (int i = 0; i < heads.Length; i++)
         {
            bodies[i].SetActive( i == skinType);
            heads[i].SetActive( i == skinType);
            pants[i].SetActive( i == skinType);
         }
      }
   }
   
   [SerializeField] private BindData character;
   [SerializeField] private GameObject healthBar;

   private MaterialPropertyBlock _matBlock;
   private static readonly int Fill = Shader.PropertyToID("_Fill");
   
   public Animator Animator => character.Animator;
   public GameObject Weapom => character.Weapon;

   private void Start()
   {
      _matBlock = new MaterialPropertyBlock();
   }

   public void AttachWeapon(GameObject weapon) => character.AttachWeapon(weapon);
   public void SetSkinType(uint skinType) => character.SetSkinType(skinType);

   public void SetHealthBarType(HealthBarType healthBarType)
   {
      switch (healthBarType)
      {
         case HealthBarType.None:
            healthBar.SetActive(false);
            break;
         case HealthBarType.Friendly:
            healthBar.SetActive(true);
            break;
         case HealthBarType.Enemy:
            healthBar.SetActive(true);
            break;
         default:
            throw new ArgumentOutOfRangeException(nameof(healthBarType), healthBarType, null);
      }
   }

   public void UpdateHealthBar(float value)
   {
      var renderer = healthBar.GetComponent<MeshRenderer>();
            
      renderer.GetPropertyBlock(_matBlock);
      //_matBlock.SetFloat(Fill, player.health / (float)player.maxHealth);
      _matBlock.SetFloat(Fill, value);
      renderer.SetPropertyBlock(_matBlock);
   }
}


