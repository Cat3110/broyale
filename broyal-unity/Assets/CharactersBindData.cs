using System;
using Scripts.Common.Tools.UI;
using UnityEngine;

public class CharactersBindData : MonoBehaviour
{
   public enum HealthBarType
   {
      None,
      Friendly,
      Enemy
   }

    public enum SkinPart
    {
        Head,
        Body,
        Pants
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

        public void SetSkinPartColor( SkinPart skinPart, uint skinType, Color color )
        {
            GameObject[] partObjs = skinPart == SkinPart.Body ? bodies : ( skinPart == SkinPart.Head ? heads : pants );

            GameObject skinTypeObj = partObjs[ skinType ];
            SkinnedMeshRenderer meshRenderer = skinTypeObj.GetComponent<SkinnedMeshRenderer>();
            if ( meshRenderer == null ) return;

            string shaderColorName = "Color_01";
            CharacterPartChangedColor changedColorComp = skinTypeObj.GetComponent<CharacterPartChangedColor>();
            if ( changedColorComp != null )
            {
                shaderColorName = changedColorComp.ColorNameInShader;
            }
            meshRenderer.material.SetColor( shaderColorName, color );
        }

        public void SetSkinPart( SkinPart skinPart, uint skinType )
        {
            GameObject[] partObjs = skinPart == SkinPart.Body ? bodies : ( skinPart == SkinPart.Head ? heads : pants );

            for ( int i = 0; i < partObjs.Length; i++ )
            {
                partObjs[ i ].SetActive( i == skinType );
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

    public void SetSkinPart( SkinPart skinPart, uint skinType )
    {
        character.SetSkinPart( skinPart, skinType );
    }

    public void SetSkinPart( SkinPart skinPart, uint skinType, Color color )
    {
        character.SetSkinPart( skinPart, skinType );
        character.SetSkinPartColor( skinPart, skinType, color );

    }

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


