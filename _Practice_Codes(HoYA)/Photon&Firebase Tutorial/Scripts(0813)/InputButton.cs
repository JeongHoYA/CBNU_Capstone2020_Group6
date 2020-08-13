using UnityEngine;

public class InputButton : MonoBehaviour
{
    public static float VerticalInput;                                      // 상하 입력값

    public enum State                                                       // 정지, 아래, 위 상태
    {
        None,
        Down,
        Up
    }

    private State state = State.None;                                       // 입력상태 변수

    private void Update()                                                   // 현재 입력상태에 따라 상하 입력값 설정
    {
        if (state == State.None)
            VerticalInput = 0f;
        else if (state == State.Up)
            VerticalInput = 1f;
        else if (state == State.Down)
            VerticalInput = -1f;
    }

    // 각각의 버튼 상황에 따른 상태 설정
    public void OnMoveUpButtonPressed()
    {
        state = State.Up;
    }

    public void OnMoveUpButtonUp()
    {
        if (state == State.Up)
            state = State.None;
    }

    public void OnMoveDownButtonPressed()
    {
        state = State.Down;
    }

    public void OnMoveDownButtonUp()
    {
        if (state == State.Down)
            state = State.None;
    }
}