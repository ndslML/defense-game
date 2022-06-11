using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//타워의 배치를 담당한다

public class ClickSpawn : MonoBehaviour
{
    public int spawnTowerKind = 0;//이 버튼에 할당된 타워의 종류
    [SerializeField] GameObject Alpha50DragTower;//드래그했을때 투명하게 보여줄 타워
    Vector3 dragOffset;
    Tower spawnTowerInfo;//타워 매니저에서 받아서 타워의 정보를 저장한다.

    Vector2 boxCastSize = new Vector2(2, 2);

    RaycastHit2D isRoad;

    private void Awake()
    {
        //투명하게 보일 타워를 비활성화한다.
        Alpha50DragTower.SetActive(false);
        
    }

    void OnMouseDown()
    {
        //타워의 정보를 타워매니저에서 가져온다.
        spawnTowerInfo = TowerManager.instance.GetTower(spawnTowerKind);
        boxCastSize = new Vector2(spawnTowerInfo.GetTowerSize, spawnTowerInfo.GetTowerSize);
        
        Alpha50DragTower.SetActive(true);

        Alpha50DragTower.transform.position = GetMousePos();
        //사거리 표시를 켠다
        UIManager.instance.RangeOn(spawnTowerInfo.GetTowerRange());

        SoundManager.instance.ButtonSound(2);
    }
    
    //void OnDrawGizmos()
    //{
    //    RaycastHit2D raycastHit = Physics2D.BoxCast(GetMousePos(), boxCastSize, 0f, Vector2.right, 0, 1 << LayerMask.NameToLayer("Road"));
    //
    //    Gizmos.color = Color.red;
    //    Gizmos.DrawWireCube(GetMousePos(), boxCastSize);
    //}

    private void OnMouseDrag()
    {//타워를 드래그해서 움직일때
        Alpha50DragTower.transform.position = GetMousePos();
        UIManager.instance.RangeOn(spawnTowerInfo.GetTowerRange());
        UIManager.instance.RangeMove();

        isRoad = Physics2D.BoxCast(GetMousePos(), boxCastSize, 0f, Vector2.zero, 0, 1 << LayerMask.NameToLayer("Road") | 1 << LayerMask.NameToLayer("PlayerAttackTower"));
        
        
        //길이 있는 지역을 확인해서
        if (isRoad.collider == null)//사거리 색을 교체한다
        {
            UIManager.instance.RangeBlack();
        }
        else
        {
            UIManager.instance.RangeRed();
        }
    }

    private void OnMouseUp()
    {
        if (isRoad.collider == null)
        {
            if (GameManager.instance.getCost() >= spawnTowerInfo.GetTowerCost())
            {//현재 가진 코스트가 타워 코스트보다 높으면
                //spawnTowerKind 를 기준으로 타워 프리펩에서 타워를 추가해준다
                TowerManager.instance.AddTower(spawnTowerKind, Alpha50DragTower.transform);
                GameManager.instance.costMinus(spawnTowerInfo.GetTowerCost());
                SoundManager.instance.ButtonSound(3);
            }

            
        }
        Alpha50DragTower.SetActive(false);
        UIManager.instance.RangeOff();
    }

    Vector3 GetMousePos()
    {//마우스 위치를 알음
        var mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0;
        return mousePos;
    }


}
