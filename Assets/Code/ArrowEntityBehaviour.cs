using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ArrowEntityBehaviour : MonoBehaviour
{
    [HideInInspector]
    public float errorDuration;

    [HideInInspector]
    public float errorSize;

    [HideInInspector]
    public float successDuration;

    [HideInInspector]
    public float successSize;

    [HideInInspector]
    public float fadeAwayDuration;

    [HideInInspector]
    public Image image;

    [HideInInspector]
    public BattleArrowKeySystem system;

    private Behaviour behaviour = Behaviour.NONE;
    private float t;
    private float remainingMoveDistance;
    private Vector2 moveTarget;

    public void ActError()
    {
        behaviour = Behaviour.ERROR;
        t = 0;
    }

    public void ActSuccess()
    {
        behaviour = Behaviour.SUCCESS;
        t = 0;
        remainingMoveDistance = system.arrowGap + system.arrowSize;
        moveTarget = transform.position + new Vector3(-remainingMoveDistance, 0);
    }

    public void ActMoveOver()
    {
        behaviour = Behaviour.MOVE_OVER;
        t = 0;
        remainingMoveDistance = system.arrowGap + system.arrowSize;
        moveTarget = transform.position + new Vector3(-remainingMoveDistance, 0);
    }

    public void ActFadeAway()
    {
        behaviour = Behaviour.FADE_AWAY;
        t = 0;
    }

    void Update()
    {
        switch (behaviour)
        {
            case Behaviour.NONE:
                {
                    // nothing to do
                    break;
                }

            case Behaviour.ERROR:
                {
                    t += Time.deltaTime;
                    float s = 1 / errorDuration;
                    float v = s * t;

                    if (t >= errorDuration)
                    {
                        behaviour = Behaviour.NONE;
                        system.Unbusy();
                        image.transform.localScale = new Vector3(1, 1, 1);
                        image.color = new Color(1, 1, 1, 1);
                        break;
                    }
                    else if (t >= errorDuration / 2)
                    {
                        v = 1 - s * t;
                    }

                    float color = 1 - v;
                    image.color = new Color(1, color, color, 1);

                    float scale = 1 + errorSize * v;
                    image.transform.localScale = new Vector3(scale, scale, 1);

                    break;
                }

            case Behaviour.SUCCESS:
                {
                    t += Time.deltaTime;
                    float s = 1 / successDuration;
                    float v = s * t;

                    if (t >= successDuration)
                    {
                        behaviour = Behaviour.NONE;
                        system.Unbusy();
                        image.transform.localScale = new Vector3(1, 1, 1);
                        image.color = new Color(1, 1, 1, 1);
                        transform.position = moveTarget;
                        break;
                    }
                    else if (t >= successDuration / 2)
                    {
                        v = 1 - s * t;
                    }

                    float color = 1 - v;
                    image.color = new Color(color, 1, color, 1);

                    float scale = 1 + successSize * v;
                    image.transform.localScale = new Vector3(scale, scale, 1);

                    float stepSize = remainingMoveDistance * successDuration / 2f;
                    transform.position += new Vector3(-stepSize, 0);

                    break;
                }

            case Behaviour.MOVE_OVER:
                {
                    t += Time.deltaTime;
                    float s = 1 / successDuration;

                    if (t >= successDuration)
                    {
                        behaviour = Behaviour.NONE;
                        transform.position = moveTarget;
                        break;
                    }

                    float stepSize = remainingMoveDistance * successDuration / 2f;
                    transform.position += new Vector3(-stepSize, 0);

                    break;
                }

            case Behaviour.FADE_AWAY:
                {
                    t += Time.deltaTime;
                    float s = 1 / fadeAwayDuration;
                    float v = s * t;

                    if (t >= fadeAwayDuration)
                    {
                        Destroy(this);
                        break;
                    }

                    float color = 1 - v;
                    image.color = new Color(1, 1, 1, color);

                    float scale = 1 - v;
                    image.transform.localScale = new Vector3(scale, scale, 1);

                    break;
                }
        }
    }

    private enum Behaviour
    {
        NONE,
        ERROR,
        SUCCESS,
        MOVE_OVER,
        FADE_AWAY
    }

}
