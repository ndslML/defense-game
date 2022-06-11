using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//적의 HP 슬라이더를 담당한다.

public class HPSliderPositionAuto : MonoBehaviour
{
    [SerializeField] Vector2 offset = Vector2.up * 20.0f;//적 위에다가 HP 를 띄우기 위한 용도
    Transform targetTransform;
    RectTransform canvasTransform;
    Transform convertTransform;
    Enemy enemyThis;

    public void Setup(Transform target,RectTransform canvas,Enemy enemy)
    {//HP 가 따라다닐 적 설정
        targetTransform = target;
        //rect 얻어오기
        convertTransform = GetComponent<RectTransform>();
        canvasTransform = canvas;
        enemyThis = enemy;
    }

    private void LateUpdate()
    {//적이 사라지면 나도 사라짐(풀링으로 교체할것)
        if (!enemyThis.isActiveAndEnabled)
        {
            UIManager.instance.DestoryBar(gameObject);
            return;
        }
        //오브젝트 월드 좌표 기준으로 화면좌표를 구해서
        Vector3 screenPosition = Camera.main.WorldToScreenPoint(targetTransform.position);
        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasTransform, screenPosition, Camera.main, out Vector2 localPos);
        convertTransform.localPosition = localPos + offset;
        //거기 + distance 만큼 떨어진 위치에 생성함
    }
}
