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
        tiles[x, y] = Instantiate(prefab, new Vector3(x, y, 0), Quaternion.identity, transform).GetComponent<TileController>();
        tiles[x, y].Init(floor.tiles[x, y]);
      }
    }
  }
}
