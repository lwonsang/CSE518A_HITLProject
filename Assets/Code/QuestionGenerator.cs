using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestionGenerator : MonoBehaviour
{
    public List<string> labels; 
    public Dictionary<string, List<Sprite>> labelToImages;

    public Question GenerateQuestion()
    {
        Question q = new Question();

        q.label = labels[Random.Range(0, labels.Count)];

        List<Sprite> correctList = labelToImages[q.label];
        int correctCount = Random.Range(2, 4); 
        q.correctIndices = new HashSet<int>();
        q.images = new List<Sprite>();

        for (int i = 0; i < correctCount; i++)
        {
            Sprite correct = correctList[Random.Range(0, correctList.Count)];
            q.correctIndices.Add(q.images.Count);
            q.images.Add(correct);
        }

        while (q.images.Count < 9)
        {
            string otherLabel;
            do
            {
                otherLabel = labels[Random.Range(0, labels.Count)];
            } while (otherLabel == q.label);

            List<Sprite> otherList = labelToImages[otherLabel];
            Sprite other = otherList[Random.Range(0, otherList.Count)];
            q.images.Add(other);
        }

        Shuffle(q.images, q.correctIndices);

        return q;
    }

    private void Shuffle(List<Sprite> images, HashSet<int> correctIndices)
    {
        for (int i = images.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            (images[i], images[j]) = (images[j], images[i]);

            bool wasI = correctIndices.Contains(i);
            bool wasJ = correctIndices.Contains(j);

            if (wasI && !wasJ)
            {
                correctIndices.Remove(i);
                correctIndices.Add(j);
            }
            else if (!wasI && wasJ)
            {
                correctIndices.Remove(j);
                correctIndices.Add(i);
            }
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        labels = new List<string>();
        labelToImages = new Dictionary<string, List<Sprite>>();

        string[] categories = {
            "Airplane",
            "Apple",
            "Banana",
            "Bear",
            "Bee",
            "Bicycle",
            "Cat",
            "Dog",
            "Elephant",
            "Horse"
        };

        foreach (string label in categories)
        {
            labels.Add(label);
            Sprite[] sprites = Resources.LoadAll<Sprite>("Images/" + label);
            labelToImages[label] = new List<Sprite>(sprites);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
