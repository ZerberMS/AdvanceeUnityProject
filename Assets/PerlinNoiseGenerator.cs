using UnityEngine;

public class PerlinNoiseGenerator : MonoBehaviour
{
    [SerializeField] int width;
    [SerializeField] int height;

    [SerializeField] float scale;

    [SerializeField] float offsetX = 0f;
    [SerializeField] float offsetY = 0f;

    [SerializeField] MeshRenderer MR;

    private void Start()
    {
        offsetX = Random.Range(0, 999);
        offsetY = Random.Range(0, 999);
        MR.material.mainTexture = generateTexture();
    }

    private Texture2D generateTexture()
    {
        Texture2D perlinTex = new Texture2D(width, height);

        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                Color color = Color.Lerp(Color.blue, Color.yellow, CalculateColor(i, j));

                perlinTex.SetPixel(i, j, color);
            }
        }
        perlinTex.Apply();
        return perlinTex;
    }

    private float CalculateColor(int x, int y)
    {
        float xCoord = (float)x / width  * scale + offsetX;
        float yCoord = (float)y / height * scale + offsetY;

        return Mathf.PerlinNoise(xCoord, yCoord);
    }
}