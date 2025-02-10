using FI.AtividadeEntrevista.BLL;
using WebAtividadeEntrevista.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FI.AtividadeEntrevista.DML;
using System.Reflection;
using System.Web.UI.WebControls;
using Newtonsoft.Json;

namespace WebAtividadeEntrevista.Controllers
{
    public class BeneficiarioController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Incluir()
        {
            return View();
        }
        // Função para validar o dígito do CPF
        private bool ValidarCpf(string cpf)
        {
            cpf = cpf.Replace(".", "").Replace("-", "");

            if (cpf.Length != 11)
                return false;

            if (cpf.All(c => c == cpf[0]))
                return false;

            int soma = 0;
            for (int i = 0; i < 9; i++)
            {
                soma += int.Parse(cpf[i].ToString()) * (10 - i);
            }
            int digito1 = 11 - (soma % 11);
            if (digito1 == 10 || digito1 == 11)
                digito1 = 0;
            if (int.Parse(cpf[9].ToString()) != digito1)
                return false;

            soma = 0;
            for (int i = 0; i < 10; i++)
            {
                soma += int.Parse(cpf[i].ToString()) * (11 - i);
            }
            int digito2 = 11 - (soma % 11);
            if (digito2 == 10 || digito2 == 11)
                digito2 = 0;
            if (int.Parse(cpf[10].ToString()) != digito2)
                return false;

            return true;
        }

        [HttpPost]
        public JsonResult Incluir(BeneficiarioModel model)
        {
            BoBeneficiario bo = new BoBeneficiario();
            bool cpfExistente = bo.VerificarExistencia(model.CPFBeneficiario);

            if (!ValidarCpf(model.CPFBeneficiario))
            {
                Response.StatusCode = 400;
                return Json("O CPF informado é inválido.");
            }

            if (cpfExistente)
            {
                Response.StatusCode = 400;
                return Json("Este CPF já está cadastrado no sistema.");
            }

            if (!this.ModelState.IsValid)
            {
                List<string> erros = (from item in ModelState.Values
                                      from error in item.Errors
                                      select error.ErrorMessage).ToList();

                Response.StatusCode = 400;
                return Json(string.Join(Environment.NewLine, erros));
            }
            else
            {

                model.Id = bo.Incluir(new Beneficiario()
                {
                    Nome = model.NomeBeneficiario,
                    CPF = model.CPFBeneficiario,
                    IDCLIENTE = model.IdCliente

                });


                return Json("Cadastro efetuado com sucesso");
            }
        }

        [HttpPost]
        public JsonResult Alterar(BeneficiarioModel model)
        {
            BoBeneficiario bo = new BoBeneficiario();

            if (!this.ModelState.IsValid)
            {
                List<string> erros = (from item in ModelState.Values
                                      from error in item.Errors
                                      select error.ErrorMessage).ToList();

                Response.StatusCode = 400;
                return Json(string.Join(Environment.NewLine, erros));
            }
            else
            {
                bo.Alterar(new Beneficiario()
                {
                    IDCLIENTE = model.IdCliente,
                    Nome = model.NomeBeneficiario,
                    CPF = model.CPFBeneficiario,
                    Id = model.Id

                });

                return Json("Cadastro alterado com sucesso");
            }
        }

        [HttpGet]
        public ActionResult Alterar(long id)
        {
            BoBeneficiario bo = new BoBeneficiario();
            Beneficiario beneficiario = bo.Consultar(id);
            Models.BeneficiarioModel model = null;

            if (beneficiario != null)
            {
                model = new BeneficiarioModel()
                {
                    IdCliente = beneficiario.IDCLIENTE,
                    NomeBeneficiario = beneficiario.CPF,
                    CPFBeneficiario = beneficiario.CPF,
                    Id = beneficiario.Id

                };


            }

            return View(model);
        }

        [HttpPost]
        public JsonResult BeneficiarioList()
        {
            try
            {
                Models.BeneficiarioModel model = null;
                model = new Models.BeneficiarioModel();


                List<Beneficiario> clientes = new BoBeneficiario().Listar(model.Id);

                return Json(new { Result = "OK", Records = clientes });
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message });
            }
 
        }


    }


}
