using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class GenerujMape : MonoBehaviour
{

    public int szerokosc, wysokosc, stopienWygl;

    [Range(0, 10)]
    public int rozmiarGranicy;

	public string seed;
	public bool seedCheck;
    


	int [,] plytka;
    private int[,] plytka2;

	[Range(0,100)]
	public int udzialNapelnienia;

	void Start() {
        transform.position = new Vector3(0f,0f,0f);
		Generuj ();
	}

	void Update() {
		if (Input.GetMouseButton (0))
			Generuj ();
	}

	void Generuj () {
	
		plytka = new int[szerokosc, wysokosc];
	    plytka2 = new int[szerokosc, wysokosc];

	    WypelnijMape();

		for (int i = 0; i < stopienWygl; i++) {
			Wygladzenie ();
		}

	    UsunDrobneElementy();


        int[,] mapaOgraniczona = new int[szerokosc + rozmiarGranicy * 2, wysokosc + rozmiarGranicy * 2];

	    for (int x = 0; x < mapaOgraniczona.GetLength(0); x++)
	    {
	        for (int y = 0; y < mapaOgraniczona.GetLength(1); y++)
	        {
	            if (x >= rozmiarGranicy && x < szerokosc + rozmiarGranicy && y >= rozmiarGranicy && y < wysokosc + rozmiarGranicy)
	            {
	                mapaOgraniczona[x, y] = plytka[x - rozmiarGranicy, y - rozmiarGranicy];
	            }
	            else
	            {
	                mapaOgraniczona[x, y] = 1;
	            }
	        }
	    }

        GenerujMesh generujMesh = GetComponent<GenerujMesh> ();
		generujMesh.UtworzSiatke (mapaOgraniczona, 1);
	
	}

    void UsunDrobneElementy()
    {
        List<List<Wspolrzedne>> strefyScian = Strefa(1);
        int rozmiarMinimalny = 20;

        foreach (List<Wspolrzedne> strefaScian in strefyScian)
        {
            if (strefaScian.Count < rozmiarMinimalny)
            {
                foreach (Wspolrzedne wybranaPlytka in strefaScian)
                {
                    plytka[wybranaPlytka.plytkaX, wybranaPlytka.plytkaY] = 0;
                }
            }
        }

        List<List<Wspolrzedne>> strefyPuste = Strefa(0);
        int pustyMinimalny = 20;

        foreach (List<Wspolrzedne> strefaPuste in strefyPuste)
        {
            if (strefaPuste.Count < pustyMinimalny)
            {
                foreach (Wspolrzedne wybranaPlytka in strefaPuste)
                {
                    plytka[wybranaPlytka.plytkaX, wybranaPlytka.plytkaY] = 1;
                }
            }
        }
    }

    List<List<Wspolrzedne>> Strefa(int rodzaj)
    {
        List<List<Wspolrzedne>> strefy = new List<List<Wspolrzedne>>();
        int[,] flagi = new int[szerokosc, wysokosc];

        for (int x = 0; x < szerokosc; x++)
        {
            for (int y = 0; y < wysokosc; y++)
            {
                if (flagi[x, y] == 0 && plytka[x,y] == rodzaj)
                {
                    List<Wspolrzedne> strefaNowa = PlytkaStrefy(x, y);
                    strefy.Add(strefaNowa);

                    foreach (Wspolrzedne wybranaPlytka in strefaNowa)
                    {
                        flagi[wybranaPlytka.plytkaX, wybranaPlytka.plytkaY] = 1;
                    }
                }
            }
        }

        return strefy; 
    }

    List<Wspolrzedne> PlytkaStrefy(int poczX, int poczY)
    {
        List<Wspolrzedne> wspolrzedne = new List<Wspolrzedne>();
        int[,] flagi = new int[szerokosc, wysokosc];
        int rodzaj = plytka[poczX, poczY];

        Queue<Wspolrzedne> queue = new Queue<Wspolrzedne>();
        queue.Enqueue(new Wspolrzedne(poczX, poczY));
        flagi[poczX, poczY] = 1;

        while (queue.Count > 0)
        {
            Wspolrzedne wybranaPlytka = queue.Dequeue();
            wspolrzedne.Add(wybranaPlytka);

            for (int x = wybranaPlytka.plytkaX - 1; x <= wybranaPlytka.plytkaX + 1; x++)
            {
                for (int y = wybranaPlytka.plytkaY - 1; y <= wybranaPlytka.plytkaY + 1; y++)
                {
                    if ((x >= 0 && x < szerokosc && y >= 0 && y < wysokosc) &&
                        (x == wybranaPlytka.plytkaX || y == wybranaPlytka.plytkaY))
                    {
                        if (flagi[x, y] == 0 && plytka[x, y] == rodzaj)
                        {
                            flagi[x, y] = 1;
                            queue.Enqueue(new Wspolrzedne(x, y));
                        }
                    }
                }

            }
        }

        return wspolrzedne;
    }




    /* Делает стенки для карты, заполняет каждую клетку случайным цветом (черный или белый) в зависимости от процента заполнения
     */

	void WypelnijMape() {
		if (seedCheck == true) {
			seed = Time.time.ToString();
		}

		System.Random generatorLiczby = new System.Random (seed.GetHashCode ());

		for (int x = 0; x < szerokosc; x++) {
			for (int y = 0; y < wysokosc; y++) {
				if (x == 0 || x == szerokosc-1 || y == 0 || y == wysokosc - 1)
					plytka [x, y] = plytka2[x, y] = 1;
				else {
					if (generatorLiczby.Next (0, 100) < udzialNapelnienia)
						plytka [x, y] = plytka2[x, y] = 1;
					else
						plytka [x, y] = plytka2[x, y] = 0;
				}

			}
		}
	}

    public struct Wspolrzedne
    {
        public int plytkaX;
        public int plytkaY;

        public Wspolrzedne(int x, int y)
        {
            plytkaX = x;
            plytkaY = y; 
        }
    }

	void Wygladzenie () {


        for (int x = 0; x < szerokosc; x++) {
			for (int y = 0; y < wysokosc; y++)
			{
			    plytka[x, y] = plytka2[x, y]; //чисто на погчампе записываешь состояние клетки параллельно 
				int przyleglaPlytka = LiczbaScian (x, y);
				if (przyleglaPlytka > 4)
					plytka2 [x, y] = 1;
				else if (przyleglaPlytka < 4)
					plytka2 [x, y] = 0;
			}
		}
		
	}

	int LiczbaScian(int scX, int scY) {
		int licznik = 0;
	    

        for (int ssX = scX - 1; ssX <= scX + 1; ssX++) {
			for (int ssY = scY - 1; ssY <= scY + 1; ssY++)
			{
			    //если соседняя клетка находится в пределах карты то берется её состояние (1 либо 0)
                if (ssX >= 0 && ssX < szerokosc && ssY >= 0 && ssY < wysokosc) {    
					if (ssX != scX || ssY != scY) {
						licznik += plytka [ssX, ssY];
                    }
				}
                //если соседняя клетка находится вне карты то за её состояние принимается единица 
                else {
					 licznik++;
				}

			}

		    

		}
		return licznik;
	}


	void OnDrawGizmos() {
		/*
		if (plytka != null) {
			for (int x = 0; x < szerokosc; x++) {
				for (int y = 0; y < wysokosc; y++) {
					if (plytka [x, y] == 1)
						Gizmos.color = Color.black;
					else
						Gizmos.color = Color.white;
					Vector3 poz = new Vector3 (-szerokosc/2 + x + .5f, 0.0f, -wysokosc/2 + y + .5f);
					Gizmos.DrawCube (poz, new Vector3(1,0,1));

				}
			}
		}
		*/
	}


}
