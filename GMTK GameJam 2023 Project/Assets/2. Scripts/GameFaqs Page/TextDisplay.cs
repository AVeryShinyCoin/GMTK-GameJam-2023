using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static UnityEditor.PlayerSettings;

public class TextDisplay : MonoBehaviour
{
    public static TextDisplay Instance;
    [SerializeField] TextMeshProUGUI testReferenceText;
    [SerializeField] GameObject textBlockPrefab;
    public int CurrentRoleInstructions;

    public int lines;
    public float currLineWidth;
    [SerializeField] float maxLineWidth;
    [SerializeField] float linesHeight;

    public List<TextBlock> ReferenceSwappableTextBlocks = new List<TextBlock>();
    public List<TextBlock> SwappableTextBlocks = new List<TextBlock>();
    [SerializeField] GameObject swappableBG;
    TextBlock swapTarget;


    [TextArea(15, 20)]
    public string[] roleInstructions;
    [SerializeField] List<GameObject> TextBlocks = new List<GameObject>();
    [SerializeField] GameObject[] headers;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }


    private void Start()
    {
        swappableBG.SetActive(false);
        GenerateRandomTextBlockSelection();

        GenerateInitialText();
        OrganizeTextBlocks();
        AssignSwappables();
    }

    void GenerateInitialText()
    {
        CurrentRoleInstructions = -1;

        foreach (string instructions in roleInstructions)
        {
            CurrentRoleInstructions++;

            string currInstructions = roleInstructions[CurrentRoleInstructions];
            string currWord = "";
            for (int i = 0; i < currInstructions.Length; i++)
            {
                string letter = currInstructions.Substring(i, 1);

                if (letter == "|")
                {
                    CreateTextBlock(currWord);
                    CreateTextBlock("LINEBREAK");
                    currWord = "";
                    continue;
                }
                if (letter == " ")
                {
                    if (currWord != "") CreateTextBlock(currWord);
                    currWord = "";
                    continue;
                }
                currWord = currWord + letter;

            }
            if (currWord != "" && currWord != " ") CreateTextBlock(currWord); // this ensures the final word gets added as well

        }
    }


    void CreateTextBlock(string text)
    {
        GameObject gob = Instantiate(textBlockPrefab, transform.position, Quaternion.identity);
        gob.transform.parent = transform;
        gob.GetComponent<TextBlock>().InitializeBlock(text);
        TextBlocks.Add(gob);
    }

    void GenerateRandomTextBlockSelection()
    {
        for (int i = 0; i < 3; i++)
        {
            GameObject gob = Instantiate(textBlockPrefab, transform.position, Quaternion.identity);
            TextBlock script = gob.GetComponent<TextBlock>();
            string text = "";

            int rrnd = Random.Range(0, 2);

            if (rrnd == 0) // Zones
            {
                int rnd = Random.Range(0, 6);
                if (rnd == 0) text = "ZON@near";
                if (rnd == 1) text = "ZON@sides";
                if (rnd == 2) text = "ZON@away";
                if (rnd == 3) text = "ZON@notfront";
                if (rnd == 4) text = "ZON@front";
                if (rnd == 5) text = "ZON@behind";
            }
            if (rrnd == 1) // Percent
            {
                int rnd = Random.Range(1, 10);
                rnd *= 10;
                text = "PER@" + rnd;
            }

            ReferenceSwappableTextBlocks.Add(script);
            SwappableTextBlocks.Add(script);
            script.InitializeBlock(text);
            script.Swappable = true;

            gob.SetActive(false);
        }
    }

    void AssignSwappables()
    {
        swappableBG.transform.parent = gameObject.transform;
        foreach (TextBlock textBlock in SwappableTextBlocks)
        {
            textBlock.transform.parent = gameObject.transform;
        }
    }


    void OrganizeTextBlocks()
    {
        lines = 2;
        currLineWidth = 0;
        int currHeader = - 1;
        foreach (GameObject textBlock in TextBlocks)
        {
            TextBlock script = textBlock.GetComponent<TextBlock>();
            if (script.InstructionBelongsToRole != currHeader)
            {
                currHeader++;
                lines += 2;
                headers[currHeader].transform.localPosition = new Vector2(0f, linesHeight * -lines);
                lines += 2;
                currLineWidth = 0;
            }
            if (script.LineBreak)
            {
                lines++;
                currLineWidth = 0;
            }
            if (currLineWidth + script.Width > maxLineWidth)
            {
                lines++;
                currLineWidth = 0;
            }
            textBlock.transform.localPosition = new Vector2(currLineWidth + script.Width / 2, linesHeight * -lines);
            currLineWidth += script.Width;
        }
    }

    public void ClickOnTextInGuide(TextBlock textBlock)
    {
        float widthOffset = 0;
        Vector2 pos = textBlock.transform.localPosition;
        foreach (TextBlock swappable in SwappableTextBlocks)
        {
            swappable.gameObject.SetActive(true);
            
            widthOffset += swappable.Width + 0.5f;
            swappable.transform.localPosition = pos + new Vector2(widthOffset - (swappable.Width + 0.5f) / 2, 0.5f);
        }
        swappableBG.SetActive(true);
        swappableBG.GetComponent<RectTransform>().sizeDelta = new Vector2(widthOffset, 0.5f);
        swappableBG.GetComponent<RectTransform>().localPosition = pos + new Vector2(0 + widthOffset / 2, 0.5f);
        swapTarget = textBlock;
    }

    public void ClickOnSwappable(TextBlock textBlock)
    {
        if (textBlock.dataType == swapTarget.dataType)
        {
            int index = TextBlocks.IndexOf(swapTarget.gameObject);

            TextBlocks.Insert(index, textBlock.gameObject);
            TextBlocks.RemoveAt(index + 1);
            textBlock.InstructionBelongsToRole = swapTarget.InstructionBelongsToRole;
            textBlock.MakeSwapped();
            Destroy(swapTarget.gameObject);
            SwappableTextBlocks.Remove(textBlock);
        }
        else
        {
            SoundManager.Instance.PlaySound("Error");
        }
        
        foreach (TextBlock swappable in SwappableTextBlocks)
        {
            swappable.gameObject.SetActive(false);
        }
        swappableBG.SetActive(false);
        swapTarget = null;

        OrganizeTextBlocks();
    }

    public void ResetSwaps()
    {
        foreach (TextBlock textBlock in ReferenceSwappableTextBlocks)
        {
            if (TextBlocks.Contains(textBlock.gameObject))
            {
                TextBlocks.Remove(textBlock.gameObject);
            }
        }

        SwappableTextBlocks.Clear();
        foreach (GameObject textBlock in TextBlocks)
        {
            Destroy(textBlock);
        }
        TextBlocks.Clear();

        foreach (TextBlock textBlock in ReferenceSwappableTextBlocks)
        {
            SwappableTextBlocks.Add(textBlock);
            textBlock.Interactable = true;
            textBlock.Swappable = true;
            textBlock.gameObject.GetComponentInChildren<TextMeshProUGUI>().color = new Color(0.0f, 0.0f, 0.6f, 1f);
            textBlock.gameObject.SetActive(false);

            textBlock.transform.parent = transform.parent;
            
        }
        swappableBG.transform.parent = transform.parent;

        GenerateInitialText();
        OrganizeTextBlocks();
        AssignSwappables();
    }

    public void CookTextBlocks()
    {
        List<TextBlock> list = new List<TextBlock>();
        foreach (GameObject gob in TextBlocks)
        {
            list.Add(gob.GetComponent<TextBlock>());
        }
        GetComponent<InstructionsParser>().ParseInstructions(list);
    }
}
