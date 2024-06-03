using OpenQA.Selenium;

namespace Automation.WebScraper
{
  public partial class Manager
  {
    public void Refresh()
    {
      this.driver.Navigate().Refresh();
      System.Threading.Thread.Sleep(this.cfg.ESPERAS["LONGA"]);
    }
    public void Parametrizar()
    {
      // DONE - Coletar a posição do horário atual `toaGantt-time-line`
      var regua_hora_atual = this.driver.FindElement(By.ClassName("toaGantt-time-line"));
      this.horario_atual = ColetarStyle(regua_hora_atual.GetDomAttribute("style"))["left"];
      // TODO - Calcular a quantidade de minutos em um pixel de deslocamento
      var regua_hora_hora = this.driver.FindElements(By.ClassName("toaGantt-hour-line"));
      var pixel_1th_hora = ColetarStyle(regua_hora_hora[1].GetDomAttribute("style"))["left"];
      var pixel_2th_hora = ColetarStyle(regua_hora_hora[0].GetDomAttribute("style"))["left"];
      this.pixels_por_hora = pixel_1th_hora - pixel_2th_hora;
      this.pixels_por_minuto = this.pixels_por_hora / 60;
    }
    public void ProximoBalde()
    {
      this.contador_de_baldes = (this.contador_de_baldes + 1) % this.cfg.PISCINAS.Count;
      System.Threading.Thread.Sleep(this.cfg.ESPERAS["LONGA"]);
    }
    public bool TemFinalizacao()
    {
      this.balde_nome = this.cfg.PISCINAS[this.contador_de_baldes].Split('>').Last();
      var filename = $"{this.cfg.DOWNFOLDER}/{this.agora.ToString("yyyyMMdd")}_{this.balde_nome}.csv";
      return System.IO.File.Exists(filename);
    }
  }
}