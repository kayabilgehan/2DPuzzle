using UnityEngine;
using UnityEngine.UI;

namespace JigsawPuzzle.PuzzleAndPieces {
	public class PieceManager : MonoBehaviour {
		[SerializeField] private Image image;
		[SerializeField] private Vector2 offset;
		
		private RectTransform imageRectTransform;
		private RectTransform parentRectTransform;
		public void Init(int row, int col, float width, float height, Sprite sprite, bool placeholder) {
			imageRectTransform = image.GetComponent<RectTransform>();
			imageRectTransform.sizeDelta = new Vector2(sprite.texture.width, sprite.texture.height);

			parentRectTransform = imageRectTransform.transform.parent.gameObject.GetComponent<RectTransform>();

			if (placeholder) {
				image.sprite = null;
			}
			else {
				image.sprite = sprite;
				imageRectTransform.anchoredPosition = new Vector2(
					(sprite.texture.width - (col * width) - (sprite.texture.width / 2) + offset.x),
					- (sprite.texture.height - (row * height) - (sprite.texture.height / 2) + offset.y));
			}
		}
		void Start() {

		}
	}
}
