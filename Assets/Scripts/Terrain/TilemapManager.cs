using Sirenix.OdinInspector;
using Sirenix.Serialization;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TilemapManager : SerializedMonoBehaviour
{
    public static TilemapManager Instance;

    [SerializeField, ReadOnly]
    public List<Tilemap> tilemapList;

    public List<TileData> tileDatas;
    [SerializeField]
    private Dictionary<TileBase, TileData> dataFromTiles;

    [SerializeField]
    private TileData defaultTileData;

    // Start is called before the first frame update
    void Awake()
    {
        Instance = this;

        // 딕셔너리가 컴파일타임에 값 할당이 안되서 우선 리스트에 데이터를 담아두고 런타임에 이를 옮겨야 함.
        dataFromTiles = new Dictionary<TileBase, TileData>();       
        foreach(var tileData in tileDatas)
        {
            foreach(var tile in tileData.tiles)
            {
                dataFromTiles.Add(tile, tileData);
            }
        }
    }

    private void Start()
    {
        UpdateTilemapList();
    }

    public void UpdateTilemapList()
    {
        // 맵에 존재하는 모든 타일맵 가져오기 (active GameObject Only)
        List<Tilemap> tilemaps = FindObjectsOfType<Tilemap>().ToList();
        // "Ground","HiddenRoom","TmpTerrain" 레이어를 가진 것들만 필터링
        int layerMask = LayerMask.GetMask("Ground", "HiddenRoom", "TmpTerrain");
        tilemaps.RemoveAll((tilemap) => { return (1 << tilemap.gameObject.layer & layerMask) == 0; });
        // 기본 타일맵 설정
        tilemapList = tilemaps;
    }

    public TileData GetTileDataByCellPosition(Vector3Int cellPosition)
    {
        Vector3 cellOffset = new Vector3(0.5f, 0.5f);

        // 가지고 있는 타일맵 리스트에서 가져오기 (최적화)
        List<TileBase> selectedTile = SearchTileInTilemaps(cellPosition);

        if(selectedTile.Count == 0)
        {
            return null;
        }

        // 타일을 찾았다면 해당 타일의 데이터 가져오기
        try
        {
            if(selectedTile.Count == 1)
                return dataFromTiles[selectedTile[0]];
            else
            {
                // 여러 타일들이 중첩되어 있는 경우, 우선순위가 높은 TileData를 사용함.
                // 우선순위
                // 1. 식물 설치 불가능한 타일
                // 2. 식물 설치 가능한 타일
                // 3. 실체가 없는 타일
                TileData mergedData = null;
                foreach(var tile in selectedTile)
                {
                    TileData tileData = dataFromTiles[tile];
                    if((mergedData==null || !mergedData.isSubstance) && tileData.isSubstance)
                    {
                        mergedData = tileData;
                    }
                    if((mergedData == null || mergedData.magicAllowed) && tileData.isSubstance && !tileData.magicAllowed)
                    {
                        mergedData = tileData;
                    }
                }
                return mergedData;
            }
        }
        catch (KeyNotFoundException)     // 혹시라도 타일데이터 설정을 빼먹은 경우
        {
            Debug.LogWarning($"타일 데이터가 설정되어있지 않음! 기본 설정을 사용합니다\n위치: {cellPosition}");
            return defaultTileData;
        }
    }

    private List<TileBase> SearchTileInTilemaps(Vector3Int cellPosition)
    {
        List<TileBase> selectedTiles = new List<TileBase>();
        if (tilemapList != null)
        {
            for (int i = 0; i < tilemapList.Count; i++)
            {
                // 삭제된 타일맵이 있다면 리스트에서 제거
                if (tilemapList[i] == null)
                {
                    tilemapList.RemoveAt(i);
                    i--;
                    continue;
                }
                // 타일맵이 유효하다면 그 타일맵에서 타일 가져오기 시도
                var tile = tilemapList[i].GetTile(cellPosition);
                if (tile != null)
                    selectedTiles.Add(tile);
            }
        }

        return selectedTiles;
    }

    public TileData GetTileDataByWorldPosition(Vector3 worldPosition)
    {
        return GetTileDataByCellPosition(WorldToCell(worldPosition));
    }

    public TileData GetTileDataByTileBase(TileBase tile)
    {
        TileData result;
        try
        {
            result = dataFromTiles[tile];
        }
        catch(KeyNotFoundException)
        {
            Debug.LogWarning($"타일 데이터가 설정되어있지 않음! 기본 설정을 사용합니다\n타일: {tile}");
            return defaultTileData;
        }
        return result;
    }

    public bool GetIfSubstanceTileExist(Vector3Int cellPosition)
    {
        Vector3 cellOffset = new Vector3(0.5f, 0.5f);

        TileBase selectedTile;
        for (int i = 0; i < tilemapList.Count; i++)
        {
            // 삭제된 타일맵이 있다면 리스트에서 제거
            if (tilemapList[i] == null)
            {
                tilemapList.RemoveAt(i);
                i--;
                continue;
            }
            // 타일맵이 유효하다면 그 타일맵에서 타일 가져오기 시도
            selectedTile = tilemapList[i].GetTile(cellPosition);
            if(selectedTile != null && dataFromTiles[selectedTile].isSubstance) 
                return true;
        }
        return false;

        //Collider2D hiddenRoomCurtain = Physics2D.OverlapPoint((Vector3)cellPosition + cellOffset, LayerMask.GetMask("HiddenRoom"));
        //if (hiddenRoomCurtain != null)
        //    return true;
        //else 
        //    return false;
    }

    public bool GetTilePlantable(Vector3Int cellPosition)
    {
        TileData td = GetTileDataByCellPosition(cellPosition);
        if (td == null)
        {
            Debug.LogError("해당 위치에 타일이 존재하지 않음");
        }
        return td.magicAllowed;
    }

    public Vector3Int WorldToCell(Vector2 worldPosition)
    {
        return tilemapList[0].WorldToCell(worldPosition);
    }
}
