using System;
using UnityEngine;

namespace JigsawPuzzle.Models {
	[Serializable]
	public class VerticalOffsetModel {

		[SerializeField] private float topPercentage = 0;
		[SerializeField] private float bottomPercentage = 0;

		public float TopPercentage => topPercentage;
		public float BottomPercentage => bottomPercentage;
	}
}
