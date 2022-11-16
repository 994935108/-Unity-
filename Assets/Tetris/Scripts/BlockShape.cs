using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Permissions;
using UnityEngine;
using UnityEngine.UI;

public class BlockShape : MonoBehaviour
{
	internal Block[] blocks;
	public void UpMove()
	{
		Vector3 pos = transform.position;
		pos.y += 1;
		transform.position = pos;
	}
	public void DownMove()
	{
		Vector3 pos = transform.position;
		pos.y -= 1;
		transform.position = pos;
	}
	public void LeftMove()
	{
		Vector3 pos = transform.position;
		pos.x -= 1;
		transform.position = pos;
	}
	public void RightMove()
	{
		Vector3 pos = transform.position;
		pos.x += 1;
		transform.position = pos;
	}
	public void LeftRotate()
	{
		transform.rotation *= Quaternion.Euler(Vector3.forward*90);
	}
	public void RightRotate()
	{
		transform.rotation *= Quaternion.Euler(-Vector3.forward * 90);
	}

	internal void SetColor(Color color)
	{
		foreach (var item in blocks)
		{
			item.GetComponent<MeshRenderer>().material.color=color;
		}
	}

	internal void Initialized()
	{
		blocks = GetComponentsInChildren<Block>();
		for (int i = 0; i < blocks.Length; i++)
		{

			blocks[i].isFull = true;
			
		}
	}
}




