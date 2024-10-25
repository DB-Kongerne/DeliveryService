public class Delivery
{
    public string AfsenderAdresse { get; set; }
    public string ModtagerAdresse { get; set; }
    public double PakkeVægt { get; set; }
    public DateTime Leveringsdato { get; set; }
    public string Status { get; set; }

    public Delivery(string afsenderAdresse, string modtagerAdresse, double pakkeVægt, DateTime leveringsdato)
    {
        AfsenderAdresse = afsenderAdresse;
        ModtagerAdresse = modtagerAdresse;
        PakkeVægt = pakkeVægt;
        Leveringsdato = leveringsdato;
        Status = "I behandling"; // Initial status
    }

    // Optional: Method to update the delivery status
    public void OpdaterStatus(string nyStatus)
    {
        Status = nyStatus;
    }
}
