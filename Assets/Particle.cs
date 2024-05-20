using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Particle : MonoBehaviour
{
    // 消滅するまでの時間
    private float lifeTime;
    // 消滅するまでの残り時間
    private float leftLifeTime;
    // 移動量
    private Vector3 velocity;
    // 初期スケール
    private Vector3 defaultScale;

    // Start is called before the first frame update
    void Start()
    {
        // 崩滅するまでの時間を0.3秒に設定
        lifeTime = 0.3f;
        // 残り時間を初期化
        leftLifeTime = lifeTime;
        // 現在のScaleを記録
        defaultScale = transform.localScale;
        // ランダムで決まる移動量の最大値
        float maxVelocity = 5.0f;
        // 各方向へランダムで飛ばす
        velocity = new Vector3
            (
            Random.Range(-maxVelocity, maxVelocity),
            Random.Range(-maxVelocity, maxVelocity),
            0
            );
    }

    // Update is called once per frame
    void Update()
    {
        // 残り時間をカウントダウン
        leftLifeTime -= Time.deltaTime;
        // 自身の座標を移動
        transform.position += velocity * Time.deltaTime;
        // 残り時間により徐々にScaleを小さくする
        transform.localScale = Vector3.Lerp
            (
                new Vector3(0, 0, 0),
                defaultScale,
                leftLifeTime / lifeTime
            );
        // 残り時間が0以下になったら自身のゲームオブジェクトを消滅
        if (leftLifeTime <= 0) { Destroy(gameObject); };
    }
}
