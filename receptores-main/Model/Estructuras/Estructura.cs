using Receptores.Model.Correo;

namespace Receptores.Model.Estructuras
{
    public class Estructura
    {
        private readonly int _id;
        private readonly CuentaCorreoReceptorBase _idReceptor;
        private readonly Receptor _r;

        public Estructura(int id, CuentaCorreoReceptorBase idreceptor, Receptor _r)
        {
            _id = id;
            _idReceptor = idreceptor;
            this._r = _r;
        }

        public int GetIdReceptor()
        {
            return this._r.IdReceptor;
        }

        public string? GetUsuario()
        {
            return this._idReceptor.Usuario;
        }

        public int GetIdCuentaCorreo()
        {
            return this._idReceptor.IdCuentaCorreo;
        }

        public string? GetNumIdentificacionReceptor()
        {
            return this._r.NumeroIdentificacionReceptor;
        }

        public int GetId()
        {
            return this._id;
        }

        public CuentaCorreoReceptorBase GetCuentaCorreoReceptorBase()
        {
            return this._idReceptor;
        }

        public Receptor GetReceptor()
        {
            return this._r;
        }
    }
}
