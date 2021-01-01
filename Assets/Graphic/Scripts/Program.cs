using System;
using System.Collections.Generic;
using System.Linq;

namespace DiceRoller_Console
{
    class Program
    {
        public static int WIELKOSC_KOSCI = 6;
        public static int ILOSC_RZUTOW = 10000;
        static void Main(string[] args)
        {
            List<int> WynikiLosowania = GenerujListeWynikowRzutemKoscia("classic");
            List<int> PodsumowanieLosowania = PodliczIloscWyrzuconychWartosciWCalymLosowaniu(WynikiLosowania);

            PokazProcentowyUdzialWylosowanychLiczb(PodsumowanieLosowania);
            Pokaz10PierwszychWylosowanychElementow(WynikiLosowania);

            //------------------------------------------------------------------------------------------------

            WynikiLosowania = GenerujListeWynikowRzutemKoscia("simple");
            PodsumowanieLosowania = PodliczIloscWyrzuconychWartosciWCalymLosowaniu(WynikiLosowania);

            PokazProcentowyUdzialWylosowanychLiczb(PodsumowanieLosowania);
            Pokaz10PierwszychWylosowanychElementow(WynikiLosowania);

            Console.ReadLine();
        }

        private static List<int> GenerujListeWynikowRzutemKoscia(string type)
        {
            List<int> wynikiLosowania = new List<int>();

            switch (type)
            {
                case "classic":
                    Console.WriteLine("\nLosowanie bardziej 'losowe'");
                    for (int i = 0; i < ILOSC_RZUTOW; i++)
                    {
                        wynikiLosowania.Add(RandomNumberGenerator.NumberBetween(1, WIELKOSC_KOSCI));
                    }
                    break;
                
                case "simple":
                    Console.WriteLine("\nProste losowanie");
                    for (int i = 0; i < ILOSC_RZUTOW; i++)
                    {
                        wynikiLosowania.Add(RandomNumberGenerator.SimpleNumberBetween(1, WIELKOSC_KOSCI));
                    }
                    break;
            }
            return wynikiLosowania;
        }
        private static List<int> PodliczIloscWyrzuconychWartosciWCalymLosowaniu(List<int> wynikiLosowania)
        {
            List<int> podsumowanie = new List<int>();
            for (int i = 1; i <= WIELKOSC_KOSCI; i++)
            {
                podsumowanie.Add(wynikiLosowania.Where(p => p.Equals(i)).Count());
            }

            return podsumowanie;
        }
        private static void Pokaz10PierwszychWylosowanychElementow(List<int> wynikiLosowania)
        {
            for (int i = 0; i < 10; i++)
            {
                Console.Write(wynikiLosowania[i].ToString() + ", ");
            }
        }
        private static void PokazProcentowyUdzialWylosowanychLiczb(List<int> podsumowanieLosowania)
        {
            for (int i = 0; i < WIELKOSC_KOSCI; i++)
            {
                Console.Write($"{i + 1} licba wystąpień = {podsumowanieLosowania[i]} [{((float)(podsumowanieLosowania[i] * 100) / ILOSC_RZUTOW)}%]\n");
            }
        }
    }
}