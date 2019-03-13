namespace MatchMaker
{
    using UnityEngine;
    using UnityEngine.UI;
    using UnityEngine.Networking;
    using UnityEngine.Networking.Match;
    using System.Collections;
    using System.Collections.Generic;
    using System;


    public class MatchServerListUI : MonoBehaviour
    {
        public RectTransform serverListRect;
        public GameObject serverEntryPrefab;
        public GameObject noServerFound;

        static Color OddServerColor = new Color(1.0f, 1.0f, 1.0f, 1.0f);
        static Color EvenServerColor = new Color(.94f, .94f, .94f, 1.0f);

        void OnEnable()
        {
            foreach (Transform t in serverListRect)
                Destroy(t.gameObject);

            noServerFound.SetActive(false);

            RequestPage();
        }

        public void OnGUIMatchList(object sender, EventArgs e)
        {
            MatchMakingManager.Instance.MatchMaker.MatchListGot -= OnGUIMatchList;
            var _responce = e as IMatchListResponce;
            if (_responce.Count == 0)
            {
                return;
            }

            noServerFound.SetActive(false);
            foreach (Transform t in serverListRect)
                Destroy(t.gameObject);

            for (int i = 0; i < _responce.Count; ++i)
            {
                GameObject o = Instantiate(serverEntryPrefab) as GameObject;

                o.GetComponent<IServerEntry>().Populate(_responce[i], MatchMakingManager.Instance.MatchMaker, (i % 2 == 0) ? OddServerColor : EvenServerColor);

                o.transform.SetParent(serverListRect, false);
            }
        }

        public void ChangePage(int dir)
        {
            RequestPage();
        }

        public void RequestPage()
        {
            MatchMakingManager.Instance.MatchMaker.MatchListGot += OnGUIMatchList;
            MatchMakingManager.Instance.MatchMaker.ListMatches();
        }
    }

}