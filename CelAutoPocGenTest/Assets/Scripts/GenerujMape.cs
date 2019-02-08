using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using Object = System.Object;

public class GenerujMape : MonoBehaviour
{

    public int szerokosc, wysokosc, liczbaIteracji, liczbaPokoji;

    [Range(0, 10)]
    public int rozmiarGranicy;

	public string seed;
	public bool seedChange;

    protected int liczbaPok;
    protected int flaga;
    protected int [,] mapaZgenerowana;

	public int [,] poziom;
    private int[,] poziom2;

	[Range(0,100)]
	public int udzialNapelnienia;


	void Start()
	{
	    if (Input.GetKeyDown(KeyCode.R))
	    {
	        mapaZgenerowana = Generuj();
	        Wyswietl(mapaZgenerowana);
	    }
	}

    

	void Update() {
	    
	        if (liczbaPok != liczbaPokoji || (Input.GetKeyDown(KeyCode.R)))
	        {
	            mapaZgenerowana = Generuj();
	           

            }
	        else 
	        {
	            Wyswietl(mapaZgenerowana);
	            
	        }
	    

	}


int[,] Generuj () {
	
		poziom = new int[szerokosc, wysokosc];
	    poziom2 = new int[szerokosc, wysokosc];
	    
	    WypelnijMape();
	    
		for (int i = 0; i < liczbaIteracji; i++) {

			    CyklZycia (i,8);  

		}

	    
	    ModyfikujMape();


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

	    return mapaOgraniczona;

	}

    void Wyswietl(int[,] mapaOgraniczona)
    {
        

            
            GenerujMesh generujMesh = GetComponent<GenerujMesh>();
            generujMesh.UtworzSiatke(mapaOgraniczona, 1);
            
    }

    void ModyfikujMape()
    {
        List<List<Wspolrzedne>> strefyScian = Strefy(1);
        int scianaNajmniejsza = 50;
        List<Pokoj> pokojePoFiltrowaniu = new List<Pokoj>();



        foreach (List<Wspolrzedne> strefaScian in strefyScian)
        {
            if (strefaScian.Count < scianaNajmniejsza)
            {
                foreach (Wspolrzedne wybranaKomorka in strefaScian)
                {
                    poziom[wybranaKomorka.komorkaX, wybranaKomorka.komorkaY] = 0;
                }
            }
        }

        List<List<Wspolrzedne>> strefyPuste = Strefy(0);
        int pokojNajmniejszy = 120;
        foreach (List<Wspolrzedne> strefaPusta in strefyPuste)
        {
            if (strefaPusta.Count < pokojNajmniejszy)
            {
                foreach (Wspolrzedne wybranaKomorka in strefaPusta)
                {
                    poziom[wybranaKomorka.komorkaX, wybranaKomorka.komorkaY] = 1;
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
        //int last = pokojePoFiltrowaniu.Count-1;
        //int minPokoj = pokojePoFiltrowaniu[last].rozmiarPokoju;


        //foreach (Pokoj p in pokojePoFiltrowaniu)
        //{
        //    print(p.rozmiarPokoju);
            
        //}
        //print("min = " + minPokoj);

        print(pokojePoFiltrowaniu.Count);

        liczbaPok = pokojePoFiltrowaniu.Count;

        PolaczNajblizszePokoje(pokojePoFiltrowaniu);
    }

    

    List<List<Wspolrzedne>> Strefy(int rodzaj)
    {
        List<List<Wspolrzedne>> strefy = new List<List<Wspolrzedne>>();
        int[,] komorkiSprawdzone = new int[szerokosc, wysokosc];

        for (int x = 0; x < szerokosc; x++)
        {
            for (int y = 0; y < wysokosc; y++)
            {
                if (komorkiSprawdzone[x, y] == 0 && poziom[x,y] == rodzaj)
                {
                    List<Wspolrzedne> strefaNowa = KomorkiStrefy(x, y);
                    strefy.Add(strefaNowa);

                    foreach (Wspolrzedne wybranaKomorka in strefaNowa)
                    {
                        komorkiSprawdzone[wybranaKomorka.komorkaX, wybranaKomorka.komorkaY] = 1;
                    }
                }
            }
        }

        return strefy; 
    }

    List<Wspolrzedne> KomorkiStrefy(int poczX, int poczY)
    {
        List<Wspolrzedne> wspolrzedne = new List<Wspolrzedne>();
        int[,] komorkiSprawdzone = new int[szerokosc, wysokosc];
        int rodzaj = poziom[poczX, poczY];

        Queue<Wspolrzedne> queue = new Queue<Wspolrzedne>();
        queue.Enqueue(new Wspolrzedne(poczX, poczY));
        komorkiSprawdzone[poczX, poczY] = 1;

        while (queue.Count > 0)
        {
            Wspolrzedne wybranaKomorka = queue.Dequeue();
            wspolrzedne.Add(wybranaKomorka);

            for (int x = wybranaKomorka.komorkaX - 1; x <= wybranaKomorka.komorkaX + 1; x++)
            {
                for (int y = wybranaKomorka.komorkaY - 1; y <= wybranaKomorka.komorkaY + 1; y++)
                {
                    if ((x >= 0 && x < szerokosc && y >= 0 && y < wysokosc) &&
                        (x == wybranaKomorka.komorkaX || y == wybranaKomorka.komorkaY))
                    {
                        if (komorkiSprawdzone[x, y] == 0 && poziom[x, y] == rodzaj)
                        {
                            komorkiSprawdzone[x, y] = 1;
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
		if (seedChange == true) {
			seed = System.DateTime.Now.Ticks.ToString(); ;
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
            listaPokojow1 = listaPokojow2 = pokoje;
        }

        int odlegloscMin = 0;

        Wspolrzedne najblizszaKomorka1 = new Wspolrzedne();
        Wspolrzedne najblizszaKomorka2 = new Wspolrzedne();

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

                for (int granicznaKomorka1 = 0; granicznaKomorka1 < pokoj1.komorkiGraniczne.Count; granicznaKomorka1++)
                {
                    for (int granicznaKomorka2 = 0; granicznaKomorka2 < pokoj2.komorkiGraniczne.Count; granicznaKomorka2++)
                    {

                        Wspolrzedne komorka1 = pokoj1.komorkiGraniczne[granicznaKomorka1];
                        Wspolrzedne komorka2 = pokoj2.komorkiGraniczne[granicznaKomorka2];

                        int odleglosc =
                            (int) (((komorka1.komorkaX - komorka2.komorkaX) * (komorka1.komorkaX - komorka2.komorkaX)) +
                                   ((komorka1.komorkaY - komorka2.komorkaY) * (komorka1.komorkaY - komorka2.komorkaY)));

                        if (odleglosc < odlegloscMin || !polaczenieMozliwe)
                        {
                            odlegloscMin = odleglosc;
                            polaczenieMozliwe = true;

                            najblizszaKomorka1 = komorka1;
                            najblizszaKomorka2 = komorka2;

                            najblizszyPokoj1 = pokoj1;
                            najblizszyPokoj2 = pokoj2;

                        }
                    }
                }
            }

            if (polaczenieMozliwe && !polaczanyPonownie)
            {
                UtworzKorytaz(najblizszyPokoj1, najblizszyPokoj2, najblizszaKomorka1, najblizszaKomorka2);

            }

        }

        if (polaczenieMozliwe && polaczanyPonownie) 
        {
            UtworzKorytaz(najblizszyPokoj1, najblizszyPokoj2, najblizszaKomorka1, najblizszaKomorka2);
            PolaczNajblizszePokoje(pokoje, true);
        }



        if (!polaczanyPonownie) // после того нашли соседей для каждой из комнат, запускаем функцию заново чтобы соединить все комнаты с главной. 
        {
            PolaczNajblizszePokoje(pokoje, true); 
        }
    }

    void UtworzKorytaz(Pokoj pokoj1, Pokoj pokoj2, Wspolrzedne komorka1, Wspolrzedne komorka2)
    {
        Pokoj.PolaczPokoje(pokoj1, pokoj2);

        List<Wspolrzedne> prosta = RysujProsta(komorka1, komorka2);
        foreach (Wspolrzedne centr in prosta)
        {
            KrokKorytarza(2, centr);
        }
    }

    void KrokKorytarza(int r, Wspolrzedne o)
    {
        for (int x = -r; x <= r; x++)
        {
            for (int y = -r; y <= r; y++)
            {
                if (x * x + y * y <= r * r)
                {
                    int natychmiastowaX = o.komorkaX + x;
                    int natychmiastowaY = o.komorkaY + y;
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

        int x = poczatek.komorkaX;
        int y = poczatek.komorkaY;

        int dx = koniec.komorkaX - poczatek.komorkaX;
        int dy = koniec.komorkaY - poczatek.komorkaY;

        bool liniaPionowa = false;
        int znak = Math.Sign(dx);
        int wspolczynnik = Math.Sign(dy);

        int pozioma = Mathf.Abs(dx);
        int pionowa = Mathf.Abs(dy);

        if (pozioma < pionowa)
        {
            liniaPionowa = true;
            pozioma = Mathf.Abs(dy);
            pionowa = Mathf.Abs(dx);

            znak = Math.Sign(dy);
            wspolczynnik = Math.Sign(dx);
        }

        int skok = pozioma / 2; //точка в которой Y увеличивается (либо Х уменьшается) 
        for (int i = 0; i < pozioma; i++)
        {
            prosta.Add(new Wspolrzedne(x, y));

            if (liniaPionowa)
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
                if (liniaPionowa)
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





    public struct Wspolrzedne
    {
        public int komorkaX;
        public int komorkaY;

        public Wspolrzedne(int x, int y)
        {
            komorkaX = x;
            komorkaY = y; 
        }
    }

    public class Pokoj : IComparable<Pokoj>
    {
        public List<Wspolrzedne> wspolrzedneKomorki;
        public int rozmiarPokoju;
        public List<Pokoj> pokojePolaczone;

        public List<Wspolrzedne> komorkiGraniczne;
        public bool polaczenieZPokojemGlownym;
        public bool pokojGlowny;

        
        public Pokoj() { }


        public Pokoj(List<Wspolrzedne> komorkiPokoju, int[,] poziom)
        {
            wspolrzedneKomorki = komorkiPokoju;
            rozmiarPokoju = wspolrzedneKomorki.Count;

            pokojePolaczone = new List<Pokoj>();
            komorkiGraniczne = new List<Wspolrzedne>();


            //считает размер комнаты
            foreach (Wspolrzedne komorka in wspolrzedneKomorki)
            {
                for (int x = komorka.komorkaX - 1; x <= komorka.komorkaX + 1; x++)
                {
                    for (int y = komorka.komorkaY - 1; y <= komorka.komorkaY + 1; y++)
                    {
                        if (x == komorka.komorkaX || y == komorka.komorkaY)
                        {
                            if (poziom[x,y] == 1)
                            {
                                komorkiGraniczne.Add(komorka);
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

            pokoj1.pokojePolaczone.Add(pokoj2);
            pokoj2.pokojePolaczone.Add(pokoj1);
        }

        public bool czyPolaczone(Pokoj pokojDowolny)
        {
            if (pokojePolaczone.Contains(pokojDowolny))
                return true;
            else return false;
        }

        public int CompareTo(Pokoj dowolnyPokoj)
        {
            return dowolnyPokoj.rozmiarPokoju.CompareTo(rozmiarPokoju);
        }
    }


    void CyklZycia (int licznik, int iteracjaGraniczna)
    {

        for (int x = 0; x < szerokosc; x++) {
			for (int y = 0; y < wysokosc; y++)
			{
                poziom[x, y] = poziom2[x, y];  //чисто на погчампе записываешь состояние клетки параллельно 
                int przyleglaKomorka = LiczbaScian (x, y);
			    if (przyleglaKomorka > 4 && licznik < iteracjaGraniczna)
			        poziom2[x, y] = 1;
			    else if (przyleglaKomorka < 4 && licznik < iteracjaGraniczna)
			        poziom2[x, y] = 0;
			    else if (przyleglaKomorka > 5 && licznik >= iteracjaGraniczna)
			        poziom2[x, y] = 1;
			    else if (przyleglaKomorka < 5 && licznik >= iteracjaGraniczna)
			        poziom2[x, y] = 0;
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





    void OnDrawGizmos()
    {
        if (poziom != null)
        {
            for (int x = 0; x < szerokosc; x++)
            {
                for (int y = 0; y < wysokosc; y++)
                {
                    Gizmos.color = (poziom[x, y] == 1) ? Color.black : Color.white;
                    Vector3 center = new Vector3(-szerokosc / 2 + x + .5f, 0, -wysokosc / 2 + y + .5f);
                    Gizmos.DrawCube(center, new Vector3(1f,0f,1f));
                }
            }
        }
    }




}
