using System.Linq;
using TMPro;
using UnityEngine;

public class CellClue : MonoBehaviour
{
    public int[] clueNumers;
    public TextMeshProUGUI text;

    private bool isSolved;

    
    private void Start()
    {
        if (text == null)
        {
            text = GetComponent<TextMeshProUGUI>();
        }
    }

    public void Initialization(int[] clueNumers, int index, bool isRow)
    {
        this.clueNumers = clueNumers;
        UpdateText();
    }

    private void UpdateText()
    {
        text.text = string.Join(" ", clueNumers.Select(c => c.ToString()).ToArray());
    }
    
    public void UpdateClueColor(bool solved)
    {
        if (solved && !isSolved)
        {
            isSolved = true;
            text.color = Color.gray;
        }
        else if (!solved && isSolved)
        {
            isSolved = false;
            text.color = Color.black;
        }
    }
}