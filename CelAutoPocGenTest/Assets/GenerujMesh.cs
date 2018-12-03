using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Networking.NetworkSystem;

public class GenerujMesh : MonoBehaviour {

    List<Vector3> wierzсholki;
    List<int> trojkaty;


    Dictionary<int, List<Trojkat>> trojkatDictionary = new Dictionary<int, List<Trojkat>>();

    List<List<int>> granicy = new List<List<int>>();
    HashSet<int> wierzcholkiSprawdzone = new HashSet<int>();

	public SiatkaKwadratow siatkaKwadratow;
    public void UtworzSiatke(int[,] plytka, float rozmiar)
    {

        siatkaKwadratow = new SiatkaKwadratow(plytka, rozmiar);

        wierzсholki = new List<Vector3>();
        trojkaty = new List<int>();

        for (int x = 0; x < siatkaKwadratow.kwadraty.GetLength(0); x++)
        {
            for (int y = 0; y < siatkaKwadratow.kwadraty.GetLength(1); y++)
            {
                Trojkatowanie(siatkaKwadratow.kwadraty[x, y]);

            }
        }

        Mesh mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;

        mesh.vertices = wierzсholki.ToArray();
        mesh.triangles = trojkaty.ToArray();
        mesh.RecalculateNormals();

        

        Vector2[] uvs = new Vector2[wierzсholki.Count];
        for (int i = 0; i < wierzсholki.Count; i++)
        {
            float percentX = Mathf.InverseLerp(-plytka.GetLength(0) / 2 * rozmiar, plytka.GetLength(0) / 2 * rozmiar, wierzсholki[i].x);
            float percentY = Mathf.InverseLerp(-plytka.GetLength(1) / 2 * rozmiar, plytka.GetLength(1) / 2 * rozmiar, wierzсholki[i].z);
            uvs[i] = new Vector2(percentX, percentY);
        }
        mesh.uv = uvs;



    }

   


	void Trojkatowanie(Kwadrat kwadrat) {
        switch (kwadrat.ksztalt)
        {
            case 0:
                break;

            //1 black
            //Не важен порядок, главное чтобы точки были по часовой стрелке
            case 1:
                Polaczenie(kwadrat.SL, kwadrat.SD, kwadrat.DL);
                break;

            case 2:
                Polaczenie(kwadrat.DP, kwadrat.SD, kwadrat.SP);
                break;

            case 4:
                Polaczenie(kwadrat.GP, kwadrat.SP, kwadrat.SG);
                break;

            case 8:
                Polaczenie(kwadrat.GL, kwadrat.SG, kwadrat.SL);
                break;

            //2 black

            case 3:
                Polaczenie(kwadrat.SP, kwadrat.DP, kwadrat.DL, kwadrat.SL);
                break;

            case 6:
                Polaczenie(kwadrat.SG, kwadrat.GP, kwadrat.DP, kwadrat.SD);
                break;

            case 9:
                Polaczenie(kwadrat.GL, kwadrat.SG, kwadrat.SD, kwadrat.DL);
                break;

            case 12:
                Polaczenie(kwadrat.GL, kwadrat.GP, kwadrat.SP, kwadrat.SL);
                break;

            case 5:
                Polaczenie(kwadrat.SG, kwadrat.GP, kwadrat.SP, kwadrat.SD, kwadrat.DL, kwadrat.SL);
                break;

            case 10:
                Polaczenie(kwadrat.GL, kwadrat.SG, kwadrat.SP, kwadrat.DP, kwadrat.SD, kwadrat.SL);
                break;

            //3 black

            case 7:
                Polaczenie(kwadrat.SG, kwadrat.GP, kwadrat.DP, kwadrat.DL, kwadrat.SL);
                break;

            case 11:
                Polaczenie(kwadrat.GL, kwadrat.SG, kwadrat.SP, kwadrat.DP, kwadrat.DL);
                break;

            case 13:
                Polaczenie(kwadrat.GL, kwadrat.GP, kwadrat.SP, kwadrat.SD, kwadrat.DL);
                break;

            case 14:
                Polaczenie(kwadrat.GL, kwadrat.GP, kwadrat.DP, kwadrat.SD, kwadrat.SL);
                break;

            // 4 black

            case 15:
                Polaczenie(kwadrat.GL, kwadrat.GP, kwadrat.DP, kwadrat.DL);
                break;





        }
	}

	void Polaczenie(params Wezel[] punkty) {

        Dopasuj(punkty);

        if (punkty.Length >= 3)
            UtwTrojkat(punkty[0],punkty[1],punkty[2]);
	    if (punkty.Length >= 4)
	        UtwTrojkat(punkty[0], punkty[2], punkty[3]);
        if (punkty.Length >= 5)
	        UtwTrojkat(punkty[0], punkty[3], punkty[4]);
	    if (punkty.Length >= 6)
	        UtwTrojkat(punkty[0], punkty[4], punkty[5]);

    }



    void Dopasuj(Wezel[] punkty)
    {
        for(int i=0; i < punkty.Length; i++)
        {
            if (punkty[i].wierzcholek == -1)
            {
                punkty[i].wierzcholek = wierzсholki.Count;
                wierzсholki.Add(punkty[i].poz);
            }
        }
    }

    void UtwTrojkat(Wezel a, Wezel b, Wezel c)
    {
        trojkaty.Add(a.wierzcholek);
        trojkaty.Add(b.wierzcholek);
        trojkaty.Add(c.wierzcholek);

        Trojkat trojkat = new Trojkat(a.wierzcholek, b.wierzcholek, c.wierzcholek);
        DodajDoSlownika(trojkat.wierzcholekA, trojkat);
        DodajDoSlownika(trojkat.wierzcholekB, trojkat);
        DodajDoSlownika(trojkat.wierzcholekC, trojkat);

    }

    public struct Trojkat
    {
        public int wierzcholekA;
        public int wierzcholekB;
        public int wierzcholekC;

        public int[] wierzcholki;

        public Trojkat(int a, int b, int c)
        {
            wierzcholekA = a;
            wierzcholekB = b;
            wierzcholekC = c;

            wierzcholki = new int[3];
            wierzcholki[0] = a;
            wierzcholki[1] = b;
            wierzcholki[2] = c;

        }

        public int this[int i]
        {
            get { return wierzcholki[i]; }
        }

        public bool Wspolne(int wierzcholek)
        {
            return wierzcholek == wierzcholekA || wierzcholek == wierzcholekB || wierzcholek == wierzcholekC;
        }

    }

    public void DodajDoSlownika(int wierzcholekKey, Trojkat trojkat)
    {
        if (trojkatDictionary.ContainsKey(wierzcholekKey)) //если в словаре есть данная вершина-ключ, то добавляет к этому ключу еще один треугольник
        {
            trojkatDictionary[wierzcholekKey].Add(trojkat);
        }
        else //если данной вершины-ключа нет то добавляет вершину-ключ в словарь и добавляет к нему треугольник 
        {
            List<Trojkat> trojkatyList = new List<Trojkat>();
            trojkatyList.Add(trojkat); 
            trojkatDictionary.Add(wierzcholekKey, trojkatyList);
        }
    }




    public bool CzyJestGranica(int a, int b)//сколько общих треугольников у вершин А и B 
    {
        List<Trojkat> trojkatZWierzcholkiemA = trojkatDictionary[a];
        int liczbaWspolnychTrojkatow = 0;

        for (int i = 0; i < trojkatZWierzcholkiemA.Count; i++)
        {
            if (trojkatZWierzcholkiemA[i].Wspolne(b))
            {
                liczbaWspolnychTrojkatow++;
                if (liczbaWspolnychTrojkatow > 1)
                {
                    break;
                }
            }
                
        }

        return liczbaWspolnychTrojkatow == 1;// если у вершин один общий треугольник, функция возвращает true, в противном случае возвращает false

    }

    public int PolaczonyGranicznyWierzcholek(int wierzcholek)
    {
        List<Trojkat> trojkatZWierzcholkiem = trojkatDictionary[wierzcholek];

        for (int i = 0; i < trojkatZWierzcholkiem.Count; i++)
        {
            Trojkat trojkat = trojkatZWierzcholkiem[i];

            for (int j = 0; j < 3; j++)
            {
                int b = trojkat[j];

                if (b != wierzcholek)
                {
                    if (CzyJestGranica(wierzcholek, b))
                    {
                        return b;
                    }
                }
                
            }
        }

        return -1;
    }

    public void RysujGranice(int wierzcholek, int granica)
    {
        granicy[granica].Add(wierzcholek);
        wierzcholkiSprawdzone.Add(wierzcholek);

        int wierzcholekNastepny = PolaczonyGranicznyWierzcholek(wierzcholek);

        if (wierzcholekNastepny != -1)
        {
            RysujGranice(wierzcholekNastepny, granica);
        }

    }

    public void ObliczGranicyMesha()
    {
        for (int wierzcholek = 0; wierzcholek < wierzсholki.Count; wierzcholek++)
        {
            if (!wierzcholkiSprawdzone.Contains(wierzcholek))
            {
                int nowyWierzcholekGraniczny = PolaczonyGranicznyWierzcholek(wierzcholek);
                if (nowyWierzcholekGraniczny != -1)
                {
                    wierzcholkiSprawdzone.Add(wierzcholek);

                    List<int> nowaGranica = new List<int>();
                    nowaGranica.Add(wierzcholek);
                    granicy.Add(nowaGranica);

                    RysujGranice(nowyWierzcholekGraniczny, granicy.Count - 1);
                    granicy[granicy.Count-1].Add(wierzcholek);
                }
            }
        }
    }





    public class Wezel
    {
	
		public Vector3 poz;
		public int wierzcholek = -1;

		public Wezel (Vector3 _poz) {
			poz = _poz;
		}
	}

	void OnDrawGizmos() {

		//if (siatkaKwadratow != null) {
		//	for(int x = 0; x < siatkaKwadratow.kwadraty.GetLength(0); x++) {
		//		for(int y = 0; y < siatkaKwadratow.kwadraty.GetLength(1); y++) {

		//			Gizmos.color = (siatkaKwadratow.kwadraty[x,y].GL.stan)?Color.black:Color.white;
		//			Gizmos.DrawCube(siatkaKwadratow.kwadraty[x,y].GL.poz,new Vector3(1,0,1) * .5f);

		//			Gizmos.color = (siatkaKwadratow.kwadraty[x,y].GP.stan)?Color.black:Color.white;
		//			Gizmos.DrawCube(siatkaKwadratow.kwadraty[x,y].GP.poz,new Vector3(1,0,1) * .5f);

		//			Gizmos.color = (siatkaKwadratow.kwadraty[x,y].DP.stan)?Color.black:Color.white;
		//			Gizmos.DrawCube(siatkaKwadratow.kwadraty[x,y].DP.poz,new Vector3(1,0,1)  * .5f);

		//			Gizmos.color = (siatkaKwadratow.kwadraty[x,y].DL.stan)?Color.black:Color.white;
		//			Gizmos.DrawCube(siatkaKwadratow.kwadraty[x,y].DL.poz,new Vector3(1,0,1)  * .5f);

		//			Gizmos.color = Color.grey;
		//			Gizmos.DrawCube(siatkaKwadratow.kwadraty[x,y].SG.poz,new Vector3(1,0,1)  * .2f);
		//			Gizmos.DrawCube(siatkaKwadratow.kwadraty[x,y].SP.poz,new Vector3(1,0,1)  * .2f);
		//			Gizmos.DrawCube(siatkaKwadratow.kwadraty[x,y].SD.poz,new Vector3(1,0,1)  * .2f);
		//			Gizmos.DrawCube(siatkaKwadratow.kwadraty[x,y].SL.poz,new Vector3(1,0,1)  * .2f);
			
		//		}
		//	}
		//}
	}

	public class SiatkaKwadratow {

		public Kwadrat[,] kwadraty;

		public SiatkaKwadratow(int[,] plytka, float rozmiar) {
			
			int licznikWezlowX = plytka.GetLength(0);
			int licznikWezlowY = plytka.GetLength(1);

			float szerokosc = licznikWezlowX * rozmiar;
			float wysokosc = licznikWezlowY * rozmiar;

			WezelKontroli[,] wezlyKontroli  = new WezelKontroli[licznikWezlowX,licznikWezlowY];

			for (int x = 0; x < licznikWezlowX; x++) {
				for (int y = 0; y < licznikWezlowY; y++) {
					Vector3 poz = new Vector3(-szerokosc/2 + x * rozmiar + rozmiar/2, 0, -wysokosc/2 + y * rozmiar + rozmiar/2);
					wezlyKontroli[x,y] = new WezelKontroli(poz,plytka[x,y] == 1, rozmiar);
				}
			}

			kwadraty = new Kwadrat[licznikWezlowX - 1,licznikWezlowY -1];

			for (int x = 0; x < licznikWezlowX - 1; x++) {
				for (int y = 0; y < licznikWezlowY - 1; y++) {
					kwadraty[x,y] = new Kwadrat(wezlyKontroli[x,y+1],wezlyKontroli[x+1,y+1],wezlyKontroli[x+1,y],wezlyKontroli[x,y]);
				}
			}
					
		}

	}

	public class Kwadrat {
		public WezelKontroli GL, GP, DP, DL;
		public Wezel SG, SP, SD, SL;
		public int ksztalt;

		public Kwadrat(WezelKontroli _GL,WezelKontroli _GP,WezelKontroli _DP,WezelKontroli _DL) {
			GL = _GL;
			GP = _GP;
			DP = _DP;
			DL = _DL;

			SG = GL.prawo;
			SP = DP.gora;
			SD = DL.prawo;
			SL = DL.gora;

			if(GL.stan)
				ksztalt+=8;
			if(GP.stan)
				ksztalt+=4;
			if(DP.stan)
				ksztalt+=2;
			if(DL.stan)
				ksztalt+=1;
			
			
			
		}

	}

		

	public class WezelKontroli : Wezel {

		public bool stan;

		public Wezel gora;
		public Wezel prawo;

		public WezelKontroli(Vector3 _poz, bool _stan, float rozmiar) : base(_poz) {
			stan = _stan;
			gora = new Wezel(poz + Vector3.forward * rozmiar/2f);
			prawo = new Wezel(poz + Vector3.right * rozmiar/2f);

		}
	}



}
