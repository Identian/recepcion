namespace Infraestructura.Services.DocumentosElectronico
{
    public class DocumentoElectronico : DocumentoElectronicoBase, IDocumentoElectronico
    {
        private readonly bool esValido;
        public bool EsValido { get { return esValido; } }

        public DocumentoElectronico()
        {

        }

        public string CreateNewFolder(string Directorio)
        {
            string result = "";
            bool existe = true;
            int i = 0;
            while (existe)
            {
                i++;
                result = Directorio + "_" + Convert.ToString(i);
                existe = Directory.Exists(result);
            }
            Directory.CreateDirectory(result);
            return result;
        }

        public override string GetDirectorioOrigenFinal()
        {
            return DirectorioOrigen!;
        }
    }
}
