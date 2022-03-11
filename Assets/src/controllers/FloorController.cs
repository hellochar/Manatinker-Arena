using System;
using UnityEngine;

public class FloorController : MonoBehaviour {
  public GameObject wallTilePrefab;
  public GameObject floorTilePrefab;
  [NonSerialized]
  private Floor floor;
  private TileController[, ] tiles;

  public void Init(Floor floor) {
    this.floor = floor;
    tiles = new TileController[floor.width, floor.height];
    for(int x = 0; x < floor.width; x++) {
      for(int y = 0; y < floor.height; y++) {
        GameObject prefab = null;
        if (floor.tiles[x, y] == TileType.FLOOR) {
          prefab = floorTilePrefab;
        } else if (floor.tiles[x, y] == TileType.WALL) {
          prefab = wallTilePrefab;
        }
        var tile = Instantiate(prefab, transform);
        tile.transform.localPosition = new Vector3(x, y, 0);
        tiles[x, y] = tile.GetComponent<TileController>();
        tiles[x, y]?.Init(floor.tiles[x, y]);
      }
    }
  }
}
