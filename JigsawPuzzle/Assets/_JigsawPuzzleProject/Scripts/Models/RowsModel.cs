using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JigsawPuzzle.Models {
	[Serializable]
	public class RowsModel {
		[SerializeField] private GameObject startPiece;
		[SerializeField] private GameObject[] midPieces;
		[SerializeField] private GameObject endPiece;

		public GameObject StartPiece => startPiece;
		public GameObject[] MidPieces => midPieces;
		public GameObject EndPiece => endPiece;
	}
} 

