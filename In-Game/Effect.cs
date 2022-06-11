using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Effect : MonoBehaviour
{
    [SerializeField] int id;

    void DistoryEffect()
    {
        TowerManager.instance.DestroyEffect(gameObject, id);
    }
}
