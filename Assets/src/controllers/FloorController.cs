using System;
using UnityEngine;

public class FloorController : MonoBehaviour {
  public GameObject wallTilePrefab;
  public GameObject floorTilePrefab;
  [NonSerialized]
  private Floor floor;
  private TileController[, ] tiles;
  public GameObject boundaryLeft, boundaryUp, boundaryRight, boundaryBottom;

  public void Init(Floor floor) {
    this.floor = floor;
    boundaryLeft.transform.position = new Vector2(0, floor.height / 2);
    boundaryRight.transform.position = new Vector2(floor.width, floor.height / 2);
    boundaryUp.transform.position = new Vector2(floor.width / 2, floor.height);
    boundaryBottom.transform.position = new Vector2(floor.width / 2, 0);
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
