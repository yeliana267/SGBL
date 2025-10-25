namespace SGBL.Infraestructure
{
    using SGBL.Application.Interfaces;
    using System.IO;
    using System.Xml.Linq;

    public class ServiceLogs: IServiceLogs
    {
       

        private  string _DirectoryFilePath ="";
        private  string _LogFilePath ="";


        public void CreateLogInfo (string sLog)
        {

            CreateLog(sLog, "Info");
        }
        public void CreateLogWarning(string sLog)
        {

            CreateLog(sLog, "Warning");
        }
        public void CreateLogError(string sLog)
        {

            CreateLog(sLog, "Error");
        }
        private void CreateLog(string sLog, string typeLog)

        { //   Obtiene el directorio raíz del proyecto(donde está el.sln)
            var rootPath = Directory.GetParent(AppContext.BaseDirectory)!.Parent!.Parent!.Parent!.Parent!.FullName;
            _DirectoryFilePath = Path.Combine(rootPath + "/Logs");
            //confirmacion de la carpeta log y info/warning/error
            CreateDirectory(_DirectoryFilePath);
            _LogFilePath = Path.Combine(_DirectoryFilePath + $"/Logs{typeLog}");
            CreateDirectory(_LogFilePath);

            string nombre = GetNameFile(typeLog);
            string cadena = "";

            cadena += DateTime.Now + " - " + sLog + Environment.NewLine;

            //agrega una nueva linea al archivo o crea uno nuevo si no existe el nombre, segun entiendo
            StreamWriter sw = new StreamWriter(_LogFilePath + "/" + nombre, true);
            sw.Write(cadena);
            sw.Close();

        }

        #region HELPER
        private string GetNameFile(string typeLog)
        {
            string nombre = "";

            nombre = $"Log{typeLog}_" + DateTime.Now.Year + "_" + DateTime.Now.Month + "_" + DateTime.Now.Day + ".txt";

            return nombre;
        }

        private void CreateDirectory(string path)
        {         
            try
            {
                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);


            }
            catch (DirectoryNotFoundException ex)
            {
                throw new Exception(ex.Message);

            }
        }
        #endregion
    }
}
