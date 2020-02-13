using UnityEngine;


public class AOEHitBox : HitBox
{
    public override void LoadHitData(HitData data)
    {
        if (hitDataID < 0)
        {
            if (data.Type == Hit.Type.AOE)
            {
                hit.LoadData(data);

                GetComponent<Recyclable>().lifeSpan = data.DamageDuration;

                transform.localScale = new Vector3(data.DamageRange, data.DamageRange, 1);
            }
            else
                Debug.LogWarningFormat("[HitBox] {0}: The hit data is not going to be loaded because of type mismatch.", gameObject.name);
        }
        else
            Debug.LogWarningFormat("[HitBox] {0}: The hit data is not going to be loaded. If you want to load new data, set hitDataID to 0.", gameObject.name);
    }
}
