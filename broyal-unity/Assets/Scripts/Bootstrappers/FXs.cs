using System;
using System.Collections;
using UnityEngine;

[Serializable]
public class FXData
{
    public GameObject FireBall;
    public GameObject MagicRay;
    public GameObject DamageZone;

    public float FxSpeed = 10.0f; 
    public float FxTime= 1.0f;

    public float FxOffset = 0.5f;
    public float FxScale = 2.5f;
    private static readonly int Radius = Shader.PropertyToID("_Radius");

    public void Start(int playerPrimarySkillId, GameObject root, Vector3 position, Vector3 direction)
    {
        if (playerPrimarySkillId == 2)
        {
            UniRx.MainThreadDispatcher.StartCoroutine(FxStartFireBall(FireBall, position, 1.0f, direction));
        }
        else if (playerPrimarySkillId == 3)
        {
            UniRx.MainThreadDispatcher.StartCoroutine(FxStartRay(MagicRay, root, position, 0.6f, direction));
        }
    }

    IEnumerator FxStartFireBall(GameObject prefab, Vector3 origPosition, float duration, Vector3 direction)
    {
        origPosition.y += FxOffset;
        
        var obj = GameObject.Instantiate(prefab, origPosition, Quaternion.identity );
        obj.transform.up = -direction;
        
        while (duration > 0.0f)
        {
            duration -= Time.deltaTime;
            var position = obj.transform.position;
            position = Vector3.Lerp(position, position + direction, Time.deltaTime * 10.0f);
            obj.transform.position = position;
            yield return null;
        }
        GameObject.Destroy(obj);
        yield return null;
    }
    
    IEnumerator FxStartRay(GameObject prefab, GameObject root, Vector3 origPosition, float duration, Vector3 direction)
    {
        origPosition.y += FxOffset;

        var obj = GameObject.Instantiate(prefab, origPosition, Quaternion.identity );
        obj.transform.up = -direction;
        var scaleorig = new Vector3(obj.transform.localScale.x, 0.2f,obj.transform.localScale.y);
        var scalenew  =  new Vector3(obj.transform.localScale.x, 1.0f,obj.transform.localScale.y);
        var time = duration;
        
        while (duration > 0.0f)
        {
            duration -= Time.deltaTime;
            
            var scale = obj.transform.localScale;
            scale = Vector3.Lerp(scaleorig, scalenew, (time - duration) / time );
            obj.transform.localScale = scale;
            obj.transform.up = root.transform.forward;
            obj.transform.position = new Vector3(root.transform.position.x, origPosition.y, root.transform.position.z  );
            
            yield return null;
        }
        
        GameObject.Destroy(obj);
        yield return null;
    }

    public void SetDeadZoneRadius(int radius)
    {
        DamageZone.GetComponent<MeshRenderer>().material.SetFloat(Radius, radius );
    }
}
