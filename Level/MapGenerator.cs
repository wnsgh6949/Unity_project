using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MapGenerator : MonoBehaviour {
	
	public Map[] maps;
	public int mapIndex;

	public Transform tilePrefab;
	public Transform[] obstaclePrefab;
	
	[Range(0,1)]
	public float outlinePercent;

	List<Coord> allTileCoords;
	Queue<Coord> shuffledTileCoords;

	Map currentMap;

	[System.Serializable]
	public struct Coord {
		public int x;
		public int y;

		public Coord(int _x, int _y) {
			x = _x;
			y = _y;
		}
	}

	[System.Serializable]
	public class Map {
		
		public Coord mapSize;
		[Range(0,1)]
		public float obstaclePercent;
		public int seed;

		public Coord mapCentre {
			get {
				return new Coord(mapSize.x/2,mapSize.y/2);
			}
		}
	}

	void Start() {
		FindObjectOfType<Spawner> ().OnNewWave += OnNewWave;
	}
	
	void OnNewWave(int waveNumber) {
		mapIndex = waveNumber - 1;
		GenerateMap();
	}

	public void GenerateMap() {
		currentMap = maps[mapIndex];

		// Generating coords
		allTileCoords = new List<Coord> ();
		for (int x = 0; x < currentMap.mapSize.x; x ++) {
			for (int y = 0; y < currentMap.mapSize.y; y ++) {
				allTileCoords.Add(new Coord(x,y));
			}
		}
		shuffledTileCoords = new Queue<Coord> (Utility.ShuffleArray (allTileCoords.ToArray (), (int)Random.Range(0, 100)));

		// Create map holder object
		string holderName = "Generated Map";
		if (transform.Find (holderName)) {
			DestroyImmediate (transform.Find (holderName).gameObject);
		}
		
		Transform mapHolder = new GameObject (holderName).transform;
		mapHolder.parent = transform;
		
		// Spawning tiles
		for (int x = 0; x < currentMap.mapSize.x; x ++) {
			for (int y = 0; y < currentMap.mapSize.y; y ++) {
				Vector3 tilePosition = CoordToPosition(x,y);
				Transform newTile = Instantiate (tilePrefab, tilePosition, Quaternion.identity) as Transform;
				newTile.localScale = Vector3.one * (1 - outlinePercent);
				newTile.parent = mapHolder;
			}
		}

		// Spawning obstacles
		bool[,] obstacleMap = new bool[(int)currentMap.mapSize.x,(int)currentMap.mapSize.y];

		int obstacleCount = (int)(currentMap.mapSize.x * currentMap.mapSize.y * currentMap.obstaclePercent);
		int currentObstacleCount = 0;

		for (int i =0; i < obstacleCount; i ++) {
			bool adjacent;
			Coord randomCoord = GetRandomCoord();
			obstacleMap[randomCoord.x,randomCoord.y] = true;
			currentObstacleCount ++;

			if (randomCoord.x != currentMap.mapCentre.x && randomCoord.y != currentMap.mapCentre.y && MapIsFullyAccessible(obstacleMap, currentObstacleCount)) {
				Vector3 obstaclePosition = CoordToPosition(randomCoord.x,randomCoord.y);

				adjacent = false;
				for (int x = -1; x <= 1; x ++) {
					for(int y = -1; y <=1; y++) {
						int neighbourX = randomCoord.x + x;
						int neighbourY = randomCoord.y + y;
						if (x != y) {
							if(neighbourX >= 0 && neighbourX < obstacleMap.GetLength(0) && neighbourY >= 0 && neighbourY < obstacleMap.GetLength(1)) {
								if(obstacleMap[neighbourX,neighbourY]) {
									adjacent = true;
									break;
								}
							}
						}
					}
				}

				if(!adjacent)
				{
					Transform newObstacle = Instantiate(obstaclePrefab[Random.Range(0, obstaclePrefab.Length)], obstaclePosition + Vector3.up * .375f, Quaternion.Euler(0, Random.Range(60, 120), 0)) as Transform;
					newObstacle.parent = mapHolder;

					Renderer obstacleRenderer = newObstacle.GetComponent<Renderer>();
				}
			}
			else {
				obstacleMap[randomCoord.x,randomCoord.y] = false;
				currentObstacleCount --;
			}
		}
		
	}

	bool MapIsFullyAccessible(bool[,] obstacleMap, int currentObstacleCount) {
		bool[,] mapFlags = new bool[obstacleMap.GetLength(0),obstacleMap.GetLength(1)];
		Queue<Coord> queue = new Queue<Coord> ();
		queue.Enqueue (currentMap.mapCentre);
		mapFlags [currentMap.mapCentre.x, currentMap.mapCentre.y] = true;

		int accessibleTileCount = 1;

		while (queue.Count > 0) {
			Coord tile = queue.Dequeue();

			for (int x = -1; x <= 1; x ++) {
				for (int y = -1; y <= 1; y ++) {
					int neighbourX = tile.x + x;
					int neighbourY = tile.y + y;
					if (x == 0 || y == 0) {
						if (neighbourX >= 0 && neighbourX < obstacleMap.GetLength(0) && neighbourY >= 0 && neighbourY < obstacleMap.GetLength(1)) {
							if (!mapFlags[neighbourX,neighbourY] && !obstacleMap[neighbourX,neighbourY]) {
								mapFlags[neighbourX,neighbourY] = true;
								queue.Enqueue(new Coord(neighbourX,neighbourY));
								accessibleTileCount ++;
							}
						}
					}
				}
			}
		}

		int targetAccessibleTileCount = (int)(currentMap.mapSize.x * currentMap.mapSize.y - currentObstacleCount);
		return targetAccessibleTileCount == accessibleTileCount;
	}

	Vector3 CoordToPosition(int x, int y) {
		return new Vector3 (-currentMap.mapSize.x / 2f + 0.5f + x, 0, -currentMap.mapSize.y / 2f + 0.5f + y);
	}

	public Coord GetRandomCoord() {
		Coord randomCoord = shuffledTileCoords.Dequeue ();
		shuffledTileCoords.Enqueue (randomCoord);
		return randomCoord;
	}
}