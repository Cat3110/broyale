using System;
using System.Collections;
using UnityEngine;

[Serializable]
public class FXData
{
    public GameObject FireBall;
    public GameObject MagicRay;
    public GameObject DamageZone;
    public GameObject AttackDirection;
    
    public float FxSpeed = 10.0f; 
    public float FxTime= 1.0f;

    public float FxOffset = 0.5f;
    public float FxScale = 2.5f;
    private static readonly int Radius = Shader.PropertyToID("_Radius");

    public void Start(int playerPrimarySkillId, GameObject root, Transform orig, Vector3 direction)
    {
        if (playerPrimarySkillId == 2)
        {
            UniRx.MainThreadDispatcher.StartCoroutine(FxStartFireBall(FireBall, orig, direction,1.0f,FxTime));
        }
        else if (playerPrimarySkillId == 3)
        {
            UniRx.MainThreadDispatcher.StartCoroutine(FxStartRay(MagicRay, root, orig, direction,0.6f,FxTime));
        }
    }

    IEnumerator FxStartFireBall(GameObject prefab, Transform orig, Vector3 direction, float duration, float delay)
    {
        yield return new WaitForSeconds(delay);

        var origPosition = orig.position;
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
    
    IEnumerator FxStartRay(GameObject prefab, GameObject root, Transform orig, Vector3 direction, float duration, float delay)
    {
        yield return new WaitForSeconds(delay);
        
        var origPosition = orig.position;
        origPosition.y += FxOffset;

        var obj = GameObject.Instantiate(prefab, origPosition, Quaternion.identity );
        obj.transform.up = -direction;
        
        var localScale = obj.transform.localScale;
        var scaleorig = new Vector3(localScale.x, 0.2f,localScale.y);
        var scalenew  =  new Vector3(localScale.x, 1.0f,localScale.y);
        
        var time = duration;
        
        while (duration > 0.0f)
        {
            duration -= Time.deltaTime;
            
            var scale = Vector3.Lerp(scaleorig, scalenew, (time - duration) / time );
            
            obj.transform.localScale = scale;
            obj.transform.up = root.transform.forward;
            
            var position = root.transform.position;
            obj.transform.position = new Vector3( position.x, origPosition.y, position.z );
            
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
