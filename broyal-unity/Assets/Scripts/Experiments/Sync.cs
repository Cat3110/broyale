using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;


[RequiresEntityConversion]
public class Sync : MonoBehaviour, IConvertGameObjectToEntity {
    [SerializeField]
    private Camera _camera = default;
    
    [SerializeField]
    private GameObject _companion = default;
    
    [SerializeField]
    private GameObject _companion2 = default;

    [SerializeField]
    private Animator _animator = default;

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem){
        //conversionSystem.AddHybridComponent(_companion);
        //conversionSystem.AddHybridComponent(_companion2);
        //conversionSystem.AddHybridComponent(_animator);
        //conversionSystem.AddHybridComponent(GetComponent<MeshFilter>());
        
        dstManager.AddComponentData(entity, new CopyTransformToGameObject());
    }
}
