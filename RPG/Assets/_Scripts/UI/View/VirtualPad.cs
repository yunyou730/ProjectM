using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class VirtualPad : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public enum State
    {
        Normal,
        Up,
        Down,
        Left,
        Right,
    }

    public Sprite normalSprite = null;
    public Sprite leftSprite = null;
    public Sprite rightSprite = null;
    public Sprite upSprite = null;
    public Sprite downSprite = null;

    Dictionary<State, Sprite> stateSpriteMap = new Dictionary<State, Sprite>();

    UnityEngine.UI.Image image = null;

    Vector2 dir;
    State state;

    private void Start()
    {
        image = GetComponent<Image>();

        stateSpriteMap.Add(State.Normal,normalSprite);
        stateSpriteMap.Add(State.Up, upSprite);
        stateSpriteMap.Add(State.Down, downSprite);
        stateSpriteMap.Add(State.Left, leftSprite);
        stateSpriteMap.Add(State.Right, rightSprite);
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        
    }

    public void OnDrag(PointerEventData eventData)
    {
        State prevState = state;
        dir = eventData.position - eventData.pressPosition;
        state = GetState();
        if (prevState != state)
        {
            UpdateSprite();
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        dir = new Vector2(0, 0);
        state = GetState();
        UpdateSprite();
    }


    public State GetState()
    {
        if (dir.magnitude <= 0.1)
        {
            return State.Normal;
        }
        else
        {
            Vector2 normalDir = dir.normalized;
            if (Mathf.Abs(dir.y) >= Mathf.Abs(dir.x))
            {
                return dir.y > 0 ? State.Up : State.Down;
            }
            else
            {
                return dir.x > 0 ? State.Right : State.Left;
            }
        }
    }

    private void UpdateSprite()
    {
        Sprite nextSprite = stateSpriteMap[state];
        image.sprite = nextSprite;
    }
}
