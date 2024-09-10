using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDamageReceiver : MonoBehaviour
{
    [SerializeField] float defaultNoDmgTime = 2f;

    PlayerRef playerRef;

    public void Start()
    {
        playerRef = PlayerRef.Instance;
    }


    public void GetDamage(GameObject target, int damage)
    {
        if(target.transform.position.y < transform.position.y - 0.73f)
        {
            Debug.Log("몬스터가 플레이어보다 아래에 있음 = 플레이어가 밟은 상황, 데미지 무시");
            return;
        }

        Debug.Log(target.name);

        playerRef.animation.BlinkEffect();
        playerRef.animation.SetTrigger("Hit");

        playerRef.movement.Knockback(new Vector2(target.transform.position.x,
                    target.transform.position.y - (target.transform.localScale.y / 2)));
        playerRef.state.TakeDamage(damage);

        int originalLayer = target.layer;
        int collisionLayer = gameObject.layer;

        // 현재 게임 오브젝트와 충돌한 오브젝트의 충돌을 무시
        Physics2D.IgnoreLayerCollision(originalLayer, collisionLayer, true);

        // 일정 시간 후 충돌 무시 해제
        StartCoroutine(RestoreCollision(originalLayer, collisionLayer, defaultNoDmgTime));
    }

    public void GetDamage(GameObject target, int damage, float ignoreDur)
    {
        Debug.Log($"다음으로부터 피격: {target.name}");

        playerRef.animation.BlinkEffect();
        playerRef.animation.SetTrigger("Hit");

        playerRef.movement.Knockback(new Vector2(target.transform.position.x,
                    target.transform.position.y - (target.transform.localScale.y / 2)));
        playerRef.state.TakeDamage(damage);

        int originalLayer = target.layer;
        int collisionLayer = gameObject.layer;

        // 현재 게임 오브젝트와 충돌한 오브젝트의 충돌을 무시
        Physics2D.IgnoreLayerCollision(originalLayer, collisionLayer, true);

        // 일정 시간 후 충돌 무시 해제
        StartCoroutine(RestoreCollision(originalLayer, collisionLayer, ignoreDur));
    }

    IEnumerator RestoreCollision(int originalLayer, int collisionLayer, float delay)
    {
        yield return new WaitForSeconds(delay);

        // 충돌 무시 해제
        Physics2D.IgnoreLayerCollision(originalLayer, collisionLayer, false);
    }
}
