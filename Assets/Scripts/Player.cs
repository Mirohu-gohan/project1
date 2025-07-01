using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
	/// <summary>
	/// 左側の線
	/// </summary>
	[SerializeField]
	private GameObject left_;

	/// <summary>
	/// 右側の線
	/// </summary>
	[SerializeField]
	private GameObject right_;

	/// <summary>
	/// 視野角(角度)
	/// </summary>
	[SerializeField]
	private float viewAngle_ = 30.0f;

	/// <summary>
	/// プレイヤーのワールド行列
	/// </summary>
	private Matrix4x4 worldMatrix_ = Matrix4x4.identity;

	public Matrix4x4 worldMatrix { get => worldMatrix_; }

	/// <summary>
	/// 視野角(ラジアン)
	/// </summary>
	public float viewRadian { get => viewAngle_ * Mathf.Deg2Rad; }

	/// <summary>
	/// 更新処理
	/// </summary>
	void Update()
	{
		// 回転
		float rad = 0.0f;
		if (Input.GetKey(KeyCode.A))
		{
			rad = -2.0f;
		}
		else if (Input.GetKey(KeyCode.S))
		{
			rad = 2.0f;
		}
		// 回転行列
		Matrix4x4 rotMatrix = Matrix4x4.Rotate(Quaternion.Euler(0, rad, 0));

		// ★★★ここから追加・修正★★★
		// 平行移動 (既存の行列計算は残しつつ、Transformの直接移動も追加)
		Vector3 moveDirection = Vector3.zero;
		if (Input.GetKey(KeyCode.UpArrow))
		{
			moveDirection.z = 0.5f; // Z軸方向へ移動
		}
		if (Input.GetKey(KeyCode.DownArrow))
		{
			moveDirection.z = -0.5f; // Z軸方向へ移動
		}
		if (Input.GetKey(KeyCode.RightArrow))
		{
			moveDirection.x = 0.5f; // X軸方向へ移動
		}
		if (Input.GetKey(KeyCode.LeftArrow))
		{
			moveDirection.x = -0.5f; // X軸方向へ移動
		}

		// 1フレームの移動量を適用
		float moveAmount = 0.2f; // 要件の移動量に合わせる
		transform.Translate(moveDirection.normalized * moveAmount, Space.World); // ワールド座標系で移動

		// ★★★ここまで追加・修正★★★

		// 平行移動行列 (既存コードをそのまま残す)
		var transMatrix = Matrix4x4.Translate(moveDirection); // このvecは↑の移動処理とは別になるが、削除しない

		// ワールド行列に現在のフレームの行列を掛け合わせて更新する (既存コードをそのまま残す)
		worldMatrix_ = worldMatrix_ * (transMatrix * rotMatrix);

		// ワールド行列から座標、回転、拡大縮小を取得して設定する (既存コードをそのまま残す)
		transform.position = worldMatrix_.GetColumn(3); // こちらが設定されるとTransform.Translateと競合するが、今回はキー入力で移動させる
		transform.rotation = worldMatrix_.rotation;
		transform.localScale = worldMatrix_.lossyScale;

	}

	/// <summary>
	/// 視野角の線を更新
	/// </summary>
	private void FixedUpdate()
	{
		// 左側
		if (left_)
		{
			var localMatrix = Matrix4x4.Rotate(Quaternion.Euler(0, -viewAngle_, 0));
			left_.transform.rotation = (worldMatrix_ * localMatrix).rotation;
			left_.transform.position = transform.position;
		}

		// 右側
		if (right_)
		{
			var localMatrix = Matrix4x4.Rotate(Quaternion.Euler(0, viewAngle_, 0));
			right_.transform.rotation = (worldMatrix_ * localMatrix).rotation;
			right_.transform.position = transform.position;
		}

	}
}
