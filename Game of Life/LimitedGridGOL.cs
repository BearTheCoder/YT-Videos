// This is attach to an empty game object in the scene.
// The only thing require is a 1m x 1m square to act as the "Alive" cell;
using UnityEngine;
using UnityEngine.SceneManagement;
public class LimitedGridGOL : MonoBehaviour {
	public GameObject AliveCell;
	private Camera MC;
	private int[,] GameGrid;
	private GameObject AliveCellParent;
	private int GridArea = 400;
	private int[,] NeighborsArray;
	private bool GameStart = false;
	private GameObject[,] InGameCells;
	private void Start() { GetReferences(); }
	private void Update() { FlowControl(); }
	private void GetReferences() {
		MC = Camera.main;
		AliveCellParent = new GameObject("AliveCellParent");
		GameGrid = new int[GridArea, GridArea];
		NeighborsArray = new int[GridArea, GridArea];
		InGameCells = new GameObject[GridArea, GridArea];
		MC.transform.position = new Vector3(GridArea / 2, GridArea / 2, -10f);
		LoadGridInit();
	}
	private void FlowControl() {
		DetectPlayerInput();
		if (GameStart) {
			GOL();
			SetGrid();
		}
	}
	private void LoadGridInit() {
		for (int x = 0; x < GridArea; x++) {
			for (int y = 0; y < GridArea; y++) {
				GameGrid[x, y] = 0;
				NeighborsArray[x, y] = 0;
			}
		}
	}
	private void DetectPlayerInput() {
		MC.orthographicSize += -Input.mouseScrollDelta.y * 2;
		Vector3 ScreenPosi = Camera.main.ScreenToWorldPoint(Input.mousePosition);
		Vector3 RoundedPosi = new Vector3(Mathf.FloorToInt(ScreenPosi.x), Mathf.FloorToInt(ScreenPosi.y), 0f);
		if (Input.GetKeyDown(KeyCode.Escape)) { SceneManager.LoadScene(0); }
		if (Input.GetKeyDown(KeyCode.Space)) { GameStart = !GameStart; }
		if (Input.GetKeyDown(KeyCode.Return)) { SceneManager.LoadScene(2); }
		if (Input.GetMouseButtonDown(2)) { MC.transform.position = ScreenPosi; }
		if (!GameStart) {
			if (Input.GetMouseButton(0)) {
				if (InGameCells[(int)RoundedPosi.x, (int)RoundedPosi.y] == null) {
					GameObject CurrentGo = Instantiate(AliveCell, RoundedPosi, Quaternion.identity, AliveCellParent.transform);
					if (CurrentGo != null) {
						GameGrid[(int)CurrentGo.transform.position.x, (int)CurrentGo.transform.position.y] = 1;
						InGameCells[(int)RoundedPosi.x, (int)RoundedPosi.y] = CurrentGo;
					}
				}
			}
			else if (Input.GetMouseButton(1)) {
				GameObject CurrentGo = InGameCells[(int)RoundedPosi.x, (int)RoundedPosi.y];
				if (CurrentGo != null) {
					GameGrid[(int)CurrentGo.transform.position.x, (int)CurrentGo.transform.position.y] = 0;
					Destroy(CurrentGo);
					InGameCells[(int)RoundedPosi.x, (int)RoundedPosi.y] = null;
				}
			}
		}

	}
	private void GOL() {
		if (GameStart) {
			for (int x = 1; x < GridArea - 1; x++) {
				for (int y = 1; y < GridArea - 1; y++) {
					int Neighbors = 0;
					for (int i = -1; i < 2; i++) {
						for (int j = -1; j < 2; j++) {
							Vector2 GridLocation = new Vector2(x + i, y + j);
							if (!(j == 0 && i == 0) && GameGrid[(int)GridLocation.x, (int)GridLocation.y] == 1) {
								Neighbors++;
							}
						}
					}
					NeighborsArray[x, y] = Neighbors;
				}
			}
		}
	}
	private void SetGrid() {
		for (int x = 0; x < GridArea; x++) {
			for (int y = 0; y < GridArea; y++) {
				GameObject CurrentGo = InGameCells[x, y];
				if ((NeighborsArray[x, y] < 2 || NeighborsArray[x, y] > 3) && GameGrid[x, y] == 1) { GameGrid[x, y] = 0; }
				if (NeighborsArray[x, y] == 3 && GameGrid[x, y] == 0) { GameGrid[x, y] = 1; }
				if (GameGrid[x, y] == 1 && CurrentGo == null) {
					CurrentGo = Instantiate(AliveCell, new Vector3(x, y, 0f), Quaternion.identity, AliveCellParent.transform);
					InGameCells[x, y] = CurrentGo;
				}
				else if (GameGrid[x, y] == 0 && CurrentGo != null) {
					Destroy(CurrentGo);
					InGameCells[x, y] = null;
				}
				NeighborsArray[x, y] = 0;
			}
		}
	}
}
