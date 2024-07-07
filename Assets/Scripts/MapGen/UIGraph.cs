using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIGraph
{

    private int numLevels;
    private RectTransform graphContainer;
    private Sprite circleSprite;
    private Sprite currentLevelSprite;
    private List<int>[] graph;
    private GameObject currentLevel;
    public GameObject[] circles;

    public UIGraph(int numLevels, RectTransform graphContainer, Sprite circleSprite, Sprite currentLevelSpriteTemp, List<int>[] graph)
    {
        this.numLevels = numLevels;
        this.graphContainer = graphContainer;
        this.circleSprite = circleSprite;
        this.currentLevelSprite = currentLevelSpriteTemp;
        this.graph = graph;
        circles = new GameObject[numLevels];
    }

    public void createUIGraph(int displayRows, float xPadding, float yPadding)
    {
        //creates circles for every node in the graph
        float graphHeight = graphContainer.sizeDelta.y;
        float graphWidth = graphContainer.sizeDelta.x;

        //draws a sprite for every node in a grid pattern
        for(int i = 0; i < numLevels; i++)
        {
            float xPos = (((int)Mathf.Ceil(i / displayRows)) * (graphWidth / Mathf.Ceil(numLevels / displayRows)) + xPadding - ((xPadding/2) * (int)(i / displayRows)));
            float yPos = ((i % displayRows) * (graphHeight / displayRows)) + yPadding; //- ((yPadding/2) * (i));
            circles[i] = createCircle(new Vector2(xPos, yPos));
        }

        //creates current level game object
        currentLevel = new GameObject("Current_Level_Indicator", typeof(Image));
        currentLevel.transform.SetParent(graphContainer, false);
        currentLevel.GetComponent<Image>().sprite = currentLevelSprite;
        RectTransform rectrans = currentLevel.GetComponent<RectTransform>();
        rectrans.anchoredPosition = new Vector2(xPadding, yPadding);
        rectrans.sizeDelta = new Vector2(20,20);
        rectrans.anchorMin = new Vector2(0,0);
        rectrans.anchorMax = new Vector2(0,0);

        createConnections();
    }

    private GameObject createCircle(Vector2 anchorPos)
    {
        GameObject gameOb = new GameObject("circle", typeof(Image));
        gameOb.transform.SetParent(graphContainer, false);
        gameOb.GetComponent<Image>().sprite = circleSprite;
        RectTransform rectrans = gameOb.GetComponent<RectTransform>();
        rectrans.anchoredPosition = anchorPos;
        rectrans.sizeDelta = new Vector2(11,11);
        rectrans.anchorMin = new Vector2(0,0);
        rectrans.anchorMax = new Vector2(0,0);
        return gameOb;
    }

    private void createConnections()
    {
        for(int i = 0; i < this.numLevels; i++)
        {
            //Debug.Log("Level: " + i);
            foreach(int connection in this.graph[i])
            {
                createConnection(circles[i].GetComponent<RectTransform>().anchoredPosition, circles[connection].GetComponent<RectTransform>().anchoredPosition);
                //Debug.Log("connection: " + connection);
            }
        }
    }

    private void createConnection(Vector2 nodePosA, Vector2 nodePosB)
    {
        GameObject gameOb = new GameObject("dotConnection", typeof(Image));
        gameOb.transform.SetParent(graphContainer, false);
        gameOb.GetComponent<Image>().color = new Color(1,1,1, .5f);
        RectTransform rectrans = gameOb.GetComponent<RectTransform>();
        Vector2 dir = (nodePosB - nodePosA);
        //Vector2 dir = new Vector2(Mathf.Abs(temp.x), Mathf.Abs(temp.y));
        dir = dir.normalized;
        float angle = Vector2.Angle(dir, Vector2.right);
        if(dir.y < 0)
        {
            angle = Vector2.Angle(dir, Vector2.left) + 180;
        }
        //Debug.Log(angle +  ", " + dir.x + ", " + dir.y);
        float distance = Vector2.Distance(nodePosA, nodePosB);
        rectrans.anchorMin = new Vector2(0,0);
        rectrans.anchorMax = new Vector2(0,0);
        rectrans.sizeDelta = new Vector2(distance,3f);
        rectrans.anchoredPosition = nodePosA + dir * distance * .5f;
        //Debug.Log(rectrans.anchoredPosition + ", " + nodePosA.x + ", " + nodePosB.x);
        rectrans.localEulerAngles = new Vector3(0,0, angle);
    }

    public void setCurrentLevel(int level)
    {
        currentLevel.GetComponent<RectTransform>().anchoredPosition = circles[level].GetComponent<RectTransform>().anchoredPosition;
    }

}
