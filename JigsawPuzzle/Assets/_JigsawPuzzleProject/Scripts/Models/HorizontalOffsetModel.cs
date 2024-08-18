using System;
using UnityEngine;

namespace JigsawPuzzle.Models {
	[Serializable]
	public class HorizontalOffsetModel {
		[SerializeField] private float leftPercentage = 0;
		[SerializeField] private float rightPercentage = 0;

		public float LeftPercentage => leftPercentage;
		public float RightPercentage => rightPercentage;
	}
}

