using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TextDisplay : MonoBehaviour
{
    public static TextDisplay Instance;
    [SerializeField] TextMeshProUGUI testReferenceText;
    [SerializeField] GameObject textBlockPrefab;
    string intialText;
    [SerializeField] List<GameObject> TextBlocks = new List<GameObject>();
    int blockIndex;
    public int CurrentRoleInstructions;

    public int lines;
    public float currLineWidth;
    [SerializeField] float maxLineWidth;
    [SerializeField] float linesHeight;

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
        CurrentRoleInstructions = -1;
        string currWord = "";
        for (int i = 0; i < testReferenceText.text.Length; i++)
        {
            
            string letter = testReferenceText.text.Substring(i, 1);

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

        OrganizeTextBlocks();
    }


    void CreateTextBlock(string text)
    {
        GameObject gob = Instantiate(textBlockPrefab, transform.position, Quaternion.identity);
        gob.transform.parent = transform;
        gob.GetComponent<TextBlock>().InitializeBlock(text);
        gob.GetComponent<TextBlock>().Index = blockIndex;
        blockIndex++;
        TextBlocks.Add(gob);
    }

    void OrganizeTextBlocks()
    {
        lines = 0;
        currLineWidth = 0;

        foreach (GameObject textBlock in TextBlocks)
        {
            TextBlock script = textBlock.GetComponent<TextBlock>();

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
