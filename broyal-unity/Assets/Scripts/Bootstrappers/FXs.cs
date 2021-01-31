using System;
using System.Collections;
using RemoteConfig;
using UnityEngine;

[Serializable]
public partial class FXData
{
    public GameObject FireBall;
    public GameObject MagicRay;
    public GameObject DamageZone;

    public GameObject Thunder;
    public GameObject MagicJump;
    public GameObject Poison;
    public GameObject TimeSlow;
    
    public GameObject FireWork;
    
    public float FxSpeed = 10.0f; 
    public float FxTime= 1.0f;

    public float FxOffset = 0.5f;
    public float FxScale = 2.5f;
    private static readonly int Radius = Shader.PropertyToID("_Radius");

    public void Start(SkillInfo skillInfo, GameObject root, Transform orig, Vector3 direction)
    {
        switch (@skillInfo.Id)
        {
            case SkillId.ID_Fireball:
                UniRx.MainThreadDispatcher.StartCoroutine(FxStartFireBall(FireBall, orig, direction,1.0f,FxTime));
                break;
            case SkillId.ID_Magicray:
                UniRx.MainThreadDispatcher.StartCoroutine(FxStartRay(MagicRay, root, orig, direction,0.6f,FxTime));
                break;
            case SkillId.ID_Thunder:
                UniRx.MainThreadDispatcher.StartCoroutine(FxStartDot(Thunder, root, orig,root.transform.position + (direction * skillInfo.Range),2.0f,FxTime));
                break;
            case SkillId.ID_Magicjump:
                UniRx.MainThreadDispatcher.StartCoroutine(FxStartDot(MagicJump, root, orig,root.transform.position,2.0f,FxTime));
                break;
            case SkillId.ID_Poison:
                UniRx.MainThreadDispatcher.StartCoroutine(FxStartDot(Poison, root, orig, root.transform.position + (direction * skillInfo.Range),2.0f,FxTime));
                break;
            case SkillId.ID_Timeslow:
                UniRx.MainThreadDispatcher.StartCoroutine(FxStartDot(TimeSlow, root, orig, root.transform.position + (direction * skillInfo.Range),2.0f,FxTime));
                break;
            case SkillId.ID_Fireworks:
                UniRx.MainThreadDispatcher.StartCoroutine(FxStartDot(FireWork, root, orig, root.transform.position + (direction * skillInfo.Range),2.0f,FxTime));
                break;
            default:
                Debug.LogWarning($"Dont have fx for {skillInfo.Id}");
                break;
        }
    }

    IEnumerator FxStartFireBall(GameObject prefab, Transform orig, Vector3 direction, float duration, float delay)
    {
        yield return new WaitForSeconds(delay);

        var origPosition = orig.position;
        origPosition.y += FxOffset;
        
        var obj = GameObject.Instantiate(prefab, origPosition, Quaternion.identity );
        obj.transform.forward = direction;
        
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
        obj.transform.forward = direction;
        
        var localScale = obj.transform.localScale;
        //var scaleorig = new Vector3(localScale.x, 0.2f,localScale.y);
        //var scalenew  =  new Vector3(localScale.x, 1.0f,localScale.y);
        
        var time = duration;
        
        while (duration > 0.0f)
        {
            duration -= Time.deltaTime;
            
            //var scale = Vector3.Lerp(scaleorig, scalenew, (time - duration) / time );
            
            //obj.transform.localScale = scale;
            //obj.transform.up = root.transform.forward;
            
            var position = root.transform.position;
            obj.transform.position = new Vector3( position.x, origPosition.y, position.z );
            
            yield return null;
        }
        
        GameObject.Destroy(obj);
        yield return null;
    }
    
    IEnumerator FxStartDot(GameObject prefab, GameObject root, Transform orig, Vector3 target, float duration, float delay)
    {
        yield return new WaitForSeconds(delay);

        Debug.Log($"FxStartDot:direction {target}");
        var obj = GameObject.Instantiate(prefab);
        obj.transform.position = new Vector3(target.x, obj.transform.position.y, target.z );

        var time = duration;
        
        while (duration > 0.0f)
        {
            duration -= Time.deltaTime;
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
