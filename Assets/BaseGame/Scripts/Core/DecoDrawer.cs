using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class DecoDrawer : MonoBehaviour
{
    [field: SerializeField] public Sprite[] Sprites {get; private set;}

    [field: SerializeField] public BoxCollider2D[] TreeRange {get; private set;}
    [field: SerializeField] public List<GameObject> ListDeco {get; private set;}
    [field: SerializeField] public float ScaleRange {get; private set;}
    [Button]
    private void GenerateTree()
    {
        TreeRange = GetComponentsInChildren<BoxCollider2D>();
        foreach (GameObject ob in ListDeco)
        {
            DestroyImmediate(ob);
        }
        ListDeco.Clear();
        foreach (BoxCollider2D treeBox in TreeRange)
        {
            float acreage = treeBox.size.x * treeBox.size.y;
            for (int i = 0; i < ScaleRange * acreage; i++)
            {
                float xPos = Random.Range(treeBox.bounds.min.x, treeBox.bounds.max.x);
                float yPos = Random.Range(treeBox.bounds.min.y, treeBox.bounds.max.y);
                float zPos = yPos;
                Vector3 treePosition = new Vector3(xPos, yPos, zPos);
                bool isOverlap = false;
                foreach (var deco in ListDeco)
                {
                    if (Vector3.Distance(treePosition, deco.transform.position) < 0.3f)
                    {
                        isOverlap = true;
                        break;
                    }
                }
                if (isOverlap) continue;
                float scale = Random.Range(0.2f, 0.5f);
                GameObject tree = new GameObject
                {
                    transform =
                    {
                        position = treePosition,
                        localScale = scale * Vector3.one,
                    },
                };
                tree.transform.SetParent(treeBox.transform);
                SpriteRenderer treeSprite = tree.AddComponent<SpriteRenderer>();
                treeSprite.sprite = Sprites[Random.Range(0, Sprites.Length)];
                treeSprite.sortingOrder = 40;
                ListDeco.Add(tree);
            }
            
        }
    } 
}