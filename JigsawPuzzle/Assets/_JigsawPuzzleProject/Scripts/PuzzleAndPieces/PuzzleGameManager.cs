using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuzzleGameManager : MonoBehaviour
{
	private bool _dragState = false;
	public bool DragState => _dragState;
	public void StartGame() {

	}

	public void ChangeDragState(bool dragState) {
		_dragState = dragState;
	}
}
