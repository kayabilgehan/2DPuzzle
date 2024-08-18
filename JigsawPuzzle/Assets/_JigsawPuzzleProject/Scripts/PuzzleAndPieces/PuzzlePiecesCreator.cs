using DG.Tweening;
using JigsawPuzzle.Models;
using JigsawPuzzle.PuzzleAndPieces;
using JigsawPuzzle.Settings;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.UI;
using static UnityEngine.Rendering.DebugUI.Table;

public class PuzzlePiecesCreator : MonoBehaviour {
	[SerializeField] private PuzzleGameManager puzzleGameManager;
	[SerializeField] private GridLayoutGroup placeHolderGrid;
	[SerializeField] private GridLayoutGroup puzzlePanel;
	[SerializeField] private RectTransform puzzleGrid;
	[SerializeField] private PieceSettings pieceSettings;
	[SerializeField] private Sprite puzzleSprite;
	[SerializeField] private Image puzzleHintImage;
	[SerializeField] private RectTransform puzzleHintPanel;
	[SerializeField] private int widthRatio = 15;
	[SerializeField] private int heightRatio = 8;
	[SerializeField] private int waitBeforeShuffle = 10;
	[SerializeField] private float pieceMarginDivider = 3f;

	private int rows = 0;
	private int cols = 0;
	private Sprite resizedSprite;
	private float horizontalMargin = 0;
	private float verticalMargin = 0;
	private float cellWidth = 0;
	private float cellHeight = 0;
	private float gridWidth = 0;
	private float gridHeight = 0;
	private PieceManager markedPlaceholderPiece = null;
	private List<PieceManager> puzzlePieces = new List<PieceManager>();
	private List<PieceManager> placeHolderPieces = new List<PieceManager>();
	private RectTransform puzzlePanelRectTransform;

	public PuzzleGameManager PuzzleGameManager => puzzleGameManager;
	public PieceSettings PieceSettings => pieceSettings;

	private void FillGrid(GridLayoutGroup grid, bool placeHolder) {
		int puzzleRows = rows;
		int puzzleCols = cols;

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
	}
	private void InstantiatePiece(GameObject piece, GridLayoutGroup grid, int row, int col, bool placeHolder) {
		GameObject pieceObject = Instantiate(piece, grid.transform);
		PieceManager pieceObjectManger = pieceObject.GetComponent<PieceManager>();
		pieceObjectManger.Init(this, row, col, grid.cellSize.x, grid.cellSize.y, resizedSprite, placeHolder);
		if (placeHolder) {
			placeHolderPieces.Add(pieceObjectManger);
		}
		else {
			puzzlePieces.Add(pieceObjectManger);
		}
	}

    private void CalculateColsAndRows() {
		cols = widthRatio;
		rows = heightRatio;
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
		//float targetScale = Mathf.Min(scaleWidth, scaleHeight);
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
		verticalMargin = (float)Screen.height * (20f / 100f);
		horizontalMargin = (float)Screen.width * (20f / 100f);

		// Calculate the scale based on the desired aspect ratio
		Vector2 currentSize = new Vector2((Screen.width - horizontalMargin), (Screen.height - verticalMargin));
		float currentAspectRatio = ((float)currentSize.x) / ((float)currentSize.y) ;
		float targetAspectRatio = (float)widthRatio / (float)heightRatio;

		if (currentAspectRatio >= targetAspectRatio) {
			// Adjust the width to maintain the aspect ratio
			float newWidth = currentSize.y * targetAspectRatio;
			puzzleGrid.sizeDelta = new Vector2(newWidth, currentSize.y);
		}
		else {
			// Adjust the height to maintain the aspect ratio
			float newHeight = currentSize.x / targetAspectRatio;
			puzzleGrid.sizeDelta = new Vector2(currentSize.x, newHeight);
		}
	}

	private void FixGridSize() {
		gridWidth = widthRatio * cellWidth;
		gridHeight = heightRatio * cellHeight;
		puzzleGrid.sizeDelta = new Vector2(gridWidth, gridHeight);
		//gridRect.sizeDelta = new Vector2(gridWidth, gridHeight);
	}

	private void Start() {
		puzzlePanelRectTransform = puzzlePanel.GetComponent<RectTransform>();
		
		CalculateGridSize();

		CalculateCellSize();

		ResizeSprite();

		CalculateColsAndRows();

		FillGrid(placeHolderGrid, true);
		FillGrid(puzzlePanel, false);

		SetupHintPanel();

		StartCoroutine(StartTheGameSequence());
	}
	private void SetupHintPanel() {
		puzzleHintPanel.sizeDelta = puzzleGrid.sizeDelta;
		puzzleHintPanel.position = puzzleGrid.position;

		puzzleHintImage.rectTransform.sizeDelta = resizedSprite.texture.Size();
		puzzleHintImage.rectTransform.anchoredPosition = new Vector2(
		(resizedSprite.texture.width - (resizedSprite.texture.width / 2)),
		-(resizedSprite.texture.height - (resizedSprite.texture.height / 2)));
		puzzleHintImage.sprite = resizedSprite;
	}
	private void SpreadParts() {
		puzzlePanel.enabled = false;
		int position = 0;

		for (int i = 0; i < puzzlePieces.Count; i++) {
			position = Random.Range(0, 4);
			float randomX = 0f;
			float randomY = 0f;

			if (position == 0) {
				// Top
				randomX = Random.Range(cellWidth / pieceMarginDivider, Screen.width - (cellWidth / pieceMarginDivider));
				randomY = Random.Range(cellHeight / pieceMarginDivider, verticalMargin + (cellHeight / pieceMarginDivider));
			}
			else if (position == 1) {
				// Bottom
				randomX = Random.Range(cellWidth / pieceMarginDivider, Screen.width - (cellWidth / pieceMarginDivider));
				randomY = Random.Range(Screen.height - verticalMargin - (cellHeight / pieceMarginDivider), Screen.height - (cellHeight / pieceMarginDivider));
			}
			else if (position == 2) {
				// Left
				randomX = Random.Range(cellWidth / pieceMarginDivider, horizontalMargin + (cellWidth / pieceMarginDivider));
				randomY = Random.Range(cellHeight / pieceMarginDivider, Screen.height - (cellHeight / pieceMarginDivider));
			}
			else if (position == 3) {
				// Right
				randomX = Random.Range(Screen.width - horizontalMargin, Screen.width - (cellWidth / pieceMarginDivider));
				randomY = Random.Range(cellHeight / pieceMarginDivider, Screen.height - (cellHeight / pieceMarginDivider));
			}
			randomX = randomX - (Screen.width / 2);
			randomY = randomY - (Screen.height / 2);

			puzzlePieces[i].MoveObject(new Vector3(randomX, randomY, puzzlePieces[i].transform.position.z));
		}
	}
	private IEnumerator StartTheGameSequence() {
		yield return new WaitForSeconds(waitBeforeShuffle);

		SpreadParts();

		puzzleGameManager.StartGame();
	}

	public void MarkPlaceholderObject(PieceManager pieceObject, Vector3 mousePosition) {
		float distance = float.MaxValue;
		PieceManager closestPiece = null;

		if (pieceObject != null) {
			distance = Vector3.Distance(mousePosition, pieceObject.transform.position);
			closestPiece = pieceObject;
		}
		else {
			markedPlaceholderPiece = null;
		}

		foreach (PieceManager piece in placeHolderPieces) {
			float pieceDistance = Vector3.Distance(mousePosition, piece.transform.position);
			if (pieceDistance < distance && pieceObject != null) {
				distance = pieceDistance;
				closestPiece = piece;
			}
			else {
				piece.UnMarkPlaceholder();
			}
		}

		if (closestPiece != null) {
			closestPiece.MarkPlaceholder();
			markedPlaceholderPiece = closestPiece;
		}
	}
	public void StoppedDragging(PieceManager draggingPiece) {
		if (markedPlaceholderPiece != null) {
			if (draggingPiece.PositionId.Equals(markedPlaceholderPiece.PositionId)) {
				draggingPiece.CorrectlyLocated(markedPlaceholderPiece);
			}
			else {
				draggingPiece.ReturnInitialPosition();
			}
		}
		ControlIfEnded();
	}

	public void HintButtonClick() {
		if (!puzzleHintPanel.gameObject.active) {
			puzzleHintPanel.gameObject.SetActive(true);
		}
		else {
			puzzleHintPanel.gameObject.SetActive(false);
		}
	}
	public void ControlIfEnded() {
		bool isEnded = true;
		foreach (PieceManager piece in puzzlePieces) {
			if (piece.IsLocated) {
				isEnded = false;
				break;
			}
		}
		if (isEnded) { 
			// Puzzle Successfully Solved
		}
	}
}
