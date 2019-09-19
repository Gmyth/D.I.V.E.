using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Serialization;
using UnityEngine.UI;
// ReSharper disable All

public enum GuiDialogueMode
{
    Default = 0,
    Multiple
}

public class GUIDialogue : GUIWindow
{
    //bound actor to certain TextMeshPro dialogue box, use it to find corresponding dialogue box
    private readonly Dictionary<string, TMP_Text> textBoxDict = new Dictionary<string, TMP_Text>();

    //store existing boxes for management
    private readonly List<GameObject> textBoxList = new List<GameObject>();

    private GuiDialogueMode mode;

    //the typewriter coroutine
    private Coroutine animateCoroutine;
    private bool isCoroutineRunning = false;

    //Keep tracks of our commands.
    private List<SpecialCommand> specialCommands;

    //Use true to skip lines
    private bool skip = false;

    //This means we can change the dialogue live and the shaking text animation will adjust itself to the new content!
    private bool hasTextChanged = false;

    //if true, paint with color c0, else paint with color c1
    private bool isColorizing = false;

    //palace holder for alternative color
    private Color32 c1 = new Color32(255,255,255,255);

    //Make the text shakes, if true before animating.
    public bool isTextShaking = true;

    //Font Size
    public float FontSize = 12f;

    //Related to Shaking Animation.
    public float AngleMultiplier = 1.0f;
    public float CurveScale = 1.0f;

    //The Speed the text is animated on screen. Waits 0.05 seconds before animating the next character.
    //Useful for letting the player accelerate the speed animation.
    public float SpeedText = 0.05f;

    public Transform dialogueTransform;

    // Start is called before the first frame update
    private void Start()
    {
        mode = GuiDialogueMode.Default;
    }

    // Update is called once per frame
    private void Update()
    {
        //Simple controls to accelerate the text speed.
        if (Input.GetKeyDown(KeyCode.Space))
        {
            SpeedText *= 100;
        }
        else if (Input.GetKeyUp(KeyCode.Space))
        {
            SpeedText = 0.05f;
        }
    }

    private void CheckActor(string actor)
    {
        switch (mode)
        {
            case GuiDialogueMode.Default:
                {
                    //if it is the first TeshMeshPro dialogue box
                    if (textBoxList.Count == 0)
                    {
                        GameObject dialogueBox = Instantiate(ResourceUtility.GetGUIPrefab<GameObject>("Dialogue"), transform, false);
                        textBoxList.Add(dialogueBox);
                    }
                    //if textBoxDict don't has the actor, add it (Only allow actor to appear once in dict)
                    if (!textBoxDict.ContainsKey(actor))
                        textBoxDict.Add(actor, textBoxList[0].GetComponentInChildren<TMP_Text>());
                    break;
                }
            case GuiDialogueMode.Multiple:
                break;

            default:
                throw new ArgumentOutOfRangeException();
        }
    }
    private void AssignTextBoxPos(string actor, Transform transform)
    {

        textBoxList[0].GetComponent<Transform>().position = dialogueTransform.position;

    }

    public bool GetTypeWriterStat(){
        return isCoroutineRunning;
    }

    public void DisplayOption(string text, string actor)
    {

    }

    public void SetText(string text, string actor)
    {
        CheckActor(actor);

        textBoxDict[actor].GetComponent<TextMeshProUGUI>().text = actor + ":" + text;
    }

    public void DisplayDialogue(string text, string actor, Transform transform)
    {
        CheckActor(actor);

        AssignTextBoxPos(actor, transform);

        if (animateCoroutine != null)
            StopCoroutine(animateCoroutine);
        animateCoroutine = StartCoroutine(AnimateText(actor, actor + ":\n" + text));

    }

    private void LoadImage(string actor)
    {
        Sprite img = ResourceUtility.GetPrefab<Sprite>("Portraits/" + actor) as Sprite;
        textBoxList[0].GetComponentInChildren<Image>().overrideSprite = img;
    }

    private IEnumerator AnimateText(string actor, string text)
    {
        isCoroutineRunning = true;
        TMP_Text dialogueBox = textBoxDict[actor].GetComponent<TextMeshProUGUI>();
        dialogueBox.text = StripAllCommands(text);
        dialogueBox.ForceMeshUpdate();

        dialogueBox.fontSize = FontSize;
        dialogueBox.alignment = TextAlignmentOptions.TopJustified;

        specialCommands = BuildSpecialCommandList(text);

        //Count how many characters we have in our new dialogue line.
        TMP_TextInfo textInfo = dialogueBox.textInfo;
        int totalCharacters = dialogueBox.textInfo.characterCount;

        LoadImage(actor);

        //Base color for our text.
        Color32 c0 = dialogueBox.color;

        //Shake text if true.
        if (isTextShaking)
        {
            StartCoroutine(ShakingText(dialogueBox));
        }

        //We now hide text based on each character's alpha value
        HideText(dialogueBox);

        var i = 0;
        while (i < totalCharacters)
        {

            //If we change the text live on runtime in our inspector, adjust the character count!
            if (hasTextChanged)
            {
                totalCharacters = textInfo.characterCount; // Update visible character count.
                hasTextChanged = false;
            }

            if (specialCommands.Count > 0)
            {
                CheckForCommands(i, dialogueBox);
            }

            //Instead of incrementing maxVisibleCharacters or add the current character to our string, we do this :

            // Get the index of the material used by the current character.
            int materialIndex = textInfo.characterInfo[i].materialReferenceIndex;

            // Get the vertex colors of the mesh used by this text element (character or sprite).
            //Color of all characters' vertices.
            var newVertexColors = textInfo.meshInfo[materialIndex].colors32;

            // Get the index of the first vertex used by this text element.
            int vertexIndex = textInfo.characterInfo[i].vertexIndex;

            // Only change the vertex color if the text element is visible. (It's visible, only the alpha color is 0)
            if (textInfo.characterInfo[i].isVisible && isColorizing == false)
            {
                UpdateVertexColors(newVertexColors, vertexIndex, c0, dialogueBox);
            }
            else if(textInfo.characterInfo[i].isVisible && isColorizing== true)
            {
                UpdateVertexColors(newVertexColors, vertexIndex, c1, dialogueBox);
            }

            i++;

            if (!skip)
            {
                yield return new WaitForSeconds(SpeedText);
            }
            
        }

        Debug.Log("Done animating!");
        isCoroutineRunning = false;
        yield return null;
    }

    public void SetSkip(bool skip)
    {
        this.skip = skip;
    }

    private void UpdateVertexColors(Color32[] newVertexColors, int vertexIndex, Color32 color, TMP_Text dialogueBox)
    {
        newVertexColors[vertexIndex + 0] = color;
        newVertexColors[vertexIndex + 1] = color;
        newVertexColors[vertexIndex + 2] = color;
        newVertexColors[vertexIndex + 3] = color;
        // New function which pushes (all) updated vertex data to the appropriate meshes when using either the Mesh Renderer or CanvasRenderer.
        dialogueBox.UpdateVertexData(TMP_VertexDataUpdateFlags.Colors32);
    }

    private static string StripAllCommands(string text)
    {
        //Regex Pattern. Remove all "{stuff:value}" from our dialogue line.
        const string pattern = "\\{.[^}]+\\}";

        //Clean string to return.
        var cleanString = Regex.Replace(text, pattern, "");

        return cleanString;
    }

    private List<SpecialCommand> BuildSpecialCommandList(string text)
    {
        List<SpecialCommand> listCommand = new List<SpecialCommand>();

        string command = "";                //Current command name
        char[] squiggles = { '{', '}' };    //Trim these characters when the command is found.

        //Go through the dialogue line, get all our special commands.
        for (int i = 0; i < text.Length; i++)
        {
            string currentChar = text[i].ToString();

            //If true, we are getting a command.
            if (currentChar == "{")
            {

                //Go ahead and get the command.
                while (currentChar != "}" && i < text.Length)
                {
                    currentChar = text[i].ToString();
                    command += currentChar;
                    text = text.Remove(i, 1);  //Remove current character. We want to get the next character in the command.
                }

                //Done getting the command.
                if (currentChar == "}")
                {
                    //Trim "{" and "}"
                    command = command.Trim(squiggles);
                    //Get Command Name and Value.
                    SpecialCommand newCommand = GetSpecialCommand(command);
                    //Command index position in the string.
                    newCommand.Index = i;
                    //Add to list.
                    listCommand.Add(newCommand);
                    //Reset
                    command = "";

                    //Take a step back otherwise a character will be skipped. 
                    //i = 0 also works, but it means going through characters we already checked.
                    i--;
                }
                else
                {
                    Debug.Log("Command in dialogue line not closed.");
                }
            }
        }

        return listCommand;
    }

    //Since our command is {command:value}, we want to extract the name of the command and its value.
    private SpecialCommand GetSpecialCommand(string text)
    {

        SpecialCommand newCommand = new SpecialCommand();

        //Regex to get the command name and the command value
        string commandRegex = "[:]";

        //Split the command and its values.
        string[] matches = Regex.Split(text, commandRegex);

        //Get the command and its values.
        if (matches.Length > 0)
        {
            for (int i = 0; i < matches.Length; i++)
            {
                //0 = command name. 1 >= value.
                if (i == 0)
                {
                    newCommand.Name = matches[i];
                }
                else
                {
                    newCommand.Values.Add(matches[i]);
                }
            }
        }
        else
        {
            //Oh no....
            return null;
        }

        return newCommand;
    }

    //Check all commands in a given index. 
    //It's possible to have two commands next to each other in the dialogue line.
    //This means both will share the same index.
    private void CheckForCommands(int index, TMP_Text teshMeshPro)
    {
        for (int i = 0; i < specialCommands.Count; i++)
        {
            if (specialCommands[i].Index == index)
            {
                //Execute if found a match.
                ExecuteCommand(specialCommands[i], teshMeshPro);

                //Remove it.
                specialCommands.RemoveAt(i);

                //Take a step back since we removed one command from the list. Otherwise, the script will skip one command.
                i--;
            }
        }
    }

    public TimelineManager CurrentTimelineManager;
    //Where you will execute your command!
    private void ExecuteCommand(SpecialCommand command, TMP_Text teshMeshPro)
    {
        if (command == null)
        {
            return;
        }

        Debug.Log(LogUtility.MakeLogStringFormat("GUIDialogue", "Command " + command.Name + " found!"));

        if (command.Name == "sound")
        {
            Debug.Log("BOOOOM! Command played a sound.");
        }
        else if (command.Name == "color")
        {
            if (command.Values[0] != "end")
            {
                c1 = new Color32(255, 0, 0, 255);
                isColorizing = true;
            }
            else
            {
                isColorizing = false;
            }
        }
        else if (command.Name == "action") {
            if (command.Values[0] == "play") {
                CurrentTimelineManager.PlayNextTimeline();
            }
        }
    }

    //Hide our text by making all our characters invisible.
    private void HideText(TMP_Text dialogueBox)
    {
        dialogueBox.ForceMeshUpdate();

        TMP_TextInfo textInfo = dialogueBox.textInfo;

        Color32[] newVertexColors;
        Color32 c0 = dialogueBox.color;

        for (int i = 0; i < textInfo.characterCount; i++)
        {

            int materialIndex = textInfo.characterInfo[i].materialReferenceIndex;

            // Get the vertex colors of the mesh used by this text element (character or sprite).
            newVertexColors = textInfo.meshInfo[materialIndex].colors32;

            // Get the index of the first vertex used by this text element.
            int vertexIndex = textInfo.characterInfo[i].vertexIndex;

            //Alpha = 0
            c0 = new Color32(c0.r, c0.g, c0.b, 0);

            //Apply it to all vertex.
            UpdateVertexColors(newVertexColors, vertexIndex, c0, dialogueBox);
        }
    }

    // Structure to hold pre-computed animation data.
    private struct VertexAnim
    {
        public float angleRange;
        public float angle;
        public float speed;
    }

    //Shaking example taken from the TextMeshPro demo.
    private IEnumerator ShakingText(TMP_Text dialogueTextPro)
    {

        // We force an update of the text object since it would only be updated at the end of the frame. Ie. before this code is executed on the first frame.
        // Alternatively, we could yield and wait until the end of the frame when the text object will be generated.
        dialogueTextPro.ForceMeshUpdate();

        TMP_TextInfo textInfo = dialogueTextPro.textInfo;

        Matrix4x4 matrix;

        int loopCount = 0;
        hasTextChanged = true;

        // Create an Array which contains pre-computed Angle Ranges and Speeds for a bunch of characters.
        VertexAnim[] vertexAnim = new VertexAnim[1024];
        for (int i = 0; i < 1024; i++)
        {
            vertexAnim[i].angleRange = UnityEngine.Random.Range(10f, 25f);
            vertexAnim[i].speed = UnityEngine.Random.Range(1f, 3f);
        }

        // Cache the vertex data of the text object as the Jitter FX is applied to the original position of the characters.
        TMP_MeshInfo[] cachedMeshInfo = textInfo.CopyMeshInfoVertexData();

        while (true)
        {

            // Get new copy of vertex data if the text has changed.
            if (hasTextChanged)
            {
                // Update the copy of the vertex data for the text object.
                cachedMeshInfo = textInfo.CopyMeshInfoVertexData();
                hasTextChanged = false;
            }

            int characterCount = textInfo.characterCount;

            // If No Characters then just yield and wait for some text to be added
            if (characterCount == 0)
            {
                yield return new WaitForSeconds(0.25f);
                continue;
            }


            for (int i = 0; i < characterCount; i++)
            {

                // Retrieve the pre-computed animation data for the given character.
                VertexAnim vertAnim = vertexAnim[i];

                // Get the index of the material used by the current character.
                int materialIndex = textInfo.characterInfo[i].materialReferenceIndex;

                // Get the index of the first vertex used by this text element.
                int vertexIndex = textInfo.characterInfo[i].vertexIndex;

                // Get the cached vertices of the mesh used by this text element (character or sprite).
                Vector3[] sourceVertices = cachedMeshInfo[materialIndex].vertices;

                // Determine the center point of each character.
                Vector2 charMidBasline = (sourceVertices[vertexIndex + 0] + sourceVertices[vertexIndex + 2]) / 2;

                // Need to translate all 4 vertices of each quad to aligned with middle of character / baseline.
                // This is needed so the matrix TRS is applied at the origin for each character.
                Vector3 offset = charMidBasline;

                Vector3[] destinationVertices = textInfo.meshInfo[materialIndex].vertices;

                destinationVertices[vertexIndex + 0] = sourceVertices[vertexIndex + 0] - offset;
                destinationVertices[vertexIndex + 1] = sourceVertices[vertexIndex + 1] - offset;
                destinationVertices[vertexIndex + 2] = sourceVertices[vertexIndex + 2] - offset;
                destinationVertices[vertexIndex + 3] = sourceVertices[vertexIndex + 3] - offset;

                vertAnim.angle = Mathf.SmoothStep(-vertAnim.angleRange, vertAnim.angleRange, Mathf.PingPong(loopCount / 25f * vertAnim.speed, 1f));
                Vector3 jitterOffset = new Vector3(UnityEngine.Random.Range(-.25f, .25f), UnityEngine.Random.Range(-.25f, .25f), 0);

                matrix = Matrix4x4.TRS(jitterOffset * CurveScale, Quaternion.Euler(0, 0, UnityEngine.Random.Range(-5f, 5f) * AngleMultiplier), Vector3.one);

                destinationVertices[vertexIndex + 0] = matrix.MultiplyPoint3x4(destinationVertices[vertexIndex + 0]);
                destinationVertices[vertexIndex + 1] = matrix.MultiplyPoint3x4(destinationVertices[vertexIndex + 1]);
                destinationVertices[vertexIndex + 2] = matrix.MultiplyPoint3x4(destinationVertices[vertexIndex + 2]);
                destinationVertices[vertexIndex + 3] = matrix.MultiplyPoint3x4(destinationVertices[vertexIndex + 3]);

                destinationVertices[vertexIndex + 0] += offset;
                destinationVertices[vertexIndex + 1] += offset;
                destinationVertices[vertexIndex + 2] += offset;
                destinationVertices[vertexIndex + 3] += offset;

                vertexAnim[i] = vertAnim;
            }

            // Push changes into meshes
            for (int i = 0; i < textInfo.meshInfo.Length; i++)
            {
                textInfo.meshInfo[i].mesh.vertices = textInfo.meshInfo[i].vertices;
                dialogueTextPro.UpdateGeometry(textInfo.meshInfo[i].mesh, i);
            }

            loopCount += 1;

            yield return new WaitForSeconds(0.1f);
        }
    }

    public void SetTextSpeed(float speed)
    {
        SpeedText = speed;
    }

    public bool CheckAnimateCoroutine()
    {
        if (isCoroutineRunning)
        {
            return true;
        }
        else
        {
            if (CurrentTimelineManager.isTheLastTimeline) {
                CurrentTimelineManager.OnTimelineEnd();
            }
            else {
                CurrentTimelineManager.PlayNextTimeline();
            }
            return false;
        }
    }
}

class SpecialCommand
{

    //Command Name
    public string Name { get; set; }

    //A list of all values needed for our command. 
    //If you only need one value per command, consider not making this a list.
    public List<string> Values { get; set; }

    //Which character index should we execute this command.
    public int Index { get; set; }

    public SpecialCommand()
    {
        Name = "";
        Values = new List<string>();
        Index = 0;
    }
}


