using UnityEngine;

public class SpineBindData : MonoBehaviour
{
   [SerializeField] private Transform rightHandPivot;

   public void AttachWeapon(Transform weapon)
   {
      weapon.SetParent(rightHandPivot, false);
   }
}
