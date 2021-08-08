
/*	Instructions:
 *		1.) Create a new C# script named "CombinedGridGOL"
 *		2.) Copy this script and paste it onto the script you created. Then save it.
 *		3.) Drag the new script onto the camera. Then press play.
 *		4.) Uncomment line 74, this will cause an error. In the parenthesis, put your scene number. If you only have 1 scene, put "0".
 *		5.) Adjust the variable "GridAreaSize" until you find a number that will run with a reasonable framerate and not throw an OutOfMemory exception. The camera will auto-center per Line 45.
 *				My computer at time of writing this had 32gb of memory and a 4GHz cpu and I could run 100 frames a second at a size of 20,000. 50,000 threw an OOM exception.
 *	Controls:
 *		Left click to add an Alive Cell.
 *		Right click to remove an Alive Cell.
 *		Middle-Mouse click to move the camera to your mouse pointer.
 *		Press Space to start the Game of Life.
 *		Press enter to reload the scene.
 *		Press escape to close the application. (If you chose to build the game) 
 */

using System;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
public class CombinedGridGOL : MonoBehaviour {
	private int[,] GameGrid;
	private int[,] NeighborCountArray;
	private GameObject AliveCell;
	private GameObject AliveCellParent;
	private bool GameStart = false;
	private int GridAreaSize = 2000;
	private List<Vector3> ListOfAllNeighbors;
	private List<GameObject> ListOfAllAliveCells;
	private void Start() { GetReferences(); }
	private void Update() { FlowControl(); }
	private void GetReferences() {
		AliveCellParent = new GameObject("AliveCellParent");
		Texture2D T2D = Texture2D.whiteTexture;
		Sprite sprite = Sprite.Create(T2D, new Rect(0, 0, T2D.width, T2D.height), new Vector2(0.5f, 0.5f), 4f);
		AliveCell = new GameObject("BaseAliveCell");
		AliveCell.transform.position = new Vector3(0, 0, -100);
		SpriteRenderer SR = AliveCell.AddComponent<SpriteRenderer>();
		SR.sprite = sprite;
		ListOfAllNeighbors = new List<Vector3>();
		ListOfAllAliveCells = new List<GameObject>();
		GameGrid = new int[GridAreaSize, GridAreaSize];
		NeighborCountArray = new int[GridAreaSize, GridAreaSize];
		transform.position = new Vector3(GridAreaSize / 2, GridAreaSize / 2, -10f);
		LoadGridInit();
	}
	private GameObject MakeSprite(Vector3 LocalV3) {
		GameObject CurrentGO = Instantiate(AliveCell, LocalV3, Quaternion.identity, AliveCellParent.transform);
		ListOfAllAliveCells.Add(CurrentGO);
		return CurrentGO;
    }
	private void FlowControl() {
		DetectPlayerInput();
		if (GameStart) {
			GOL();
			SetGrid();
		}
	}
	private void LoadGridInit() {
		for (int x = 0; x < GridAreaSize; x++) {
			for (int y = 0; y < GridAreaSize; y++) {
				GameGrid[x, y] = 0;
				NeighborCountArray[x, y] = 0;
			}
		}
	}
	private void DetectPlayerInput() {
		try {
			GetComponent<Camera>().orthographicSize += -Input.mouseScrollDelta.y * 2;
			Vector3 ScreenPosi = Camera.main.ScreenToWorldPoint(Input.mousePosition);
			Vector3 RoundedPosi = new Vector3(Mathf.FloorToInt(ScreenPosi.x), Mathf.FloorToInt(ScreenPosi.y), 0f);
			if (Input.GetKeyDown(KeyCode.Escape)) { Application.Quit(); }
			//if (Input.GetKeyDown(KeyCode.Return)) { SceneManager.LoadScene(); }
			if (Input.GetKeyDown(KeyCode.Space)) { GameStart = !GameStart; }
			if (Input.GetMouseButtonDown(2)) { GetComponent<Camera>().transform.position = ScreenPosi; }
			if (!GameStart) {
				if (Input.GetMouseButton(0)) {
					if (GameGrid[(int)RoundedPosi.x, (int)RoundedPosi.y] == 0) {
						GameObject CurrentGo = MakeSprite(RoundedPosi);
						if (CurrentGo != null) { GameGrid[(int)CurrentGo.transform.position.x, (int)CurrentGo.transform.position.y] = 1; }
					}
				}
				else if (Input.GetMouseButton(1)) {
					GameObject CurrentGo = ListOfAllAliveCells.Find(LOAC => LOAC.transform.position == RoundedPosi);
					if (CurrentGo != null) {
						GameGrid[(int)CurrentGo.transform.position.x, (int)CurrentGo.transform.position.y] = 0;
						ListOfAllAliveCells.Remove(CurrentGo);
						Destroy(CurrentGo);
					}
				}
			}
		}
		catch (Exception) { }
	}
	private void GOL() {
		if (GameStart) {
			try {
				foreach (GameObject GO in ListOfAllAliveCells) {
					Vector2 CurrentPOS = new Vector2(GO.transform.position.x, GO.transform.position.y);
					if (GameGrid[(int)CurrentPOS.x, (int)CurrentPOS.y] == 1) {
						int Neighbors = 0;
						for (int i = -1; i < 2; i++) {
							for (int j = -1; j < 2; j++) {
								Vector2 GridLocation = new Vector2(CurrentPOS.x + i, CurrentPOS.y + j);
								if (!(j == 0 && i == 0)) {
									ListOfAllNeighbors.Add(GridLocation);
									if (GameGrid[(int)GridLocation.x, (int)GridLocation.y] == 1) { Neighbors++; }
								}
							}
						}
						NeighborCountArray[(int)CurrentPOS.x, (int)CurrentPOS.y] = Neighbors;
					}
				}
				foreach (Vector3 V3 in ListOfAllNeighbors) {
					if (GameGrid[(int)V3.x, (int)V3.y] == 0) {
						int Neighbors = 0;
						for (int i = -1; i < 2; i++) {
							for (int j = -1; j < 2; j++) {
								Vector2 GridLocation = new Vector2(V3.x + i, V3.y + j);
								if (!(j == 0 && i == 0) && GameGrid[(int)GridLocation.x, (int)GridLocation.y] == 1) { Neighbors++; }
							}
						}
						if (NeighborCountArray[(int)V3.x, (int)V3.y] == 0) { NeighborCountArray[(int)V3.x, (int)V3.y] = Neighbors; }
					}
				}
			}
			catch (Exception) { }
		}
	}
	private void SetGrid() {
		List<GameObject> ObjectsToBeRemoved = new List<GameObject>();
		foreach (GameObject GO in ListOfAllAliveCells) {
			int x = (int)GO.transform.position.x;
			int y = (int)GO.transform.position.y;
			if ((NeighborCountArray[x, y] < 2 || NeighborCountArray[x, y] > 3) && GameGrid[x, y] == 1) { 
				GameGrid[x, y] = 0;
				ObjectsToBeRemoved.Add(GO);
				Destroy(GO);
			}
			NeighborCountArray[x, y] = 0;
		}
		foreach(GameObject GO in ObjectsToBeRemoved) { ListOfAllAliveCells.Remove(GO); }
		foreach (Vector3 V3 in ListOfAllNeighbors) {
			int x = (int)V3.x;
			int y = (int)V3.y;
			if (NeighborCountArray[x, y] == 3 && GameGrid[x, y] == 0) { 
				GameGrid[x, y] = 1;
				MakeSprite(new Vector3(x, y, 0f));
			}
			NeighborCountArray[x, y] = 0;
		}
		ListOfAllNeighbors = new List<Vector3>();
	}
}
