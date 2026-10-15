// Conversor "Unidade SAP (MEINS)" - CalculadoraVermelha
// Tabela das unidades de medida padrão do SAP (tabela T006 / transação CUNI),
// agrupadas por dimensão (T006D). Cada unidade guarda o fator para a unidade SI
// da sua dimensão: valor_SI = valor * Factor + Additive (igual a ZAEHL/NENNR*10^EXP10 + ADDKO).
//
// Só existe conversão entre unidades da MESMA dimensão (ex.: G <-> KG, L <-> M3).
// Unidades de embalagem (CX, PAL, FD...) dependem do material (MARM / MM03 > Unidades
// alternativas) e por isso não entram aqui.
//
// Para incluir uma unidade Z do seu sistema, basta acrescentar uma linha na lista abaixo.

namespace CalculatorApp.ViewModel.DataLoaders
{
    internal sealed class SapUnitOfMeasure
    {
        public SapUnitOfMeasure(string internalCode, string externalCode, string description, string dimension, decimal factor, decimal additive = 0m)
        {
            InternalCode = internalCode;
            ExternalCode = externalCode;
            Description = description;
            Dimension = dimension;
            Factor = factor;
            Additive = additive;
        }

        /// <summary>Código interno gravado em MEINS (ex.: ST, TAG, CCM).</summary>
        public string InternalCode { get; }

        /// <summary>Código comercial/externo exibido na tela (ex.: PC, DAY, CM3).</summary>
        public string ExternalCode { get; }

        public string Description { get; }

        /// <summary>Dimensão (T006D) - só converte dentro da mesma dimensão.</summary>
        public string Dimension { get; }

        /// <summary>Fator para a unidade SI da dimensão.</summary>
        public decimal Factor { get; }

        /// <summary>Constante aditiva para a unidade SI (usada só em temperatura).</summary>
        public decimal Additive { get; }

        /// <summary>Temperatura usa constante aditiva, então não aceita fator de material.</summary>
        public bool IsTemperature => Dimension == "Temperatura";

        /// <summary>Temperatura e Proporção (%, ‰, PPM) não fazem sentido com fator de material.</summary>
        public bool AllowsMaterialFactor => !IsTemperature && Dimension != "Proporção";

        public string DisplayName
        {
            get
            {
                string code = InternalCode == ExternalCode ? ExternalCode : ExternalCode + " (" + InternalCode + ")";
                return code + " — " + Description + " · " + Dimension;
            }
        }
    }

    internal static class SapUnitsOfMeasure
    {
        // Faixa de IDs reservada para não colidir com as unidades nativas (1..168) nem com as moedas (169+).
        public const int FirstId = 5000;

        // Unidades padrão ao abrir o conversor (o exemplo do G -> KG).
        public const string DefaultFrom = "G";
        public const string DefaultTo = "KG";

        private const string Massa = "Massa";
        private const string Volume = "Volume";
        private const string Comprimento = "Comprimento";
        private const string Area = "Área";
        private const string Quantidade = "Quantidade";
        private const string Proporcao = "Proporção";
        private const string Tempo = "Tempo";
        private const string Temperatura = "Temperatura";
        private const string Energia = "Energia";
        private const string Potencia = "Potência";
        private const string Pressao = "Pressão";
        private const string Velocidade = "Velocidade";

        // A ordem desta lista é a ordem em que as unidades aparecem no combo.
        public static readonly SapUnitOfMeasure[] All = new[]
        {
            // ---- MASSA (SI: KG) ----
            new SapUnitOfMeasure("MG",  "MG",  "Miligrama",              Massa, 0.000001m),
            new SapUnitOfMeasure("G",   "G",   "Grama",                  Massa, 0.001m),
            new SapUnitOfMeasure("KG",  "KG",  "Quilograma",             Massa, 1m),
            new SapUnitOfMeasure("TO",  "TO",  "Tonelada",               Massa, 1000m),
            new SapUnitOfMeasure("OZ",  "OZ",  "Onça",                   Massa, 0.028349523125m),
            new SapUnitOfMeasure("LB",  "LB",  "Libra",                  Massa, 0.45359237m),
            new SapUnitOfMeasure("TON", "TON", "Tonelada curta (EUA)",   Massa, 907.18474m),

            // ---- VOLUME (SI: M3) ----
            new SapUnitOfMeasure("MMQ", "MM3", "Milímetro cúbico",       Volume, 0.000000001m),
            new SapUnitOfMeasure("ML",  "ML",  "Mililitro",              Volume, 0.000001m),
            new SapUnitOfMeasure("CCM", "CM3", "Centímetro cúbico",      Volume, 0.000001m),
            new SapUnitOfMeasure("L",   "L",   "Litro",                  Volume, 0.001m),
            new SapUnitOfMeasure("CDM", "DM3", "Decímetro cúbico",       Volume, 0.001m),
            new SapUnitOfMeasure("HL",  "HL",  "Hectolitro",             Volume, 0.1m),
            new SapUnitOfMeasure("M3",  "M3",  "Metro cúbico",           Volume, 1m),
            new SapUnitOfMeasure("IN3", "IN3", "Polegada cúbica",        Volume, 0.000016387064m),
            new SapUnitOfMeasure("PT",  "PT",  "Pinta (EUA)",            Volume, 0.000473176473m),
            new SapUnitOfMeasure("QT",  "QT",  "Quarto (EUA)",           Volume, 0.000946352946m),
            new SapUnitOfMeasure("GLL", "GAL", "Galão (EUA)",            Volume, 0.003785411784m),
            new SapUnitOfMeasure("GLI", "GLI", "Galão imperial (UK)",    Volume, 0.00454609m),
            new SapUnitOfMeasure("FT3", "FT3", "Pé cúbico",              Volume, 0.028316846592m),
            new SapUnitOfMeasure("YD3", "YD3", "Jarda cúbica",           Volume, 0.764554857984m),

            // ---- COMPRIMENTO (SI: M) ----
            new SapUnitOfMeasure("MIM", "µM",  "Micrômetro",             Comprimento, 0.000001m),
            new SapUnitOfMeasure("MM",  "MM",  "Milímetro",              Comprimento, 0.001m),
            new SapUnitOfMeasure("CM",  "CM",  "Centímetro",             Comprimento, 0.01m),
            new SapUnitOfMeasure("DM",  "DM",  "Decímetro",              Comprimento, 0.1m),
            new SapUnitOfMeasure("M",   "M",   "Metro",                  Comprimento, 1m),
            new SapUnitOfMeasure("KM",  "KM",  "Quilômetro",             Comprimento, 1000m),
            new SapUnitOfMeasure("IN",  "IN",  "Polegada",               Comprimento, 0.0254m),
            new SapUnitOfMeasure("FT",  "FT",  "Pé",                     Comprimento, 0.3048m),
            new SapUnitOfMeasure("YD",  "YD",  "Jarda",                  Comprimento, 0.9144m),
            new SapUnitOfMeasure("MI",  "MI",  "Milha",                  Comprimento, 1609.344m),

            // ---- ÁREA (SI: M2) ----
            new SapUnitOfMeasure("MMK", "MM2", "Milímetro quadrado",     Area, 0.000001m),
            new SapUnitOfMeasure("CM2", "CM2", "Centímetro quadrado",    Area, 0.0001m),
            new SapUnitOfMeasure("DM2", "DM2", "Decímetro quadrado",     Area, 0.01m),
            new SapUnitOfMeasure("M2",  "M2",  "Metro quadrado",         Area, 1m),
            new SapUnitOfMeasure("HAR", "HA",  "Hectare",                Area, 10000m),
            new SapUnitOfMeasure("KMK", "KM2", "Quilômetro quadrado",    Area, 1000000m),
            new SapUnitOfMeasure("IN2", "IN2", "Polegada quadrada",      Area, 0.00064516m),
            new SapUnitOfMeasure("FT2", "FT2", "Pé quadrado",            Area, 0.09290304m),
            new SapUnitOfMeasure("YD2", "YD2", "Jarda quadrada",         Area, 0.83612736m),
            new SapUnitOfMeasure("ACR", "ACR", "Acre",                   Area, 4046.8564224m),
            new SapUnitOfMeasure("MI2", "MI2", "Milha quadrada",         Area, 2589988.110336m),

            // ---- PROPORÇÃO (base: fração 1) - separada de PC para não poluir as sugestões ----
            new SapUnitOfMeasure("PPM", "PPM", "Partes por milhão",      Proporcao, 0.000001m),
            new SapUnitOfMeasure("%O",  "‰",   "Por mil",                Proporcao, 0.001m),
            new SapUnitOfMeasure("%",   "%",   "Percentual",             Proporcao, 0.01m),

            // ---- QUANTIDADE / SEM DIMENSÃO (base: PC) ----
            new SapUnitOfMeasure("ST",  "PC",  "Peça",                   Quantidade, 1m),
            new SapUnitOfMeasure("DZN", "DZN", "Dúzia",                  Quantidade, 12m),
            new SapUnitOfMeasure("GRO", "GRO", "Grosa (12 dúzias)",      Quantidade, 144m),

            // ---- TEMPO (SI: S) ----
            new SapUnitOfMeasure("MS",  "MS",  "Milissegundo",           Tempo, 0.001m),
            new SapUnitOfMeasure("S",   "S",   "Segundo",                Tempo, 1m),
            new SapUnitOfMeasure("MIN", "MIN", "Minuto",                 Tempo, 60m),
            new SapUnitOfMeasure("H",   "H",   "Hora",                   Tempo, 3600m),
            new SapUnitOfMeasure("TAG", "DAY", "Dia",                    Tempo, 86400m),
            new SapUnitOfMeasure("WCH", "WK",  "Semana",                 Tempo, 604800m),
            new SapUnitOfMeasure("MON", "MON", "Mês (30 dias)",          Tempo, 2592000m),
            new SapUnitOfMeasure("JHR", "YR",  "Ano (365 dias)",         Tempo, 31536000m),

            // ---- TEMPERATURA (SI: K) ----
            new SapUnitOfMeasure("CEL", "°C",  "Grau Celsius",           Temperatura, 1m, 273.15m),
            new SapUnitOfMeasure("FA",  "°F",  "Grau Fahrenheit",        Temperatura, 5m / 9m, 459.67m * 5m / 9m),
            new SapUnitOfMeasure("K",   "K",   "Kelvin",                 Temperatura, 1m),

            // ---- ENERGIA (SI: J) ----
            new SapUnitOfMeasure("J",   "J",   "Joule",                  Energia, 1m),
            new SapUnitOfMeasure("KJ",  "KJ",  "Quilojoule",             Energia, 1000m),
            new SapUnitOfMeasure("MJ",  "MJ",  "Megajoule",              Energia, 1000000m),
            new SapUnitOfMeasure("KWH", "KWH", "Quilowatt-hora",         Energia, 3600000m),

            // ---- POTÊNCIA (SI: W) ----
            new SapUnitOfMeasure("W",   "W",   "Watt",                   Potencia, 1m),
            new SapUnitOfMeasure("KW",  "KW",  "Quilowatt",              Potencia, 1000m),
            new SapUnitOfMeasure("MW",  "MW",  "Megawatt",               Potencia, 1000000m),

            // ---- PRESSÃO (SI: PA) ----
            new SapUnitOfMeasure("PA",  "PA",  "Pascal",                 Pressao, 1m),
            new SapUnitOfMeasure("MBA", "MBAR","Milibar",                Pressao, 100m),
            new SapUnitOfMeasure("KPA", "KPA", "Quilopascal",            Pressao, 1000m),
            new SapUnitOfMeasure("PSI", "PSI", "Libra/pol² (PSI)",       Pressao, 6894.757293168m),
            new SapUnitOfMeasure("BAR", "BAR", "Bar",                    Pressao, 100000m),

            // ---- VELOCIDADE (SI: M/S) ----
            new SapUnitOfMeasure("KMH", "KM/H","Quilômetro por hora",    Velocidade, 1m / 3.6m),
            new SapUnitOfMeasure("M/S", "M/S", "Metro por segundo",      Velocidade, 1m),
        };

        /// <summary>
        /// Conversão de 'from' para 'to': y = x * ratio + offset (offset aplicado depois).
        /// Dimensões diferentes não são conversíveis: retorna ratio 0.
        /// </summary>
        public static (double ratio, double offset) GetConversion(SapUnitOfMeasure from, SapUnitOfMeasure to)
        {
            if (from.Dimension != to.Dimension)
            {
                return (0d, 0d);
            }

            decimal ratio = from.Factor / to.Factor;
            decimal offset = (from.Additive - to.Additive) / to.Factor;
            return ((double)ratio, (double)offset);
        }
    }
}
