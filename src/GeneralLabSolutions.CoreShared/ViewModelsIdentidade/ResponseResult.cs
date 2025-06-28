namespace GeneralLabSolutions.CoreShared.ViewModelsIdentidade
{
    public class ResponseResult
    {
        public string Title { get; set; }
        public int Status { get; set; }
        // Inicializar Errors aqui
        public ResponseErrorMessages Errors { get; set; } = new ResponseErrorMessages();

        // Construtor opcional para garantir a inicialização,
        // mas a inicialização inline acima é suficiente.
        public ResponseResult()
        {
            Errors = new ResponseErrorMessages();
        }
    }
}