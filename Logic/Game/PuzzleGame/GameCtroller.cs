using System.Collections;
using System.Collections.Generic;
using Manager;
using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.UI;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

struct Move
{
    public GameObject objToMov;
    public int direction;
    public Vector3 startPos, EndPos;
}

struct Direction
{
    public int dx, dy;

    public Direction(int dx, int dy)
    {
        this.dx = dx;
        this.dy = dy;
    }
}

public class GameCtroller : MonoBehaviour
{
    public GameObject canvasObject;
    public GameObject originImageObject;
    private GameObject[,] puzzleObjects;
    private int[,] map;
    private const int hardValue = 2;
    private const int changeTimes = hardValue * hardValue * hardValue * hardValue;

    private const float deltaPos = 0.002f;
    private const float eps = (float) 1e-2;
    private float velocity = 0.45f;
    private Move move = new Move();
    private Direction[] direction;
    public GameObject gameText;
    private bool hasInitial = false;
    private int stage = 1; // 0 finish, 1 in progess
    private string finishString = "恭喜你!\n拼图完成!";
    private string startString = "游戏进行中";
    private int lackValue = -1;

    // Start is called before the first frame update
    void Start()
    {
        Initiate();
    }

    // Update is called once per frame
    void Update()
    {
        if (move.direction != -1)
        {
            Vector3 nowPos = move.objToMov.transform.localPosition;
            if (Vector3.Distance(nowPos, move.EndPos) < eps)
            {
                move.direction = -1;
                move.objToMov.transform.localPosition = move.EndPos;
            }
            else
            {
                Vector3 normal = Vector3.Normalize(move.EndPos - nowPos);
                move.objToMov.transform.localPosition = nowPos + normal * velocity * Time.deltaTime;
            }
        }
        else
        {
            if (stage == 1 && JudgeGameWin())
            {
                stage = 0;
                gameText.GetComponent<TextMesh>().text = finishString;
                GameEnd();
            }
        }
    }

    string RandomImagePath()
    {
        return "Images/gansu";
    }

    bool JudgeGameWin()
    {
        int i, j, k;
        for (i = 0; i < hardValue; i++)
        {
            for (j = 0; j < hardValue; j++)
            {
                if (map[i, j] != -1 && map[i, j] != i * hardValue + j)
                {
                    return false;
                }
            }
        }

        return true;
    }

    public void GameEnd()
    {
        int i, j;
        move.direction = -1;
        AudioManager.Instance.Play(Manager.GameAudio.sGameEnd);
    }

    // Sprite GetSubSprite(Sprite old,int row,int column)
    // {
    //     // Sprite result=new Sprite()
    // }
    public void Initiate()
    {
        int i, j, k;
        gameText.GetComponent<TextMesh>().text = startString;
        string imagePath = RandomImagePath();
        direction = new Direction[4];
        direction[0] = new Direction(0, 1);
        direction[1] = new Direction(0, -1);
        direction[2] = new Direction(1, 0);
        direction[3] = new Direction(-1, 0);
        originImageObject.GetComponent<Image>().sprite = Resources.Load<Sprite>(imagePath);
        puzzleObjects = new GameObject[hardValue, hardValue];
        map = new int[hardValue, hardValue];
        var originTransform = originImageObject.GetComponent<RectTransform>();
        var originSprite = originImageObject.GetComponent<Image>().sprite;
        Vector3 leftUp = new Vector3(originTransform.position.x, originTransform.position.y,
            originTransform.position.z);
        move.direction = -1;
        leftUp.x -= originTransform.sizeDelta.x * originTransform.lossyScale.x / 2;
        leftUp.y -= originTransform.sizeDelta.y * originTransform.lossyScale.y / 2;
        leftUp.z = originTransform.position.z;
        hasInitial = true;
        for (i = 0; i < hardValue; i++)
        {
            for (j = 0; j < hardValue; j++)
            {
                map[i, j] = hardValue * i + j;
                puzzleObjects[i, j] = GameObject.Instantiate(originImageObject);
                var puzzleHere = puzzleObjects[i, j];
                var hereTrans = puzzleHere.GetComponent<RectTransform>();
                hereTrans.SetParent(canvasObject.transform);
                // puzzleHere.transform.rotation = originImageObject.transform.rotation;
                hereTrans.localScale = originTransform.localScale / hardValue;
                Vector3 loc = new Vector3(leftUp.x, leftUp.y, leftUp.z);
                float szX = hereTrans.sizeDelta.x * hereTrans.lossyScale.x,
                    szY = hereTrans.sizeDelta.y * hereTrans.lossyScale.y;
                loc.x += szX * j;
                loc.x += szX / 2;
                loc.y -= szY * i;
                loc.y -= szY / 2;
                puzzleHere.transform.position = loc;
                Image image = puzzleHere.GetComponent<Image>();
                int textureH = originSprite.texture.height, textureW = originSprite.texture.width;
                image.sprite = Sprite.Create(originSprite.texture,
                    new Rect(textureW / hardValue * j, textureH / hardValue * (hardValue - 1 - i), textureW / hardValue,
                        textureH / hardValue), new Vector2(0.5f, 0.5f));
                puzzleHere.AddComponent<Interactable>();
                puzzleHere.AddComponent<NearInteractionTouchable>();
                puzzleHere.AddComponent<PressableButtonHoloLens2>();
                PressableButtonHoloLens2 button = puzzleHere.GetComponent<PressableButtonHoloLens2>();
                button.ButtonPressed.AddListener(() =>
                {
                    lock (this)
                    {
                        if (stage == 1 && move.direction == -1)
                        {
                            var pos = transform.position;
                            move.objToMov = gameObject;
                            int k, l, m;
                            bool find = false;
                            for (k = 0; k < hardValue; k++)
                            {
                                for (l = 0; l < hardValue; l++)
                                {
                                    if (puzzleObjects[k, l] == puzzleHere)
                                    {
                                        ClickPuzzle(k, l);
                                        find = true;
                                        break;
                                    }
                                }
                                if (find) break;
                            }
                        }
                    }
                });
            }
        }

        DisorderPuzzles();
    }

    void ClickPuzzle(int x, int y)
    {
        int i, j;
        AudioManager.Instance.Play(Manager.GameAudio.sButtonPressed);
        for (i = 0; i < 4; i++)
        {
            int targetX = x + direction[i].dx, targetY = y + direction[i].dy;
            if (Legal(targetX, targetY) && map[targetX, targetY] == -1)
            {
                move.objToMov = puzzleObjects[x, y];
                move.direction = i;
                move.startPos = move.objToMov.transform.localPosition;
                move.EndPos = puzzleObjects[targetX, targetY].transform.localPosition;
                puzzleObjects[targetX, targetY].transform.localPosition = move.startPos;
                map[targetX, targetY] = map[x, y];
                map[x, y] = -1;
                var tmp = puzzleObjects[x, y];
                puzzleObjects[x, y] = puzzleObjects[targetX, targetY];
                puzzleObjects[targetX, targetY] = tmp;
                return;
            }
        }
    }

    bool Legal(int x, int y)
    {
        return 0 <= x && x < hardValue && 0 <= y && y < hardValue;
    }

    public void Restart()
    {
        int i, j;
        gameText.GetComponent<TextMesh>().text = startString;
        for (i = 0; i < hardValue; i++)
        {
            for (j = 0; j < hardValue; j++)
            {
                puzzleObjects[i, j].SetActive(true);
                if (map[i, j] == -1)
                {
                    map[i, j] = lackValue;
                }
            }
        }
        DisorderPuzzles();
    }

    public void DisorderPuzzles()
    {
        System.Random random = new System.Random();
        int lacking = random.Next(0, hardValue * hardValue);
        int lackingX = lacking / hardValue, lackingY = lacking % hardValue;
        puzzleObjects[lackingX, lackingY].SetActive(false);
        lackValue = map[lackingX, lackingY];
        map[lackingX, lackingY] = -1;
        int i, j, k;
        for (i = 0; i < changeTimes; i++)
        {
            int directionInd = random.Next(0, 4);
            Direction directionHere = direction[directionInd];
            if (Legal(lackingX + directionHere.dx, lackingY + directionHere.dy))
            {
                int fromPos = lackingX * hardValue + lackingY,
                    toPosition = (lackingX + directionHere.dx) * hardValue + lackingY + directionHere.dy;
                int tmp = map[fromPos / hardValue, fromPos % hardValue];
                map[fromPos / hardValue, fromPos % hardValue] = map[toPosition / hardValue, toPosition % hardValue];
                map[toPosition / hardValue, toPosition % hardValue] = tmp;
                Vector3 temPos = puzzleObjects[fromPos / hardValue, fromPos % hardValue].transform.position;
                puzzleObjects[fromPos / hardValue, fromPos % hardValue].transform.position =
                    puzzleObjects[toPosition / hardValue, toPosition % hardValue].transform.position;
                puzzleObjects[toPosition / hardValue, toPosition % hardValue].transform.position = temPos;
                var tmpObj = puzzleObjects[fromPos / hardValue, fromPos % hardValue];
                puzzleObjects[fromPos / hardValue, fromPos % hardValue] =
                    puzzleObjects[toPosition / hardValue, toPosition % hardValue];
                puzzleObjects[toPosition / hardValue, toPosition % hardValue] = tmpObj;
                lackingX += directionHere.dx;
                lackingY += directionHere.dy;
            }
        }
        stage = 1;
    }
}