using DG.Tweening;
using JigsawPuzzle.Models;
using JigsawPuzzle.Settings;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace JigsawPuzzle.PuzzleAndPieces {
	public class PieceManager : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler {
		[SerializeField] private RectTransform pieceTransform;
		[SerializeField] private Image image;
		[SerializeField] private RectTransform piece;
		[SerializeField] private HorizontalOffsetModel horizontalOffset;
		[SerializeField] private VerticalOffsetModel verticalOffset;

		private Image pieceImage;
		private PuzzlePiecesCreator _puzzleCreator;
		private RectTransform imageRectTransform;
		private RectTransform parentRectTransform;
		private RectTransform pieceImageRectTransform;
		private Vector2 positionId;
		private Vector3 initialPosition;
		private Canvas parentCanvas;
		private BoxCollider2D collider;
		private bool isDraggable = false;
		private bool _placeholder = false;
		private PuzzleGameManager gameManager;
		private float _width = 0f;
		private float _height = 0f;
		private bool isLocated = false;


		public Vector2 PositionId => positionId;
		public bool IsPlaceholder => _placeholder;
		public bool IsLocated => isLocated;

		public void Init(PuzzlePiecesCreator puzzleCreator, int row, int col, float width, float height, Sprite sprite, bool placeholder) {
			_width = width;
			_height = height;

			_placeholder = placeholder;
			isDraggable = !placeholder;
			_puzzleCreator = puzzleCreator;
			positionId = new Vector2(row, col);

			pieceTransform = gameObject.GetComponent<RectTransform>();
			pieceImage = piece.GetComponent<Image>();
			collider = piece.GetComponent<BoxCollider2D>();
			parentCanvas = GetComponentInParent<Canvas>();
			imageRectTransform = image.GetComponent<RectTransform>();
			pieceImageRectTransform = piece.GetComponent<RectTransform>();

			imageRectTransform.sizeDelta = new Vector2(sprite.texture.width, sprite.texture.height);

			parentRectTransform = imageRectTransform.transform.parent.gameObject.GetComponent<RectTransform>();


			float leftOffset = width * (horizontalOffset.LeftPercentage / 100f);
			float rightOffset = width * (horizontalOffset.RightPercentage / 100f);
			float topOffset = height * (verticalOffset.TopPercentage / 100f);
			float bottomOffset = height * (verticalOffset.BottomPercentage / 100f);

			Vector2 size = new Vector2(width + leftOffset + rightOffset,
					height + topOffset + bottomOffset);
			if (piece != null) {
				piece.sizeDelta = size;
			}
			
			if (placeholder) {
				//image.sprite = null;
				image.enabled = false;
				imageRectTransform.anchoredPosition = new Vector2(0, 0);
				piece.gameObject.GetComponent<Mask>().enabled = false;
			}
			else {
				image.sprite = sprite;

				imageRectTransform.anchoredPosition = new Vector2(
					(sprite.texture.width - (col * width) - (sprite.texture.width / 2) + leftOffset),
					-(sprite.texture.height - (row * height) - (sprite.texture.height / 2) + topOffset));
			}

			//collider.size = size;
			collider.size = new Vector2(width, height);

			/*Vector2 anchoredPosition = new Vector2(
					(sprite.texture.width - (col * width) - (sprite.texture.width / 2) + leftOffset),
					-(sprite.texture.height - (row * height) - (sprite.texture.height / 2) + topOffset));*/

			SetColliderPosition();
		}
		void SetColliderPosition() {
			float colliderAnchorX = 0f;
			float colliderAnchorY = 0f;
			if (pieceImageRectTransform.pivot.x == 0.5f) {
				colliderAnchorX = 0f;
			}
			else if (pieceImageRectTransform.pivot.x == 1f) {
				colliderAnchorX = - _width / 2f;
			}
			else {
				colliderAnchorX = _width / 2f;
			}
			if (pieceImageRectTransform.pivot.y == 0.5f) {
				colliderAnchorY = 0f;
			}
			else if (pieceImageRectTransform.pivot.y == 1) {
				colliderAnchorY = - _height / 2f;
			}
			else {
				colliderAnchorY = _height / 2f;
			}


			Vector2 colliderAnchoredPosition = new Vector2(colliderAnchorX, colliderAnchorY);
			collider.offset = colliderAnchoredPosition;
		}

		public void OnBeginDrag(PointerEventData eventData) {
			if (isDraggable) {
				gameObject.transform.SetAsLastSibling();
				initialPosition = gameObject.transform.localPosition;
				image.color = _puzzleCreator.PieceSettings.DraggingHighlightColor;
				_puzzleCreator.PuzzleGameManager.ChangeDragState(true);
			}
		}
		RaycastHit2D[] hits;
		Vector2 mousePosition;
		public void OnDrag(PointerEventData eventData) {
			if (isDraggable) {
				pieceTransform.anchoredPosition += eventData.delta / parentCanvas.scaleFactor;
				_puzzleCreator.PuzzleGameManager.ChangeDragState(false);

				mousePosition = Input.mousePosition;
				hits = Physics2D.RaycastAll(mousePosition, Vector2.zero, Mathf.Infinity);

				if (hits.Length > 0) {
					foreach (RaycastHit2D hit in hits) {
						if (hit.collider != null) {
							if (hit.collider.gameObject.transform.parent.TryGetComponent(out PieceManager collidedPieceManager)) {
								if (collidedPieceManager.IsPlaceholder) {
									_puzzleCreator.MarkPlaceholderObject(collidedPieceManager, mousePosition);
								}
							}
						}
					}
				}
				else {
					_puzzleCreator.MarkPlaceholderObject(null, Vector3.zero);
				}
			}
		}
		public void OnEndDrag(PointerEventData eventData) {
			if (isDraggable) {
				_puzzleCreator.StoppedDragging(this);
				image.color = Color.white;
				_puzzleCreator.MarkPlaceholderObject(null, Vector3.zero);
			}
		}
		public void MarkPlaceholder() {
			//if (_placeholder && _puzzleCreator.PuzzleGameManager.DragState) {
			if (_placeholder) {
				pieceImage.color = _puzzleCreator.PieceSettings.MatchedBackgroundHighlightColor;
			}
		}
		public void UnMarkPlaceholder() {
			if (_placeholder) {
				pieceImage.color = Color.white;
			}
		}
		public void ReturnInitialPosition() {
			MoveObject(initialPosition);
		}
		public void CorrectlyLocated(PieceManager correctMatchedPiece) {
			gameObject.transform.SetAsFirstSibling();
			isLocated = true;
			isDraggable = false;
			collider = null;
			correctMatchedPiece.isLocated = true;
			correctMatchedPiece.collider = null;

			Debug.Log("CorrectlyLocated - " + correctMatchedPiece.transform.parent.name + " - " + correctMatchedPiece.transform.localPosition);

			MoveObject(correctMatchedPiece.transform.localPosition);
		}
		public void MoveObject(Vector3 position) {
			gameObject.transform.DOLocalMove(position, _puzzleCreator.PieceSettings.PieceMoveDuration);
		}
	}
}
