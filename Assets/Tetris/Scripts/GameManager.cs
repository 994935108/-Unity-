using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameType {
	ClassicMode,//经典模式
	SpeedOfLifeAndAeathMode,//生死时速
	MatchingMode//匹配模式
}
public class GameManager : MonoBehaviour
{
	BlockController blockController;
	private void Start()
	{
	    blockController= PoolManager.Instance.GetElement("BlockController", "Prefabs/Shape/BlockController").GetComponent<BlockController>();
		blockController.OnFinish+=OnFinish;
		blockController.Initialized();
	}

	public void Update()
	{
		if (Input.GetKeyDown(KeyCode.Space)) {
			RestartGame();
;		}
	}

	private void OnFinish() {
		  
	}

	public void PauseGame()
	{
		blockController.Pause();
	}
	public void RestartGame() { 
		blockController.ReStart();
	}

}
