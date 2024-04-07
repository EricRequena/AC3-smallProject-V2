using System;
using System.Collections.Generic;
using System.Formats.Asn1;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using CsvHelper;

namespace ConsoleApp1
{
    [XmlRoot("Comarques")]
    public class ComarquesXml
    {
        [XmlElement("Comarca")]
        public List<ComarcaXml> Comarques { get; set; }
    }

    class Program
    {
        static List<Comarca> comarques = new List<Comarca>();

        static void Main(string[] args)
        {
            string rutaRelativa = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "dades.csv");
            LlegirDadesCSV(rutaRelativa);

            const string menuOptionPrompt = "Escull una opció (1-5):";
            Console.WriteLine("1. Identificar comarques amb població superior a 200000");
            Console.WriteLine("2. Calcular consum domèstic mitjà per comarca");
            Console.WriteLine("3. Mostrar comarques amb consum domèstic per càpita més alt");
            Console.WriteLine("4. Mostrar comarques amb consum domèstic per càpita més baix");
            Console.WriteLine("5. Filtrar comarques per nom o codi");
            Console.WriteLine(menuOptionPrompt);

            int opcio = int.Parse(Console.ReadLine());

            switch (opcio)
            {
                case 1:
                    IdentificarComarquesPoblacioSuperior();
                    break;
                case 2:
                    CalcularConsumMitjaPerComarca();
                    break;
                case 3:
                    MostrarComarquesConsumPerCapitaMesAlt();
                    break;
                case 4:
                    MostrarComarquesConsumPerCapitaMesBaix();
                    break;
                case 5:
                    FiltrarPerNomOCodi();
                    break;
                default:
                    Console.WriteLine("Opció no vàlida.");
                    break;
            }
        }

        static void LlegirDadesCSV(string path)
        {
            using (var reader = new StreamReader(path))
            using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
            {
                csv.Read();
                csv.ReadHeader();
                while (csv.Read())
                {
                    var any = csv.GetField<int>("Any");
                    var codiComarca = csv.GetField<int>("Codi comarca");
                    var nomComarca = csv.GetField("Comarca");
                    var poblacio = csv.GetField<int>("Població");
                    var domesticXarxa = csv.GetField<int>("Domèstic xarxa");
                    var activitatsEconomiques = csv.GetField<int>("Activitats econòmiques i fonts pròpies");
                    var total = csv.GetField<int>("Total");
                    var consumDomesticPerCapita = csv.GetField<double>("Consum domèstic per càpita");

                    var comarca = new Comarca
                    {
                        Any = any,
                        CodiComarca = codiComarca,
                        NomComarca = nomComarca,
                        Poblacio = poblacio,
                        DomesticXarxa = domesticXarxa,
                        ActivitatsEconomiques = activitatsEconomiques,
                        Total = total,
                        ConsumDomesticPerCapita = consumDomesticPerCapita
                    };

                    comarques.Add(comarca);
                }
            }
            var comarquesXml = new ComarquesXml
            {
                Comarques = comarques.Select(c => new ComarcaXml
                {
                    Any = c.Any,
                    CodiComarca = c.CodiComarca,
                    NomComarca = c.NomComarca,
                    Poblacio = c.Poblacio,
                    DomesticXarxa = c.DomesticXarxa,
                    ActivitatsEconomiques = c.ActivitatsEconomiques,
                    Total = c.Total,
                    ConsumDomesticPerCapita = c.ConsumDomesticPerCapita
                }).ToList()
            };

            XmlSerializer serializer = new XmlSerializer(typeof(ComarquesXml));
            using (StreamWriter sw = new StreamWriter("comarques.xml"))
            {
                serializer.Serialize(sw, comarquesXml);
            }
        }

        static void IdentificarComarquesPoblacioSuperior()
        {
            var comarquesPoblacioSuperior = comarques.Where(c => c.Poblacio > 200000);
            ImprimirComarques(comarquesPoblacioSuperior);
        }

        static void CalcularConsumMitjaPerComarca()
        {
            var consumMitjaPerComarca = comarques.GroupBy(c => c.NomComarca)
                .Select(g => new { Comarca = g.Key, ConsumMitja = g.Average(c => c.Total) });
            foreach (var item in consumMitjaPerComarca)
            {
                Console.WriteLine($"Comarca: {item.Comarca}, Consum Mitjà: {item.ConsumMitja}");
            }
        }

        static void MostrarComarquesConsumPerCapitaMesAlt()
        {
            var comarquesConsumPerCapitaMesAlt = comarques.Where(c => c.ConsumDomesticPerCapita == comarques.Max(x => x.ConsumDomesticPerCapita));
            ImprimirComarques(comarquesConsumPerCapitaMesAlt);
        }

        static void MostrarComarquesConsumPerCapitaMesBaix()
        {
            var comarquesConsumPerCapitaMesBaix = comarques.Where(c => c.ConsumDomesticPerCapita == comarques.Min(x => x.ConsumDomesticPerCapita));
            ImprimirComarques(comarquesConsumPerCapitaMesBaix);
        }

        static void FiltrarPerNomOCodi()
        {
            const string filterPrompt = "Introdueix el nom o codi de la comarca:";
            Console.WriteLine(filterPrompt);
            string consulta = Console.ReadLine();
            var comarquesFiltrades = comarques.Where(c => c.NomComarca.Contains(consulta) || c.CodiComarca.ToString().Contains(consulta));
            ImprimirComarques(comarquesFiltrades);
        }

        static void ImprimirComarques(IEnumerable<Comarca> comarques)
        {
            foreach (var comarca in comarques)
            {
                Console.WriteLine($"Any: {comarca.Any}, Codi Comarca: {comarca.CodiComarca}, Nom Comarca: {comarca.NomComarca}, Poblacio: {comarca.Poblacio}, Domestic Xarxa: {comarca.DomesticXarxa}, Activitats Economiques: {comarca.ActivitatsEconomiques}, Total: {comarca.Total}, Consum Domèstic Per Càpita: {comarca.ConsumDomesticPerCapita}");
            }
        }
    }
}
