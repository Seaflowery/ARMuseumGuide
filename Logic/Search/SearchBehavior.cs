using System.Collections.Generic;

namespace UnityEngine
{
    public class SearchBehavior : MonoBehaviour
    {
        private static Dictionary<string, SearchNode> dicSearchNode = new Dictionary<string, SearchNode>();

        private static Stack<string> stackSearchPath = new Stack<string>(); // 搜索路径

        private static string sDefaultContent = "入口"; // 初始页面默认

        
        /// <summary>
        /// 获得当前搜索的内容
        /// </summary>
        /// <returns></returns>
        public static string GetLatestSearchContent()
        {
            if (stackSearchPath.Count == 0)
            {
                return sDefaultContent;
            }
            return stackSearchPath.Peek();
        }

        /// <summary>
        /// 获得返回上一级搜索的内容
        /// </summary>
        /// <returns></returns>
        public static string GetPrevSearchContent()
        {
            if (stackSearchPath.Count == 0)
            {
                return sDefaultContent;
            }

            stackSearchPath.Pop();
            if (stackSearchPath.Count == 0)
            {
                return sDefaultContent;
            }
            return stackSearchPath.Peek();
        }



        public static SearchNode GetNodeByContent(string content)
        {
            if (dicSearchNode.ContainsKey(content))
            {
                return dicSearchNode[content];
            }
            stackSearchPath.Push(content);
            return null;
        }

        public static void AddNode(string content, SearchNode node)
        {
            dicSearchNode[content] = node;
        }
    }
}