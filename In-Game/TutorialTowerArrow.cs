using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialTowerArrow : MonoBehaviour
{
    [SerializeField] GameObject arrow;
    private void OnMouseDown()
    {
        arrow.SetActive(false);
    }
}
