using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserPositionAttacher : MonoBehaviour
{
    [Header("オブジェクト設定")]
    [SerializeField] TrackingPointIntegration trackingPointIntegration;
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
            if(trackingPointIntegration.integratedPositions.Count > 0)
            {
                this.transform.position = trackingPointIntegration.integratedPositions[0];
            }
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

        
        // if (Input.GetMouseButtonDown(0))
        // {
        //     newAngle = transform.localEulerAngles;
        //     lastMousePosition = Input.mousePosition;
        // }
        // else if (Input.GetMouseButton(0))
        // {
        //     newAngle.y -= (lastMousePosition.x - Input.mousePosition.x) * rotationSpeed.y;
        //     newAngle.x -= (Input.mousePosition.y - lastMousePosition.y) * rotationSpeed.x;

        //     transform.localEulerAngles = newAngle;
        //     lastMousePosition = Input.mousePosition;
        // }

        //transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z);

    }
}
