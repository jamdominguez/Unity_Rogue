using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardManager : MonoBehaviour {

    public int colums, rows;
    public GameObject[] floorTiles, outerWallTiles, wallTiles, foodTiles, enemyTiles;
    public GameObject exit;
    public GameObject player;

    private Transform boardHolder;
    private List<Vector2> freeGridPositions;


    public void SetupScene(int level) {
        BoardSetup(); // floor, outerWall and free positions
        ObjectsSetup(wallTiles, 5, 9); // walls
        ObjectsSetup(foodTiles, 1, 5); // food
        int enemyCount = (int)Mathf.Log(level, 2);
        ObjectsSetup(enemyTiles, enemyCount, enemyCount); // enemies
        ExitSetup();
        PlayerSetup();
    }

    private void BoardSetup() {
        boardHolder = new GameObject("Board").transform;
        freeGridPositions = new List<Vector2>();
        for (int c = -1; c < colums + 1; c++) {
            for (int r = -1; r < rows + 1; r++) {
                // create terrain
                bool isOuterWall = c == -1 || c == colums || r == -1 || r == rows;
                GameObject tileToAdd = isOuterWall ? GetRandomObj(outerWallTiles) : GetRandomObj(floorTiles);
                Instantiate(tileToAdd, new Vector2(c, r), Quaternion.identity).transform.SetParent(boardHolder);
                // add free positions to add items or enemies in the future
                bool isFreePosition = c > 0 && c < colums - 1 && r > 0 && r < rows - 1;
                if (isFreePosition) AddFreePosition(new Vector2(c, r));
            }
        }
    }

    private void ObjectsSetup(GameObject[] goArray, int min, int max) {
        int goCount = Random.Range(min, max + 1);
        for (int i = 0; i < goCount; i++) {
            Instantiate(GetRandomObj(goArray), GetRandomFreePosition(), Quaternion.identity);//.transform.SetParent(boardHolder);
        }
    }

    private void ExitSetup() {
        Instantiate(exit, new Vector2(colums - 1, rows - 1), Quaternion.identity);//.transform.SetParent(boardHolder);
    }

    private void PlayerSetup() {
        Instantiate(player, new Vector2(0, 0), Quaternion.identity);//.transform.SetParent(boardHolder);
    }

    private void AddFreePosition(Vector2 position) {
        freeGridPositions.Add(position);
    }

    private GameObject GetRandomObj(GameObject[] goArray) {
        return goArray[Random.Range(0, goArray.Length)];
    }

    private Vector2 GetRandomFreePosition() {
        int randomIndex = Random.Range(0, freeGridPositions.Count);
        Vector2 randomPosition = freeGridPositions[randomIndex];
        freeGridPositions.RemoveAt(randomIndex);
        return randomPosition;
    }
}
