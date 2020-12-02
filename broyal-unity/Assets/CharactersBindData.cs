using System;
using Scripts.Common.Data;
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

        public void SetSkinColor( SkinPart skinPart, uint skinType, Color? color1, Color? color2 )
        {
            if ( skinPart == SkinPart.Head && color1 == null ) return;
            if ( skinPart != SkinPart.Head && color1 == null && color2 == null ) return;

            GameObject[] partObjs = skinPart == SkinPart.Body ? bodies : ( skinPart == SkinPart.Head ? heads : pants );

            GameObject skinTypeObj = partObjs[ skinType ];
            SkinnedMeshRenderer meshRenderer = skinTypeObj.GetComponent<SkinnedMeshRenderer>();
            if ( meshRenderer == null ) return;

            if ( skinPart == SkinPart.Head )
            {
                meshRenderer.material.SetColor( "Color_01", color1.Value );
            }
            else
            {
                meshRenderer.material.SetColor( "Color_02", color1.Value );
                meshRenderer.material.SetColor( "Color_03", color2.Value );
            }
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

   public void SetSkinData(CurrentSkinData skinData)
   {
      SetSkinPart( SkinPart.Head, skinData.HeadIndex, skinData.HeadColor );
      SetSkinPart( SkinPart.Body, skinData.BodyIndex, skinData.Costume1Color, skinData.Costume2Color );
      SetSkinPart( SkinPart.Pants, skinData.PantsIndex, skinData.Costume1Color, skinData.Costume2Color );
   }

    public void SetSkinPart( SkinPart skinPart, uint skinType, Color? color1 = null, Color? color2 = null )
    {
        character.SetSkinPart( skinPart, skinType );
        character.SetSkinColor( skinPart, skinType, color1, color2 );
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
      _matBlock.SetFloat(Fill, value);
      renderer.SetPropertyBlock(_matBlock);
   }
}


