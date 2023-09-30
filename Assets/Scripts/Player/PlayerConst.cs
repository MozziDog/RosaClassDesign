using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 플레이어와 관련한 여러가지 상수 모음
/// </summary>

public enum PlayerMoveState
{
    WALK,
    CLIMB,
    MONKEY, // 천장에 매달린 상태. 더 적절한 이름이 있으면 바꿀 것
    CANNOTMOVE, // 피격 등으로 인해 일시적으로 움직일 수 없는 상태
}

public enum PlantMagicCode  // 더 좋은 이름 없을지 고민중. MagicCode라고 하기엔 너무 범용적인 이름이라
{
    IVY,
    MANGROVE,
    SQUIRTING_CUCUMBER,
    MYOSOTIS,

    CISTUS,
    FLYTRAP,
    WALLTERMELON,
    LIGHTBLOOM,
}

/// <summary> 설치 가능 지형별로 구분한 식물마법 타입 </summary>
public enum MagicCastType
{
    /// <summary> 지면에다만 설치 가능 (오이, 파리지옥, 수박 등) </summary>
    GROUND_ONLY,
    /// <summary> 벽에다만 설치 가능 (담쟁이 등) </summary>
    WALL_ONLY,
    /// <summary> 천장에다만 설치 가능 (맹그로브 등) </summary>
    CEIL_ONLY,
    /// <summary> 지면/벽/천장 아무데나 설치 가능 (조명꽃 등) </summary>
    EVERYWHERE,
}

public enum TerrainType
{
    /// <summary> 천장 </summary>
    Ceil,
    /// <summary> 바닥 </summary>
    Floor,
    /// <summary> 오른쪽을 보고 있는 벽 </summary>
    Wall_facing_right,
    /// <summary> 왼쪽을 보고 있는 벽 </summary>
    Wall_facing_left,
}

public static class TerrainTypeExtend
{
    /// <summary>
    /// 60분법의 각도로 변환. 위를 바라보는 상태를 0도 기준으로 반시계방향.
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static float toAngle(this TerrainType value)
    {
        switch(value)
        {
            case TerrainType.Wall_facing_left:
                return 90f;
            case TerrainType.Ceil:
                return 180f;
            case TerrainType.Wall_facing_right:
                return 270f;
            default:   //case TerrainType.Floor:
                return 0f;
        }
    }
}
