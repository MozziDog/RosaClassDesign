using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 몬스터 종유석 패턴에서 사용되는 오브젝트 스크립트
/// </summary>
[RequireComponent(typeof(Rigidbody2D))]
public class MonsterStalactite : MonoBehaviour
{
    private new Rigidbody2D rigidbody;
    private new Collider2D collider;

    [SerializeField]
    private float dropSpeed = 3f;

    public void Init()
    {
        rigidbody = GetComponent<Rigidbody2D>();
        collider = GetComponent<Collider2D>();
        rigidbody.isKinematic = true;
    }

    public void Launch()
    {
        rigidbody.velocity = Vector2.down * dropSpeed;
        rigidbody.isKinematic = false;
    }

    private void DoDestroy()
    {
        Destroy(gameObject, 0.3f);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            rigidbody.isKinematic = true;
            rigidbody.velocity = Vector2.zero;
            collider.enabled = false;
            DoDestroy();
        }
    }
}
