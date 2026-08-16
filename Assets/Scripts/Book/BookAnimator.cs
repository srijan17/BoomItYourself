using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class BookAnimator : MonoBehaviour
{

    public float AnimationDuration = 0.3f; // Duration of the page flip animation
    public GameObject lastPage; // Reference to the last page of the book
    public GameObject firstPage; // Reference to the first page of the book
    public List<PageAnimate> pages; // List of PageAnimate components for each page

    public float pageCount ; // Number of pages in the book
    [SerializeField] private int currentPageIndex; // Index of the current page being displayed
    private void Awake()
    {
        // Get all PageAnimate components in the children of this GameObject
        pages = new List<PageAnimate>(GetComponentsInChildren<PageAnimate>());
        pageCount = pages.Count;
        currentPageIndex = 0; // Start with the first page
        //Hide all pages except the first one
        for (int i = 1; i < pages.Count; i++)
        {
            pages[i].SetSpeed(AnimationDuration);
            pages[i].gameObject.SetActive(false);
        }
    }

    public void NextPage(){
        
        if (currentPageIndex < pages.Count - 1)
        {
            //Activate the next page and animate the current page flip
            pages[currentPageIndex + 1].gameObject.SetActive(true);
            pages[currentPageIndex].AnimatePageFlip();
            currentPageIndex++;
        }
        if(currentPageIndex == pages.Count - 1)
        {
            // If we are at the last page, deactivate the first page
            lastPage.SetActive(true);
            firstPage.SetActive(true);
        }
        else{
            lastPage.SetActive(false);
            StartCoroutine(ActivatePageAfterDelay(firstPage, AnimationDuration));

        }
    }

    public void PreviousPage(){

        if (currentPageIndex > 0)
        {
            //Activate the previous page and animate the current page flip back
            currentPageIndex--;
            pages[currentPageIndex].gameObject.SetActive(true);
            pages[currentPageIndex].AnimatePageFlipBack();
            StartCoroutine(DeactivatePageAfterDelay(pages[currentPageIndex + 1].gameObject, AnimationDuration));

        }

        if(currentPageIndex == 0)
        {
            // If we are at the first page, deactivate the last page
            firstPage.SetActive(false);
        }
        else{
            firstPage.SetActive(true);
        }
       
        // Remove this block as it is redundant and causes errors
    }

    private IEnumerator DeactivatePageAfterDelay(GameObject page, float delay)
    {
        yield return new WaitForSeconds(delay);
        page.SetActive(false);
    }
    private IEnumerator ActivatePageAfterDelay(GameObject page, float delay)
    {
        yield return new WaitForSeconds(delay);
        page.SetActive(true);
    }
}