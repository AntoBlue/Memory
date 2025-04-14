using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class NewMonoBehaviourScript : MonoBehaviour
{
    [SerializeField] GameObject card;
    [SerializeField] Vector3[] cards;
    [SerializeField] Texture2D[] images;

    [SerializeField] float startX;
    [SerializeField] float startY;
    [SerializeField] float planeZ;

    [SerializeField] float deltaX = 1.1f;
    [SerializeField] float deltaY = 1.1f;
    [SerializeField] int columns = 5;
    [SerializeField] int rows = 6;

    [SerializeField] GameObject winUI;
    [SerializeField] AudioSource matchedPairAudioSource;

    int pairs;

    //interact
    InteractiveCard selectedCard1;
    InteractiveCard selectedCard2;


    private void Start()
    {
        if (rows * columns != images.Length * 2)
        {
            Debug.LogWarning("Number of r * c is not equal to provided cards, quitting...");
            return;
        }

        pairs = columns * rows / 2;

        System.Random random = new System.Random();
        images = images.OrderBy(x => random.Next()).ToArray();

        cards = new Vector3[rows * columns];
        
        float dx = startX;
        float dy = startY;

        int counter = 0;


        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < columns; j++)
            {
                cards[counter++] = new Vector3(dx, dy, planeZ);
                dx += deltaX;
            }

            dx = startX;
            dy += deltaY; 
        }

        cards = cards.OrderBy(x => random.Next()).ToArray();

        //instantiate cards
        counter = 0;

        int row = 0;

        foreach (Vector3 pos in cards)
        {
            GameObject go = Instantiate(card);

            go.SetActive(true);

            go.transform.position = pos;

            //We set card texture (using shader graph)
            go.GetComponent<MeshRenderer>().material.SetTexture("_MainTexture", images[row]);

            //this event will be called on this class by the InteractiveCard
            go.GetComponent<InteractiveCard>().OnClicked += SelectedCard;

            //We set Interactive card cover image name
            go.GetComponent<InteractiveCard>().imageName = images[row].name;

            counter++;

            //Check if end of row, if this -> next row
            if (counter % 2 == 0)
            {
                row++;
            }

        }
    }

    

    private void SelectedCard(InteractiveCard card, bool selected)
    {
        

        if (selectedCard1 == null && selected)
        {
            selectedCard1 = card;
        }

        else if(selectedCard1 == card && !selected)
        {
            selectedCard1.ResetMe();
            selectedCard1 = null;
        }

        else if(selectedCard2 != null && card == selectedCard2 && !selected)
        {
            selectedCard2.ResetMe();
            selectedCard2 = null;
        }

        else if (selectedCard2 == null & card != selectedCard1 && selected)
        {
            selectedCard2 = card;

            if (selectedCard1.Compare(selectedCard2))
            {
                //ok match!

                matchedPairAudioSource.Play();

                selectedCard1.HideAndDestroy();
                selectedCard2.HideAndDestroy();

                selectedCard1 = null;
                selectedCard2 = null;

                pairs--;

                if (pairs == 0)
                {
                    //GameOver

                    winUI.SetActive(true);
                    winUI.GetComponent<AudioSource>().Play();
                }

            }

            else 
            {
                //flip back
                selectedCard1.ResetMe();
                selectedCard2.ResetMe();

                selectedCard1 = null;
                selectedCard2 = null;
            }
        }

        


    }

    private void Update()
    {
        ////Debug
        
        //insta win
        if (Input.GetKeyDown(KeyCode.A))
        {
            winUI.SetActive(true);
            winUI.GetComponent<AudioSource>().Play();
        }
    }

    public void PlayAgain()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }






}
