using System;
using System.Collections.Generic;

namespace UMT
{
    class Program
    {

        //Am decis sa reprezind o zi ca fiind o lista de valori bool ene astfel true reprezinta ca minutul corespunzator pozitiei este liber,iar
        //false inseamna ocupat. Lista are o dimensiune cunoscuta 24*60 si la inceput toate valorile sunt true deci toate sunt libere.

        //Transforma o ora sub forma de string in int reprezentant cate minute au trecut de la '0:00'
        // de exemplu '3:30' => min = 3 * 60 + 30
        public static int getMinute(string ora)
        {
            var impartit = ora.Split(':');
            int oraInt = int.Parse(impartit[0]);
            int minuteInt = int.Parse(impartit[1]);
            int minute = 60 * oraInt + minuteInt;
            return minute;
        }

        //Sterge dintr-un string primul si ultimul caracter
        //Am nevoie de aceasta functie pentru eliminarea caracterelor "[", "]", "'"
        public static string stergeCaractere(string interval)
        {
            interval = interval.Remove(0, 1);
            interval = interval.Remove(interval.Length - 1);
            return interval;
        }

        //Primeste un vector de stringuri care reprezinta intervalele de timp rezervate in calendar
        //Returneaza un calendar corespunzator
        public static List<bool> getCalendar(string[] rezervari)
        {
            List<bool> calendar = new List<bool>();
            for(int i = 0; i < 60 * 24; i++)
            {
                calendar.Add(true);
            }
            foreach(string rezervare in rezervari){
                var ore = stergeCaractere(rezervare).Split(',');
                var oraInceput = getMinute(stergeCaractere(ore[0]));
                var oraSfarsit = getMinute(stergeCaractere(ore[1]));
                for(int i = oraInceput; i <= oraSfarsit; i++)
                {
                    calendar[i] = false;
                }
            }
            return calendar;
        }

        //Primeste cele 2 stringuri reprezentand limitele fiecarui calendar si o returneaza pe valoarea cea mai mare de inceput
        //si cea mai mica de final sub forma de lista
        public static List<int> getRangeLimits(string r1,string r2)
        {
            List<int> lista = new List<int>();
            var ore1 = stergeCaractere(r1).Split(',');
            var ore2 = stergeCaractere(r2).Split(',');
            var oraInceput1 = getMinute(stergeCaractere(ore1[0]));
            var oraInceput2 = getMinute(stergeCaractere(ore2[0]));
            var oraSfarsit1 = getMinute(stergeCaractere(ore1[1]));
            var oraSfarsit2 = getMinute(stergeCaractere(ore2[1]));
            if (oraInceput1 < oraInceput2)
            {
                lista.Add(oraInceput2);
            }
            else
            {
                lista.Add(oraSfarsit1);
            }
            if (oraSfarsit1 < oraSfarsit2)
            {
                lista.Add(oraSfarsit1);
            }
            else
            {
                lista.Add(oraSfarsit2);
            }
            return lista;
        }

        //Combina cele doua calendare pentru a determina intervalele libere/rezervate dintre ele.
        //Pentru ca un anumit minut sa fie liber e nevoie ca sa fie liber in ambele calendare, aplicam si pe valorile bool ene
        public static List<bool> getReuniune(List<bool> calendar1, List<bool> calendar2)
        {
            for(int i = 0; i < calendar1.Count; i++)
            {
                calendar1[i] = calendar1[i] && calendar2[i];
            }
            return calendar1;
        }

        //Tranforma un intreg reprezentand minutele in string
        //De exemplu 60 => '1:00'
        public static string getOraString(int ora)
        {
            string s = "'";
            s += (ora / 60).ToString();
            s += ":";
            int min = ora % 60;
            if (min < 10)
            {
                s += "0" + min.ToString() + "'";
            }
            else s += min.ToString() + "'";
            return s;
        }

        //Construieste un string care reprezinta un interval
        //De exemplu pentru 60 - 120 => ['1:00','2:00']
        public static string transformToString(int ora1,int ora2)
        {
            string interval= "[";
            interval += getOraString(ora1)+",";
            interval += getOraString(ora2);
            interval += "]";
            return interval;
        }

        //Parcurge un calendar pe baza limitelor de inceput si sfarsit si pe baza duratei determina intervalele libere
        public static string getIntervaleDisponibile(List<bool> calendar,List<int> limite,int durata)
        {
            string rezultat = "[";
            int poz = -1; //daca valoarea poz este -1 atunci nu am gasit un inceput de interval
            for(int i = limite[0]; i <= limite[1]; i++)
            {

                if (calendar[i] == true)
                {
                    if (poz == -1)
                    {
                        if (i == 0) poz = 0; //gasim o valoare libera si salvam pozitia de inceput care este pozitia curenta - 1 
                        else poz = i - 1;    // 0 daca limita inferioara este 0 si suntem pe prima pozitie
                    }
                    if (i == limite[1] && poz != -1 && i - poz >= durata) //daca ajunge la limita superioara si avem o valoare poz pt inceputul de
                    {                                                     //interval atunci limita superioara reprezinta si limita sup a intervalului 
                        if (!rezultat.Equals("[")) rezultat += ", ";      //curent
                        rezultat += transformToString(poz, i);
                    }
                }
                else
                {
                    if (poz == -1) continue;
                    if (i - poz >= durata)
                    {
                        if (!rezultat.Equals("[")) rezultat += ", ";
                        rezultat += transformToString(poz, i);
                    }
                    poz = -1;
                }
            }
            rezultat += "]";
            return rezultat;
        }

        static void Main(string[] args)
        {
            List<string> argsFromFile = new List<string>();
            foreach (string line in System.IO.File.ReadLines("input.txt"))
            {
                argsFromFile.Add(line);
            }  
            var cal1 = getCalendar(stergeCaractere(argsFromFile[0]).Split(", "));
            var cal2 = getCalendar(stergeCaractere(argsFromFile[2]).Split(", "));
            var cal = getReuniune(cal1, cal2);
            var limite = getRangeLimits(argsFromFile[1], argsFromFile[3]);
            var intervale = getIntervaleDisponibile(cal, limite, int.Parse(argsFromFile[4]));
            Console.WriteLine(intervale);
        }
    }
}
