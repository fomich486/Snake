using UnityEngine;

namespace MatchMaker
{
    public class LanCreateParameters : MonoBehaviour, IMatchCreateParameters, IInspectorparametersDraw
    {
        public int PlayersCount;
        public string AppId;
        public void DrawInspector()
        {
            
        }
    }
}