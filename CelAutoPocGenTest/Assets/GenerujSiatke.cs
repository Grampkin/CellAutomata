using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenerujSiatke : MonoBehaviour {

    List<Vector3> wierzholki;
    List<int> trojkaty;

    

	public SiatkaKwadratow siatkaKwadratow;
    public void UtworzSiatke(int[,] plytka, float rozmiar)
    {

        siatkaKwadratow = new SiatkaKwadratow(plytka, rozmiar);

        wierzholki = new List<Vector3>();
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

        mesh.vertices = wierzholki.ToArray();
        mesh.triangles = trojkaty.ToArray();
        mesh.RecalculateNormals();
        


    }

   


	void Trojkatowanie(Kwadrat kwadrat) {
        switch (kwadrat.ksztalt)
        {
            case 0:
                break;

            //1 black
            //Не важен порядок, главное чтобы точки были по часовой стрелке
            case 1:
                Polaczenie(kwadrat.DL, kwadrat.SL, kwadrat.SD);
                break;

            case 2:
                Polaczenie(kwadrat.SP, kwadrat.DP, kwadrat.SD);
                break;

            case 4:
                Polaczenie(kwadrat.SG, kwadrat.GP, kwadrat.SP);
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
            utwTrojkat(punkty[0],punkty[1],punkty[2]);
	    if (punkty.Length >= 4)
	        utwTrojkat(punkty[0], punkty[2], punkty[3]);
        if (punkty.Length >= 5)
	        utwTrojkat(punkty[0], punkty[3], punkty[4]);
	    if (punkty.Length >= 6)
	        utwTrojkat(punkty[0], punkty[4], punkty[5]);

    }



    void Dopasuj(Wezel[] punkty)
    {
        for(int i=0; i < punkty.Length; i++)
        {
            if (punkty[i].wierzcholek == -1)
            {
                punkty[i].wierzcholek = wierzholki.Count;
                wierzholki.Add(punkty[i].poz);
            }
        }
    }

    void utwTrojkat(Wezel a, Wezel b, Wezel c)
    {
        trojkaty.Add(a.wierzcholek);
        trojkaty.Add(b.wierzcholek);
        trojkaty.Add(c.wierzcholek);
    }

    	public class Wezel {
	
		public Vector3 poz;
		public int wierzcholek = -1;

		public Wezel (Vector3 _poz) {
			poz = _poz;
		}
	}

	void OnDrawGizmos() {

		if (siatkaKwadratow != null) {
			for(int x = 0; x < siatkaKwadratow.kwadraty.GetLength(0); x++) {
				for(int y = 0; y < siatkaKwadratow.kwadraty.GetLength(1); y++) {

					Gizmos.color = (siatkaKwadratow.kwadraty[x,y].GL.stan)?Color.black:Color.white;
					Gizmos.DrawCube(siatkaKwadratow.kwadraty[x,y].GL.poz,new Vector3(1,0,1) * .5f);

					Gizmos.color = (siatkaKwadratow.kwadraty[x,y].GP.stan)?Color.black:Color.white;
					Gizmos.DrawCube(siatkaKwadratow.kwadraty[x,y].GP.poz,new Vector3(1,0,1) * .5f);

					Gizmos.color = (siatkaKwadratow.kwadraty[x,y].DP.stan)?Color.black:Color.white;
					Gizmos.DrawCube(siatkaKwadratow.kwadraty[x,y].DP.poz,new Vector3(1,0,1)  * .5f);

					Gizmos.color = (siatkaKwadratow.kwadraty[x,y].DL.stan)?Color.black:Color.white;
					Gizmos.DrawCube(siatkaKwadratow.kwadraty[x,y].DL.poz,new Vector3(1,0,1)  * .5f);

					Gizmos.color = Color.grey;
					Gizmos.DrawCube(siatkaKwadratow.kwadraty[x,y].SG.poz,new Vector3(1,0,1)  * .2f);
					Gizmos.DrawCube(siatkaKwadratow.kwadraty[x,y].SP.poz,new Vector3(1,0,1)  * .2f);
					Gizmos.DrawCube(siatkaKwadratow.kwadraty[x,y].SD.poz,new Vector3(1,0,1)  * .2f);
					Gizmos.DrawCube(siatkaKwadratow.kwadraty[x,y].SL.poz,new Vector3(1,0,1)  * .2f);
			
				}
			}
		}
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
