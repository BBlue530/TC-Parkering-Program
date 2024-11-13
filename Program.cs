using System;

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
            Console.WriteLine("Välkommen kund!");
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

            // Automatgenerera registreringsnummer
            parkeringsplats parkering = new parkeringsplats();
            string regnummer = parkering.Reggnummer();

            // Ange parkeringstid
            Console.WriteLine("Ange hur länge du vill parkera (i sekunder):");
            int parkeringstid;
            while (!int.TryParse(Console.ReadLine(), out parkeringstid) || parkeringstid <= 0)
            {
                Console.WriteLine("Vänligen ange ett giltigt antal sekunder.");
            }

            // Parkera fordonet
            DateTime startTid = DateTime.Now;
            string parkeringPlats = parkering.ParkeraBil(fordonstyp, regnummer, startTid, parkeringstid);

            Console.WriteLine($"Fordonet med registreringsnummer {regnummer} har parkerat på: {parkeringPlats}");

            // Visa alla parkerade fordon
            parkering.SkrivUtParkeradeFordonsLista();

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

        // Automatgenerera registreringsnummer
        public string Reggnummer()
        {
            const string bokstäver = "ABCDEFGHIJKLMNOPQRSTUVWHYZ";
            const string siffror = "0123456789";

            char[] reg = new char[6];
            for (int i = 0; i < 3; i++)
            {
                reg[i] = bokstäver[random.Next(bokstäver.Length)];
            }

            for (int i = 3; i < 6; i++)
            {
                reg[i] = siffror[random.Next(siffror.Length)];
            }

            return new string(reg);
        }

        // Parkera fordonet
        public string ParkeraBil(string fordonstyp, string regnummer, DateTime startTid, int parkeringstid)
        {
            if (fordonstyp == "bil")
            {
                for (int i = 0; i < fordon.Length; i++)
                {
                    if (fordon[i] == null)  // Ledig plats
                    {
                        fordon[i] = regnummer;
                        this.parkeringstid[i] = startTid;
                        this.fordonstid[i] = parkeringstid;
                        this.fordonstyp[i] = fordonstyp;
                        this.fordonRegNr[i] = regnummer;
                        return $"Plats {i + 1}";
                    }
                }
            }
            else if (fordonstyp == "buss")
            {
                // Buss kräver 2 intilliggande platser
                for (int i = 0; i < fordon.Length - 1; i++)
                {
                    if (fordon[i] == null && fordon[i + 1] == null)  // Två intilliggande lediga platser
                    {
                        fordon[i] = regnummer;
                        fordon[i + 1] = regnummer;
                        this.parkeringstid[i] = startTid;
                        this.parkeringstid[i + 1] = startTid;
                        this.fordonstid[i] = parkeringstid;
                        this.fordonstid[i + 1] = parkeringstid;
                        this.fordonstyp[i] = "buss";
                        this.fordonstyp[i + 1] = "buss";
                        this.fordonRegNr[i] = regnummer;
                        this.fordonRegNr[i + 1] = regnummer;
                        return $"Plats {i + 1}-{i + 2}";
                    }
                }
            }
            else if (fordonstyp == "motorcykel")
            {
                // Motorcykel kan dela plats
                for (int i = 0; i < fordon.Length; i++)
                {
                    if (fordon[i] == null || fordon[i].StartsWith("MC"))  // Ledig eller delad MC-plats
                    {
                        if (fordon[i] == null)
                        {
                            fordon[i] = "MC-" + regnummer;
                            this.parkeringstid[i] = startTid;
                            this.fordonstid[i] = parkeringstid;
                            this.fordonstyp[i] = fordonstyp;
                            this.fordonRegNr[i] = regnummer;
                        }
                        else if (fordon[i].StartsWith("MC") && !fordon[i].Contains("+"))
                        {
                            fordon[i] += "+" + regnummer;  // Lägg till ytterligare MC på samma plats
                        }
                        return $"Plats {i + 1}";
                    }
                }
            }

            return "Inga lediga parkeringsplatser för fordonstypen.";
        }

        // Skriv ut alla parkerade fordon
        public void SkrivUtParkeradeFordonsLista()
        {
            Console.WriteLine("\nAlla parkerade fordon:");
            for (int i = 0; i < fordon.Length; i++)
            {
                if (fordon[i] != null)
                {
                    string status = (fordonstid[i] > 0) ? $"{fordonstid[i]} sek kvar" : "Tiden ute";
                    Console.WriteLine($"Plats {i + 1}: {fordonstyp[i]} {fordonRegNr[i]} Tiden: {status}");
                }
            }
        }

        // Checka ut ett fordon
        public void CheckaUtBil(string regnummer)
        {
            for (int i = 0; i < fordon.Length; i++)
            {
                if (fordonRegNr[i] == regnummer)
                {
                    // Rensa platsen och tid
                    fordon[i] = null;
                    parkeringstid[i] = DateTime.MinValue;
                    fordonstid[i] = 0;
                    fordonstyp[i] = null;
                    fordonRegNr[i] = null;
                    break;
                }
            }
        }
    }
}
