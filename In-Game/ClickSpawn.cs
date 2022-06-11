using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Ÿ���� ��ġ�� ����Ѵ�

public class ClickSpawn : MonoBehaviour
{
    public int spawnTowerKind = 0;//�� ��ư�� �Ҵ�� Ÿ���� ����
    [SerializeField] GameObject Alpha50DragTower;//�巡�������� �����ϰ� ������ Ÿ��
    Vector3 dragOffset;
    Tower spawnTowerInfo;//Ÿ�� �Ŵ������� �޾Ƽ� Ÿ���� ������ �����Ѵ�.

    Vector2 boxCastSize = new Vector2(2, 2);

    RaycastHit2D isRoad;

    private void Awake()
    {
        //�����ϰ� ���� Ÿ���� ��Ȱ��ȭ�Ѵ�.
        Alpha50DragTower.SetActive(false);
        
    }

    void OnMouseDown()
    {
        //Ÿ���� ������ Ÿ���Ŵ������� �����´�.
        spawnTowerInfo = TowerManager.instance.GetTower(spawnTowerKind);
        boxCastSize = new Vector2(spawnTowerInfo.GetTowerSize, spawnTowerInfo.GetTowerSize);
        
        Alpha50DragTower.SetActive(true);

        Alpha50DragTower.transform.position = GetMousePos();
        //��Ÿ� ǥ�ø� �Ҵ�
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
    {//Ÿ���� �巡���ؼ� �����϶�
        Alpha50DragTower.transform.position = GetMousePos();
        UIManager.instance.RangeOn(spawnTowerInfo.GetTowerRange());
        UIManager.instance.RangeMove();

        isRoad = Physics2D.BoxCast(GetMousePos(), boxCastSize, 0f, Vector2.zero, 0, 1 << LayerMask.NameToLayer("Road") | 1 << LayerMask.NameToLayer("PlayerAttackTower"));
        
        
        //���� �ִ� ������ Ȯ���ؼ�
        if (isRoad.collider == null)//��Ÿ� ���� ��ü�Ѵ�
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
            {//���� ���� �ڽ�Ʈ�� Ÿ�� �ڽ�Ʈ���� ������
                //spawnTowerKind �� �������� Ÿ�� �����鿡�� Ÿ���� �߰����ش�
                TowerManager.instance.AddTower(spawnTowerKind, Alpha50DragTower.transform);
                GameManager.instance.costMinus(spawnTowerInfo.GetTowerCost());
                SoundManager.instance.ButtonSound(3);
            }

            
        }
        Alpha50DragTower.SetActive(false);
        UIManager.instance.RangeOff();
    }

    Vector3 GetMousePos()
    {//���콺 ��ġ�� ����
        var mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0;
        return mousePos;
    }


}
