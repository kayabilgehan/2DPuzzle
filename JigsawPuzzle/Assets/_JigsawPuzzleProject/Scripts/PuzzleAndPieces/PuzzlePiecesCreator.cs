using JigsawPuzzle.Models;
using JigsawPuzzle.PuzzleAndPieces;
using JigsawPuzzle.Settings;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.Rendering.DebugUI.Table;

public class PuzzlePiecesCreator : MonoBehaviour {
	[SerializeField] private GridLayoutGroup placeHolderGrid;
	[SerializeField] private GridLayoutGroup puzzlePanel;
	[SerializeField] private RectTransform puzzleGrid;
	[SerializeField] private PieceSettings pieceSettings;
	[SerializeField] private Sprite puzzleSprite;
	[SerializeField] private Image puzzleImage;
	[SerializeField] private int widthRatio = 15;
	[SerializeField] private int heightRatio = 8;

	//private RectTransform gridRect;

	private int rows = 0;
	private int cols = 0;
	private Sprite resizedSprite;
	private float top = 0;
	private float bottom = 0;
	private float right = 0;
	private float left = 0;
	private float cellWidth = 0;
	private float cellHeight = 0;
	private float gridWidth = 0;
	private float gridHeight = 0;


	private void FillGrid(GridLayoutGroup grid, bool placeHolder) {
		int puzzleRows = rows;// - ((rows - 3) % 2);
		int puzzleCols = cols;// - ((cols - 3) % 2);

		for (int i = 0; i < puzzleRows; i++) {
			for (int j = 0; j < puzzleCols; j++) {
				RowsModel rowModel;
				if (i == 0) {
					// Starting row
					rowModel = pieceSettings.topRowPieces;
				}
				else if (i == puzzleRows - 1) {
					// Ending row
					rowModel = pieceSettings.bottomRowPieces;
				}
				else {
					// Regular rows
					if (i % 2 == 1) {
						rowModel = pieceSettings.mid1RowPieces;
					}
					else {
						rowModel = pieceSettings.mid2RowPieces;
					}
				}
				DecidePieceAndInstantiate(grid, rowModel, j, i, placeHolder);

			}
		}
	}
	private void DecidePieceAndInstantiate(GridLayoutGroup grid, RowsModel rowPieces, int col, int row, bool placeHolder) {
		GameObject objectToInstantiate;
		if (col == 0) {
			// Starting row
			objectToInstantiate = rowPieces.StartPiece;
		}
		else if (col == cols - 1) {
			// Ending row
			objectToInstantiate = rowPieces.EndPiece;
		}
		else {
			// Regular rows
			int instantiatingPieceIndex = (col + 1) % 2;
			objectToInstantiate = rowPieces.MidPieces[instantiatingPieceIndex];
		}
		DecideGridOfPiece(grid,objectToInstantiate, row, col, placeHolder);
	}
	private void DecideGridOfPiece(GridLayoutGroup grid, GameObject piece, int row, int col, bool placeHolder) {
		InstantiatePiece(piece, grid, row, col, placeHolder);
		//if (placeHolder) {
		//	InstantiatePiece(piece, placeHolderGrid, row, col, placeHolder);
		//}
		//else {
		//	InstantiatePiece(piece, puzzlePanel, row, col, placeHolder);
		//}
	}
	private void InstantiatePiece(GameObject piece, GridLayoutGroup grid, int row, int col, bool placeHolder) {
		GameObject pieceObject = Instantiate(piece, grid.transform);
		PieceManager pieceObjectManger = pieceObject.GetComponent<PieceManager>();
		pieceObjectManger.Init(row, col, grid.cellSize.x, grid.cellSize.y, resizedSprite, placeHolder);
	}

    private void CalculateColsAndRows() {
		//      cols = ((int)gridRect.rect.width / (int)placeHolderGrid.cellSize.x);
		//rows = ((int)gridRect.rect.height / (int)placeHolderGrid.cellSize.y);

		//rows = rows - ((rows - 3) % 2);
		//cols = cols - ((cols - 3) % 2);

		cols = widthRatio;
		rows = heightRatio;
	}
    private void LogRowsAndColumns() {
		Debug.Log(string.Format("Row: {0} Cols: {1}", rows, cols));
	}

	private void ResizeSprite() {
		if (puzzleSprite == null) {
			Debug.LogError("No Sprite assigned to the target Image.");
			return;
		}

		// Get the sprite's original size
		float spriteWidth = puzzleSprite.texture.width;
		float spriteHeight = puzzleSprite.texture.height;

		// Calculate the target scale based on the aspect ratio
		float scaleWidth = puzzleGrid.rect.width / spriteWidth;
		float scaleHeight = puzzleGrid.rect.height / spriteHeight;

		// Use the smaller scale to maintain the aspect ratio
		float targetScale = Mathf.Max(scaleWidth, scaleHeight);
		//float targetScale = scaleWidth;

		// Calculate the new size
		int newWidth = Mathf.RoundToInt(spriteWidth * targetScale);
		int newHeight = Mathf.RoundToInt(spriteHeight * targetScale);

		// Resize the sprite's texture
		Texture2D resizedTexture = ResizeTexture(puzzleSprite.texture, newWidth, newHeight);
		resizedSprite = Sprite.Create(resizedTexture, new Rect(0, 0, newWidth, newHeight), new Vector2(0.5f, 0.5f));

		// Apply the resized sprite to the Image component
		//targetImage.sprite = resizedSprite;

		Debug.Log(string.Format("X: {0} Y: {1}", resizedSprite.rect.width, resizedSprite.rect.height));
	}
	private Texture2D ResizeTexture(Texture2D source, int targetWidth, int targetHeight) {
		RenderTexture rt = RenderTexture.GetTemporary(targetWidth, targetHeight);
		rt.filterMode = FilterMode.Bilinear;

		RenderTexture.active = rt;
		Graphics.Blit(source, rt);
		Texture2D result = new Texture2D(targetWidth, targetHeight);
		result.ReadPixels(new Rect(0, 0, targetWidth, targetHeight), 0, 0);
		result.Apply();

		RenderTexture.active = null;
		RenderTexture.ReleaseTemporary(rt);

		return result;
	}
	private void ResizePuzzleGrid() {
		//top = (Screen.height * 15) / 100;
		//bottom = - (Screen.height * 15) / 100;
		//right = (Screen.width * 10) / 100;
		//left = -(Screen.width * 10) / 100;
		//puzzleGrid.offsetMin = new Vector2(right, top);
		//puzzleGrid.offsetMax = new Vector2(left, bottom);
	}
	private void CalculateCellSize() {
		cellWidth = (int)puzzleGrid.rect.width / widthRatio;
		cellHeight = (int)puzzleGrid.rect.height / heightRatio;

		if (cellWidth > cellHeight) {
			cellWidth = cellHeight;
		}
		else {
			cellHeight = cellWidth;
		}

		placeHolderGrid.cellSize = new Vector2(cellWidth, cellHeight);
		puzzlePanel.cellSize = new Vector2(cellWidth, cellHeight);
	}
	private void CalculateGridSize() {
		//gridRect.rect.width
		//gridRect.rect.height

		// Calculate the scale based on the desired aspect ratio
		float currentAspectRatio = ((float)Screen.width * (90f / 100f)) / ((float)Screen.height * (85f / 100f)) ;
		float targetAspectRatio = (float)widthRatio / (float)heightRatio;

		if (currentAspectRatio >= targetAspectRatio) {
			// Adjust the width to maintain the aspect ratio
			float newWidth = puzzleGrid.rect.height * targetAspectRatio;
			puzzleGrid.sizeDelta = new Vector2(newWidth, puzzleGrid.rect.height);
		}
		else {
			// Adjust the height to maintain the aspect ratio
			float newHeight = puzzleGrid.rect.width / targetAspectRatio;
			puzzleGrid.sizeDelta = new Vector2(puzzleGrid.rect.width, newHeight);
		}

		//gridWidth = cols * cellWidth;
		//gridHeight = rows * cellHeight;
		//gridRect.sizeDelta = new Vector2(gridWidth, gridHeight);
		//gridRect.sizeDelta = new Vector2(gridWidth, gridHeight);
	}

	private void FixGridSize() {
		gridWidth = widthRatio * cellWidth;
		gridHeight = heightRatio * cellHeight;
		puzzleGrid.sizeDelta = new Vector2(gridWidth, gridHeight);
		//gridRect.sizeDelta = new Vector2(gridWidth, gridHeight);
	}

	private void Start() {
		//gridRect = placeHolderGrid.gameObject.GetComponent<RectTransform>();

		//ResizePuzzleGrid();

		CalculateGridSize();

		CalculateCellSize();

		//FixGridSize();

		ResizeSprite();

		CalculateColsAndRows();

		LogRowsAndColumns();

		//FillGrid(placeHolderGrid, true);
		FillGrid(puzzlePanel, false);

		//puzzleImage.sprite = puzzleSprite;
	}

	private void Update() {

	}
}
