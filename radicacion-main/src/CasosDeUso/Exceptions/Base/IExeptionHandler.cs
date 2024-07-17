using Microsoft.AspNetCore.Mvc.Filters;

namespace CasosDeUso.Exceptions.Base
{
    public interface IExeptionHandler
    {
        Task Handle(ExceptionContext context);
    }
}
