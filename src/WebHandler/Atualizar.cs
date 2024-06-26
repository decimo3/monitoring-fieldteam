using OpenQA.Selenium;
namespace Automation.WebScraper;
public partial class Manager
{
  public void Atualizar(String piscina)
  {
    // Verifica se não foi direcionado a página de logout
    var sub_baldes = piscina.Split('>');
    var baldes = this.driver.FindElements(By.ClassName("edt-item"));
    var i = 0;
    var j = 0;
    while(true)
    {
      if(i == baldes.Count)
      {
        ProximoBalde();
        throw new InvalidOperationException($"O balde {sub_baldes[j]} não foi encontrado!");
      }
      var texto = baldes[i].GetAttribute("innerText");
      if(String.IsNullOrEmpty(texto)) continue;
      if(texto.Contains(sub_baldes[j]))
      {
        if(j > 0 && texto.Contains(sub_baldes[j - 1]))
        {
          i++;
          continue;
        }
        if(j == (sub_baldes.Count() - 1))
        {
          baldes[i].Click();
          break;
        }
        var tree_arrow = baldes[i].FindElements(By.XPath("./div/button")).First();
        if(tree_arrow.GetAttribute("class").Contains("ptplus")) tree_arrow.Click();
        System.Threading.Thread.Sleep(this.cfg.ESPERAS["CURTA"]);
        baldes = this.driver.FindElements(By.ClassName("edt-item"));
        i = 0;
        j++;
        continue;
      }
      i++;
    }

    System.Threading.Thread.Sleep(this.cfg.ESPERAS["MEDIA"]);
    // Selecionar a visualização do gráfico de Gantt
    this.driver.FindElements(By.ClassName("oj-ux-ico-clock")).First().Click();
    // Abrir menu de seleção de preferências
    this.driver.FindElements(By.ClassName("toolbar-item")).Where(e => e.Text == "Exibir").First().Click();
    System.Threading.Thread.Sleep(this.cfg.ESPERAS["CURTA"]);
    // Selecionar para exibir de forma herarquica
    var checkbox_hierarquico = this.driver.FindElement(By.XPath(this.cfg.CAMINHOS["CHECK_TREE"]));
    if(!checkbox_hierarquico.Selected) checkbox_hierarquico.Click();
    // Selecionar para exibir a rota do recurso
    var checkbox_exibir_rota = this.driver.FindElement(By.XPath(this.cfg.CAMINHOS["CHECK_ROUTE"]));
    if(!checkbox_exibir_rota.Selected) checkbox_exibir_rota.Click();
    // Ajusta o zoom da página para visualizar toda linha do tempo
    // this.driver.FindElement(By.XPath(this.cfg.CAMINHOS["ZOOM_FIT"])).Click();
    // Aplicar as preferências de seleções
    this.driver.FindElements(By.ClassName("app-button-title")).Where(e => e.Text == "Aplicar").First().Click();
    System.Threading.Thread.Sleep(this.cfg.ESPERAS["LONGA"]);
    this.datalabel = DateOnly.Parse(this.driver.FindElement(By.ClassName("toolbar-date-picker-button")).Text);
    this.espelhos = new();
    this.relatorios = new();
    this.agora = DateTime.Now;
  }
}
