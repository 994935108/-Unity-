
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
public class BlockController : MonoBehaviour
{


	private static int row = 23;//行数
	private static int column = 10;//列数
	private Block[,] blocks;//构建填充二维数组
	private BlockShape currentShape = null;//当前操作的方块

	private float fallTimer = 0f;//下落时间
	private float fallTimerSpan = 0.2f;//下落时间间隔
	public float fallSpeed = 1;//下落速度

	private float moveTimer = 0f;//左右移动的时间
	private float moveTimerSpan = 0.8f;//左右移动的时间间隔
	private  float moveSpeed = 0.005f;//左右移动的速度

	[SerializeField]
	private Color[] colors; //方块颜色
	[SerializeField]
	private BlockShape[] blockShapes;//不同方块种类
	[SerializeField]
	private Block blockPrefabs;//填充二维数组的预制体
	private bool isPause = true;//是否暂停游戏
	private List<BlockShape> blockShapeList = new List<BlockShape>();//已经生成的方块数组，方便重置游戏的时候清空掉

	internal UnityAction OnFinish;



	private void Update()
	{
		if (isPause)
		{
			return;
		}
		if (currentShape == null)
		{
			SpawnShape();
		}
		DownMove();


		if (Input.GetKeyDown(KeyCode.LeftArrow))
		{
			LeftMove();
		}
        else
        {
            if (Input.GetKey(KeyCode.LeftArrow))
            {
				ContinuedLeftMove();

			}
        }
        if (Input.GetKeyDown(KeyCode.RightArrow))
		{
			RighttMove();
		}
        else
        {
            if (Input.GetKey(KeyCode.RightArrow))
            {
				ContinuedRighttMove();
				
            }
        }
        if (Input.GetKeyDown(KeyCode.UpArrow))
		{
			Rotate();
		}


		if (Input.GetKey(KeyCode.DownArrow))
		{
			ContinuedDownMove();
		}

	}

	private void DownMove() {
		fallTimer += Time.deltaTime * fallSpeed;
		if (fallTimer > fallTimerSpan)
		{
			fallTimer = 0;
			currentShape.DownMove();
			if (!IsValid(currentShape))
			{
				currentShape.UpMove();
				PlaceBlock(currentShape);
				currentShape = null;
				fallTimer = fallTimerSpan;
				return;
			}
		}
	}
	/// <summary>
	/// 加速下落
	/// </summary>
	public void ContinuedDownMove() {
		fallTimer += Time.deltaTime * 10;
	}

	/// <summary>
	/// 左移
	/// </summary>
	public void LeftMove() {
		Move(true);
		moveSpeed = -0.04f;
		moveTimer = 0;
	}

	/// <summary>
	/// 加速左移
	/// </summary>
	public void ContinuedLeftMove() {
		moveTimer += moveSpeed;
		moveSpeed += Time.deltaTime;
		if (moveTimer > moveTimerSpan)
		{
			Move(true);
			moveTimer = 0;
		}
	}
	/// <summary>
	/// 右移
	/// </summary>
	public void RighttMove()
	{
		Move(false);
		moveSpeed = -0.04f;
		moveTimer = 0;
	}
	/// <summary>
	/// 加速右移
	/// </summary>
	public void ContinuedRighttMove()
	{
		moveTimer += moveSpeed;
		moveSpeed += Time.deltaTime;
		if (moveTimer > moveTimerSpan)
		{
			Move(false);
			moveTimer = 0;
		}
	}

	/// <summary>
	/// 旋转
	/// </summary>
	public void Rotate() {
		currentShape.LeftRotate();
		if (!IsValid(currentShape))
		{
			currentShape.RightRotate();

		}
	}

	public void SetSpeed(float deltaTime) {
		fallSpeed += deltaTime;
	}

	private void Move(bool isLeft)
	{
		if (isLeft)
		{
			currentShape.LeftMove();
			if (!IsValid(currentShape))
			{
				currentShape.RightMove();
			}
		}
		else {
			currentShape.RightMove();
			if (!IsValid(currentShape))
			{
				currentShape.LeftMove();
			}
		}
		
	}
	/// <summary>
	/// 重新开始游戏
	/// </summary>
	public void ReStart()
	{
		for (int i = blockShapeList.Count - 1; i >= 0; i--)
		{
			Destroy(blockShapeList[i].gameObject);
		}
		blockShapeList.Clear();
		if (blocks != null)
		{
			for (int i = 0; i < blocks.GetLength(0); i++)
			{
				for (int j = 0; j < blocks.GetLength(1); j++)
				{

					blocks[i, j].isFull = false;

				}
			}
		}
		currentShape = null;
		isPause = false;
	}

	public void Pause()
	{
		this.isPause = !this.isPause;
	}
	public void Initialized()
	{
		fallTimer = fallTimerSpan;

		blocks = new Block[row, column];
		for (int i = 0; i < blocks.GetLength(0); i++)
		{
			for (int j = 0; j < blocks.GetLength(1); j++)
			{
				blocks[i, j] = Instantiate(blockPrefabs, transform);
				blocks[i, j].index = new Vector2(i, j);
				blocks[i, j].transform.position = transform.localPosition + new Vector3(j, i, 0);
				blocks[i, j].name = blocks[i, j].transform.position.ToString();
			}
		}

		isPause = false;
	}

	/// <summary>
	/// 生成随即形状的方块
	/// </summary>
	private void SpawnShape()
	{
		int shapeIndex = Random.Range(0, blockShapes.Length);
		int colorIndex = Random.Range(0, colors.Length);
		currentShape = Instantiate(blockShapes[shapeIndex], transform);
		currentShape.Initialized();
		currentShape.SetColor(colors[colorIndex]);
		blockShapeList.Add(currentShape);
	}
	/// <summary>
	/// 控制输入
	/// </summary>


	/// <summary>
	/// 判断当前方块移动是否是有效的
	/// </summary>
	/// <param name="blockShape"></param>
	/// <returns></returns>

	private bool IsValid(BlockShape blockShape)
	{
		if (blockShape == null)
		{
			Debug.Log(1234654);
		}
		for (int i = 0; i < blockShape.blocks.Length; i++)
		{
			Vector2 index = FindIndex(blockShape.blocks[i]);
			if (index.x < 0) return false;
			if (index.y < 0) return false;
			if (index.y > column) return false;
			if (blocks[(int)index.x, (int)index.y].isFull) return false;
		}
		return true;
	}

	/// <summary>
	/// 查找当前小方块的坐标索引
	/// </summary>
	/// <param name="block"></param>
	/// <returns></returns>
	private Vector2 FindIndex(Block block)
	{
		for (int i = 0; i < blocks.GetLength(0); i++)
		{
			for (int j = 0; j < blocks.GetLength(1); j++)
			{
				if (Mathf.Abs(block.transform.position.x - blocks[i, j].transform.position.x) < 0.1f)
				{
					if (Mathf.Abs(block.transform.position.y - blocks[i, j].transform.position.y) < 0.1f)
					{
						return new Vector2(i, j);
					}
				}
			}
		}
		return new Vector2(-1, -1);
	}
	/// <summary>
	///  将方块填充到格子里面
	/// </summary>
	/// <param name="blockShape"></param>
	private void PlaceBlock(BlockShape blockShape)
	{
		for (int i = 0; i < blockShape.blocks.Length; i++)
		{
			Vector2 index = FindIndex(blockShape.blocks[i]);
			if ((int)index.x != -1 && (int)index.y != -1)
			{
				blocks[(int)index.x, (int)index.y].isFull = true;
				blocks[(int)index.x, (int)index.y].obj = blockShape.blocks[i].gameObject;
			}
			else
			{
				Debug.Log("游戏结束");
				isPause = true;

				if (OnFinish!=null) { 
			    	OnFinish.Invoke();
				}
			}
		}
		currentShape = null;
		//放置好以后进行消解
		Dissolution();
	}
	/// <summary>
	/// 消除方块
	/// </summary>

	private void Dissolution()
	{
		for (int i = 0; i < blocks.GetLength(0); i++)
		{
			RowDissolution(i);
		}
	}

	/// <summary>
	/// 行消除
	/// </summary>
	private void  RowDissolution(int row)
	{
		if (IsCanDissolution(row))
		{
			for (int j = 0; j < blocks.GetLength(1); j++)
			{
				DestroyImmediate(blocks[row, j].obj);
				blocks[row, j].isFull = false;
			}
			RowFull(row);
			RowDissolution(row);
		}
	}
	/// <summary>
	///  判断当前行是否可以消除
	/// </summary>
	/// <param name="row"></param>
	/// <returns></returns>
	private bool IsCanDissolution(int row)
	{
		for (int j = 0; j < blocks.GetLength(1); j++)
		{
			if (!blocks[row, j].isFull) return false;
		}
		return true;
	}

	/// <summary>
	/// 填充方块
	/// </summary>
	/// <param name="row"></param>

	private void RowFull(int row)
	{
		Debug.Log("填充   " + row);

		for (int i = row; i < blocks.GetLength(0) - 1; i++)
		{
			for (int j = 0; j < blocks.GetLength(1); j++)
			{
				if (blocks[i + 1, j].isFull)
				{
					blocks[i + 1, j].obj.transform.position = transform.TransformPoint(blocks[i, j].transform.localPosition);
					blocks[i, j].obj = blocks[i + 1, j].obj;
					blocks[i, j].isFull = true;
					blocks[i + 1, j].isFull = false;
				}
			}

		}
	}
}
