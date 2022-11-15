using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
public class perlinnoise : MonoBehaviour {
    // Global Variables
    public Slider WidthSlider, HeightSlider, ScaleSlider, XOffsetSlider, YOffsetSlider, OctaveSlider, FrequencySlider, FeatheringSlider;
    public Text WidthSliderValueText, HeightSliderValueText, ScaleSliderValueText, XOffsetSliderValueText, 
        YOffsetSliderValueText, OctaveSliderValueText, FrequencySliderValueText, FeatheringSliderValueText;
    public Button NextButton, PreviousButton;
    private float MaxValue, MinValue;
    private void Start() {
        NextButton.onClick.AddListener(LoadNextScene);
        PreviousButton.onClick.AddListener(LoadPreviousScene);
    }
    private void Update() {
        Renderer QuadRenderer = GetComponent<Renderer>();
        QuadRenderer.material.mainTexture = GenerateTexture();
        SetTextValues();
        if (Input.GetKeyDown(KeyCode.Escape)) { Application.Quit(); }
    }
    Texture2D GenerateTexture() {
        int TextureWidth = (int)WidthSlider.value;
        int TextureHeight = (int)HeightSlider.value;
        MaxValue = 0f;
        MinValue = 0f;
        Texture2D QuadTexture = new Texture2D(TextureWidth, TextureHeight);
        List<Color> ColorList = new List<Color>();
        for (int x = 0; x < TextureWidth; x++) {
            for(int y = 0; y < TextureHeight; y++) {
                float Result = 0f;
                float OctaveOpacity = 1;
                float OctaveFrequency = 1;
                for (int i = 0; i < OctaveSlider.value; i++)
                {
                    float xCoord = ((((float)x / TextureWidth) * OctaveFrequency) * ScaleSlider.value) + XOffsetSlider.value;
                    float yCoord = ((((float)y / TextureHeight) * OctaveFrequency) * ScaleSlider.value) + YOffsetSlider.value;
                    float PerlinOutput = Mathf.PerlinNoise(xCoord, yCoord) * 2 -1;
                    Result += PerlinOutput * OctaveOpacity;
                    OctaveOpacity *= FeatheringSlider.value;
                    OctaveFrequency *= FrequencySlider.value;
                }
                if (Result > MaxValue) { MaxValue = Result; }
                else if (Result < MinValue) { MinValue = Result; }
                ColorList.Add(new Color(Result, Result, Result));
            }
        }
        for (int x = 0, i = 0; x < TextureWidth; x++) {
            for (int y = 0; y < TextureHeight; y++) {
                float InverseLerpResult = Mathf.InverseLerp(MinValue, MaxValue, ColorList[i].r);
                QuadTexture.SetPixel(x, y, new Color(InverseLerpResult, InverseLerpResult, InverseLerpResult));
                i++;
            }
        }
        QuadTexture.Apply();
        return QuadTexture;
    }


    // Scene Management vvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvv
    private void LoadNextScene() {
        SceneManager.LoadScene(3);
    }
    private void LoadPreviousScene() {
        SceneManager.LoadScene(1);
    }
    private void SetTextValues() {
        WidthSliderValueText.text = WidthSlider.value.ToString();
        HeightSliderValueText.text = HeightSlider.value.ToString();
        ScaleSliderValueText.text = ScaleSlider.value.ToString();
        XOffsetSliderValueText.text = XOffsetSlider.value.ToString();
        YOffsetSliderValueText.text = YOffsetSlider.value.ToString();
        OctaveSliderValueText.text = OctaveSlider.value.ToString();
        FrequencySliderValueText.text = FrequencySlider.value.ToString();
        FeatheringSliderValueText.text = FeatheringSlider.value.ToString();
    }
}
