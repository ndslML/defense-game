using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//적의 체력을 HP 슬라이더에 올리는 용도

public class EnemyHPViewer : MonoBehaviour
{
    Enemy enemy;
    Slider hpSlider;

    public void Setup(Enemy enemy)
    {
        this.enemy = enemy;//this 는 지역변수, 그냥은 매개변수
        hpSlider = GetComponent<Slider>();
    }

    private void Update()
    {
        hpSlider.value = enemy.GetCurrentHP() / enemy.GetMaxHP();
    }
}
