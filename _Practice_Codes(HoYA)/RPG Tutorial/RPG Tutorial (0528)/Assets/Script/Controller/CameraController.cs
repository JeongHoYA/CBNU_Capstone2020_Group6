using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform target;                                                                            // 카메라 타겟 트랜스폼

    public Vector3 offset;                                                                              // 카메라 위치 오프셋

    private float currentZoom = 10f;                                                                    // 카메라와 타겟 사이의 거리
    public float zoomSpeed = 4f;                                                                        // 거리조절 변수
    public float minZoom = 5f;                                                                          // 최소 거리
    public float maxZoom = 15f;                                                                         // 최대 거리

    public float pitch = 2f;                                                                            // 피치(타겟의 높이)

    public float yawSpeed = 100f;                                                                       // 카메라 회전 속도 변수
    private float currentYaw = 0f;                                                                      // 카메라 회전값

    void Update()
    {
        // 줌 업데이트
        currentZoom -= Input.GetAxis("Mouse ScrollWheel") * zoomSpeed;
        currentZoom = Mathf.Clamp(currentZoom, minZoom, maxZoom);

        // 카메라 z축 회전값 업데이트
        currentYaw -= Input.GetAxis("Horizontal") * yawSpeed * Time.deltaTime;
    }


    void LateUpdate()
    {
        // 줌 카메라 반영
        transform.position = target.position - offset * currentZoom;
        transform.LookAt(target.position + Vector3.up * pitch);

        // z축 회전 반영
        transform.RotateAround(target.position, Vector3.up, currentYaw);
    }
}
