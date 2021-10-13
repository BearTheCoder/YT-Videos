/*
 * Code iterated from Coding Train for Unity by BearTheCoder: https://www.youtube.com/watch?v=QHEQuoIKgNE
 * 
 * This currently works best at 1920x1080(FHD) or multiples of that (e.g. 480x270 is 4x smaller).
 * For best results, allow the application to run in background and let the program to run for longer.
 * 
 * Instructions:
 *  1.) Import the FHD or similar image that you wish to use with the algorithm. (Algorith will ignore full transparent pixels)
 *  2.) In the import settings for the image ensure that Read/Write enabled box is checked.
 *  3.) Drag the image into the scene and move it away from the view of the camera. (Negative X or Y Direction)
 *  4.) Drag this script onto the image.
 *  5.) The parameter "Circle" can be any 2D sprite or image with the two conditions listed below. (Circular is prefered)
 *          A.) If you choose to use an image as your "Circle", if your image is 500x550 for example, in the import settings, set the "pixels 
 *          per unit" to the largest of the two. (e.g 550x500 PPU = 550)
 *          B.) While any image can be used for the "Circle", if it is not white in color it will not be tinted properly.
 *  6.) Drag the "Circle" into the scene, reset the transform, and set the X scale and Y scale to "0.01". (Make it small so it can grow)
 *  7.) Make a prefab out of the "Circle" by dragging it into the project folder, then delete it from the scene.
 *  8.) Drag the newly created prefab onto the "Circle" parameter on the script.
 *  9.) The "DownscalingFactor" saves redundant computation by skipping pixels essentially "down scaling" the result. 
 *      (Size of 2 would only sample every other pixel, size of 5 would be every fifth pixel)
 *      Set to 1 for no down scaling. (Recommended 2-4 for a 1920x1080 FHD image.)
 *  10.) The "GrowSpeed" controls how fast the "Circles" grow. (Lower = Faster)
 *  11.) The "SizeToGrowPerFrame" controls by how much each circle grow per frame.
 *  
 *              If you set all numbers to 0 it will load in my "recommended" values for a FHD image containing large text.
 *              This is not a perfect value, every image is different, play with the numbers until you get what you want.
 *  
 *  [OPTIONALS]
 *  1.) Change the camera color to a neutral grayscale color. (Anywhere from white to black)
 *  2.) I also iterated on a screenshot script from CodeMonkey that is great for this project.
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
public class CirclePackingWithImage : MonoBehaviour {
    [SerializeField] GameObject Circle; 
    [SerializeField] float GrowSpeed;
    [SerializeField] float SizeToGrowPerFrame;
    [SerializeField] int DownscalingFactor;
    private List<Color> ColorList;
    private List<Vector2> Vector2List;
    private List<GameObject> AllCircles;
    private List<GameObject> CirclesToGrow;
    void Start() {
        GrowSpeed = GrowSpeed <= 0f ? .02f : GrowSpeed;
        DownscalingFactor = DownscalingFactor <= 0 ? 1 : DownscalingFactor;
        SizeToGrowPerFrame = SizeToGrowPerFrame <= 0f ? .02f : SizeToGrowPerFrame;
        ColorList = new List<Color>();
        Vector2List = new List<Vector2>();
        AllCircles = new List<GameObject>();
        CirclesToGrow = new List<GameObject>();
        Camera.main.transform.position = new Vector3(9.44445f, 5f, -10f);
        ReadTexture();
        SpawnFirstCircle();
        StartCoroutine("GrowCircles");
        StartCoroutine("SpawnNewCircle");
    }
    private void ReadTexture() {
        Sprite sprite = gameObject.GetComponent<SpriteRenderer>().sprite;
        for (int i = 0; i < sprite.texture.width; i += DownscalingFactor) {
            for (int j = 0; j < sprite.texture.height; j += DownscalingFactor) {
                if (sprite.texture.GetPixel(i, j).a != 0) {
                    float XLoc = ((float)i / (float)sprite.texture.width) * 18.88889f;
                    float YLoc = ((float)j / (float)sprite.texture.height) * 10f;
                    Vector2List.Add(new Vector2(XLoc, YLoc));
                    ColorList.Add(sprite.texture.GetPixel(i, j));
                }
            }
        }
    }
    private void SpawnFirstCircle() {
        int RandNum = Random.Range(0, Vector2List.Count);
        Vector3 Location = Vector2List[RandNum];
        GameObject CurrentGO = Instantiate(Circle, Location, Quaternion.Euler(0,0, Random.Range(0, 360)), transform);
        CurrentGO.GetComponent<SpriteRenderer>().color = ColorList[RandNum];
        AllCircles.Add(CurrentGO);
        CirclesToGrow.Add(CurrentGO);
        Vector2List.RemoveAt(RandNum);
        ColorList.RemoveAt(RandNum);
    }
    private IEnumerator SpawnNewCircle() {
        bool LocationsAvailable = true; // Prevents error from all available locations being used.
        while (LocationsAvailable) {
            yield return new WaitForSeconds(0f);
            try {
                int RandNum = Random.Range(0, Vector2List.Count);
                Vector3 Location = Vector2List[RandNum];
                bool CanSpawn = true;
                for (int i = 0; i < AllCircles.Count; i++) {
                    float Dist = Vector3.Distance(AllCircles[i].transform.position, Location);
                    if (Dist - (AllCircles[i].transform.localScale.x / 2) < 0) {
                        CanSpawn = false;
                        Vector2List.RemoveAt(RandNum);
                        ColorList.RemoveAt(RandNum);
                        break;
                    }
                }
                if (CanSpawn) {
                    GameObject GO = Instantiate(Circle, Location, Quaternion.Euler(0, 0, Random.Range(0, 360)), transform);
                    GO.GetComponent<SpriteRenderer>().color = ColorList[RandNum];
                    AllCircles.Add(GO);
                    CirclesToGrow.Add(GO);
                    Vector2List.RemoveAt(RandNum);
                    ColorList.RemoveAt(RandNum);
                }
            }
            catch { LocationsAvailable = false; }
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
