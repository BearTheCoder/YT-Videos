using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class CubeSpawner : MonoBehaviour {
    public GameObject Cube;
    public Slider ScalingFactor, PerlinScale, XOffset, YOffset;
    public Text ScalingFactorValue, PerlinScaleValue, XOffsetValue, YOffsetValue, ButtonOutputText;
    public Button IntOrFloatButton, NextButton, PreviousButton;
    private bool ReturValueAsInt;
    private int XMap, YMap;
    private List<GameObject> CubeList = new List<GameObject>();
    void Start() {
        XMap = 50;
        YMap = 50;
        IntOrFloatButton.onClick.AddListener(SetOutput);
        NextButton.onClick.AddListener(LoadNext);
        PreviousButton.onClick.AddListener(LoadPrevious);
        for (int x = 0; x < XMap; x++) {
            for (int y = 0; y < YMap; y++) {
                SpawnCube(SetCubePos(x, y, XMap, YMap));
            }
        }
    }
    private void Update() {
        foreach (GameObject cube in CubeList) {
            cube.transform.localPosition = SetCubePos((int)cube.transform.localPosition.x, (int)cube.transform.localPosition.z, XMap, YMap);
            SetMinecraftColor(cube);
        }
        transform.RotateAround(transform.position, Vector3.up, 10 * Time.deltaTime);
        ScalingFactorValue.text = ScalingFactor.value.ToString();
        PerlinScaleValue.text = PerlinScale.value.ToString();
        XOffsetValue.text = XOffset.value.ToString();
        YOffsetValue.text = YOffset.value.ToString();
        if (Input.GetKeyDown(KeyCode.Escape)) { Application.Quit(); }
    }
    private Vector3 SetCubePos(int x, int y, int XMap, int YMap){
        float xCoord = (float)x / XMap * PerlinScale.value + XOffset.value;
        float yCoord = (float)y / YMap * PerlinScale.value + YOffset.value;
        float PerlinValue = Mathf.PerlinNoise(xCoord, yCoord);
        float Value = PerlinValue * ScalingFactor.value;
        if (ReturValueAsInt) { ButtonOutputText.text = "Output: INTERGER"; return new Vector3(x, (int)Value, y); }
        else { ButtonOutputText.text = "Output: FLOAT"; return new Vector3(x, Value, y); }
    }
    private void SetMinecraftColor(GameObject cube)
    {
        if (cube.transform.position.y < 3)
        {
            cube.GetComponent<Renderer>().material.SetColor("_Color", Color.blue);
        }
        else if (cube.transform.position.y == 3)
        {
            cube.GetComponent<Renderer>().material.SetColor("_Color", Color.yellow);
        }
        else if (cube.transform.position.y > 3 && cube.transform.position.y < 12)
        {
            cube.GetComponent<Renderer>().material.SetColor("_Color", Color.green);
        }
        else
        {
            cube.GetComponent<Renderer>().material.SetColor("_Color", Color.white);
        }
    }
    private void SpawnCube(Vector3 CubePos){
        CubeList.Add(Instantiate(Cube, CubePos, Quaternion.identity, transform));
    }
    private void SetOutput()
    {
        ReturValueAsInt = !ReturValueAsInt;
    }
    private void LoadNext()
    {
        SceneManager.LoadScene(5);
    }
    private void LoadPrevious()
    {
        SceneManager.LoadScene(3);
    }
}
