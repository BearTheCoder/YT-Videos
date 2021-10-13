/*
 * Code iterated from Coding Train for Unity by BearTheCoder: https://www.youtube.com/watch?v=QHEQuoIKgNE
 * 
 * Code is designed around 1920x1080(FHD). Ensure that the resolution of the game view is set to FHD.
 * 
 * Instructions:
 *  1.) Create an empty game object in the scene.
 *  2.) Download this code, import it, and attach it to the game object.
 *  3.) The parameter "Circle" can be any 2D sprite or image with the condition listed below. (Circular is prefered)
 *          A.) If you choose to use an image as your "Circle", if your image is 500x550 for example, in the import settings, set the "pixels 
 *          per unit" to the largest of the two. (e.g 550x500 PPU = 550)
 *  4.) Drag the "Circle" into the scene, reset the transform, and set the X scale and Y scale to "0.01". (Make it small so it can grow)
 *  5.) Make a prefab out of the "Circle" by dragging it into the project folder, then delete it from the scene.
 *  6.) Drag the newly created prefab onto the "Circle" parameter on the script.
 *  7.) The "DownscalingFactor" saves redundant computation by skipping pixels essentially "down scaling" the result. 
 *      (Size of 2 would only sample every other pixel, size of 5 would be every fifth pixel)
 *      Set to 1 for no down scaling. (Recommended 2-4 for a 1920x1080 FHD image.)
 *  8.) The "GrowSpeed" controls how fast the "Circles" grow. (Lower = Faster)
 *  9.) The "SizeToGrowPerFrame" controls by how much each circle grow per frame.
 *  10.) The "MaximumCircleCount" limits the amount of circles that can be spawned in.
 * 
 *              If you set all number parameters to 0 it will load in my "recommended" values.
 * 
 *  [OPTIONALS]
 *  1.) I also iterated on a screenshot script from CodeMonkey that is great for this project.
 *      Add it to the camera to save the project as an image in the pictures folder.
 *      Code can be found here: https://github.com/BearTheCoder/YT-Videos/blob/main/ScreenshotController.cs
 *  
 *  This code was used in this YouTube video: (if this is still here, I haven't released the video yet, or I am lazy and haven't updated this)
 *  Follow me on YouTube: https://www.youtube.com/channel/UCWg8LAQk6NLQfj4Wr3zImKA
 *  Yell at me on Twitter: https://twitter.com/BearTheCoder
 *
 *  Ok, bye.
 *  
 */

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class CirclePackingFullScreen : MonoBehaviour {
    [SerializeField] GameObject Circle;
    [SerializeField] int MaximumCircleCount;
    [SerializeField] float GrowSpeed;
    [SerializeField] float SizeToGrowPerFrame;
    [SerializeField] int DownscalingFactor;
    private List<Vector2> Locations;
    private int CountOfCurrentCirlces;
    private List<GameObject> AllCircles;
    private List<GameObject> CirclesToGrow;
    void Start() {
        GrowSpeed = GrowSpeed <= 0f ? .01f : GrowSpeed;
        DownscalingFactor = DownscalingFactor <= 0 ? 1 : DownscalingFactor;
        MaximumCircleCount = MaximumCircleCount <= 0 ? 5000 : MaximumCircleCount;
        SizeToGrowPerFrame = SizeToGrowPerFrame <= 0f ? .02f : SizeToGrowPerFrame;
        CountOfCurrentCirlces = 0;
        Locations = new List<Vector2>();
        AllCircles = new List<GameObject>();
        CirclesToGrow = new List<GameObject>();
        Camera.main.transform.position = new Vector3(9.44445f, 5f, -10f);
        CreateLocations();
        SpawnFirstCircle();
        StartCoroutine("GrowCircles");
        StartCoroutine("SpawnNewCircle");
    }
    private void CreateLocations() {
        for (int i = 0; i < 1920; i += DownscalingFactor) {
            for (int j = 0; j < 1080; j += DownscalingFactor) {
                float XLoc = ((float)i / (float)1920) * 18.88889f;
                float YLoc = ((float)j / (float)1080) * 10f;
                Locations.Add(new Vector2(XLoc, YLoc));
            }
        }
    }
    private void SpawnFirstCircle() {
        Vector3 Location = Locations[Random.Range(0, Locations.Count)];
        GameObject CurrentGO = Instantiate(Circle, Location, Quaternion.Euler(0, 0, Random.Range(0, 360)), transform);
        AllCircles.Add(CurrentGO);
        CirclesToGrow.Add(CurrentGO);
        CountOfCurrentCirlces++;
    }
    private IEnumerator SpawnNewCircle() {
        bool LocationAvailable = true; // Prevents errors caused by to low of a resolution and too high of a maximum circle count.
        while (LocationAvailable) {
            yield return new WaitForSeconds(0f);
            try {
                if (CountOfCurrentCirlces < MaximumCircleCount) {
                    int RandNum = Random.Range(0, Locations.Count);
                    Vector3 Location = Locations[RandNum];
                    bool CanSpawn = true;
                    for (int i = 0; i < AllCircles.Count; i++) {
                        float Dist = Vector3.Distance(AllCircles[i].transform.position, Location);
                        if (Dist - (AllCircles[i].transform.localScale.x / 2) < 0) {
                            CanSpawn = false;
                            Locations.RemoveAt(RandNum);
                            break;
                        }
                    }
                    if (CanSpawn) {
                        GameObject GO = Instantiate(Circle, Location, Quaternion.Euler(0, 0, Random.Range(0, 360)), transform);
                        AllCircles.Add(GO);
                        CirclesToGrow.Add(GO);
                        Locations.RemoveAt(RandNum);
                        CountOfCurrentCirlces++;
                    }
                }
            }
            catch { LocationAvailable = false; }
        }
    }
    private IEnumerator GrowCircles() {
        while (true) {
            yield return new WaitForSeconds(GrowSpeed);
            for (int i = 0; i < CirclesToGrow.Count; i++) {
                bool CanGrow = true;
                for (int j = 0; j < AllCircles.Count; j++) {
                    if (CirclesToGrow[i] != AllCircles[j]) {
                        float Dist = Vector3.Distance(CirclesToGrow[i].transform.position, AllCircles[j].transform.position);
                        float Radi = (CirclesToGrow[i].transform.localScale.x / 2) + (AllCircles[j].transform.localScale.x / 2);
                        if (Radi >= Dist) {
                            CanGrow = false;
                            CirclesToGrow.RemoveAt(i);
                            break;
                        }
                    }
                }
                if (CanGrow) { CirclesToGrow[i].transform.localScale += new Vector3(SizeToGrowPerFrame, SizeToGrowPerFrame, 0); }
            }
        }
    }
}
