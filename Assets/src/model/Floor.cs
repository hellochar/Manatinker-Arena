using System;
using System.Collections.Generic;
using UnityEngine;

public enum TileType {
  FLOOR = 0,
  WALL = 1,
}

public class Floor {
  public TileType[,] tiles;
  internal int width;
  internal int height;

  public Floor(int width, int height) {
    this.width = width;
    this.height = height;
    tiles = new TileType[width, height];
  }

  internal Floor surroundWithWalls() {
    foreach(var pos in EnumeratePerimeter()) {
      tiles[pos.x, pos.y] = TileType.WALL;
    }
    return this;
  }

  public IEnumerable<Vector2Int> EnumeratePerimeter(int inset = 0) {
    // top edge, including top-left, excluding top-right
    for (int x = inset; x < width - inset - 1; x++) {
      yield return new Vector2Int(x, inset);
    }
    // right edge
    for (int y = inset; y < height - inset - 1; y++) {
      yield return new Vector2Int(width - 1 - inset, y);
    }
    // bottom edge, now going right-to-left, now excluding bottom-left
    for (int x = width - inset - 1; x > inset; x--) {
      yield return new Vector2Int(x, height - 1 - inset);
    }
    // left edge
    for (int y = height - inset - 1; y > inset; y--) {
      yield return new Vector2Int(inset, y);
    }
  }
}
