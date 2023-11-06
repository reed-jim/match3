using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SwipeDirection
{
    Up,
    Down,
    Left,
    Right
}

public class InputManager : MonoBehaviour
{
    [Header("REFERENCE")]
    [SerializeField] private GameManager gameManager;

    private SwipeDirection _swipeDirection;
    private bool _isAbleToClick = true;

    private void Start()
    {
        StartCoroutine(Swap());
    }

    private IEnumerator Swap()
    {
        int numSwipePositionDetected = 0;
        Vector2 startMousePosition = Vector2.zero;
        Vector2 endMousePosition = Vector2.zero;

        while (true)
        {
            if (Input.GetMouseButton(0) && _isAbleToClick)
            {
                Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                mousePosition.z = gameManager.Blocks[0].transform.position.z;

                if (numSwipePositionDetected == 0)
                {
                    startMousePosition = mousePosition;
                    numSwipePositionDetected++;
                }
                else
                {
                    endMousePosition = mousePosition;

                    _swipeDirection = GetSwipeDirection(startMousePosition, endMousePosition);

                    Debug.Log("test: " + startMousePosition + "/" + endMousePosition);

                    for (int i = 0; i < gameManager.NumBlock; i++)
                    {
                        if (gameManager.BlockColliders[i].bounds.Contains(startMousePosition))
                        {
                            Debug.Log("test: " + i + "/" + _swipeDirection);

                            gameManager.Move(i, _swipeDirection);

                            numSwipePositionDetected = 0;

                            _isAbleToClick = false;

                            break;
                        }
                    }
                }
            }

            yield return new WaitForSeconds(0.05f);
        }
    }

    private SwipeDirection GetSwipeDirection(Vector2 start, Vector2 end)
    {
        if (Mathf.Abs(end.x - start.x) > Mathf.Abs(end.y - start.y))
        {
            if (end.x > start.x) return SwipeDirection.Right;
            else return SwipeDirection.Left;
        }
        else
        {
            if (end.y > start.y) return SwipeDirection.Up;
            else return SwipeDirection.Down;
        }
    }
}
