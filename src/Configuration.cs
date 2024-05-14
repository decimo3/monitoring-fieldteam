using OpenQA.Selenium.Chrome;
namespace Automation;
public class Configuration
{
  public const int ESPERA_LONGA = 5000;
  public const int ESPERA_MEDIA = 3000;
  public const int ESPERA_CURTA = 1500;
  public const int ESPERA_TOTAL = 60_000;
  public readonly string usuario;
  public readonly string palavra;
  public readonly ChromeOptions options = new();
  public readonly string caminho;
  public readonly string website;
  public readonly string gchrome;
  public readonly string recurso;
  public readonly Dictionary<String, String> pathfind = new();
  public Int32 minutes_per_pixel { get; set; }
  public Configuration()
  {
    var configuracoes = ArquivoConfiguracao();
    this.usuario = configuracoes["USUARIO"];
    this.palavra = configuracoes["PALAVRA"];
    this.website = configuracoes["WEBSITE"];
    this.recurso = configuracoes["RECURSO"];
    this.gchrome = configuracoes["GCHROME"];
    this.caminho = @$"{System.IO.Directory.GetCurrentDirectory()}\www";
    this.options.AddArgument($@"--user-data-dir={this.caminho}");
    this.options.AddArgument($@"--app={this.website}");
    this.options.BinaryLocation = this.gchrome;
    VerificarAtributos();
    DefinicaoCaminho();
  }
  private void VerificarAtributos()
  {
    var atributos = typeof(Configuration).GetProperties();
    foreach (var atributo in atributos)
    {
      if (!atributo.CanRead) continue;
      var erro = $"O parâmetro {atributo.Name.ToUpper()} não foi encontrado!";
      if (atributo.GetValue(this) == null)
        throw new InvalidOperationException(erro);
      if (atributo.PropertyType == typeof(String))
        if ((String)atributo.GetValue(this)! != String.Empty)
          throw new InvalidOperationException(erro);
    }
  }
  private Dictionary<string,string> ArquivoConfiguracao()
  {
    var parametros = new Dictionary<string,string>();
    var file = System.IO.File.ReadAllLines(".env");
    foreach (var line in file)
    {
      if (line == null) continue;
      if (line == String.Empty) continue;
      var args = line.Split('=');
      if (args.Length != 2) throw new IndexOutOfRangeException("O arquivo contém valores inválidos!");
      var cfg = args[0];
      var val = args[1];
      parametros.Add(cfg, val);
    }
    return parametros;
  }
  private void DefinicaoCaminho()
  {
    var file = System.IO.File.ReadAllLines("path.conf");
    foreach (var line in file)
    {
      if(String.IsNullOrEmpty(line)) continue;
      var args = line.Split('=');
      if (args.Length != 2) continue;
      var cfg = args[0];
      var val = args[1];
      this.pathfind.Add(cfg, val);
    }
  }
}
