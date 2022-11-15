using System.Collections.Generic;
using UnityEngine;
public class MyPerlinNoiseScript : MonoBehaviour {
    int Texture_X = 50;
    int Texture_Y = 50;
    float ScalingFactor = 5f;
    int XOffset = 100;
    int YOffset = 58;
    private void Start() {
        //SingleGridPointGradient();
        PerlinNoise2();
    }
    private void PerlinNoise2() {
        List<PerlinGrid> PG = new List<PerlinGrid>();
        for (int X = XOffset; X < Mathf.CeilToInt(ScalingFactor) + XOffset + 1; X++) {
            for (int Y = YOffset; Y < Mathf.CeilToInt(ScalingFactor) + YOffset + 1; Y++) {
                PG.Add(new PerlinGrid(X, Y, Random.Range(0f, 360f)));
            }
        }
        Renderer QuadRenderer = GetComponent<Renderer>();
        Texture2D QuadTexture = new Texture2D(Texture_X, Texture_Y);
        for (int X = 0; X < Texture_X; X++) {
            for (int Y = 0; Y < Texture_Y; Y++) {
                int YMin = 0, YMax = 0, XMin = 0, XMax = 0;
                Vector2 UserInput = new Vector2(((((float)X / (float)Texture_X)) * ScalingFactor) + XOffset, ((((float)Y / (float)Texture_Y)) * ScalingFactor) + YOffset);
                if (UserInput.x % 1 == 0) {
                    UserInput.x += .01f;
                    XMin = Mathf.FloorToInt(UserInput.x);
                    XMax = Mathf.CeilToInt(UserInput.x);
                }
                else if (UserInput.x % 1 != 0) {
                    XMin = Mathf.FloorToInt(UserInput.x);
                    XMax = Mathf.CeilToInt(UserInput.x);
                }
                if (UserInput.y % 1 == 0) {
                    UserInput.y += .01f;
                    YMin = Mathf.FloorToInt(UserInput.y);
                    YMax = Mathf.CeilToInt(UserInput.y);
                }
                else if (UserInput.y % 1 != 0) {
                    YMin = Mathf.FloorToInt(UserInput.y);
                    YMax = Mathf.CeilToInt(UserInput.y);
                }
                Vector2[] GridPoints = new Vector2[] {
                    new Vector2(XMin, YMin),
                    new Vector2(XMax, YMin),
                    new Vector2(XMin, YMax),
                    new Vector2(XMax, YMax)
                };
                List<float> DotProductsToLerp = new List<float>();
                foreach (Vector2 CurrentGridPoint in GridPoints) {
                    foreach (PerlinGrid CurrentPGPoint in PG) {
                        if (CurrentPGPoint.GridXLocation == CurrentGridPoint.x && CurrentPGPoint.GridYLocation == CurrentGridPoint.y) {
                            Vector2 UnitVectorCoord = new Vector2(Mathf.Cos(CurrentPGPoint.RandomAngle * Mathf.Deg2Rad) + CurrentGridPoint.x,
                                Mathf.Sin(CurrentPGPoint.RandomAngle * Mathf.Deg2Rad) + CurrentGridPoint.y);
                            Vector2 UnitVectorComponentForm = new Vector2((UnitVectorCoord.x - CurrentGridPoint.x), (UnitVectorCoord.y - CurrentGridPoint.y));
                            Vector2 UserComponentForm = new Vector2((UserInput.x - CurrentGridPoint.x), (UserInput.y - CurrentGridPoint.y));
                            float DP = (((UserComponentForm.x * UnitVectorComponentForm.x) + (UserComponentForm.y * UnitVectorComponentForm.y)) / 2) + .5f;
                            DotProductsToLerp.Add(DP);
                            break;
                        }
                    }
                }
                float FadeXT = (6 * Mathf.Pow((UserInput.x - GridPoints[0].x), 5)) - (15 * Mathf.Pow((UserInput.x - GridPoints[0].x), 4)) + (10 * Mathf.Pow((UserInput.x - GridPoints[0].x), 3));
                float FadeYT = (6 * Mathf.Pow((UserInput.y - GridPoints[0].y), 5)) - (15 * Mathf.Pow((UserInput.y - GridPoints[0].y), 4)) + (10 * Mathf.Pow((UserInput.y - GridPoints[0].y), 3));
                float Result1 = Mathf.Lerp(DotProductsToLerp[0], DotProductsToLerp[1], FadeXT);
                float Result2 = Mathf.Lerp(DotProductsToLerp[2], DotProductsToLerp[3], FadeXT);
                float Output = Mathf.Lerp(Result1, Result2, FadeYT);
                QuadTexture.SetPixel(X, Y, new Color(Output, Output, Output));
            }
        }
        QuadTexture.filterMode = FilterMode.Point;
        QuadTexture.Apply();
        QuadRenderer.material.mainTexture = QuadTexture;
    }
    private void SingleGridPointGradient()
    {
        Renderer QuadRenderer = GetComponent<Renderer>();
        Texture2D QuadTexture = new Texture2D(Texture_X, Texture_Y);
        float UnitVectorAngle = 5;
        float UnitVectorInRadians = Mathf.Deg2Rad * UnitVectorAngle;
        Vector2 GridCoord = new Vector2(1f, 1f);
        Vector2 UnitVectorCoord = new Vector2(Mathf.Cos(UnitVectorInRadians) + GridCoord.x, Mathf.Sin(UnitVectorInRadians) + GridCoord.y);
        Vector2 UnitVectorComponentForm = new Vector2((UnitVectorCoord.x - GridCoord.x), (UnitVectorCoord.y - GridCoord.y));
        for (int x = 0; x < Texture_X; x++) {
            for (int y = 0; y < Texture_Y; y++) {
                Vector2 UserInput = new Vector2((((float)x / (float)Texture_X) * 2), (((float)y / (float)Texture_Y) * 2));
                Vector2 UserComponentForm = new Vector2((UserInput.x - GridCoord.x), (UserInput.y - GridCoord.y));
                float DP = (((UserComponentForm.x * UnitVectorComponentForm.x) + (UserComponentForm.y * UnitVectorComponentForm.y)) / 2) + .5f;
                QuadTexture.SetPixel(x, y, new Color(DP, DP, DP));
            }
        }
        QuadTexture.filterMode = FilterMode.Point;
        QuadTexture.Apply();
        QuadRenderer.material.mainTexture = QuadTexture;
    }

    private float[] CreateSeed(int SeedCount)
    {
        float[] Seed = new float[SeedCount];
        for (int i = 0; i < SeedCount; i++)
        {
            Seed[i] = Random.Range(0f, 360f);
        }
        return Seed;
    }
}
