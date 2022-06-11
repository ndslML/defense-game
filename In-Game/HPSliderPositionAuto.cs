using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//���� HP �����̴��� ����Ѵ�.

public class HPSliderPositionAuto : MonoBehaviour
{
    [SerializeField] Vector2 offset = Vector2.up * 20.0f;//�� �����ٰ� HP �� ���� ���� �뵵
    Transform targetTransform;
    RectTransform canvasTransform;
    Transform convertTransform;
    Enemy enemyThis;

    public void Setup(Transform target,RectTransform canvas,Enemy enemy)
    {//HP �� ����ٴ� �� ����
        targetTransform = target;
        //rect ������
        convertTransform = GetComponent<RectTransform>();
        canvasTransform = canvas;
        enemyThis = enemy;
    }

    private void LateUpdate()
    {//���� ������� ���� �����(Ǯ������ ��ü�Ұ�)
        if (!enemyThis.isActiveAndEnabled)
        {
            UIManager.instance.DestoryBar(gameObject);
            return;
        }
        //������Ʈ ���� ��ǥ �������� ȭ����ǥ�� ���ؼ�
        Vector3 screenPosition = Camera.main.WorldToScreenPoint(targetTransform.position);
        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasTransform, screenPosition, Camera.main, out Vector2 localPos);
        convertTransform.localPosition = localPos + offset;
        //�ű� + distance ��ŭ ������ ��ġ�� ������
    }
}
