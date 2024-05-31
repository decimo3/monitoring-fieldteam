﻿namespace Automation;
public class Startup
{
  public static void Main(string[] args)
  {
    var cfg = new Configuration();
    using var WebHandler = new WebScraper.Manager(cfg);
    WebHandler.Autenticar();
    while(true)
    {
      try
      {
        Console.WriteLine($"{DateTime.Now} - Atualizando a página...");
        WebHandler.Atualizar();
        Console.WriteLine($"{DateTime.Now} - Atualizando os parâmetros...");
        WebHandler.Parametrizar();
        Console.WriteLine($"{DateTime.Now} - Coletando as informações...");
        WebHandler.Coletor();
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
      catch (System.Exception erro)
      {
        WebHandler.Refresh();
        Console.WriteLine($"{DateTime.Now} - Houve um problema na coleta...");
        Console.WriteLine(erro.Message);
        Console.WriteLine(erro.StackTrace);
      }
    }
  }
}