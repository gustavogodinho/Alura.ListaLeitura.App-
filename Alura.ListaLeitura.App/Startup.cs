using Alura.ListaLeitura.App.Negocio;
using Alura.ListaLeitura.App.Repositorio;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Alura.ListaLeitura.App
{
    public class Startup
    {
        readonly LivroRepositorioCSV _repositorio = new LivroRepositorioCSV();

        public void Configure(IApplicationBuilder app)
        {
            var builder = new RouteBuilder(app);
            builder.MapRoute("Livros/ParaLer", LivrosParaLer);
            builder.MapRoute("Livros/Lendo", LivrosLendo);
            builder.MapRoute("Livros/Lidos", LivrosLidos);
            builder.MapRoute("cadastro/novolivro/{nome}/{autor}", CadastroNovoLivro);
            builder.MapRoute("Livros/Detalhes/{id:int}", ExibeDetalhes);
            builder.MapRoute("cadastro/novolivro", ExibeNovoLivro);
            builder.MapRoute("Cadastro/Incluir", ProcessaFormulario);
            var rotas = builder.Build();
            app.UseRouter(rotas);
        }

        private Task ExibeDetalhes(HttpContext context)
        {
            try
            {
                int id = Convert.ToInt32(context.GetRouteValue("id"));

                var livro = _repositorio.BuscaLivroPorId(id);

                return context.Response.WriteAsync(livro.Detalhes());
            }
            catch (Exception e)
            {
                return context.Response.WriteAsync("Não encontrado!");
            }

        }
        private Task ProcessaFormulario(HttpContext context)
        {
                var livro = new Livro()
                {
                    Titulo = context.Request.Query["titulo"].First(),
                    Autor = context.Request.Query["autor"].First()
                };
                _repositorio.Incluir(livro);

                return context.Response.WriteAsync("Cadastrado com sucesso!");
        }

        private Task ExibeNovoLivro(HttpContext context)
        {
            var html = @"<html>
                        <form action='/Cadastro/Incluir'>
                       <input name='titulo' />
                        <input name='autor' />
                        <button>Gravar</button>
                    </form>
                </html>";

            return context.Response.WriteAsync(html);
        }

        private Task CadastroNovoLivro(HttpContext context)
        {
            var livro = new Livro
            {
                Titulo = context.GetRouteValue("nome").ToString(),
                Autor = context.GetRouteValue("autor").ToString()
            };
            _repositorio.Incluir(livro);
            return context.Response.WriteAsync("Livro adicionado com sucesso!");
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddRouting();
        }

        private Task LivrosLendo(HttpContext context)
        {
            return context.Response.WriteAsync(_repositorio.Lendo.ToString());
        }

        private Task LivrosLidos(HttpContext context)
        {
            return context.Response.WriteAsync(_repositorio.Lidos.ToString());
        }

        private Task LivrosParaLer(HttpContext context)
        {
            return context.Response.WriteAsync(_repositorio.ParaLer.ToString());
        }
    }
}
