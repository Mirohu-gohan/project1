using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer; 
public class Enemy : MonoBehaviour
{
	/// <summary>
	/// プレイヤー
	/// </summary>
	[SerializeField] private Player player_ = null;

	/// <summary>
	/// ワールド行列
	/// </summary>
	private Matrix4x4 worldMatrix_ = Matrix4x4.identity; // 未使用なので残しても問題なし

	// ★★★ここから追加★★★
	// 敵の視野角（自身の前方±20度）
	private const float ENEMY_VIEW_ANGLE = 20.0f;
	// 1フレームの旋回最大角度
	private const float MAX_ENEMY_TURN_ANGLE = 10.0f;
	// 1フレームの移動量
	private const float ENEMY_MOVE_SPEED = 0.2f;
	// ★★★ここまで追加★★★

	/// <summary>
	/// ターゲットとして設定する
	/// </summary>
	/// <param name="enable">true:設定する / false:解除する</param>
	public void SetTarget(bool enable)
	{
		// マテリアルの色を変更する
		MeshRenderer meshRenderer = GetComponent<MeshRenderer>();
		if (meshRenderer != null && meshRenderer.materials.Length > 0)
		{
			// 敵は青矢印なので、通常時は青、追跡時は赤にする
			meshRenderer.materials[0].color = enable ? Color.red : Color.blue;
		}
	}

	/// <summary>
	/// 開始処理
	/// </summary>
	public void Start()
	{
		// プレイヤーが設定されているか確認
		if (player_ == null)
		{
			Debug.LogError("EnemyスクリプトにPlayerが設定されていません。InspectorでPlayerオブジェクトをplayer_フィールドにドラッグ＆ドロップしてください。");
		}
		// 初期色を青に設定
		SetTarget(false);
	}

	/// <summary>
	/// 更新処理
	/// </summary>
	public void Update()
	{
		if (player_ == null) return;

		// 敵からプレイヤーへの方向ベクトル (Y軸成分は無視してXZ平面で考える)
		Vector3 directionToPlayer = (player_.transform.position - transform.position);
		Vector3 directionToPlayerXZ = new Vector3(directionToPlayer.x, 0f, directionToPlayer.z).normalized;

		// 敵の前方ベクトル (Y軸成分は無視してXZ平面で考える)
		Vector3 enemyForwardXZ = new Vector3(transform.forward.x, 0f, transform.forward.z).normalized;

		// プレイヤーが敵の視界に入っているか判定 (前方±20度)
		float angleBetweenEnemyAndPlayer = Vector3.Angle(enemyForwardXZ, directionToPlayerXZ);

		if (angleBetweenEnemyAndPlayer <= ENEMY_VIEW_ANGLE)
		{
			// プレイヤーが視界に入っている場合
			SetTarget(true); // 敵の色を赤にする

			// プレイヤーがいる方向へY軸旋回する
			// 目標の回転を計算
			Quaternion targetRotation = Quaternion.LookRotation(directionToPlayerXZ);

			// 1フレームの最大旋回角度で、現在の回転から目標の回転へ向かって回転
			transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, MAX_ENEMY_TURN_ANGLE);

			// プレイヤーに向かって移動 (自身の前方へ移動)
			transform.position += transform.forward * ENEMY_MOVE_SPEED;
		}
		else
		{
			// プレイヤーが視界に入っていない場合
			SetTarget(false); // 敵の色を青に戻す
							  // プレイヤーを追わないので、移動も旋回もしない
		}
	}
}
