using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Security.Cryptography.X509Certificates;
using Object = System.Object;

public class GenerujMape : MonoBehaviour
{

    public int szerokosc, wysokosc, stopienWygl;

    [Range(0, 10)]
    public int rozmiarGranicy;

	public string seed;
	public bool seedCheck;
    


	int [,] poziom;
    private int[,] poziom2;

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
	
		poziom = new int[szerokosc, wysokosc];
	    poziom2 = new int[szerokosc, wysokosc];

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
	                mapaOgraniczona[x, y] = poziom[x - rozmiarGranicy, y - rozmiarGranicy];
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
        int rozmiarMinimalny = 40;
        List<Pokoj> pokojePoFiltrowaniu = new List<Pokoj>();



        foreach (List<Wspolrzedne> strefaScian in strefyScian)
        {
            if (strefaScian.Count < rozmiarMinimalny)
            {
                foreach (Wspolrzedne wybranaPlytka in strefaScian)
                {
                    poziom[wybranaPlytka.plytkaX, wybranaPlytka.plytkaY] = 0;
                }
            }
        }

        List<List<Wspolrzedne>> strefyPuste = Strefa(0);
        int pustyMinimalny = 40;

        foreach (List<Wspolrzedne> strefaPusta in strefyPuste)
        {
            if (strefaPusta.Count < pustyMinimalny)
            {
                foreach (Wspolrzedne wybranaPlytka in strefaPusta)
                {
                    poziom[wybranaPlytka.plytkaX, wybranaPlytka.plytkaY] = 1;
                }

            }
            else
            {
                pokojePoFiltrowaniu.Add(new Pokoj(strefaPusta, poziom));
            }
        }

        pokojePoFiltrowaniu.Sort();

        pokojePoFiltrowaniu[0].pokojGlowny = true;
        pokojePoFiltrowaniu[0].polaczenieZPokojemGlownym = true;


        PolaczNajblizszePokoje(pokojePoFiltrowaniu);
    }

    List<List<Wspolrzedne>> Strefa(int rodzaj)
    {
        List<List<Wspolrzedne>> strefy = new List<List<Wspolrzedne>>();
        int[,] flagi = new int[szerokosc, wysokosc];

        for (int x = 0; x < szerokosc; x++)
        {
            for (int y = 0; y < wysokosc; y++)
            {
                if (flagi[x, y] == 0 && poziom[x,y] == rodzaj)
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
        int rodzaj = poziom[poczX, poczY];

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
                        if (flagi[x, y] == 0 && poziom[x, y] == rodzaj)
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
					poziom [x, y] = poziom2[x, y] = 1;
				else {
					if (generatorLiczby.Next (0, 100) < udzialNapelnienia)
						poziom [x, y] = poziom2[x, y] = 1;
					else
						poziom [x, y] = poziom2[x, y] = 0;
				}

			}
		}
	}

    void PolaczNajblizszePokoje(List<Pokoj> pokoje, bool polaczanyPonownie = false)
    {

        List<Pokoj> listaPokojow1 = new List<Pokoj>();
        List<Pokoj> listaPokojow2 = new List<Pokoj>();
        
        //комнаты которые соединены прямо или косвенно с главной комнатой добавляем в список 2, а те которые не подключены добавляем в список 1

        if (polaczanyPonownie)
        {
            foreach (Pokoj pokoj in pokoje)
            {
                if (pokoj.polaczenieZPokojemGlownym)
                {
                    listaPokojow2.Add(pokoj);
                }
                else
                {
                    listaPokojow1.Add(pokoj);
                }
            }

           
        }
        else
        {
            listaPokojow1 = pokoje;
            listaPokojow2 = pokoje;
        }

        int odlegloscMin = 0;

        Wspolrzedne najblizszaPlytka1 = new Wspolrzedne();
        Wspolrzedne najblizszaPlytka2 = new Wspolrzedne();

        Pokoj najblizszyPokoj1 = new Pokoj();
        Pokoj najblizszyPokoj2 = new Pokoj();

        bool polaczenieMozliwe = false;

        


        foreach (Pokoj pokoj1 in listaPokojow1) 
        {
            if (!polaczanyPonownie)
            {
                polaczenieMozliwe = false;
                if (pokoj1.pokojePolaczone.Count > 0)
                {
                    continue;
                }
            }

            //итерация 2: если комната из списка не соединенных с главной, ищем для нее соединение с другими комнатами. 

            foreach (Pokoj pokoj2 in listaPokojow2)
            {
                if (pokoj1 == pokoj2 || pokoj1.czyPolaczone(pokoj2))
                {
                    continue;
                }

                

                for (int granicznaPlytka1 = 0; granicznaPlytka1 < pokoj1.plytkiGraniczne.Count; granicznaPlytka1++)
                {
                    for (int granicznaPlytka2 = 0; granicznaPlytka2 < pokoj2.plytkiGraniczne.Count; granicznaPlytka2++)
                    {

                        Wspolrzedne plytka1 = pokoj1.plytkiGraniczne[granicznaPlytka1];
                        Wspolrzedne plytka2 = pokoj2.plytkiGraniczne[granicznaPlytka2];

                        int odleglosc =
                            (int) (((plytka1.plytkaX - plytka2.plytkaX) * (plytka1.plytkaX - plytka2.plytkaX)) +
                                   ((plytka1.plytkaY - plytka2.plytkaY) * (plytka1.plytkaY - plytka2.plytkaY)));

                        if (odleglosc < odlegloscMin || !polaczenieMozliwe)
                        {
                            odlegloscMin = odleglosc;
                            polaczenieMozliwe = true;

                            najblizszaPlytka1 = plytka1;
                            najblizszaPlytka2 = plytka2;

                            najblizszyPokoj1 = pokoj1;
                            najblizszyPokoj2 = pokoj2;

                        }


                    }
                }
            }

            if (polaczenieMozliwe && !polaczanyPonownie)
            {
                UtworzKorytaz(najblizszyPokoj1, najblizszyPokoj2, najblizszaPlytka1, najblizszaPlytka2);

            }

        }

  

        if (polaczenieMozliwe && polaczanyPonownie) 
        {
            UtworzKorytaz(najblizszyPokoj1, najblizszyPokoj2, najblizszaPlytka1, najblizszaPlytka2);
            PolaczNajblizszePokoje(pokoje, true);
        }



        if (!polaczanyPonownie) // после того нашли соседей для каждой из комнат, запускаем функцию заново чтобы соединить все комнаты с главной. 
        {
            PolaczNajblizszePokoje(pokoje, true); 
        }
    }

    void UtworzKorytaz(Pokoj pokoj1, Pokoj pokoj2, Wspolrzedne plytka1, Wspolrzedne plytka2)
    {
        Pokoj.PolaczPokoje(pokoj1, pokoj2);
            
        //Debug.DrawLine(WspolrzedneToVector3(plytka1),WspolrzedneToVector3(plytka2), Color.cyan, 10);

        List<Wspolrzedne> prosta = RysujProsta(plytka1, plytka2);
        foreach (Wspolrzedne centr in prosta)
        {
            Okreg(1, centr);
        }
    }

    void Okreg(int r, Wspolrzedne o)
    {
        for (int x = -r; x <= r; x++)
        {
            for (int y = -r; y <= r; y++)
            {
                if (x * x + y * y <= r * r)
                {
                    int natychmiastowaX = o.plytkaX + x;
                    int natychmiastowaY = o.plytkaY + y;
                    if (natychmiastowaX >= 0 && natychmiastowaX < szerokosc &&
                        natychmiastowaY >= 0 && natychmiastowaY < wysokosc)
                    {
                        poziom[natychmiastowaX, natychmiastowaY] = 0;
                    }

                }
            }
        }
    }

    List<Wspolrzedne> RysujProsta(Wspolrzedne poczatek, Wspolrzedne koniec)
    {
        List<Wspolrzedne> prosta = new List<Wspolrzedne>();

        int x = poczatek.plytkaX;
        int y = poczatek.plytkaY;

        int dx = koniec.plytkaX - poczatek.plytkaX;
        int dy = koniec.plytkaY - poczatek.plytkaY;

        bool obliczeniePionowe = false;
        int znak = Math.Sign(dx);
        int wspolczynnik = Math.Sign(dy);

        int pozioma = Mathf.Abs(dx);
        int pionowa = Mathf.Abs(dy);

        if (pozioma < pionowa)
        {
            obliczeniePionowe = true;
            pozioma = Mathf.Abs(dy);
            pionowa = Mathf.Abs(dx);

            znak = Math.Sign(dy);
            wspolczynnik = Math.Sign(dx);
        }

        int skok = pozioma / 2;
        for (int i = 0; i < pozioma; i++)
        {
            prosta.Add(new Wspolrzedne(x, y));

            if (obliczeniePionowe)
            {
                y += znak;
            }
            else
            {
                x += znak;
            }

            skok += pionowa;
            if (skok >= pozioma)
            {
                if (obliczeniePionowe)
                {
                    x += wspolczynnik;
                }
                else
                {
                    y += wspolczynnik;
                }
                skok -= pozioma;
            }
        }

        return prosta;
    }



    Vector3 WspolrzedneToVector3(Wspolrzedne plytka)
    {
        return new Vector3(-szerokosc/2 + .5f + plytka.plytkaX, 2, -wysokosc/2 + .5f + plytka.plytkaY);
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

    public class Pokoj : IComparable<Pokoj>
    {
        public List<Wspolrzedne> wspolrzednePlytki;
        public List<Wspolrzedne> plytkiGraniczne;
        public List<Pokoj> pokojePolaczone;
        public int rozmiarPokoju;

        public bool polaczenieZPokojemGlownym;
        public bool pokojGlowny;

        
        public Pokoj() { }


        public Pokoj(List<Wspolrzedne> plytkiPokoju, int[,] poziom)
        {
            wspolrzednePlytki = plytkiPokoju;
            rozmiarPokoju = wspolrzednePlytki.Count;

            pokojePolaczone = new List<Pokoj>();
            plytkiGraniczne = new List<Wspolrzedne>();

            foreach (Wspolrzedne plytka in wspolrzednePlytki)
            {
                for (int x = plytka.plytkaX - 1; x <= plytka.plytkaX + 1; x++)
                {
                    for (int y = plytka.plytkaY - 1; y <= plytka.plytkaY + 1; y++)
                    {
                        if (x == plytka.plytkaX || y == plytka.plytkaY)
                        {
                            if (poziom[x,y] == 1)
                            {
                                plytkiGraniczne.Add(plytka);
                            }
                        }
                    }

                }
            }
        }

        public void PolaczenieZPokojemGlownym()
        {
            if (!polaczenieZPokojemGlownym)
            {
                polaczenieZPokojemGlownym = true;
                foreach (Pokoj pokojPolaczony in pokojePolaczone)
                {
                    pokojPolaczony.PolaczenieZPokojemGlownym();
                }
            }
        }

        public static void PolaczPokoje(Pokoj pokoj1, Pokoj pokoj2)
        {
            if (pokoj1.polaczenieZPokojemGlownym)
            {
                pokoj2.PolaczenieZPokojemGlownym();
            }
            else if (pokoj2.polaczenieZPokojemGlownym)
            {
                pokoj1.PolaczenieZPokojemGlownym();
            }

            {
                
            }
            pokoj1.pokojePolaczone.Add(pokoj2);
            pokoj2.pokojePolaczone.Add(pokoj1);
        }

        public bool czyPolaczone(Pokoj pokojDowolny)
        {
            return pokojePolaczone.Contains(pokojDowolny);
        }

        public int CompareTo(Pokoj dowolnyPokoj)
        {
            return dowolnyPokoj.rozmiarPokoju.CompareTo(rozmiarPokoju);
        }
    }


    void Wygladzenie () {


        for (int x = 0; x < szerokosc; x++) {
			for (int y = 0; y < wysokosc; y++)
			{
			    poziom[x, y] = poziom2[x, y]; //чисто на погчампе записываешь состояние клетки параллельно 
				int przyleglaPlytka = LiczbaScian (x, y);
				if (przyleglaPlytka > 4)
					poziom2 [x, y] = 1;
				else if (przyleglaPlytka < 4)
					poziom2 [x, y] = 0;
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
						licznik += poziom [ssX, ssY];
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



	//void OnDrawGizmos() {
		
	    
	//    /*
	//	if (poziom != null) {
	//		for (int x = 0; x < szerokosc; x++) {
	//			for (int y = 0; y < wysokosc; y++) {
	//				if (poziom [x, y] == 1)
	//					Gizmos.color = Color.black;
	//				else
	//					Gizmos.color = Color.white;
	//				Vector3 poz = new Vector3 (-szerokosc/2 + x + .5f, 0.0f, -wysokosc/2 + y + .5f);
	//				Gizmos.DrawCube (poz, new Vector3(1,0,1));

	//			}
	//		}
	//	}
	//	*/
	//}


}
