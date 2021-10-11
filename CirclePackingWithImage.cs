/*
 * Code iterated from CodingTrain for Unity: https://www.youtube.com/watch?v=QHEQuoIKgNE
 * 
 * This currently works best at 1920x1080(FHD) or multiples of that (e.g. 480x270 is 4x smaller).
 * 
 * Instructions:
 *  1.) Import the FHD or similar image that you wish to use with the algorithm. (Algorith will ignore full transparent pixels)
 *  2.) In the import settings for the image ensure that Read/Write enabled box is checked.
 *  3.) Drag the image into the scene and move it away from the view of the camera. (Negative X or Y Direction)
 *  4.) Drag this script onto the image.
 *  5.) The parameter "Circle" can be any 2D sprite or image with the two conditions listed below. (Also circular is prefered)
 *          A.) If you choose to use an image as your "Circle", if your image is 500x550 for example, in the import settings, set the "pixels 
 *          per unit" to the largest of the two. (e.g 550x500 PPU = 550)
 *          B.) While any image can be used for the "Circle", if it is not white in color it will not be tinted properly.
 *  6.) Drag the "Circle" into the scene, reset the transform, and set the X scale and Y scale to "0.01". (Make it small so it can grow)
 *  7.) Make a prefab out of the "Circle" by dragging it into the project folder, then delete it from the scene.
 *  8.) Drag the newly created prefab onto the "Circle" parameter on the script.
 *  9.) The "ResolutionSize" saves redundant computation by skipping pixels essentially "down scaling" the result. 
 *      (Size of 2 would only sample every other pixel, size of 5 would be every fifth pixel)
 *      Set to 1 for no down scaling. (Recommended 2-4 for a 1920x1080 FHD image.)
 *  10.) The "GrowSpeed" controls how fast the "Circles" grow, lower = faster. (0.2 is default, test what works best for your image.)
 *  
 *  [OPTIONALS]
 *  1.) Change the camera color to a neutral grayscale color. (Anywhere from white to black)
 *  2.) I also iterated on a screenshot script from CodeMonkey that is great for this project.
 *      Add it to the camera to save the project as an image.
 *      Code can be found here: https://github.com/BearTheCoder/YT-Videos/blob/main/ScreenshotController.cs
 *  
 *  Follow me on YT: https://www.youtube.com/channel/UCWg8LAQk6NLQfj4Wr3zImKA
 *  Yell at me on Twitter: https://twitter.com/BearTheCoder
 *
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class CirclePackingWithImage : MonoBehaviour
{
    [SerializeField] GameObject Circle;
    [SerializeField] int ResolutionSize;
    [SerializeField] float GrowSpeed;
    private List<GameObject> AllCircles;
    private List<GameObject> CirclesToGrow;
    private List<Vector2> Vector2List;
    private List<Color> ColorList;
    void Start()
    {
        ResolutionSize = ResolutionSize <= 0 ? 1 : ResolutionSize;
        GrowSpeed = GrowSpeed < .2f ? .2f : GrowSpeed;
        AllCircles = new List<GameObject>();
        CirclesToGrow = new List<GameObject>();
        Vector2List = new List<Vector2>();
        ColorList = new List<Color>();
        Camera.main.transform.position = new Vector3(9.44445f, 5f, -10f);
        ReadTexture();
        SpawnFirstCircle();
        StartCoroutine("SpawnNewCircle");
        StartCoroutine("GrowCircles");
    }
    private void ReadTexture()
    {
        Sprite sprite = gameObject.GetComponent<SpriteRenderer>().sprite;
        for (int i = 0; i < sprite.texture.width; i += ResolutionSize)
        {
            for (int j = 0; j < sprite.texture.height; j += ResolutionSize)
            {
                if (sprite.texture.GetPixel(i, j).a != 0)
                {
                    float XLoc = ((float)i / (float)sprite.texture.width) * 18.88889f;
                    float YLoc = ((float)j / (float)sprite.texture.height) * 10f;
                    Vector2List.Add(new Vector2(XLoc, YLoc));
                    ColorList.Add(sprite.texture.GetPixel(i, j));
                }
            }
        }
    }
    private void SpawnFirstCircle()
    {
        int RandNum = Random.Range(0, Vector2List.Count);
        Vector3 Location = Vector2List[RandNum];
        GameObject CurrentGO = Instantiate(Circle, Location, Quaternion.Euler(0, 0, Random.Range(0, 360)), transform);
        CurrentGO.GetComponent<SpriteRenderer>().color = ColorList[RandNum];
        AllCircles.Add(CurrentGO);
        CirclesToGrow.Add(CurrentGO);
        Vector2List.RemoveAt(RandNum);
        ColorList.RemoveAt(RandNum);
    }
    private IEnumerator SpawnNewCircle()
    {
        while (true)
        {
            yield return new WaitForSeconds(0f);
            int RandNum = Random.Range(0, Vector2List.Count);
            Vector3 Location = Vector2List[RandNum];
            bool CanSpawn = true;
            for (int i = 0; i < AllCircles.Count; i++)
            {
                float Dist = Vector3.Distance(AllCircles[i].transform.position, Location);
                if (Dist - (AllCircles[i].transform.localScale.x / 2) < 0)
                {
                    CanSpawn = false;
                    Vector2List.RemoveAt(RandNum);
                    ColorList.RemoveAt(RandNum);
                    break;
                }
            }
            if (CanSpawn)
            {
                GameObject GO = Instantiate(Circle, Location, Quaternion.Euler(0, 0, Random.Range(0, 360)), transform);
                GO.GetComponent<SpriteRenderer>().color = ColorList[RandNum];
                AllCircles.Add(GO);
                CirclesToGrow.Add(GO);
                Vector2List.RemoveAt(RandNum);
                ColorList.RemoveAt(RandNum);
            }
        }
    }
    private IEnumerator GrowCircles()
    {
        while (true)
        {
            yield return new WaitForSeconds(GrowSpeed);
            for (int i = 0; i < CirclesToGrow.Count; i++)
            {
                bool CanGrow = true;
                for (int j = 0; j < AllCircles.Count; j++)
                {
                    if (CirclesToGrow[i] != AllCircles[j])
                    {
                        float Dist = Vector3.Distance(CirclesToGrow[i].transform.position, AllCircles[j].transform.position);
                        float Radi = (CirclesToGrow[i].transform.localScale.x / 2) + (AllCircles[j].transform.localScale.x / 2);
                        if (Radi >= Dist)
                        {
                            CanGrow = false;
                            CirclesToGrow.RemoveAt(i);
                            break;
                        }
                    }
                }
                if (CanGrow)
                {
                    CirclesToGrow[i].transform.localScale += new Vector3(.03f, .03f, 0);
                }
            }
        }
    }
}
