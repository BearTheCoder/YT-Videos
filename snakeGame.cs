using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Linq;
using System.Xml.Linq;
using System.IO;
public class SnakeGameController : MonoBehaviour{
    public InputField ESInitialsField;
    public Canvas SSCanvas, GSCanvas, ESCanvas;
    public Button SSStartButton, SSExitButton, ESSubmitButton, ESSkipButton;
    public Text SSGameSpeedValueText, SSGameSizeValueText, SSLeaderboardText, GSScoreValueText, GSApplesToWinValueText, ESScoreValueText;
    public Slider SSGameSpeedSlider, SSGameSizeSlider;
    public GameObject AppleBlock, SnakeBlock, BodyBlock, BorderBlock;
    public AudioSource TitleSong, SnakeDeathSound, AppleEatSound, ButtonSelectSound;
    private Vector2 CameraBounds;
    private List<XElement> ListOfLeaderboardXElements;
    private List<Vector3> ListOfMoves;
    private List<GameObject> ListOfBodyParts, ListOfBorderBlocks;
    private GameObject SnakeHead, CurrentApple;
    private int CountOfSnakeBodyParts, GameSpeedCounter, GameSpeed, ApplesEaten, ApplesToEatMath;
    private bool GameStart, MoveUp, MoveDown, MoveLeft, MoveRight, SelectionMade, GameOver;
    private float DeathBorderX, DeathBorderY, BorderAreaX, BorderAreaY, MovementX, MovementY, GameSize, InternalGameScore;
    private readonly string LeaderboardPath = "Leaderboard.xml";

    // Unity Functions
    void Start() {
        InitializeVariables(); // DeadEnd
        ControlBools(true, false, false, false, false, false); //DeadEnd
        LoadLeaderboard();
    }
    void Update() {
        if (GameStart && !GameOver) {
            MoveSnakeHead();
            GameOverIf();
            AppleCheck();
            UpdatePositions(); //DeadEnd
        }
        else if (!GameOver) {
            SSGameSpeedValueText.text = SSGameSpeedSlider.value.ToString();
            SSGameSizeValueText.text = SSGameSizeSlider.value.ToString();
        }
        if (Input.GetKeyDown(KeyCode.Escape)) {
            Application.Quit();
        }
    }

    //Player Functions
    private void InitializeVariables() {

        // Summary:
        //      The three "AddListener" functions assign functions to the "click" event of the in game buttons. The functions fire when the button is clicked.
        //      The variable "Counter" increments from 0 to the user defined GameSpeed. 
        //          >>> It helps controls the timing of the game and needs to be reset to 0 at every start of the game.
        //      The variable "CountOfSnakeBodyParts" controls exactly what it sounds like.
        //      The variable "InternalGameScore" is the game score that is seen by the code. How it is displayed in game is controlled elsewhere.
        //          >>> This needs to be reset at every start for obvious reason.
        //      The list "ListOfMoves" is just that, a list of every move taken by the snake head.
        //          >>> This is used in tandem with "ListOfBodyParts" to move the body in a classic snake game fashion.
        //      The list "ListOfLearderBoardXElements" is a entire list of XML elements from "Leaderboard.xml" that will be sorted to find the Leaderboard's top ten.
        //      The lists "ListOfBodyParts" and "ListOfBorderBlocks" are both self explanitory.

        TitleSong.Play();
        Application.targetFrameRate = 60;
        GameSpeedCounter = 0;
        InternalGameScore = 0;
        CountOfSnakeBodyParts = 0;
        SSStartButton.onClick.AddListener(InitializeGame);
        SSExitButton.onClick.AddListener(ReturnToMainMenu);
        ESSkipButton.onClick.AddListener(SkipLeaderboard);
        ESSubmitButton.onClick.AddListener(EndGame);
        ListOfMoves = new List<Vector3>();
        ListOfBodyParts = new List<GameObject>();
        ListOfBorderBlocks = new List<GameObject>();
        ListOfLeaderboardXElements = new List<XElement>();
    }
    private void InitializeGame() {

        // Summary:
        //      The following functions are order specific.

        ButtonSelectSound.Play();
        CameraBounds = GetCameraBounds(Camera.main); //DeadEnd
        AssignSpecifications(CameraBounds); //DeadEnd
        DrawBorder(CameraBounds); //DeadEnd
        SpawnApple(CameraBounds); //DeadEnd
        Vector2 ApplesToEatVector = new Vector2(((CameraBounds.x / GameSize) * 2) - 1, ((CameraBounds.y / GameSize) * 2) - 1);
        ApplesToEatMath = (int)((ApplesToEatVector.x * ApplesToEatVector.y) - 1);
        GSApplesToWinValueText.text = ApplesToEatMath.ToString();
        SnakeHead = Instantiate(SnakeBlock, new Vector3(GameSize, GameSize, 0f), Quaternion.identity);
        SnakeHead.transform.localScale = new Vector3(GameSize, GameSize, 0f);
        TitleSong.Stop();
        ControlBools(false, true, false, false, true, false);
    }
    private void ReturnToMainMenu() {
        ButtonSelectSound.Play();
        SceneManager.LoadScene("GameSelectionScreen");
    }
    private void SkipLeaderboard() {
        ButtonSelectSound.Play();
        SceneManager.LoadScene("SnakeGame");
    }
    private void EndGame() {
        ButtonSelectSound.Play();
        if (ESInitialsField.text.Length == 3) {
            string CurrentInfo = ESInitialsField.text.ToUpper() + " " + DateTime.Now + " " + SSGameSizeValueText.text + "-" + (int)GameSpeed + " SCORE: ";
            string CurrentScore = InternalGameScore.ToString();
            if (File.Exists(LeaderboardPath)) {
                XDocument Doc = XDocument.Load(LeaderboardPath);
                XElement Root = Doc.Element("ROOT");
                XElement Child = new XElement("INFO");
                XElement Grandchild = new XElement("SCORE");
                Child.Add(CurrentInfo);
                Grandchild.Add(CurrentScore);
                Child.Add(Grandchild);
                Root.Add(Child);
                Doc.Save(LeaderboardPath);
            }
            else {
                XDocument Doc = new XDocument();
                XDeclaration Declerations = new XDeclaration("1.0", string.Empty, string.Empty);
                XElement Root = new XElement("ROOT");
                XElement Child = new XElement("INFO");
                XElement Grandchild = new XElement("SCORE");
                Child.Add(CurrentInfo);
                Grandchild.Add(CurrentScore);
                Child.Add(Grandchild);
                Root.Add(Child);
                Doc.Add(Root);
                Doc.Save(LeaderboardPath);
            }
            SceneManager.LoadScene("SnakeGame");
        }
    }
    private void ControlBools(bool UICLocal, bool GCLocal, bool ESCLocal, bool IFLocal, bool GSLocal, bool GOLocal) {

        // Summary:
        //     Instead of using different scenes, three UI canvas' were used.
        //     The three "Canvas" variables below control the visibility of each canvas.
        //     Because of this the "InitialsField" variable had to be turned on and off as it was still taking input while it's parent canvas was turned off.
        //     The "GameStart" and "GameOver" variable do exactly what it sounds like, signal the start and stop of the game.

        SSCanvas.enabled = UICLocal;
        GSCanvas.enabled = GCLocal;
        ESCanvas.enabled = ESCLocal;
        ESInitialsField.enabled = IFLocal;
        GameStart = GSLocal;
        GameOver = GOLocal;
    }
    private void LoadLeaderboard() {

        // Summary:
        //      LoadLeaderboard checks to see if the XML file "Leaderboard.xml" exists and if it does then it sorts all entries in the XML file into a list.
        //          >>> This list is sorted by the "SCORE" element from highest to lowest. Ultimately the first ten are displayed on the Leaderboard.
        //      We load the file with an XDocument varable acheived through System.XML.Linq.
        //      We grab all elements from the XML file using "Doc.Descendants("INFO")" and save it as an IEnumerable.
        //      We used a nested foreach loop here to sort the XElements into the list "ListOfLeaderboardXElements".
        //          >>> The sorting is done with the highscore.
        //          >>> The second foreach loop looks at each XElement and if it is higher that the prior XElement, then it is set as the "HighestNumber".
        //          >>> At the end of the second foreach loop "HighestNumber" is literally just that, the highest score in the XML file.
        //              >>> This "HighestNumber" is then added into "ListOfLeaderboardXElements".
        //          >>> Now with the second foreach loop over, the first foreach loop iterates and the second foreach loop starts again
        //          >>> The "HighestNumber" from the previous iteration needs to be removed.
        //              >>> If not removed, each entry into "ListOfLeaderboardXElements" will all be the same number.
        //          >>> Though you can't remove items from a list when iterating over them.
        //          >>> Therefore each XElement is time stamped making each entry unique, with the added benefit of giving the leaderboard more info.
        //              >>> Therefore if we add the XElement to a list, with each iteration we can check this list to see if it has already been added.
        //                  >>> If it hasn't we continue as normal, if it has, we skip that entry.
        //                  >>> The result is that "ListOfLeaderboardXElements" is now sorted from highest to lowest score starting a index 0.
        //          >>> We then call "FillLeaderboard" to format and display the results.

        if (File.Exists(LeaderboardPath)) {
            XDocument Doc = XDocument.Load(LeaderboardPath);
            IEnumerable<XElement> XList = Doc.Descendants("INFO");
            foreach (XElement XE1 in XList) {
                XElement XElementToBeAdded = null;
                float HighestNumber = -1;
                foreach (XElement XE2 in XList) {
                    if (!ListOfLeaderboardXElements.Contains(XE2)) {
                        if (float.Parse(XE2.Descendants("SCORE").First().Value) >= HighestNumber) {
                            HighestNumber = float.Parse(XE2.Descendants("SCORE").First().Value);
                            XElementToBeAdded = XE2;
                        }
                    }
                }
                if (XElementToBeAdded != null) {
                    ListOfLeaderboardXElements.Add(XElementToBeAdded);
                }
            }
            FillLeaderboard(); //DeadEnd
        }
    }
    private void FillLeaderboard() {

        // Summary:
        //      This function formats the "SCORE" element into a more readable percentage format using ".ToString("p")".
        //          >>> This was done this way because if you save the score as a percentage instead of a float, you cannot parse the string back to a float.
        //          >>> You need the score to be parse-able in order to sort the scores from highest to lowest.
        //      We for loop X-amount of times to get the first X-amount of entries of the list to have the top X-amount of scores.
        //      We use ".Nodes().OfType<XText>().First().Value" to grab only the text and not the children's text so we can format the score.
        //      Lastly everything is concatenated together to form one string, and then displayed in game.

        string LeaderboardString = "LEADERBOARDS" + "\n" + "\n";
        for (int i = 0; i < 20; i++) {
            try {
                if (ListOfLeaderboardXElements[i] != null) {
                    float CurrentScore = float.Parse(ListOfLeaderboardXElements[i].Descendants("SCORE").First().Value);
                    LeaderboardString = LeaderboardString + (i + 1) + ". " + ListOfLeaderboardXElements[i].Nodes().OfType<XText>().First().Value
                        + CurrentScore.ToString("p") + "\n" + "\n";
                }
            }
            catch (Exception ex) { };
        }
        SSLeaderboardText.text = LeaderboardString;
    }
    private Vector2 GetCameraBounds(Camera MainCamera) {
        Camera MC = MainCamera;
        Vector2 PixelSize = new Vector2(MC.pixelWidth, MC.pixelHeight);
        Vector2 MCSize = new Vector2((Mathf.Floor((PixelSize.x * MC.orthographicSize) / PixelSize.y)), MC.orthographicSize);
        return new Vector2(MCSize.x - 1, MCSize.y - 1); // Full HD = (7, 4)
    }
    private void AssignSpecifications(Vector2 CameraBoundsLocal) {
        switch (SSGameSizeSlider.value) {
            case 1:
                GameSize = 0.1f;
                break;
            case 2:
                GameSize = 0.2f;
                break;
            case 3:
                GameSize = 0.5f;
                break;
            case 4:
                GameSize = 1;
                break;
        }
        GameSpeed = (int)SSGameSpeedSlider.value;
        DeathBorderX = CameraBoundsLocal.x - (GameSize / 2); //Camera Bounds X is rounded down we minus 1 because the playable area is one less than the camera bounds.
        DeathBorderY = CameraBoundsLocal.y - (GameSize / 2); //Camera Bounds Y is always 5 so minus one block for edge of screen, you should never have to change this.
        BorderAreaX = ((CameraBoundsLocal.x / GameSize) * 2) + 1; //Determines how many blocks are drawn and has no effect on gameplay
        BorderAreaY = ((CameraBoundsLocal.y / GameSize) * 2);//Determines how many blocks are drawn and has no effect on gameplay
    }
    private void DrawBorder(Vector2 CameraBoundsLocal) {
        //A portion of the following code may seem redundant at a glance, but it isn't. DO THE MATH AS CODED AND YOU WILL UNDERSTAND.
        for (int i = 0; i < BorderAreaX; i++) {
            GameObject XBlockA = Instantiate(BorderBlock, new Vector3(((i - (CameraBoundsLocal.x / GameSize)) * GameSize), CameraBoundsLocal.y, 0f), Quaternion.identity);
            GameObject XBlockB = Instantiate(BorderBlock, new Vector3(((i - (CameraBoundsLocal.x / GameSize)) * GameSize), -CameraBoundsLocal.y, 0), Quaternion.identity);
            XBlockA.transform.localScale = new Vector3(GameSize, GameSize, 0f);
            XBlockB.transform.localScale = new Vector3(GameSize, GameSize, 0f);
            ListOfBorderBlocks.Add(XBlockA);
            ListOfBorderBlocks.Add(XBlockB);
        }
        for (int i = 0; i < BorderAreaY; i++) {
            GameObject YBlockA = Instantiate(BorderBlock, new Vector3(CameraBoundsLocal.x, (i - (CameraBoundsLocal.y / GameSize)) * GameSize, 0), Quaternion.identity);
            GameObject YBlockB = Instantiate(BorderBlock, new Vector3(-(CameraBoundsLocal.x), (i - (CameraBoundsLocal.y / GameSize)) * GameSize, 0), Quaternion.identity);
            YBlockA.transform.localScale = new Vector3(GameSize, GameSize, 0f);
            YBlockB.transform.localScale = new Vector3(GameSize, GameSize, 0f);
            ListOfBorderBlocks.Add(YBlockA);
            ListOfBorderBlocks.Add(YBlockB);
        }
    }
    private void SpawnApple(Vector2 CameraBoundsLocal) {
        Vector2 GridArea = new Vector2(CameraBoundsLocal.x / GameSize, CameraBoundsLocal.y / GameSize);
        Vector2 SpawnInArea = new Vector2((float)UnityEngine.Random.Range((int)-GridArea.x, (int)GridArea.x), (float)UnityEngine.Random.Range((int)-GridArea.y, (int)GridArea.y));
        if (SpawnInArea.x < 0 && SpawnInArea.y >= 0) {
            CurrentApple = Instantiate(AppleBlock, new Vector3((SpawnInArea.x * GameSize) + GameSize, (SpawnInArea.y * GameSize) - GameSize, 0f), Quaternion.identity);
        }
        else if (SpawnInArea.x >= 0 && SpawnInArea.y > 0) {
            CurrentApple = Instantiate(AppleBlock, new Vector3((SpawnInArea.x * GameSize) - GameSize, (SpawnInArea.y * GameSize) - GameSize, 0f), Quaternion.identity);
        }
        else if (SpawnInArea.x <= 0 && SpawnInArea.y < 0) {
            CurrentApple = Instantiate(AppleBlock, new Vector3((SpawnInArea.x * GameSize) + GameSize, (SpawnInArea.y * GameSize) + GameSize, 0f), Quaternion.identity);
        }
        else if (SpawnInArea.x > 0 && SpawnInArea.y <= 0) {
            CurrentApple = Instantiate(AppleBlock, new Vector3((SpawnInArea.x * GameSize) - GameSize, (SpawnInArea.y * GameSize) + GameSize, 0f), Quaternion.identity);
        }
        else {
            CurrentApple = Instantiate(AppleBlock, new Vector3((SpawnInArea.x * GameSize), (SpawnInArea.y * GameSize), 0f), Quaternion.identity);
        }
        CurrentApple.transform.localScale = new Vector3(GameSize, GameSize, 0f); 
        foreach (GameObject GO in ListOfBodyParts) {
            if (GO.transform.position == CurrentApple.transform.position) {
                Destroy(CurrentApple);
                SpawnApple(CameraBounds);
            }
        }
    }
    private void MoveSnakeHead() {
        if (Input.anyKeyDown) {
            if (Input.GetKeyDown(KeyCode.W) && !SelectionMade && !MoveDown) {
                MovementCheck(true, false, false, false, 0f, GameSize); //DeadEnd
            }
            else if (Input.GetKeyDown(KeyCode.S) && !SelectionMade && !MoveUp) {
                MovementCheck(false, true, false, false, 0f, -GameSize); //DeadEnd
            }
            else if (Input.GetKeyDown(KeyCode.A) && !SelectionMade && !MoveRight) {
                MovementCheck(false, false, true, false, -GameSize, 0f); //DeadEnd
            }
            else if (Input.GetKeyDown(KeyCode.D) && !SelectionMade && !MoveLeft) {
                MovementCheck(false, false, false, true, GameSize, 0f); //DeadEnd
            }
        }
        if (GameSpeedCounter == GameSpeed) {
            SnakeHead.transform.position += new Vector3(MovementX, MovementY);
            SelectionMade = false;
        }
    }
    private void MovementCheck(bool UP, bool DOWN, bool LEFT, bool RIGHT, float MoveX, float MoveY) {
        SelectionMade = true;
        MoveUp = UP;
        MoveDown = DOWN;
        MoveLeft = LEFT;
        MoveRight = RIGHT;
        MovementX = MoveX;
        MovementY = MoveY;
    }
    private void GameOverIf() {
        if (SnakeHead.transform.position.x < -DeathBorderX || SnakeHead.transform.position.x > DeathBorderX ||
            SnakeHead.transform.position.y < -DeathBorderY || SnakeHead.transform.position.y > DeathBorderY) {
            SnakeDeathSound.Play();
            EndScreen();
        }
        foreach (GameObject GO in ListOfBodyParts) {
            if (SnakeHead.transform.position == GO.transform.position) {
                SnakeDeathSound.Play();
                EndScreen();
            }
        }
    }
    private void EndScreen() {
        TitleSong.Play();
        ControlBools(false, false, true, true, true, true); //DeadEnd
        Destroy(SnakeHead);
        Destroy(CurrentApple);
        foreach(GameObject GO in ListOfBodyParts) {
            Destroy(GO);
        }
        foreach(GameObject GO in ListOfBorderBlocks) {
            Destroy(GO);
        }
        ESScoreValueText.text = InternalGameScore.ToString("p");
    }
    private void AppleCheck() {
        if (SnakeHead.transform.position == CurrentApple.transform.position) {
            AppleEatSound.Play();
            ApplesEaten++;
            ApplesToEatMath--;
            GSApplesToWinValueText.text = ApplesToEatMath.ToString();
            Vector2 PlayableArea = new Vector2(((CameraBounds.x / GameSize) * 2) - 1, ((CameraBounds.y / GameSize) * 2) - 1);
            InternalGameScore = ApplesEaten / ((PlayableArea.x * PlayableArea.y) - 1);
            GSScoreValueText.text = InternalGameScore.ToString("p");
            Destroy(CurrentApple);
            GameObject CurrentBodyBlock = Instantiate(BodyBlock, ListOfMoves[CountOfSnakeBodyParts], Quaternion.identity);
            CurrentBodyBlock.transform.localScale = new Vector3(GameSize, GameSize, 0f);
            ListOfBodyParts.Add(CurrentBodyBlock);
            SpawnApple(CameraBounds); //DeadEnd
            CountOfSnakeBodyParts++;
        }
    }
    private void UpdatePositions() {
        if (GameSpeedCounter == GameSpeed) {
            int i = 0;
            foreach (GameObject GO in ListOfBodyParts) {
                GO.transform.position = ListOfMoves[i];
                i++;
            }
            ListOfMoves.Insert(0, SnakeHead.transform.position);
            GameSpeedCounter = 0;
            i = 0;
        }
        GameSpeedCounter++;
    }
}
