using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserPositionAttacher : MonoBehaviour
{
    [Header("オブジェクト設定")]
    [SerializeField] TrackingUserIntegration trackedUserPositionIntegration;
    //[SerializeField] trackedUserPositionIntegration trackedUserPositionIntegration;
    [SerializeField] StateManager stateManager;
    [Header("実機使用")]
    [SerializeField] bool useRealTrackingData = false;
    [Header("シミュレーション設定")]
    [SerializeField] float moveSpeed = 3.0f;
    [SerializeField] Vector2 rotationSpeed = new Vector2(0.1f, 0.1f);

    Vector2 lastMousePosition;
    Vector2 newAngle = Vector2.zero;

    void Update()
    {
        if(useRealTrackingData)
        {
            this.transform.position = trackedUserPositionIntegration.integratedPosition;
        }
        else
        {
            stateManager.isTrackingUser = true;
            SimulationPositionControl();
        }
    }

    void SimulationPositionControl()
    {
        if (Input.GetKey(KeyCode.W))
        {
            transform.position += transform.forward * moveSpeed * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.S))
        {
            transform.position -= transform.forward * moveSpeed * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.D))
        {
            transform.position += transform.right * moveSpeed * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.A))
        {
            transform.position -= transform.right * moveSpeed * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.E))
        {
            transform.position += transform.up * moveSpeed * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.Q))
        {
            transform.position -= transform.up * moveSpeed * Time.deltaTime;
        }
    }
}
