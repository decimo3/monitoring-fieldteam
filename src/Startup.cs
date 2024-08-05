using Automation.Helpers;
namespace Automation;
public class Startup
{
  public static void Main(string[] args)
  {
    while (true)
    {
    try
    {
    var cfg = new Configuration();
    Updater.Update(cfg);
    using var WebHandler = new WebScraper.Manager(cfg);
    WebHandler.Autenticar();
    WebHandler.VerificarPagina();
    WebHandler.Retroativo();
    while(true)
    {
      try
      {
        Console.WriteLine($"{DateTime.Now} - Verificando solicitações...");
        if(WebHandler.Solicitacoes())
        {
          Console.WriteLine($"{DateTime.Now} - Solicitação respondida!");
          continue;
        }
        if(!WebHandler.TemFinalizacao())
        {
        WebHandler.VerificarPagina();
        Console.WriteLine($"{DateTime.Now} - Atualizando a página...");
        WebHandler.Atualizar(cfg.PISCINAS[WebHandler.contador_de_baldes]);
        Console.WriteLine($"{DateTime.Now} - Atualizando os parâmetros...");
        WebHandler.Parametrizar();
        if(WebHandler.Solicitacoes())
        {
          Console.WriteLine($"{DateTime.Now} - Solicitação respondida!");
          continue;
        }
        Console.WriteLine($"{DateTime.Now} - Coletando as informações...");
        if(WebHandler.Coletor())
        {
        Console.WriteLine($"{DateTime.Now} - Comparando os resultados...");
        WebHandler.Comparar();
        Console.WriteLine($"{DateTime.Now} - Exportando as análises...");
        WebHandler.Relatorio();
        Console.WriteLine($"{DateTime.Now} - Realizando análise final...");
        WebHandler.Finalizacao();
        if(cfg.ENVIRONMENT)
        {
          Console.WriteLine($"{DateTime.Now} - Realizando a captura de tela...");
          WebHandler.Fotografo();
        }
        }
        }
        WebHandler.ProximoBalde();
      }
      catch (System.Exception erro)
      {
        Console.WriteLine($"{DateTime.Now} - Houve um problema na coleta...");
        Console.WriteLine(erro.Message);
        Console.WriteLine(erro.StackTrace);
        WebHandler.Refresh();
      }
    }
    }
    catch (System.Exception erro)
    {
      Console.WriteLine($"{DateTime.Now} - Houve um problema crítico!");
      Console.WriteLine($"{DateTime.Now} - {erro.Message}");
      Console.WriteLine($"{DateTime.Now} - {erro.StackTrace}");
      Console.WriteLine($"{DateTime.Now} - Tentando reiniciar o sistema...");
      var executable = System.IO.Path.Combine(System.IO.Directory.GetCurrentDirectory(), "ofs.exe");
      var arguments = System.Environment.GetCommandLineArgs();
      System.Diagnostics.Process.Start(executable, String.Join(' ', arguments.Skip(1).ToArray()));
      System.Environment.Exit(0);
    }
    }
  }
}