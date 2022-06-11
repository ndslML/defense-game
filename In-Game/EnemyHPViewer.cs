using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//���� ü���� HP �����̴��� �ø��� �뵵

public class EnemyHPViewer : MonoBehaviour
{
    Enemy enemy;
    Slider hpSlider;

    public void Setup(Enemy enemy)
    {
        this.enemy = enemy;//this �� ��������, �׳��� �Ű�����
        hpSlider = GetComponent<Slider>();
    }

    private void Update()
    {
        hpSlider.value = enemy.GetCurrentHP() / enemy.GetMaxHP();
    }
}
