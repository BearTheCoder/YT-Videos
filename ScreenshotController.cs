/* 
 * Code iterated from Code Monkey for Unity on Windows by BearTheCoder: https://www.youtube.com/watch?v=lT-SRLKUe5k
 *
 * The camera background will not save and instead will be rendered as a transparent alpha.
 *
 * Instructions:
 *  1.) Attach file to the camera in the scene.
 *  2.) Press '0' on the top of the keyboard to take screen shot.
 *  3.) Screenshots will save to Pictures folder.
 *  4.) Uncomment line 42 if you are like me and need feedback that the code worked.
 *  
 *  Follow me on YT: https://www.youtube.com/channel/UCWg8LAQk6NLQfj4Wr3zImKA
 *  Yell at me on Twitter: https://twitter.com/BearTheCoder
 *  
 *  Ok, bye.
 *  
 */

using UnityEngine;
public class ScreenshotController : MonoBehaviour {
    private bool CanTakeScreenshot = false;
    private void Update() {
        if (Input.GetKeyDown(KeyCode.Alpha0)) { CanTakeScreenshot = true; }
    }
    private void OnPostRender() {
        if (CanTakeScreenshot) {
            gameObject.GetComponent<Camera>().targetTexture = RenderTexture.GetTemporary(Screen.width, Screen.height);
            RenderTexture CameraTexture = gameObject.GetComponent<Camera>().targetTexture;
            Texture2D NewTexture = new Texture2D(CameraTexture.width, CameraTexture.height, TextureFormat.ARGB32, false);
            Rect NewRectangle = new Rect(0, 0, CameraTexture.width, CameraTexture.height);
            NewTexture.ReadPixels(NewRectangle, 0, 0);
            byte[] ImageBytes = NewTexture.EncodeToPNG();
            System.IO.File.WriteAllBytes(System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyPictures)
                + "/Screenshot_" + System.DateTime.Now.ToString("MMddyyyyhhmmss") + ".png", ImageBytes);
            RenderTexture.ReleaseTemporary(CameraTexture);
            gameObject.GetComponent<Camera>().targetTexture = null;
            CanTakeScreenshot = false;
            Debug.Log("Screenshot saved.");
        }
    }
}
