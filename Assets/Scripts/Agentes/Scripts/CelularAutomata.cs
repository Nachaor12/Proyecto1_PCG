using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CelularAutomata : MonoBehaviour
{
    [SerializeField] private int tamaño = 40;
    [SerializeField][Range(0, 100)] private int porcentajeParedInicial = 45;
    [SerializeField] private int Smoothness = 5;
    [SerializeField] private int neighbor = 4;
    [SerializeField] private GameObject prefabPared;
    [SerializeField] private float prefabSize = 0.5f;

    private int[,] mapa;

    void Start()
    {
        GenerarCueva();
    }

    void GenerarCueva()
    {
        mapa = new int[tamaño, tamaño];
        InicializarMapaAleatorio();

        for (int i = 0; i < Smoothness; i++)
        {
            SuavizarMapa();
        }

        DibujarMapa();
    }

    void InicializarMapaAleatorio()
    {
        Random.InitState((int)System.DateTime.Now.Ticks);

        for (int x = 0; x < tamaño; x++)
        {
            for (int y = 0; y < tamaño; y++)
            {
                if (x == 0 || x == tamaño - 1 || y == 0 || y == tamaño - 1)
                {
                    mapa[x, y] = 1;
                }
                else
                {
                    mapa[x, y] = (Random.Range(0, 100) < porcentajeParedInicial) ? 1 : 0;
                }
            }
        }
    }

    void SuavizarMapa()
    {
        int[,] mapaTemporal = new int[tamaño, tamaño];

        for (int x = 0; x < tamaño; x++)
        {
            for (int y = 0; y < tamaño; y++)
            {
                int paredesVecinas = ContarParedesVecinas(x, y);

                if (paredesVecinas > neighbor)
                    mapaTemporal[x, y] = 1;
                else if (paredesVecinas < neighbor)
                    mapaTemporal[x, y] = 0;
                else
                    mapaTemporal[x, y] = mapa[x, y];

                if (x == 0 || x == tamaño - 1 || y == 0 || y == tamaño - 1)
                {
                    mapaTemporal[x, y] = 1;
                }
            }
        }

        mapa = mapaTemporal;
    }

    int ContarParedesVecinas(int gridX, int gridY)
    {
        int contadorParedes = 0;

        for (int vecinoX = gridX - 1; vecinoX <= gridX + 1; vecinoX++)
        {
            for (int vecinoY = gridY - 1; vecinoY <= gridY + 1; vecinoY++)
            {
                if (vecinoX >= 0 && vecinoX < tamaño && vecinoY >= 0 && vecinoY < tamaño)
                {
                    if (vecinoX != gridX || vecinoY != gridY)
                    {
                        contadorParedes += mapa[vecinoX, vecinoY];
                    }
                }
                else
                {
                    contadorParedes++;
                }
            }
        }

        return contadorParedes;
    }

    void DibujarMapa()
    {
        if (prefabPared == null) return;

        for (int x = 0; x < tamaño; x++)
        {
            for (int y = 0; y < tamaño; y++)
            {
                if (mapa[x, y] == 1)
                {
                    Vector3 posicionMundo = new Vector3(x * prefabSize, y * prefabSize, 0f);
                    Instantiate(prefabPared, posicionMundo, Quaternion.identity, transform);
                }
            }
        }
    }
}
