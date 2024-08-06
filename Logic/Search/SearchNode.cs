using System.Collections.Generic;
using LitJson;

public class SearchNode
{
    public string sNodeName;
    public string sNodeType;
    public List<string> listSubNodes;
    public string sIntroduction;
    public List<string> listPicturePath;

    public SearchNode(string nodeName, string nodeType, List<string> subNodes, string introduction,
        List<string> picturePath)
    {
        sNodeName = nodeName;
        sNodeType = nodeType;
        listSubNodes = subNodes;
        sIntroduction = introduction;
        listPicturePath = picturePath;
    }
    
    public override string ToString()
    {
        return string.Format("NodeName:{0}\nNodeType={1}\nSubNodes={2}\nIntroduction={3}\nPicturePath={4}\n",
            sNodeName, sNodeType, string.Join(",", listSubNodes.ToArray()), sIntroduction,
            string.Join(",", listPicturePath.ToArray()));
    }


    public static SearchNode GetNodeByJson(JsonData result)
    {
        string name = result["name"].ToString();
        string type = result["nodetype"].ToString();
        List<string> subNodes = new List<string>(result["subnodes"].ToString().Split('&'));
        string introduction = result["introduction"].ToString();
        List<string> picturePath = new List<string>(result["picturePath"].ToString().Split('&'));
        return new SearchNode(name, type, subNodes, introduction, picturePath);
    }
}