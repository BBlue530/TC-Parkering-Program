using System;
using System.Collections.Generic;

namespace TC_Parkering_Program
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string användarval;
            Console.WriteLine("Välj din roll");
            Console.WriteLine("1: Kund");
            Console.WriteLine("2: Parkeringsvakt");
            Console.WriteLine("3: Ägare");
            användarval = Console.ReadLine();
            switch (användarval)
            {
                case "1":
                    KundUI();
                    break;

                case "2":
                    Vakt();
                    break;

                case "3":
                    Ägare();
                    break;

                default:
                    Console.WriteLine("Ogiltigt");
                    break;
            }
        }

        static void KundUI()
        {
            Console.WriteLine("Välkommen !");
            Console.WriteLine("Skriv in ditt registreringsnummer:");
            string regnummer = Console.ReadLine();

            Console.WriteLine("Välj fordonstyp:");
            Console.WriteLine("1: Bil");
            Console.WriteLine("2: Buss (Tar två platser)");
            Console.WriteLine("3: Motorcykel (Kan dela plats)");
            string fordonstypVal = Console.ReadLine();

            string fordonstyp = fordonstypVal switch
            {
                "1" => "bil",
                "2" => "buss",
                "3" => "motorcykel",
                _ => "bil" // Om användaren inte väljer ett giltigt alternativ
            };

            // Ange parkeringstid
            Console.WriteLine("Ange hur länge du vill parkera (i sekunder):");
            int parkeringstid;
            while (!int.TryParse(Console.ReadLine(), out parkeringstid) || parkeringstid <= 0)
            {
                Console.WriteLine("Vänligen ange ett giltigt antal sekunder.");
            }

            // Parkera fordonet
            parkeringsplats parkering = new parkeringsplats();
            DateTime startTid = DateTime.Now;
            string parkeringPlats = parkering.ParkeraBil(fordonstyp, regnummer, startTid, parkeringstid);

            Console.WriteLine($"Fordonet med registreringsnummer {regnummer} har parkerat på: {parkeringPlats}");

            

            // Checka ut eller fortsätt
            Console.WriteLine("\nVill du checka ut?");
            Console.WriteLine("1: Ja");
            Console.WriteLine("2: Nej");
            string kundVal = Console.ReadLine();

            if (kundVal == "1")
            {
                // Beräkna kostnad för parkering
                DateTime slutTid = DateTime.Now;
                double parkeringstidSek = (slutTid - startTid).TotalSeconds;
                decimal pris = (decimal)parkeringstidSek * 1.5m; // 1,5 kr per sekund
                Console.WriteLine($"Parkeringstid: {parkeringstidSek:F0} sekunder.");
                Console.WriteLine($"Belopp att betala: {pris:C}");

                // Checka ut fordonet
                parkering.CheckaUtBil(regnummer);
                Console.WriteLine("Tack för ditt besök!");
            }
        }

        static void Vakt()
        {
            Console.WriteLine("Vaktfunktion...");
            // Här kan funktionalitet för parkeringsvakt läggas till
        }

        static void Ägare()
        {
            Console.WriteLine("Ägarefunktion...");
            // Här kan funktionalitet för ägare läggas till
        }
    }

    public class parkeringsplats
    {
        private string[] fordon = new string[25];  // Parkeringsplatser, max 25
        private DateTime[] parkeringstid = new DateTime[25];  // Tidpunkt för parkering
        private int[] fordonstid = new int[25];  // Tiden för parkering i sekunder
        private string[] fordonstyp = new string[25];  // Typ av fordon (Bil, Buss, Motorcykel)
        private string[] fordonRegNr = new string[25];  // Registreringsnummer för fordon

        private Random random = new Random();

        // Slumpa ledig plats
        private int HittaLedigPlats(int platserBehovda, bool delaPlats = false)
        {
            List<int> ledigaPlatser = new List<int>();

            if (platserBehovda == 1 && delaPlats) // För motorcykel
            {
                for (int i = 0; i < fordon.Length; i++)
                {
                    if (fordon[i] == null || fordon[i].StartsWith("MC"))
                    {
                        ledigaPlatser.Add(i);
                    }
                }
            }
            else if (platserBehovda == 1) // För bil
            {
                for (int i = 0; i < fordon.Length; i++)
                {
                    if (fordon[i] == null)
                    {
                        ledigaPlatser.Add(i);
                    }
                }
            }
            else if (platserBehovda == 2) // För buss
            {
                for (int i = 0; i < fordon.Length - 1; i++)
                {
                    if (fordon[i] == null && fordon[i + 1] == null)
                    {
                        ledigaPlatser.Add(i);
                    }
                }
            }

            if (ledigaPlatser.Count > 0)
            {
                // Slumpa en av de lediga platserna
                return ledigaPlatser[random.Next(ledigaPlatser.Count)];
            }

            return -1; // Ingen ledig plats hittades
        }

        // Parkera fordonet
        public string ParkeraBil(string fordonstyp, string regnummer, DateTime startTid, int parkeringstid)
        {
            int ledigPlats;

            if (fordonstyp == "bil")
            {
                ledigPlats = HittaLedigPlats(1); // En plats behövs
                if (ledigPlats != -1)
                {
                    fordon[ledigPlats] = regnummer;
                    this.parkeringstid[ledigPlats] = startTid;
                    this.fordonstid[ledigPlats] = parkeringstid;
                    this.fordonstyp[ledigPlats] = fordonstyp;
                    this.fordonRegNr[ledigPlats] = regnummer;
                    return $"Plats {ledigPlats + 1}";
                }
            }
            else if (fordonstyp == "buss")
            {
                ledigPlats = HittaLedigPlats(2); // Två intilliggande platser behövs
                if (ledigPlats != -1)
                {
                    fordon[ledigPlats] = regnummer;
                    fordon[ledigPlats + 1] = regnummer;
                    this.parkeringstid[ledigPlats] = startTid;
                    this.parkeringstid[ledigPlats + 1] = startTid;
                    this.fordonstid[ledigPlats] = parkeringstid;
                    this.fordonstid[ledigPlats + 1] = parkeringstid;
                    this.fordonstyp[ledigPlats] = fordonstyp;
                    this.fordonstyp[ledigPlats + 1] = fordonstyp;
                    this.fordonRegNr[ledigPlats] = regnummer;
                    this.fordonRegNr[ledigPlats + 1] = regnummer;
                    return $"Plats {ledigPlats + 1}-{ledigPlats + 2}";
                }
            }
            else if (fordonstyp == "motorcykel")
            {
                ledigPlats = HittaLedigPlats(1, true); // En plats, tillåter delning
                if (ledigPlats != -1)
                {
                    if (fordon[ledigPlats] == null)
                    {
                        fordon[ledigPlats] = "MC-" + regnummer;
                        this.parkeringstid[ledigPlats] = startTid;
                        this.fordonstid[ledigPlats] = parkeringstid;
                        this.fordonstyp[ledigPlats] = fordonstyp;
                        this.fordonRegNr[ledigPlats] = regnummer;
                    }
                    else if (fordon[ledigPlats].StartsWith("MC") && !fordon[ledigPlats].Contains("+"))
                    {
                        fordon[ledigPlats] += "+" + regnummer; // Lägg till ytterligare MC på samma plats
                    }
                    return $"Plats {ledigPlats + 1}";
                }
            }

            return "Inga lediga parkeringsplatser för fordonstypen.";
        }

        

        // Checka ut fordonet
        public void CheckaUtBil(string regnummer)
        {
            for (int i = 0; i < fordon.Length; i++)
            {
                if (fordon[i] != null && fordon[i].Contains(regnummer))
                {
                    fordon[i] = null;
                    fordonstid[i] = 0;
                    parkeringstid[i] = DateTime.MinValue;
                    fordonstyp[i] = null;
                    fordonRegNr[i] = null;
                }
            }
        }
    }
}

