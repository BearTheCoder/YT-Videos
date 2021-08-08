// This is attach to an empty game object in the scene.
// The only thing require is a 1m x 1m square to act as the "Alive" cell;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class InfiniteGridGOL : MonoBehaviour {
    public GameObject AliveCell;
    private GameObject AliveCellsParent;
    private List<GameObject> InGameCells;
    private List<AliveCell> AliveCells;
    private List<DeadCell> DeadCells;
    private Camera MC;
    private bool GameStart = false;
    void Start() { GetReferences(); }
    void Update() {
        CheckForGameStart();
        DetectUserInput();
        GameOfLife();
    }
    private void GetReferences() {
        InGameCells = new List<GameObject>();
        AliveCellsParent = new GameObject("AliveCellsParent");
        MC = Camera.main;
    }
    private void CheckForGameStart() { if (Input.GetKeyDown(KeyCode.Space)) { GameStart = !GameStart; } }
    private void DetectUserInput() {
        Vector3 ScreenPosi = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3 RoundedPosi = new Vector3(Mathf.FloorToInt(ScreenPosi.x), Mathf.FloorToInt(ScreenPosi.y), 0f);
        MC.orthographicSize += -Input.mouseScrollDelta.y * 2;
        if (Input.GetKeyDown(KeyCode.Escape)) { SceneManager.LoadScene(0); }
        if (Input.GetKeyDown(KeyCode.Return)) { SceneManager.LoadScene(1); }
        if (Input.GetMouseButtonDown(2)) { MC.transform.position = ScreenPosi; }
        if (!GameStart) {
            bool CanSpawn = true;
            if (Input.GetMouseButton(0)) {
                foreach (GameObject GO in InGameCells) {
                    if (RoundedPosi == GO.transform.position) {
                        CanSpawn = false;
                        break;
                    }
                }
                if (CanSpawn) {
                    GameObject CurrentBeing = Instantiate(AliveCell, RoundedPosi, Quaternion.identity, AliveCellsParent.transform);
                    CurrentBeing.name = "Being " + CurrentBeing.transform.position.ToString();
                    InGameCells.Add(CurrentBeing);
                }
            }
            else if (Input.GetMouseButton(1)) {
                List<GameObject> NewInGameSqaures = MakeNewGOList(InGameCells);
                foreach (GameObject GO in InGameCells) {
                    if (RoundedPosi == GO.transform.position) {
                        NewInGameSqaures.Remove(GO);
                        Destroy(GO);
                        break;
                    }
                }
                InGameCells = NewInGameSqaures;
            }
        }
    }
    private void GameOfLife() {
        if (GameStart) {
            AliveCells = new List<AliveCell>();
            DeadCells = new List<DeadCell>();
            CreateSnapShot();
            List<AliveCell> CurrentACs = AliveCells.FindAll(GSSAC => GSSAC.Neighbors < 2 || GSSAC.Neighbors > 3);
            foreach (AliveCell AC in CurrentACs) {
                InGameCells.Remove(AC.GO);
                Destroy(AC.GO);
            }
            List<DeadCell> CurrentDCs = DeadCells.FindAll(GSSDC => GSSDC.Neighbors == 3);
            foreach(DeadCell DC in CurrentDCs) {
                GameObject CurrentBeing = Instantiate(AliveCell, DC.Location, Quaternion.identity, AliveCellsParent.transform);
                CurrentBeing.name = "Being " + CurrentBeing.transform.position.ToString();
                InGameCells.Add(CurrentBeing);
            }
        }
    }
    private void CreateSnapShot() {
        List<AliveCell> LocalSSAliveCells = new List<AliveCell>();
        List<DeadCell> LocalSSDeadCells = new List<DeadCell>();
        foreach (GameObject GO in InGameCells) {
            List<Vector3> CurrentNeighborsPoints = new List<Vector3>();
            for (int x = -1; x < 2; x++) {
                for (int y = -1; y < 2; y++) {
                    Vector3 CurrentLocation = new Vector3(GO.transform.position.x + x, GO.transform.position.y + y, 0f);
                    if (CurrentLocation != GO.transform.position) { 
                        Vector3 NeighborPoint = new Vector3(GO.transform.position.x + x, GO.transform.position.y + y, 0f);
                        CurrentNeighborsPoints.Add(NeighborPoint);
                        bool CanMakeObject = true;
                        if (InGameCells.Find(IGS => IGS.transform.position == CurrentLocation) != null) { CanMakeObject = false; }
                        if (LocalSSDeadCells.Find(e => e.Location == CurrentLocation) != null && CanMakeObject) { CanMakeObject = false; }
                        if (CanMakeObject) { LocalSSDeadCells.Add(new DeadCell(CurrentLocation, 0)); }
                    }
                }
            }
            AliveCell CurrentAliveCell = new AliveCell(GO, CurrentNeighborsPoints, 0);
            LocalSSAliveCells.Add(CurrentAliveCell);
        }
        foreach (AliveCell AC in LocalSSAliveCells) {
            foreach (Vector3 V3 in AC.Neighborhood) {
                GameObject CurrentGO = InGameCells.Find(IGS => IGS.transform.position == V3);
                if (CurrentGO != null && AC.GO != CurrentGO) { AC.Neighbors++; }
            }
        }
        foreach (DeadCell DC in LocalSSDeadCells) {
            List<AliveCell> CurrentACs = LocalSSAliveCells.FindAll(LSSAC => LSSAC.Neighborhood.Contains(DC.Location));
            foreach(AliveCell AC in CurrentACs) { DC.Neighbors++; }
        }
        AliveCells = LocalSSAliveCells;
        DeadCells = LocalSSDeadCells;
    }
    private List<GameObject> MakeNewGOList(List<GameObject> LocalOldList) {
        List<GameObject> NewList = new List<GameObject>();
        foreach (GameObject GO in LocalOldList) { NewList.Add(GO); }
        return NewList;
    }
}
class AliveCell {
    internal GameObject GO { get; set; }
    internal List<Vector3> Neighborhood { get; set; }
    internal int Neighbors { get; set; }
    internal AliveCell(GameObject LocalGO, List<Vector3> LocalNeighborhood, int LocalNeighbors) {
        GO = LocalGO;
        Neighborhood = LocalNeighborhood;
        Neighbors = LocalNeighbors;
    }
}
class DeadCell {
    internal Vector3 Location { get; set; }
    internal int Neighbors { get; set; }
    internal DeadCell(Vector3 LocalLocation, int LocalNeighbors) {
        Neighbors = LocalNeighbors;
        Location = LocalLocation;
    }
}
