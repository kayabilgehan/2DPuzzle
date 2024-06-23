using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PuzzlePiecesCreator : MonoBehaviour {
    [SerializeField] private GridLayoutGroup grid;
	[SerializeField] private Sprite puzzleSprite;

	[SerializeField] private GameObject[] topRowPieces;
	[SerializeField] private GameObject[] normalRowPieces;
	[SerializeField] private GameObject[] bottomRowPieces;

    private RectTransform gridRect;

	private int rows = 0;
	private int cols = 0;
	void Start()
    {
        gridRect = grid.gameObject.GetComponent<RectTransform>();

        CalculateColsAndRows();

        LogRowsAndColumns();
    }

    void Update()
    {
        
    }

    private void CalculateColsAndRows() {
        cols = ((int)gridRect.rect.width / (int)grid.cellSize.x);
		rows = ((int)gridRect.rect.height / (int)grid.cellSize.y);
	}
    private void LogRowsAndColumns() {
		Debug.Log(string.Format("Row: {0} Cols: {1}", rows, cols));
	}
}
