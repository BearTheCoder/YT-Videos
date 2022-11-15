using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(BoxCollider))]
[RequireComponent(typeof(Rigidbody))]
public class TileEditor : MonoBehaviour
{
    /*
    The following code creates a sqaure mesh in unity then sets the mesh's texture to Perlin Noise.
    */

    public int OctaveValue, PerlinScale, Frequency;
    public float Feathering;
    Vector3[] Vertices;
    int[] Triangles;
    void Start(){
        SetTrigger();
        SetToplogy();
        DrawMesh();
        SetPerlinMaterial();
    }
    private void SetTrigger()
    {
        Rigidbody RB = GetComponent<Rigidbody>();
        BoxCollider BC = GetComponent<BoxCollider>();
        RB.mass = 0;
        RB.angularDrag = 0;
        BC.isTrigger = true;
    }
    private void SetToplogy() {
        Vertices = new Vector3[4];
        Triangles = new int[6];
        Vertices[0] = new Vector3(-5.5f, 0, -5.5f);
        Vertices[1] = new Vector3(-5.5f, 0, 5.5f);
        Vertices[2] = new Vector3(5.5f, 0, -5.5f);
        Vertices[3] = new Vector3(5.5f, 0, 5.5f);
        Triangles[0] = 0;
        Triangles[1] = 1;
        Triangles[2] = 2;
        Triangles[3] = 2;
        Triangles[4] = 1;
        Triangles[5] = 3;
    }
    private void DrawMesh(){
        Vector2[] UVVectors = {
            new Vector2(-5.5f, -5.5f),
            new Vector2(-5.5f, 5.5f),
            new Vector2(5.5f, -5.5f),
            new Vector2(5.5f, 5.5f)
        };
        Mesh CurrentMesh = new Mesh();
        CurrentMesh.Clear();
        CurrentMesh.vertices = Vertices;
        CurrentMesh.triangles = Triangles;
        CurrentMesh.RecalculateNormals();
        CurrentMesh.name = "Tile";
        CurrentMesh.uv = UVVectors;
        GetComponent<MeshFilter>().mesh = CurrentMesh;
    }


    private void SetPerlinMaterial()
    {
        Vector3 Pos = transform.position / 11;
        Shader shader = Shader.Find("Unlit/Texture");
        MeshRenderer MeshRendererComponent = GetComponent<MeshRenderer>();
        MeshRendererComponent.material = new Material(shader);
        const int TextureSize = 200;
        float MaxValue = 0f;
        float MinValue = 0f;
        Texture2D TileTexture = new Texture2D(TextureSize, TextureSize);
        List<Color> ColorList = new List<Color>();
        for (int x = 0; x < TextureSize; x++)
        {
            for (int y = 0; y < TextureSize; y++)
            {
                float Result = 0f;
                float OctaveOpacity = 1;
                float OctaveFrequency = 1;
                for (int i = 0; i < OctaveValue; i++)
                {
                    float xCoord = ((((float)x / TextureSize) * OctaveFrequency) * PerlinScale);
                    float yCoord = ((((float)y / TextureSize) * OctaveFrequency) * PerlinScale);
                    float PerlinOutput = Mathf.PerlinNoise(xCoord, yCoord) * 2 - 1;
                    Result += PerlinOutput * OctaveOpacity;
                    OctaveOpacity *= Feathering;
                    OctaveFrequency *= Frequency;
                }
                if (Result > MaxValue) { MaxValue = Result; }
                else if (Result < MinValue) { MinValue = Result; }
                ColorList.Add(new Color(Result, Result, Result));
            }
        }
        for (int x = 0, i = 0; x < TextureSize; x++)
        {
            for (int y = 0; y < TextureSize; y++)
            {
                if (x % 20 == 10 || y % 20 == 10)
                {
                    TileTexture.SetPixel(x, y, new Color(0f, .2f, 0f));
                }
                else
                {
                    float InverseLerpResult = Mathf.InverseLerp(MinValue, MaxValue, ColorList[i].r);
                    Color PixelColor = new Color(.1f, InverseLerpResult, .3f, InverseLerpResult);
                    float Hue, Saturation, Value;
                    Color.RGBToHSV(PixelColor, out Hue, out Saturation, out Value);
                    Color HSV = Color.HSVToRGB(Hue, .7f, .7f);
                    TileTexture.SetPixel(x, y, HSV);
                }
                i++;
            }
        }
        TileTexture.Apply();
        MeshRendererComponent.material.mainTextureScale = new Vector2(.1f, .1f);
        MeshRendererComponent.material.mainTextureOffset = new Vector2(.5f, .5f);
        MeshRendererComponent.material.mainTexture = TileTexture;
    }
}
