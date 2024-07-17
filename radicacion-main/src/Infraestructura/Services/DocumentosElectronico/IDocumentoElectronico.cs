namespace Infraestructura.Services.DocumentosElectronico
{
    public interface IDocumentoElectronico
    {
        bool EsValido { get; }

        string CreateNewFolder(string Directorio);
        string GetDirectorioOrigenFinal();
    }
}