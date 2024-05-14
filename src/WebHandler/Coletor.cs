using Automation.Persistence;
using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;
namespace Automation.WebScraper;
public partial class Manager
{
  public void Coletor()
  {
    // Coleta de nome de recurso e par_pid
    for(var i = 1; true; i++)
    {
      var recursos_path = this.configuration.pathfind["RECURSOS_"].Replace("_", i.ToString());
      var recursos_query = this.driver.FindElements(By.XPath(recursos_path));
      if(!recursos_query.Any()) break;
      var recursos = recursos_query.Single();
      var texto = recursos.FindElement(By.XPath(".//div")).Text;
      while(String.IsNullOrEmpty(texto))
      {
        // TODO - Scroll down para obter nome dos demais recursos
        var tabela = this.driver.FindElement(By.XPath("/html/body/div[14]/div[1]/main/div/div[2]/div[3]/div[1]/div[2]/div/div[2]/div[3]/div/div[1]/table/tbody/tr[2]"));
        var scrollOrigin = new WheelInputDevice.ScrollOrigin { Element = tabela };
        new Actions(this.driver).ScrollFromOrigin(scrollOrigin, 0, 1).Perform();
        texto = recursos.FindElement(By.XPath(".//div")).Text;
      }
      var par_pid = Int32.Parse(recursos.GetAttribute("par_pid"));
      this.atual.Add(new Espelho(texto, par_pid));
    }
    /////////////////////////////////
    //                             //
    // Até aqui está tudo bem!     //
    //                             //
    /////////////////////////////////
    // TODO -  Retornar até o topo da lista
    for(var i = 1; true; i++)
    {
      var gantt_path = this.configuration.pathfind["ESPELHOS_"].Replace("_", i.ToString());
      var gantt_query = this.driver.FindElements(By.XPath(gantt_path));
      if(!gantt_query.Any()) break;
      var gantt = gantt_query.Single();
      var gantt_classes = gantt.GetAttribute("class").Split(" ");
      if(gantt_classes.Contains("toaGantt-hour-line"))
      {
        // TODO - mapear colunas de horários
      }
      if(gantt_classes.Contains("toaGantt-tl"))
      {
        System.Console.WriteLine(gantt.GetAttribute("par_pid"));
        var servicos = gantt.FindElements(By.XPath($".//div"));
        if(!servicos.Any()) break;
        foreach (var servico in servicos)
        {
          var ordem_classes = servico.GetAttribute("class").Split(" ");
          // Verifica se é uma ordem de servico
          if(ordem_classes.Contains("toaGantt-tb"))
          {
            if(ordem_classes.Contains("final")) continue;
            var servico_obj = new Servico();
            servico_obj.par_pid = Int32.Parse(servico.GetDomAttribute("par_pid"));
            servico_obj.aid = Int32.Parse(servico.GetDomAttribute("aid"));
            servico_obj.start = Int32.Parse(servico.GetDomAttribute("start"));
            servico_obj.dur = Int32.Parse(servico.GetDomAttribute("dur"));
            servico_obj.data_activity_eta = Int32.Parse(servico.GetDomAttribute("data-activity-eta"));
            servico_obj.data_activity_status = (int)Enum.Parse<Servico.Status>(servico.GetDomAttribute("data-activity-status"));
            servico_obj.data_activity_worktype = Int32.Parse(servico.GetDomAttribute("data-activity-worktype"));
            servico_obj.data_activity_duration = Int32.Parse(servico.GetDomAttribute("data-activity-duration"));
            this.atual.Where(s => s.par_pid == servico_obj.par_pid).Single().servicos.Add(servico_obj);
          }
          // Verifica se é uma janela de tempo
          if(ordem_classes.Contains("toaGantt-tl-shift")) {}
          // verifica se é uma alteração da jornada
          if(ordem_classes.Contains("toaGantt-queue")) {}
          // Verifica se é uma alteração na tempo
          if(ordem_classes.Contains("toaGantt-tl-gpsmark")) {}
        }
      }
    }
    foreach (var espelho in this.atual)
    {
      var json_conf = new System.Text.Json.JsonSerializerOptions();
      json_conf.WriteIndented = true;
      var json_text = System.Text.Json.JsonSerializer.Serialize<Espelho>(espelho, json_conf);
      System.Console.WriteLine(json_text);
    }
  }
}