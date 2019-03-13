using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UISystem
{
    /// <summary>
    /// Class of UI manager - with it you can perform all pages open-close operations
    /// </summary>
    public class UIManager : Singleton<UIManager>
    {
        #region VARIABLES
        /// <summary>
        /// List of pages
        /// </summary>
        public List<UIPageData> Pages = new List<UIPageData>();
        /// <summary>
        /// Names of pages
        /// </summary>
        public string[] PageNames;
        /// <summary>
        /// Queue of pages 
        /// </summary>
        public LinkedList<string> PagesQueue = new LinkedList<string>();
        /// <summary>
        /// Name of current separate page(last)
        /// </summary>
        private string CurrentSeparatePage = "";
        /// <summary>
        /// Flag if for now some transaction is processing (we cant open another page if it's true)
        /// </summary>
        public bool ProcessingTransition = false;
        #endregion

        #region METHODS 
        /// <summary>
        /// Called once in the start
        /// </summary>
        //private void Awake()
        //{
        //    base.Awake();
         //if (Pages.Count > 0)
         //   {
         //       ShowPageImmediate(Pages[0].Name);
         //   }
        //}
        private void Start()
        {
            if (Pages.Count > 0)
            {
                ShowPageImmediate(Pages[0].Name);
            }
        }

        /// <summary>
        /// Call it to clear queue of pages
        /// </summary>
        public void ClearQueue()
        {
            PagesQueue.Clear();
        }

        /// <summary>
        /// Call it to open previous page
        /// </summary>
        public void OpenPreviousPage()
        {
            StartCoroutine(ProcessOpeningPreviousPage());
        }

        IEnumerator ProcessOpeningPreviousPage()
        {
            string _lastName = PagesQueue.Last.Value;

            PagesQueue.RemoveLast();
            if (PagesQueue.Count > 0)
                ShowPage(PagesQueue.Last.Value);
            yield return null;
        }

        /// <summary>
        /// Call it to show some page with delay
        /// </summary>
        /// <param name="_pageName">Name of page</param>
        public void ShowPage(string _pageName)
        {
            if (ProcessingTransition)
                return;

            StartCoroutine(ShowPageWaiting(_pageName));
        }

        /// <summary>
        /// Call it to show page immediate (without transitions)
        /// </summary>
        /// <param name="_pageName"></param>
        public void ShowPageImmediate(string _pageName)
        {
            if (ProcessingTransition)
                return;

            Debug.Log("Showing first page..." + _pageName);
            //looking page
            UIPageData _selectedPage = null;
            bool _foundPage = false;
            foreach (UIPageData _page in Pages)
            {
                if (_page != null && _page.Name == _pageName)
                {
                    _selectedPage = _page;
                    _foundPage = true;
                }
            }

            float _maxTime = 0f;

            if (_foundPage)
            {
                //process reaction of other pages on showing this page
                foreach (UIPageData _page in Pages)
                {
                    if (_page != null)
                    {
                        if (_page.ProcessReactionOnShow(_pageName) == PagesRelations.Hide)
                        {
                            if (_page.SeparatePage)
                            {
                                float _curTime = _page.GetWaitTime();
                                if (_maxTime < _curTime)
                                {
                                    _maxTime = _curTime;
                                }
                            }
                        }
                    }
                }
                //if it's separete page - add it to queue
                if (_selectedPage.SeparatePage)
                {
                    if (PagesQueue != null && PagesQueue.Last != null && PagesQueue.Last.Value == _selectedPage.Name)
                    {
                        //havent add if already contains that page
                    }
                    else
                    {
                        PagesQueue.AddLast(_selectedPage.Name);
                        CurrentSeparatePage = _selectedPage.Name;
                    }
                }

                if (_selectedPage != null)
                {
                    _selectedPage.TurnOnPageNow();
                }
            }
        }

        /// <summary>
        /// Coroutine of showing some page with delay
        /// </summary>
        /// <param name="_pageName"></param>
        /// <returns></returns>
        IEnumerator ShowPageWaiting(string _pageName)
        {
            if (ProcessingTransition)
                yield break; 

            //looking page
            UIPageData _selectedPage = null;
            bool _foundPage = false;
            foreach (UIPageData _page in Pages)
            {
                if (_page != null && _page.Name == _pageName)
                {
                    _selectedPage = _page;
                    _foundPage = true;
                }
            }

            if (_selectedPage != null && _selectedPage.SeparatePage)
                ProcessingTransition = true;

            //looking for max transtition time
            float _maxTime = 0f;
            if (_foundPage)
            {
                foreach (UIPageData _page in Pages)
                {
                    if (_page != null)
                    {
                        if (_page.ProcessReactionOnShow(_pageName) == PagesRelations.Hide)
                        {
                            if (_page.SeparatePage)
                            {
                                float _curTime = _page.GetWaitTime();
                                if (_maxTime < _curTime)
                                {
                                    _maxTime = _curTime;
                                }
                            }
                        }
                    }
                }
                //if separate - add to queue
                if (_selectedPage.SeparatePage)
                {
                    if (PagesQueue != null && PagesQueue.Last != null && PagesQueue.Last.Value == _selectedPage.Name)
                    {
                        //havent add if already contains that page
                    }
                    else
                    {
                        //Debug.Log("ShowPage  " + PagesQueue.Last.Value + " and new " + _selectedPage.Name);
                        PagesQueue.AddLast(_selectedPage.Name);
                        CurrentSeparatePage = _selectedPage.Name;
                    }
                }

                if (_selectedPage != null)
                {
                    //waiting for longest animation/transition
                    Log.Write("Max time:" + _maxTime);

                    if (_maxTime > 0f)
                        yield return new WaitForSeconds(_maxTime);

                    if (_selectedPage.GetWaitTimeOut() > 0f)
                        yield return new WaitForSeconds(_selectedPage.GetWaitTimeOut());

                    ProcessingTransition = false;

                    //showing selected page
                    _selectedPage.TurnOnPage();
                    Log.Write("Turning page:" + _pageName);
                }
            }
            else
            {
                Log.Write("Haven't found page with name:" + _pageName, LogColors.Yellow);
            }

        }

        /// <summary>
        /// Call it to hide some page
        /// </summary>
        /// <param name="_pageName">Name of page</param>
        public void HidePage(string _pageName)
        {
            //Log.Write("Hiding page:" + _pageName);
            foreach (UIPageData _page in Pages)
            {
                if (_page != null && _page.Name == _pageName)
                {
                    _page.TurnOffPage();
                }
            }
        }
        #endregion
    }
}