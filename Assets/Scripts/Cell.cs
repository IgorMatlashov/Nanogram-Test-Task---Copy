using UnityEngine;
using UnityEngine.UI;

public class Cell : MonoBehaviour
{
    public Color selectedColor;
    public Color deselectedColor;
    
    public int row;
    public int column;
    public bool isFilled = false;
    public Image cellImage;

    private NonogramManager manager;

    void Start()
    {
        cellImage = GetComponent<Image>();
    }
    public void Initialize(int row, int column, NonogramManager manager)
    {
        this.row = row;
        this.column = column;
        this.manager = manager;
    }
    public void ToggleState()
    {
        isFilled = !isFilled;
        cellImage.color = isFilled ? selectedColor : deselectedColor;
        
        manager.OnCellStateChanged(row, column, isFilled);
    }
}