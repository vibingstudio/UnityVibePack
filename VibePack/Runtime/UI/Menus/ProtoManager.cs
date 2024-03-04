using VibePack.Extensions;
using System.Collections;
using VibePack.Utility;
using UnityEngine;
using System;

namespace VibePack.UI
{
    public enum PageDirection
    {
        None = 0,
        Previous = -1,
        Next = 1,
    }

    public class PageManager : MonoBehaviour, IAwaitable
    {
        [Title("Page Manager", 0)]
        [SerializeField] Optional<Page> page;
        [SerializeField] Optional<int> openFirst;
        [SerializeField] Page[] pages;

        int openPageIndex = -1;
        bool isTransitioning;

        private void Start()
        {
            if (!page)
                return;

            if (page.Value == null)
                page = GetComponent<Page>();

            if (page.Value != null && openFirst)
                page.Value.onOpened.AddListener(OpenFirst);
        }

        private IEnumerator Switch(int oldIndex, int newIndex)
        {
            if (ShouldWait())
                yield break;
            
            isTransitioning = true;
            Page newPage = newIndex.InRange(0, pages.Length) ? pages[newIndex] : null;
            newPage.Open();
            newPage.onClose.AddListener(OnPageClosed);

            if (oldIndex.InRange(0, pages.Length) && pages[oldIndex] is not null and Page openPage && openPage.IsOpen())
            {
                openPage.onClose.RemoveListener(OnPageClosed);
                openPage.Close();
                yield return openPage.Await();
            }

            yield return newPage.Await();
            openPageIndex = newIndex;
            isTransitioning = false;
        }

        private void OnPageClosed()
        {
            if (ShouldWait())
                openPageIndex = -1;
        }

        public Coroutine Switch(int pageIndex) => StartCoroutine(Switch(openPageIndex, pageIndex));

        public Coroutine Switch(Page page) => StartCoroutine(Switch(openPageIndex, Array.IndexOf(pages, page)));

        public Coroutine Switch(PageDirection direction) => StartCoroutine(Switch(openPageIndex, (openPageIndex + ((int)direction)).Wrap(0, pages.Length - 1)));

        public void OpenFirst()
        {
            if (!openFirst || !openFirst.Value.InRange(0, pages.Length))
                return;

            page.Value.onOpened.RemoveListener(OpenFirst);
            pages[openPageIndex = openFirst].Open();
        }

        public void CloseAll()
        {
            if (!openPageIndex.InRange(0, pages.Length))
                return;

            pages[openPageIndex].Close();
        }

        public virtual bool ShouldWait() => isTransitioning || (openPageIndex.InRange(0, pages.Length) && pages[openPageIndex] && pages[openPageIndex].ShouldWait());

        public CustomYieldInstruction Await() => new Awaiter(this);

        public static implicit operator Awaiter(PageManager protoable) => new Awaiter(protoable);
    }
}
