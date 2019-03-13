using SFXManagerModule;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace UISystem
{
    /// <summary>
    /// Custom serialiable class for drawing it in unity editor 
    /// </summary>
    [Serializable]
    public class ExampleEvent : UnityEvent { }

    /// <summary>
    /// Manager of UI pages
    /// </summary>
    public class UIPageData : MonoBehaviour
    {
        #region VARIABLES
        /// <summary>
        /// Flag if we have to call OnShowEvent in the start
        /// </summary>
        public bool CalledOnStart = false;
        /// <summary>
        /// Callback, that Invokes when page becames active
        /// </summary>
        public ExampleEvent OnShowEvent;
        /// <summary>
        /// Callback, that invokes when page becames inactive
        /// </summary>
        public ExampleEvent OnHideEvent;
        /// <summary>
        /// Custom name of page
        /// </summary>
        public string Name;
        /// <summary>
        /// Cashed canvas group
        /// </summary>
        private CanvasGroup pageGroup;
        /// <summary>
        /// Cashed animator
        /// </summary>
        private Animation anim;
        /// <summary>
        /// Animation of becoming active
        /// </summary>
        public AnimationClip ShowAnimation;
        /// <summary>
        /// Animation of becoming inactive
        /// </summary>
        public AnimationClip HideAnimation;
        /// <summary>
        /// Flag if we need to make fade
        /// </summary>
        public bool MakeFade = true;
        /// <summary>
        /// Flag if animations have to depend on real timescale, or on virtual
        /// </summary>
        public bool DependsOnReal = true;
        /// <summary>
        /// Time of fade when becoming active 
        /// </summary>
        public float FadeInTime = 0f;
        /// <summary>
        /// Time of fade when becoming inactive
        /// </summary>
        public float FadeOutTime = 0f;
        /// <summary>
        /// flag if page currently active
        /// </summary>
        public bool CurrentlyActive = false;
        /// <summary>
        /// List of relationships 
        /// </summary>
        public List<RelationshipData> Relations = new List<RelationshipData>();
        /// <summary>
        /// Name of music theme for current page
        /// </summary>
        public string MusicTheme = "";
        /// <summary>
        /// Flag if this page takes part in global queue
        /// </summary>
        public bool SeparatePage = true;
        #endregion

        #region EDITOR_VARIABLES
        /// <summary>
        /// flag if we have to show deep options of page in editor
        /// </summary>
        public bool ShowInfo = false;
        #endregion

        #region METHODS
        /// <summary>
        /// Called once in the start
        /// </summary>
        private void Awake()
        {
            InstallVariables();
        }

        /// <summary>
        /// Call it to install local variables
        /// </summary>
        public void InstallVariables()
        {
            pageGroup = this.gameObject.GetComponent<CanvasGroup>();
            anim = this.gameObject.GetComponent<Animation>();
        }

        /// <summary>
        /// Call it to turn on page smoothly
        /// </summary>
        public void TurnOnPage()
        {
            if (CurrentlyActive)
                return;

            if (CalledOnStart)
            {
                //.Write("Invoking methods on page:"+Name);
                OnShowEvent.Invoke();
            }

            //setting active panel
            pageGroup.transform.GetChild(0).gameObject.SetActive(true);
            //switching music theme
            SFXManager.Instance.SwitchMusicTheme(MusicTheme);
            CurrentlyActive = true;
            //is we have show animation - turn it on
            if (ShowAnimation != null)
            {
                if (!MakeFade)
                {
                    StartCoroutine(TurnOnPageAnimation(true));
                }
                else
                {
                    if (ShowAnimation.length > FadeInTime)
                    {
                        StartCoroutine(TurnOnPageSmoothly(false));
                        StartCoroutine(TurnOnPageAnimation(true));
                    }
                    else
                    {
                        StartCoroutine(TurnOnPageSmoothly(true));
                        StartCoroutine(TurnOnPageAnimation(false));
                    }
                }
            }
            //else - just make transition
            else
            {
                StartCoroutine(TurnOnPageSmoothly(true));
            }
        }

        /// <summary>
        /// IEnumerator of fade animation 
        /// </summary>
        /// <param name="_callActivation">Flag if after animation TurnOnPageProcess() have to be called</param>
        /// <returns></returns>
        IEnumerator TurnOnPageSmoothly(bool _callActivation)
        {
            if (FadeInTime > 0f)
            {
                float _lerpCoef = 0f;
                float _lerpSpeed = 1f / FadeInTime;
                while (_lerpCoef <= 1)
                {
                    pageGroup.alpha = Mathf.Lerp(0f, 1f, _lerpCoef);

                    if (DependsOnReal)
                        _lerpCoef += Time.deltaTime * _lerpSpeed;
                    else
                        _lerpCoef += TimeScaleManager.Instance.GetDelta() * _lerpSpeed;

                    yield return new WaitForEndOfFrame();
                }
            }

            pageGroup.alpha = 1f;
            pageGroup.blocksRaycasts = true;
            pageGroup.interactable = true;
            if (_callActivation && !CalledOnStart)
            {
                TurnOnPageProcess();
            }
        }

        /// <summary>
        /// IEnumerator of animation
        /// </summary>
        /// <param name="_callActivation">Flag if after animation TurnOnPageProcess() have to be called</param>
        /// <returns></returns>
        IEnumerator TurnOnPageAnimation(bool _callActivation)
        {
            //making in or animation
            pageGroup.transform.GetChild(0).gameObject.SetActive(true);
            pageGroup.alpha = 1f;
            if (DependsOnReal)
            {
                anim.clip = ShowAnimation;
                anim.Play();
                anim[ShowAnimation.name].speed = Time.timeScale;
                if (ShowAnimation.length > 0f)
                    yield return new WaitForSeconds(ShowAnimation.length);
            }
            else
            {
                anim.clip = ShowAnimation;
                anim.Play();
                anim[ShowAnimation.name].speed = TimeScaleManager.Instance.GetScale();
                if (ShowAnimation.length * TimeScaleManager.Instance.GetScale() > 0f)
                    yield return new WaitForSeconds(ShowAnimation.length * TimeScaleManager.Instance.GetScale());
            }
            //if we need it - turning on page, making it interactable, etc 
            if (_callActivation)
            {
                TurnOnPageProcess();
                pageGroup.blocksRaycasts = true;
                pageGroup.interactable = true;
            }
        }

        /// <summary>
        /// Call it to show page immediate
        /// </summary>
        public void TurnOnPageNow()
        {
            if (CurrentlyActive)
            {
                return;
            }
            if (pageGroup != null)
            {
                print("pageGroup not null");
                pageGroup.transform.GetChild(0).gameObject.SetActive(true);
            }
            SFXManager.Instance.SwitchMusicTheme(MusicTheme);
            CurrentlyActive = true;
            if (ShowAnimation != null)
            {
                anim.clip = ShowAnimation;
                anim[ShowAnimation.name].time = ShowAnimation.length;
                anim.Play();
                //anim.Stop();
            }
            pageGroup.alpha = 1f;
            pageGroup.blocksRaycasts = true;
            pageGroup.interactable = true;

            TurnOnPageProcess();
        }

        /// <summary>
        /// Call it to invoke OnShow callbacks
        /// </summary>
        void TurnOnPageProcess()
        {
            OnShowEvent.Invoke();
        }

        /// <summary>
        /// Call it to turn off page
        /// </summary>
        public void TurnOffPage()
        {
            if (!CurrentlyActive)
                return;

            OnHideEvent.Invoke();
            CurrentlyActive = false;

            if (HideAnimation != null)
            {
                if (!MakeFade)
                {
                    StartCoroutine(TurnOffPageAnimation(true));
                }
                else
                {
                    StartCoroutine(TurnOffPageSmoothly());
                    StartCoroutine(TurnOffPageAnimation());
                }
            }
            else
            {
                StartCoroutine(TurnOffPageSmoothly());
            }
        }

        /// <summary>
        /// Call it to get the biggest waiting time of page showing (of animation, or fadein)
        /// </summary>
        /// <returns></returns>
        public float GetWaitTimeOut()
        {
            if (ShowAnimation != null)
            {
                if (!MakeFade)
                {
                    if (DependsOnReal)
                        return ShowAnimation.length;
                    else
                        return ShowAnimation.length * TimeScaleManager.Instance.GetScale();
                }
                else
                {
                    if (ShowAnimation.length > FadeInTime)
                    {
                        if (DependsOnReal)
                            return ShowAnimation.length;
                        else
                            return ShowAnimation.length * TimeScaleManager.Instance.GetScale();
                    }
                    else
                    {
                        return 0f;
                    }
                }
            }
            return 0f;
        }

        /// <summary>
        /// Call it to get wait time for closig of this page (fadeout or animation)
        /// </summary>
        /// <returns></returns>
        public float GetWaitTime()
        {
            if (!this.transform.GetChild(0).gameObject.activeSelf)
                return 0f;

            if (HideAnimation != null)
            {
                if (!MakeFade)
                {
                    if (DependsOnReal)
                        return HideAnimation.length;
                    else
                        return HideAnimation.length * TimeScaleManager.Instance.GetScale();
                }
                else
                {
                    if (HideAnimation.length > FadeOutTime)
                    {
                        if (DependsOnReal)
                            return HideAnimation.length;
                        else
                            return HideAnimation.length * TimeScaleManager.Instance.GetScale();
                    }
                    else
                    {
                        return 0f;
                    }
                }
            }
            return 0f;
        }

        /// <summary>
        /// IEnumerator of fade animation 
        /// </summary>
        /// <param name="_callActivation">Flag if after animation TurnOnPageProcess() have to be called</param>
        /// <returns></returns>
        IEnumerator TurnOffPageSmoothly()
        {
            if (FadeOutTime > 0f)
            {
                float _lerpCoef = 0f;
                float _lerpSpeed = 1f / FadeOutTime;
                while (_lerpCoef <= 1)
                {
                    pageGroup.alpha = Mathf.Lerp(1f, 0f, _lerpCoef);

                    if (DependsOnReal)
                        _lerpCoef += Time.deltaTime * _lerpSpeed;
                    else
                        _lerpCoef += TimeScaleManager.Instance.GetDelta() * _lerpSpeed;

                    yield return new WaitForEndOfFrame();
                }
            }

            pageGroup.alpha = 0f;
            TurnOffPageProcess();
        }

        /// <summary>
        /// IEnumerator of animation
        /// </summary>
        /// <param name="_callActivation">Flag if after animation TurnOnPageProcess() have to be called</param>
        /// <returns></returns>
        IEnumerator TurnOffPageAnimation(bool _processOffPage = false)
        {
            if (DependsOnReal)
            {
                anim.clip = HideAnimation;
                anim.Play();
                anim[HideAnimation.name].speed = Time.timeScale;
                yield return new WaitForSeconds(HideAnimation.length);
            }
            else
            {
                anim.clip = HideAnimation;
                anim.Play();
                anim[HideAnimation.name].speed = TimeScaleManager.Instance.GetScale();
                yield return new WaitForSeconds(HideAnimation.length * TimeScaleManager.Instance.GetScale());
            }

            if (_processOffPage)
                TurnOffPageProcess();
        }

        /// <summary>
        /// Call it to hide page right now
        /// </summary>
        public void TurnOffPageNow()
        {
            if (!CurrentlyActive)
                return;

            CurrentlyActive = false;
            OnHideEvent.Invoke();
            TurnOffPageProcess();

            if (HideAnimation != null)
            {
                anim.clip = HideAnimation;
                anim[HideAnimation.name].time = HideAnimation.length;
                anim.Stop();
            }
        }

        /// <summary>
        /// Call it to process turning off(hiding canvas group)
        /// </summary>
        void TurnOffPageProcess()
        {
            pageGroup.alpha = 0f;
            pageGroup.blocksRaycasts = false;
            pageGroup.interactable = false;
            pageGroup.transform.GetChild(0).gameObject.SetActive(false);
        }

        /// <summary>
        /// Call it to process reaction on showing some page
        /// </summary>
        /// <param name="_pageName">Name of showing page</param>
        public PagesRelations ProcessReactionOnShow(string _pageName)
        {
            if (_pageName == Name)
                return PagesRelations.Ignore;

            foreach (RelationshipData _data in Relations)
            {
                if (_data.PageName == _pageName)
                {
                    switch (_data.RelationType)
                    {
                        case PagesRelations.Hide:
                            UIManager.Instance.HidePage(Name);
                            break;
                        case PagesRelations.Show:
                            UIManager.Instance.ShowPage(Name);
                            break;
                        case PagesRelations.Ignore:
                            break;
                    }
                    return _data.RelationType;
                }
            }
            return PagesRelations.Ignore;
        }

        /// <summary>
        /// Call it to update list of relationships with saving previous options(if have)
        /// </summary>
        /// <param name="_rels"></param>
        public void UpdateRelations(List<RelationshipData> _rels)
        {
            List<RelationshipData> _relations = new List<RelationshipData>();

            foreach (RelationshipData _data in _rels)
            {
                if (GetIsContain(_data.PageName))
                {
                    _data.RelationType = GetRelation(_data.PageName);
                }
                _relations.Add(new RelationshipData(_data));
            }
            Relations = _relations;
        }

        /// <summary>
        /// Call it to get relation with some Page
        /// </summary>
        /// <param name="_name">Name of some page</param>
        /// <returns></returns>
        PagesRelations GetRelation(string _name)
        {
            foreach (RelationshipData _data in Relations)
            {
                if (_data.PageName == _name)
                {
                    return _data.RelationType;
                }
            }

            return PagesRelations.Ignore;
        }

        /// <summary>
        /// Call it to get know if we have relation with some page
        /// </summary>
        /// <param name="_name"></param>
        /// <returns></returns>
        bool GetIsContain(string _name)
        {
            foreach (RelationshipData _data in Relations)
            {
                if (_data.PageName == _name)
                {
                    return true;
                }
            }
            return false;
        }
        #endregion
    }

    /// <summary>
    /// Data of page relation
    /// </summary>
    [Serializable]
    public class RelationshipData
    {
        /// <summary>
        /// name of page
        /// </summary>
        public string PageName;
        /// <summary>
        /// Type of relation with page with name: PageName
        /// </summary>
        public PagesRelations RelationType = PagesRelations.Hide;

        /// <summary>
        /// Default constructor
        /// </summary>
        public RelationshipData()
        {
            //nothing
        }

        /// <summary>
        /// Custom constructor with copying info
        /// </summary>
        /// <param name="_data"></param>
        public RelationshipData(RelationshipData _data)
        {
            PageName = _data.PageName;
            RelationType = _data.RelationType;
        }
    }

    /// <summary>
    /// Relation type between pages
    /// </summary>
    public enum PagesRelations
    {
        Ignore,
        Show,
        Hide,
    }
}