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
	}
}

