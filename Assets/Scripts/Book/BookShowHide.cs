using UnityEngine;

public class BookShowHide : MonoBehaviour
{
    public GameObject book; // Reference to the book GameObject
    public bool isBookVisible = false; // Flag to track the visibility of the book
    public void ShowBook()
    {
        isBookVisible = true;
        book.SetActive(true);
    }
    public void HideBook()
    {
        isBookVisible = false;
        book.SetActive(false);
    }

    public void ToggleBookVisibility()
    {
        isBookVisible = !isBookVisible;
        book.SetActive(isBookVisible);
    }
}