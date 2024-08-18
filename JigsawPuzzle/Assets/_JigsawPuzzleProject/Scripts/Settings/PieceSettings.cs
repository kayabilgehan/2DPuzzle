using JigsawPuzzle.Models;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JigsawPuzzle.Settings {
	[CreateAssetMenu(fileName = "PieceSettings", menuName = "Settings/PieceSettings", order = 1)]
	public class PieceSettings : ScriptableObject {
		public RowsModel topRowPieces;
		public RowsModel mid1RowPieces;
		public RowsModel mid2RowPieces;
		public RowsModel bottomRowPieces;

		[SerializeField] private Color draggingHighlightColor = Color.white;
		[SerializeField] private Color matchedBackgroundHighlightColor = Color.white;


		[SerializeField] private float pieceMoveDuration = 0.5f;

		public Color DraggingHighlightColor => draggingHighlightColor;
		public Color MatchedBackgroundHighlightColor => matchedBackgroundHighlightColor;

		public float PieceMoveDuration => pieceMoveDuration;
	}
}

