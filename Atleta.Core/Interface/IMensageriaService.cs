namespace Atleta.Core.Interface
{
    public interface IMensageriaService
    {
        void PublicarMensagem<T>(string fila, T mensagem);
    }
}