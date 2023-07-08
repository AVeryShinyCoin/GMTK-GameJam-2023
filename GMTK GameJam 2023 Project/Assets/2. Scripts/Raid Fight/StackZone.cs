using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StackZone : MonoBehaviour
{
    public List<GameObject> UnitsInZone = new List<GameObject>();
    private Collider2D coll;

    [SerializeField] bool outerZone;
    public bool OnFire;


    private void Start()
    {
        coll = GetComponent<Collider2D>();
    }


    public Vector2 RandomPointInZone()
    {
        var bounds = coll.bounds;
        var center = bounds.center;

        float x = 0;
        float y = 0;

        if (outerZone)
        {
            do
            {
                x = UnityEngine.Random.Range(center.x - bounds.extents.x, center.x + bounds.extents.x);
                y = UnityEngine.Random.Range(center.y - bounds.extents.y, center.y + bounds.extents.y);
            } while (Physics2D.OverlapPointAll(new Vector2(x, y)).Length != 1);
        }
        else
        {
            x = UnityEngine.Random.Range(center.x - bounds.extents.x, center.x + bounds.extents.x);
            y = UnityEngine.Random.Range(center.y - bounds.extents.y, center.y + bounds.extents.y);
        }
        return new Vector2(x, y);
    }


    public void SetOnFire()
    {
        OnFire = true;
    }

    public void StopFire()
    {
        OnFire = false;
    }
}



